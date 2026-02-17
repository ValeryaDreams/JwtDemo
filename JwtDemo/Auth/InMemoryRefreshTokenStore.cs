using System.Collections.Concurrent;

namespace JwtDemo.Auth
{
        public class InMemoryRefreshTokenStore
        {
                private readonly ConcurrentDictionary<string, RefreshTokenRecord> _byHash = new();

                public void Save(RefreshTokenRecord record)
                {
                        _byHash[record.TokenHash] = record;
                }

                public RefreshTokenRecord? FindByHash(string tokenHash)
                {
                        return _byHash.TryGetValue(tokenHash, out var r) ? r : null;
                }
        }
}