using System;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeCore.Entities
{
    public sealed class UserEntity
    {
        public long Id { get; }

        public string Email { get; }

        public string UserName { get; }

        public UserEntity(long id, string email, string userName)
        {
            Id = id;
            Validate.That(email, nameof(email)).IsNot(null);
            Validate.That(email.IndexOf("@"), "email.IndexOf(@)")
                .IsGreaterThan(0);
            Validate.That(email.Contains("@"), "email.Contains(@)").Is(true);
            Validate.That(email.Contains("."), "email.Contains(.)").Is(true);
            var indexOfAt = email.IndexOf("@");
            Validate.That(indexOfAt + 1 < email.IndexOf(".", indexOfAt),
                "@ comes before domain.tld").Is(true);
            Email = email;

            Validate.That(userName, nameof(userName)).IsNot(null);
            UserName = userName;
        }
    }
}
