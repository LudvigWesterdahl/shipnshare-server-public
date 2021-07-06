using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ShipWithMeCore.Entities;

namespace ShipWithMeInfrastructure.Models
{
    public sealed class User : IdentityUser<long>
    {
        public ICollection<Post> Posts { get; set; }

        public ICollection<UserBlock> UserBlocks { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }

        public string ResetPasswordKey { get; set; }

        public DateTime? ResetPasswordKeyCreatedAt { get; set; }

        public UserEntity ToUserEntity()
        {
            return new UserEntity(Id, Email, UserName);
        }
    }
}
