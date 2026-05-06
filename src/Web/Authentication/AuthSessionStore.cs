using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;
using Web.Common.Constants;

namespace Web.Authentication
{
    public class AuthSessionStore : ITicketStore
    {
        private readonly IDistributedCache _distributedCache;

        public AuthSessionStore(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            string? key = ticket.Principal.FindFirst(CustomClaimTypes.SessionId)?.Value;
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("セッションIDが不明です。");
            }

            await RenewAsync(key, ticket);

            return key;
        }

        public async Task<AuthenticationTicket?> RetrieveAsync(string key)
        {
            var bytes = await _distributedCache.GetAsync(key);
            if (bytes is null)
            {
                return null;
            }

            var ticket = TicketSerializer.Default.Deserialize(bytes);

            return ticket;
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var bytes = TicketSerializer.Default.Serialize(ticket);

            var options = new DistributedCacheEntryOptions();
            if (ticket.Properties.ExpiresUtc.HasValue)
            {
                options.AbsoluteExpiration = ticket.Properties.ExpiresUtc.Value;
            }
            else
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(AuthSessionSettings.ExpiredMinutes);
            }

            await _distributedCache.SetAsync(key, bytes, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
    }
}