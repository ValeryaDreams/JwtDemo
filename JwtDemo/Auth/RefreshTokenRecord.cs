namespace JwtDemo.Auth
{
        public class RefreshTokenRecord
        {
                public required string UserId { get; init; }
                public required string TokenHash { get; init; }

                public DateTime CreatedAtUtc { get; init; }
                public DateTime ExpreseAtUtc { get; init; }

                public DateTime? RevokedAtUtc { get; set; }
                public string? ReplacedByTokenHash { get; set; }

                public bool IsExpired => DateTime.UtcNow > ExpreseAtUtc;
                public bool IsRevoked => RevokedAtUtc != null;

        }
}
