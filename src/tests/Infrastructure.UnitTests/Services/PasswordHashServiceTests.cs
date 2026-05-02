using Application.Services;
using Domain.Exceptions;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Infrastructure.UnitTests.Services
{
    /// <summary>
    /// <see cref="PasswordHashService"/> のテスト
    /// </summary>
    public class PasswordHashServiceTests
    {
        private readonly ServiceCollection _services;
        private readonly IConfigurationRoot _configuration;
        private readonly ITestOutputHelper _outputHelper;

        public PasswordHashServiceTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;

            _services = new();

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            _services.AddSingleton(_configuration);

            _services.AddLogging(options =>
            {
                options.ClearProviders();
            });

            _services.AddScoped<IPasswordHashService, PasswordHashService>();
        }

        [Theory(DisplayName = "パスワードハッシュの生成に成功する")]
        [InlineData("admin")]
        [InlineData("abc12345ABC67890!?")]
        public void Generate_And_Verify_Should_Success(string password)
        {
            // 準備
            using var provider = _services.BuildServiceProvider();
            var service = provider.GetRequiredService<IPasswordHashService>();

            // 実行
            var passwordhash = service.GenerateHash(password);
            _outputHelper.WriteLine($"password: {password}, passwordhash: {passwordhash.Value}");

            // 検証: ハッシュとの一致で相互に確認
            var isValid = service.VerifyPassword(password, passwordhash);
            Assert.True(isValid);

            // 検証: ハッシュ化すると平分と異なることを確認
            Assert.NotEqual(password, passwordhash.Value);
        }

        [Fact(DisplayName = "間違ったパスワードで検証に失敗する")]
        public void Verify_With_Wrong_Password_Should_Fail()
        {
            // 準備
            using var provider = _services.BuildServiceProvider();
            var service = provider.GetRequiredService<IPasswordHashService>();

            // 実行
            var passwordhash = service.GenerateHash("StrongP@ssw0rd");

            // 検証: 間違ったパスワードを与えて、検証に失敗することを確認
            var isValid = service.VerifyPassword("WrongP@ssw0rd", passwordhash);
            Assert.False(isValid);
        }

        [Fact(DisplayName = "パスワードが同じでも毎回異なるハッシュが生成される")]
        public void GenerateHash_Should_Be_Different_Each_Time()
        {
            // 準備
            using var provider = _services.BuildServiceProvider();
            var service = provider.GetRequiredService<IPasswordHashService>();
            const string password = "StrongP@ssw0rd";

            // 実行
            var passwordHashs = new List<string>();
            for (var i = 0; i < 3; i++)
            {
                var passwordHash = service.GenerateHash(password);
                passwordHashs.Add(passwordHash.Value);
            }

            // 検証: 重複を取り除いた要素数から、毎回異なるハッシュが生成されていることを確認
            Assert.True(passwordHashs.Count == passwordHashs.Distinct().Count());
        }

        [Theory(DisplayName = "パスワードが空文字の場合、ハッシュ生成に失敗する")]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("　")]
        [InlineData("　　")]
        [InlineData(" 　")]
        public void GenerateHash_With_Empty_Password_Should_Be_Fail(string password)
        {
            // 準備
            using var provider = _services.BuildServiceProvider();
            var service = provider.GetRequiredService<IPasswordHashService>();

            // 実行 & 検証
            Assert.Throws<AppValidateException>(() => service.GenerateHash(password));
        }
    }
}