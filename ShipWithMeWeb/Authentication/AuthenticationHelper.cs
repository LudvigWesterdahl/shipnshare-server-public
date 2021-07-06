using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ShipWithMeCore.ExternalServices;
using ShipWithMeCore.SharedKernel;
using ShipWithMeInfrastructure;
using ShipWithMeInfrastructure.Models;
using ShipWithMeInfrastructure.Repositories;
using ShipWithMeWeb.RequestInputs;

namespace ShipWithMeWeb.Authentication
{
    public sealed class AuthenticationHelper
    {
        private readonly ILogger<AuthenticationHelper> logger;

        private readonly UserManager<User> userManager;

        private readonly RoleManager<IdentityRole<long>> roleManager;

        private readonly MainDbContext mainDbContext;

        private readonly IEmailService emailService;

        private readonly string secret;

        private readonly string issuer;

        private readonly string audience;

        private readonly int validMinutes;

        public const string AdminRole = "Admin";

        public const string ModeratorRole = "Moderator";

        public const string CustomerRole = "Customer";

        public const string AdminRights = "AdminRights";

        public const string ModeratorRights = "ModeratorRights";

        public const string CustomerRights = "CustomerRights";


        public AuthenticationHelper(
            ILogger<AuthenticationHelper> logger,
            UserManager<User> userManager,
            RoleManager<IdentityRole<long>> roleManager,
            MainDbContext mainDbContext,
            IEmailService emailService,
            string secret,
            string issuer,
            string audience,
            int validMinutes)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mainDbContext = mainDbContext;
            this.emailService = emailService;
            this.secret = secret;
            this.issuer = issuer;
            this.audience = audience;
            this.validMinutes = validMinutes;
        }

        private bool SetPassword(User user, string newPassword)
        {
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, newPassword);

            logger.LogInformation("Setting password to {} with hash {}", newPassword, user.PasswordHash);

            foreach (var validator in userManager.PasswordValidators)
            {
                var valid = validator.ValidateAsync(userManager, user, newPassword).GetAwaiter().GetResult().Succeeded;

                if (!valid)
                {
                    return false;
                }
            }

            var success = userManager.UpdateAsync(user).GetAwaiter().GetResult().Succeeded;

            logger.LogInformation("Success: {}", success);

            user.ResetPasswordKeyCreatedAt = null;
            mainDbContext.SaveChanges();
            return success;
        }

        public Tuple<long, string> RegisterAccount(CreateUser createUser)
        {
            var newUser = new User
            {
                Email = createUser.Email,
                UserName = createUser.UserName
            };

            var createUserResult = userManager.CreateAsync(newUser, createUser.Password)
                .GetAwaiter()
                .GetResult();

            if (!createUserResult.Succeeded)
            {
                return null;
            }

            var addCustomerRoleResult = userManager.AddToRoleAsync(newUser, CustomerRole)
                .GetAwaiter()
                .GetResult();

            if (!addCustomerRoleResult.Succeeded)
            {
                userManager.DeleteAsync(newUser).GetAwaiter().GetResult();
                return null;
            }

            var token = userManager.GenerateEmailConfirmationTokenAsync(newUser).GetAwaiter().GetResult();

            return Tuple.Create(newUser.Id, token);
        }

        public bool ConfirmEmail(long userId, string token)
        {
            Validate.That(userId, nameof(userId)).IsGreaterThan(0L);
            Validate.That(token, nameof(token)).IsNot(null);

            var user = mainDbContext.Users.Where(u => u.Id == userId).FirstOrDefault();
            Validate.That(user, nameof(user)).IsNot(null);

            var result = userManager.ConfirmEmailAsync(user, token).GetAwaiter().GetResult();

            if (result.Succeeded)
            {
                emailService.Send("ShipnShare account created",
                    $"<p>Welcome {user.UserName}!</p>"
                    + $"<p>Your account has successfully been created. If you have any questions or concerns, "
                    + $"don't hesitate to send a message to indecodeab@gmail.com for further assistance.</p>"
                    + $"<p>We hope you will enjoy this service while saving the forest, one box at a time!</p>",
                    user.Email)
                .GetAwaiter().GetResult();

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Generates a new unique authentication token.
        /// </summary>
        /// <returns>the token and expire date</returns>
        private async Task<Tuple<string, DateTime>> GenerateToken(User user)
        {
            var expiration = DateTime.UtcNow.AddMinutes(validMinutes);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Expiration, DateTimeUtils.ToString(expiration)),
                new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
            };

            var roleNames = await userManager.GetRolesAsync(user);

            foreach (var roleName in roleNames)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: expiration,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Tuple.Create(new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
        }

        /// <summary>
        /// Generates a new unique refresh token.
        /// </summary>
        /// <returns>the refresh token</returns>
        private async Task<string> GenerateRefreshToken()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];

            rngCryptoServiceProvider.GetBytes(randomBytes);
            var refreshTokenString = Convert.ToBase64String(randomBytes);

            while (await mainDbContext.RefreshTokens.FindAsync(refreshTokenString) != null)
            {
                rngCryptoServiceProvider.GetBytes(randomBytes);
                refreshTokenString = Convert.ToBase64String(randomBytes);
            }

            return refreshTokenString;
        }


        /// <summary>
        /// Stores and returns a refresh token belonging to a particular user and deletes expired refresh tokens.
        /// </summary>
        /// <returns>the new refresh token belonging by the user</returns>
        private async Task<RefreshToken> SaveRefreshToken(User user)
        {
            var refreshTokenString = await GenerateRefreshToken();

            if (user == null || refreshTokenString == null)
            {
                return null;
            }

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                Expires = DateTime.UtcNow.AddDays(30),
                UserId = user.Id
            };

            await mainDbContext.RefreshTokens.AddAsync(refreshToken);
            await mainDbContext.SaveChangesAsync();

            var utcNow = DateTime.UtcNow;

            var refreshTokensToDelete = mainDbContext.RefreshTokens
                .Where(rt => rt.UserId == refreshToken.UserId)
                .Where(rt => rt.Expires < utcNow)
                .AsEnumerable();

            mainDbContext.RefreshTokens.RemoveRange(refreshTokensToDelete);
            await mainDbContext.SaveChangesAsync();

            return refreshToken;
        }

        /// <summary>
        /// Authenticates a user and returns a tuple containing the token and the expiration.
        /// </summary>
        /// <param name="email">the user email</param>
        /// <param name="password">the user password</param>
        /// <returns>token, token expiry, refresh token, refresh token expiry if successful, null otherwise</returns>
        public async Task<Tuple<string, DateTime, string, DateTime>> Authenticate(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            var validPassword = await userManager.CheckPasswordAsync(user, password);
            if (!validPassword)
            {
                return null;
            }

            var confirmedEmail = await userManager.IsEmailConfirmedAsync(user);

            if (!confirmedEmail)
            {
                return null;
            }

            var token = await GenerateToken(user);

            var refreshToken = await SaveRefreshToken(user);

            return Tuple.Create(
                token.Item1,
                token.Item2,
                refreshToken.Token,
                refreshToken.Expires);
        }

        /// <summary>
        /// Refreshes an authentication given a refresh token. The returned refresh token might be the same.
        /// </summary>
        /// <param name="refreshTokenString">the refresh token</param>
        /// <returns>token, token expiry, refresh token, refresh token expiry if successful, null otherwise</returns>
        public async Task<Tuple<string, DateTime, string, DateTime>> RefreshAuthentication(string refreshTokenString)
        {
            var refreshToken = await mainDbContext.RefreshTokens
                .Include(rt => rt.User)
                .Where(rt => rt.Token == refreshTokenString)
                .FirstOrDefaultAsync();

            if (refreshToken == null || refreshToken.Expires < DateTime.UtcNow)
            {
                return null;
            }

            var user = refreshToken.User;

            bool generateNewRefreshToken = refreshToken.Expires.AddDays(-6) <= DateTime.UtcNow;

            var token = await GenerateToken(user);

            if (!generateNewRefreshToken)
            {
                return Tuple.Create(
                    token.Item1,
                    token.Item2,
                    refreshToken.Token,
                    refreshToken.Expires);
            }

            var newRefreshToken = await SaveRefreshToken(user);

            return Tuple.Create(
                token.Item1,
                token.Item2,
                newRefreshToken.Token,
                newRefreshToken.Expires);
        }



        /// <summary>
        /// Blocks a user.
        /// </summary>
        /// <param name="userBlockInfo">the information</param>
        /// <returns>all blocks on this user</returns>
        public async Task<IEnumerable<UserBlock>> BlockUser(UserBlockInfo userBlockInfo)
        {
            var user = await userManager.FindByEmailAsync(userBlockInfo.Email);
            Validate.That(user, nameof(user)).IsNot(null);


            var id = await RepositoryUtils.NewGuidString(guid => mainDbContext.UserBlocks.FindAsync(guid));

            var blockedUser = new UserBlock
            {
                Id = id,
                Version = 1,
                UserId = user.Id,
                Reason = userBlockInfo.Reason,
                From = DateTimeUtils.Parse(userBlockInfo.From),
                To = DateTimeUtils.Parse(userBlockInfo.To),
                Active = true,
            };

            await mainDbContext.UserBlocks.AddAsync(blockedUser);
            await mainDbContext.SaveChangesAsync();

            logger.LogInformation("Blocked user with email {Email} from {From} to {To}",
                userBlockInfo.Email,
                userBlockInfo.From,
                userBlockInfo.To);

            var currentUserBlocks = mainDbContext.UserBlocks
                .Include(bu => bu.User)
                .Where(bu => bu.UserId == blockedUser.UserId)
                .ToList();

            return currentUserBlocks;
        }

        /// <summary>
        /// Unblocks a user.
        /// </summary>
        /// <param name="userUnblockInfo">the information</param>
        /// <returns>all blocks on the user</returns>
        public async Task<IEnumerable<UserBlock>> UnblockUser(UserUnblockInfo userUnblockInfo)
        {
            var user = await userManager.FindByEmailAsync(userUnblockInfo.Email);
            Validate.That(user, nameof(user)).IsNot(null);

            var userBlocks = mainDbContext.UserBlocks.Where(bu => bu.UserId == user.Id).ToList();

            var userBlocksToUpdate = new List<UserBlock>();
            foreach (var userBlock in userBlocks)
            {
                if (userBlock.Active)
                {
                    userBlock.Active = false;
                }
            }

            mainDbContext.UserBlocks.UpdateRange(userBlocksToUpdate);
            await mainDbContext.SaveChangesAsync();

            var currentUserBlocks = mainDbContext.UserBlocks
                .Include(bu => bu.User)
                .Where(bu => bu.UserId == user.Id)
                .ToList();

            return currentUserBlocks;
        }

        /// <summary>
        /// Checks if a user with the given email is blocked.
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <returns>true if blocked, false otherwise</returns>
        public async Task<bool> IsBlocked(string email)
        {
            Validate.That(email, nameof(email)).IsNot(null);

            var user = await userManager.FindByEmailAsync(email);
            Validate.That(user, nameof(user)).IsNot(null);

            var currentDateTime = DateTime.UtcNow;

            var currentUserBlocks = mainDbContext.UserBlocks
                .Where(bu => bu.UserId == user.Id)
                .ToList();

            foreach (var userBlock in currentUserBlocks)
            {
                var started = userBlock.From <= currentDateTime;
                var notEnded = currentDateTime <= userBlock.To;

                if (userBlock.Active && started && notEnded)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns all current blocks on the user.
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <returns>all blocks</returns>
        public async Task<IEnumerable<UserBlock>> GetUserBlocks(string email)
        {
            Validate.That(email, nameof(email)).IsNot(null);

            var user = await userManager.FindByEmailAsync(email);
            Validate.That(user, nameof(user)).IsNot(null);

            var currentUserBlocks = mainDbContext.UserBlocks
                .Include(bu => bu.User)
                .Where(bu => bu.UserId == user.Id)
                .ToList();

            return currentUserBlocks;
        }

        /// <summary>
        /// Returns the user id.
        /// </summary>
        /// <param name="claimsPrincipal">the claims</param>
        /// <returns>user id</returns>
        public static long GetUserId(ClaimsPrincipal claimsPrincipal)
        {
            var claim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            Validate.That(claim, nameof(claim)).IsNot(null);

            return Convert.ToInt64(claim.Value);
        }

        /// <summary>
        /// Returns the user email.
        /// </summary>
        /// <param name="claimsPrincipal">the claims</param>
        /// <returns>user email or null if not authenticated</returns>
        public static string GetUserEmail(ClaimsPrincipal claimsPrincipal)
        {
            var claim = claimsPrincipal.FindFirst(ClaimTypes.Email);

            if (claim == null)
            {
                return null;
            }

            return claim.Value;
        }

        /// <summary>
        /// Returns the user name.
        /// </summary>
        /// <param name="claimsPrincipal">the claims</param>
        /// <returns>user email or null if not authenticated</returns>
        public static string GetUserName(ClaimsPrincipal claimsPrincipal)
        {
            var claim = claimsPrincipal.FindFirst(ClaimTypes.Name);

            if (claim == null)
            {
                return null;
            }

            return claim.Value;
        }


        /// <summary>
        /// Seeds the roles used.
        /// </summary>
        /// <param name="userManager">the user manager</param>
        /// <param name="roleManager">the role manager</param>
        /// <returns></returns>
        private static async Task SeedRolesAsync(RoleManager<IdentityRole<long>> roleManager)
        {
            Console.WriteLine("Seeding roles.");
            bool createAdminRole = !await roleManager.RoleExistsAsync(AdminRole);

            if (createAdminRole)
            {
                var createAdminRoleResult = await roleManager.CreateAsync(new IdentityRole<long> { Name = AdminRole });

                if (!createAdminRoleResult.Succeeded)
                {
                    throw new Exception("Failed to create Admin role.");
                }
            }

            bool createModeratorRole = !await roleManager.RoleExistsAsync(ModeratorRole);

            if (createModeratorRole)
            {
                var createModeratorRoleResult = await roleManager.CreateAsync(
                    new IdentityRole<long> { Name = ModeratorRole });

                if (!createModeratorRoleResult.Succeeded)
                {
                    throw new Exception("Failed to create Moderator role.");
                }
            }

            bool createCustomerRole = !await roleManager.RoleExistsAsync(CustomerRole);

            if (createCustomerRole)
            {
                var createCustomerRoleResult = await roleManager.CreateAsync(
                    new IdentityRole<long> { Name = CustomerRole });

                if (!createCustomerRoleResult.Succeeded)
                {
                    throw new Exception("Failed to create Customer role.");
                }
            }
        }

        /// <summary>
        /// Seeds a given admin user
        /// </summary>
        /// <param name="userName">the admin user name</param>
        /// <param name="email">the admin email</param>
        /// <param name="password">the admin password</param>
        /// <param name="userManager">the user manager</param>
        /// <param name="roleManager">the role manager</param>
        /// <returns></returns>
        public static async Task SeedAdminUserAsync(
            string userName,
            string email,
            string password,
            UserManager<User> userManager,
            RoleManager<IdentityRole<long>> roleManager)
        {
            await SeedRolesAsync(roleManager);

            Console.WriteLine("Seeding admin user");

            var adminUser = new User()
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
            };

            var createAdminUser = await userManager.FindByEmailAsync(adminUser.Email) == null;

            if (createAdminUser)
            {
                var createAdminUserResult = await userManager.CreateAsync(adminUser, password);

                if (!createAdminUserResult.Succeeded)
                {
                    throw new Exception("Failed to create Admin user.");
                }

                var addAdminRoleResult = await userManager.AddToRoleAsync(adminUser, AdminRole);

                if (!addAdminRoleResult.Succeeded)
                {
                    throw new Exception("Failed to add Admin role to the admin user");
                }
            }
        }

        /// <summary>
        /// Seeds the users and roles.
        /// </summary>
        /// <param name="userManager">the user manager</param>
        /// <param name="roleManager">the role manager</param>
        /// <returns></returns>
        public static async Task SeedUserAndRolesAsync(
        UserManager<User> userManager, RoleManager<IdentityRole<long>> roleManager)
        {
            await SeedRolesAsync(roleManager);

            Console.WriteLine("Seeding users and roles.");
            bool createAdminRole = !await roleManager.RoleExistsAsync(AdminRole);


            var adminUser = new User()
            {
                UserName = "admin",
                Email = "ludvigwesterdahl@gmail.com",
                EmailConfirmed = true,
            };

            var createAdminUser = await userManager.FindByEmailAsync(adminUser.Email) == null;

            if (createAdminUser)
            {
                var createAdminUserResult = await userManager.CreateAsync(adminUser, "Testar123");

                if (!createAdminUserResult.Succeeded)
                {
                    throw new Exception("Failed to create Admin user.");
                }

                var addAdminRoleResult = await userManager.AddToRoleAsync(adminUser, AdminRole);

                if (!addAdminRoleResult.Succeeded)
                {
                    throw new Exception("Failed to add Admin role to the Admin user");
                }
            }

            var moderatorUser = new User()
            {
                UserName = "moderator",
                Email = "ludvigwesterdahl@moderator.com",
                EmailConfirmed = true,
            };

            var createModeratorUser = await userManager.FindByEmailAsync(moderatorUser.Email) == null;

            if (createModeratorUser)
            {
                var createModeratorUserResult = await userManager.CreateAsync(moderatorUser, "TestTest12345!!!!");

                if (!createModeratorUserResult.Succeeded)
                {
                    throw new Exception("Failed to create Admin user.");
                }

                var addModeratorRoleResult = await userManager.AddToRoleAsync(moderatorUser, ModeratorRole);

                if (!addModeratorRoleResult.Succeeded)
                {
                    throw new Exception("Failed to add Moderator role to the Moderator user");
                }
            }

            var customerUser = new User()
            {
                UserName = "customer",
                Email = "ludvigwesterdahl@customer.com",
                EmailConfirmed = true
            };

            var createCustomerUser = await userManager.FindByEmailAsync(customerUser.Email) == null;

            if (createCustomerUser)
            {
                var createCustomerUserResult = await userManager.CreateAsync(customerUser, "TestTest12345!!!!");

                if (!createCustomerUserResult.Succeeded)
                {
                    throw new Exception("Failed to create Customer user.");
                }

                var addCustomerRoleResult = await userManager.AddToRoleAsync(customerUser, CustomerRole);

                if (!addCustomerRoleResult.Succeeded)
                {
                    throw new Exception("Failed to add Customer role to the Customer user");
                }
            }

        }

        /// <summary>
        /// Seeds the users and roles synchronously.
        /// </summary>
        /// <param name="userManager">the user manager</param>
        /// <param name="roleManager">the role manager</param>
        public static void SeedUserAndRoles(
            UserManager<User> userManager, RoleManager<IdentityRole<long>> roleManager)
        {
            SeedUserAndRolesAsync(userManager, roleManager).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Seeds an admin user synchronously.
        /// </summary>
        /// <param name="userName">the admin user name</param>
        /// <param name="email">the admin email</param>
        /// <param name="password">the admin password</param>
        /// <param name="userManager">the user manager</param>
        /// <param name="roleManager">the role manager</param>
        public static void SeedAdminUser(
            string userName,
            string email,
            string password,
            UserManager<User> userManager,
            RoleManager<IdentityRole<long>> roleManager)
        {
            SeedAdminUserAsync(userName, email, password, userManager, roleManager).GetAwaiter().GetResult();
        }


        public string GenerateResetPasswordToken(long userId)
        {
            var user = mainDbContext.Users.Where(u => u.Id == userId).FirstOrDefault();

            if (user == null)
            {
                throw new ArgumentException("Invalid userId");
            }

            var token = userManager.GeneratePasswordResetTokenAsync(user).GetAwaiter().GetResult();

            return token;
        }

        public bool ResetPasswordWithToken(long userId, string token, string newPassword)
        {
            Validate.That(userId, nameof(userId)).IsGreaterThan(0L);
            Validate.That(token, nameof(token)).IsNot(null);
            Validate.That(newPassword, nameof(newPassword)).IsNot(null);

            var user = mainDbContext.Users.Where(u => u.Id == userId).FirstOrDefault();
            Validate.That(user, nameof(user)).IsNot(null);

            var result = userManager.ResetPasswordAsync(user, token, newPassword).GetAwaiter().GetResult();

            if (result.Succeeded)
            {
                user.EmailConfirmed = true;
                mainDbContext.SaveChanges();

                emailService.Send("ShipnShare Password reset",
                    $"<p>Your password was successfully reset!</p>",
                    user.Email)
                    .GetAwaiter().GetResult();

                return true;
            }
            else
            {
                emailService.Send("ShipnShare Password reset",
                    $"<p>Someone tried to reset your password, please contact support if this was not you!</p>",
                    user.Email)
                    .GetAwaiter().GetResult();

                return false;
            }
        }

        public string GenerateResetPasswordKey(long userId)
        {
            var user = mainDbContext.Users.Where(u => u.Id == userId).FirstOrDefault();

            if (user == null)
            {
                throw new ArgumentException("Invalid userId");
            }

            if (user.ResetPasswordKeyCreatedAt?.AddMinutes(15) > DateTime.UtcNow)
            {
                throw new ArgumentException("Cannot reset password again within 15 minutes.");
            }

            var possibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

            var rnd = new Random();

            var stringBuilder = new StringBuilder();

            for (int i = 0; i < 6; i++)
            {
                stringBuilder.Append(possibleChars[rnd.Next(possibleChars.Length)]);
            }

            emailService.Send("ShipnShare Password reset key",
                $"<p>Here is your password reset key</p><p><b>{user.ResetPasswordKey}</b></p>",
                user.Email)
                .GetAwaiter().GetResult();

            user.ResetPasswordKey = stringBuilder.ToString();
            user.ResetPasswordKeyCreatedAt = DateTime.UtcNow;
            mainDbContext.SaveChanges();

            return stringBuilder.ToString();
        }

        public bool ResetPassword(long userId, string resetPasswordKey, string newPassword)
        {
            var user = mainDbContext.Users.Where(u => u.Id == userId).FirstOrDefault();

            if (user == null)
            {
                throw new ArgumentException("Invalid userId");
            }

            if (resetPasswordKey == null)
            {
                throw new ArgumentException("Invalid resetPasswordKey");
            }

            if (user.ResetPasswordKeyCreatedAt == null)
            {
                return false;
            }

            if (user.ResetPasswordKeyCreatedAt?.AddMinutes(15) < DateTime.UtcNow)
            {
                return false;
            }

            if (!resetPasswordKey.Equals(user.ResetPasswordKey))
            {
                return false;
            }

            if (!SetPassword(user, newPassword))
            {
                return false;
            }

            emailService.Send("ShipnShare Password reset",
                $"<p>Your password was successfully reset!</p>",
                user.Email)
                .GetAwaiter().GetResult();

            return true;
        }
    }
}
