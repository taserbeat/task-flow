using Application.Repositories;
using Application.Services;
using Domain.Entities.Auth;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Auth
{
    /// <summary>
    /// ログインのユースケース
    /// </summary>
    public class LoginUseCase
    {
        private readonly ILogger<LoginUseCase> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashService _passwordHashService;

        public LoginUseCase(ILogger<LoginUseCase> logger, IUserRepository userRepository, IPasswordHashService passwordHashService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
        }

        /// <summary>
        /// ログインを実行する
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="AppValidateException" />
        /// <returns></returns>
        public async Task<LoginResult> ExecuteAsync(LoginRequest request)
        {
            // ユーザーを取得
            var userEm = await _userRepository.GetForLoginAsync(request.Email);
            if (userEm is null)
            {
                throw new AppValidateException("メールアドレスまたはパスワードが間違っています。");
            }

            // パスワードを検証
            if (!_passwordHashService.VerifyPassword(request.Password, userEm.PasswordHash))
            {
                throw new AppValidateException("メールアドレスまたはパスワードが間違っています。");
            }

            var sessionId = SessionId.New();

            return new LoginResult(
                TenantId: userEm.TenantId,
                UserId: userEm.Id,
                Email: userEm.Email,
                RoleId: userEm.RoleId,
                RoleName: userEm.Role.Name,
                SessionId: sessionId
            );
        }
    }

    /// <summary>
    /// ログインの入力
    /// </summary>
    /// <param name="Email"></param>
    /// <param name="Password"></param>
    /// <returns></returns>
    public record LoginRequest(UserEmail Email, string Password);

    /// <summary>
    /// ログインの出力
    /// </summary>
    /// <param name="TenantId"></param>
    /// <param name="UserId"></param>
    /// <param name="Email"></param>
    /// <param name="RoleId"></param>
    /// <param name="RoleName"></param>
    /// <returns></returns>
    public record LoginResult(
        TenantId TenantId,
        UserId UserId,
        UserEmail Email,
        RoleId RoleId,
        RoleNameEnum RoleName,
        SessionId SessionId
    );
}