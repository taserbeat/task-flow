using Domain.Entities.Tenants;
using Domain.Exceptions;
using Xunit;
using Xunit.Abstractions;

namespace Domain.UnitTests.Entities.Tenants
{
    /// <summary>
    /// <see cref="TenantName"/>のテスト
    /// </summary>
    public class TenantNameTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public TenantNameTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact(DisplayName = "テナント名の生成に成功する")]
        public void Constructor_Should_Success()
        {
            var tenantName = new TenantName("テナントA");
            Assert.Equal("テナントA", tenantName.Value);
        }

        [Theory(DisplayName = "テナント名の生成に失敗する (空文字または空白)")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("　")]
        [InlineData("　　")]
        public void Constructor_Should_Fail_When_Value_Is_Whitespace(string name)
        {
            _outputHelper.WriteLine($"入力値: {name}");
            Assert.Throws<AppValidateException>(() => new TenantName(name));
        }

        [Fact(DisplayName = "テナント名の生成に失敗する (最大文字数オーバー)")]
        public void Constructor_Should_Fail_When_Value_Exceeds_MaxLength()
        {
            var longName = new string('a', TenantName.MaxLength + 1);
            Assert.Throws<AppValidateException>(() => new TenantName(longName));
        }
    }
}