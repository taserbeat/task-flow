using Domain.Entities.Users;
using Domain.Exceptions;
using Xunit;
using Xunit.Abstractions;

namespace Domain.UnitTests.Entities.Users
{
    /// <summary>
    /// <see cref="UserName"/>のテスト
    /// </summary>
    public class UserNameTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public UserNameTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact(DisplayName = "氏名の生成に成功する")]
        public void Constructor_Should_Success()
        {
            var name = new UserName("田中", "太郎");
            Assert.Equal("田中", name.LastName);
            Assert.Equal("太郎", name.FirstName);
            Assert.Equal("田中 太郎", name.FullName);
        }

        [Theory(DisplayName = "氏名の生成に成功する (空文字または空白)")]
        [InlineData("", "太郎")]
        [InlineData(" ", "太郎")]
        [InlineData("田中", "")]
        [InlineData("田中", " ")]
        [InlineData("", "")]
        public void Constructor_Should_Success_When_EmptyOrWhitespace(string lastName, string firstName)
        {
            _outputHelper.WriteLine($"入力値: {lastName}, {firstName}");
            var name = new UserName(lastName, firstName);
            Assert.Equal(lastName, name.LastName);
            Assert.Equal(firstName, name.FirstName);
        }

        [Fact(DisplayName = "氏名の生成に失敗する (姓の最大文字数オーバー)")]
        public void Constructor_Should_Failed_When_Exceeds_MaxLastNameLength()
        {
            var longLastName = new string('a', UserName.MaxLastNameLength + 1);
            Assert.Throws<AppValidateException>(() => new UserName(longLastName, "太郎"));
        }

        [Fact(DisplayName = "氏名の生成に失敗する (名の最大文字数オーバー)")]
        public void Constructor_Should_Failed_When_Exceeds_MaxFirstNameLength()
        {
            var longFirstName = new string('a', UserName.MaxFirstNameLength + 1);
            Assert.Throws<AppValidateException>(() => new UserName("田中", longFirstName));
        }
    }
}