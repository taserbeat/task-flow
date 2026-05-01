using Infrastructure.Contexts;
using Web.Common.Constants;

namespace Web.Contexts
{
    public class RlsContext : IRlsContext
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AsyncLocal<RlsContextData> _context = new();

        public RlsContext(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string? TenantId
        {
            get
            {
                // コンテキストで明示的に設定されていれば、そのテナントIDを使用
                if (_context.Value?.TenantId != null)
                {
                    return _context.Value.TenantId;
                }

                // なければHttpContextから取得
                return GetClaim(CustomClaimTypes.TenantId);
            }
        }

        public bool IsBypass
        {
            get
            {
                return _context.Value?.IsBypass ?? false;
            }
        }

        public IDisposable CreateTenantIdScope(string tenantId)
        {
            var prevContext = _context.Value?.Clone();
            _context.Value = new RlsContextData
            {
                TenantId = tenantId,
                IsBypass = false,
            };

            return new RlsContextScope(() =>
            {
                if (prevContext != null)
                {
                    _context.Value = prevContext;
                }
            });
        }

        public IDisposable CreateBypassScope()
        {
            var prevContext = _context.Value?.Clone();
            _context.Value = new RlsContextData
            {
                TenantId = null,
                IsBypass = true,
            };

            return new RlsContextScope(() =>
            {
                if (prevContext != null)
                {
                    _context.Value = prevContext;
                }
            });
        }

        private string? GetClaim(string claimType)
        {
            var claimPrincipal = _contextAccessor.HttpContext?.User;
            var claimValue = claimPrincipal?.FindFirst(claimType)?.Value;

            return claimValue;
        }
    }

    /// <summary>
    /// Row Level Securityのコンテキストデータ
    /// </summary>
    /// <returns></returns>
    public class RlsContextData
    {
        /// <summary>
        /// テナントID
        /// </summary>
        /// <value></value>
        public string? TenantId { get; set; }

        /// <summary>
        /// RLSをバイパスするかどうか?
        /// </summary>
        /// <value></value>
        public bool IsBypass { get; set; }

        public RlsContextData Clone()
        {
            return new RlsContextData
            {
                TenantId = TenantId,
                IsBypass = IsBypass,
            };
        }
    }

    public class RlsContextScope : IDisposable
    {
        private readonly Action _onDispose;

        public RlsContextScope(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            _onDispose();
        }
    }
}