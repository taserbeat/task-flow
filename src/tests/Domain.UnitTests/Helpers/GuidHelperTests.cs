using Domain.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Domain.UnitTests.Helpers
{
    public class GuidHelperTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public GuidHelperTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact(DisplayName = "GUIDの生成に成功する")]
        public void Success()
        {
            // NOTE:
            // このテストはGuidの生成を行いたい場合に実行する。
            // 生成したGuidはITestOutputHelperの出力で確認する。
            for (int i = 0; i < 3; i++)
            {
                var guid = GuidHelper.NewGuid();
                _outputHelper.WriteLine($"guid: {guid}");

                /*** 検証 ***/
                Assert.True(Guid.TryParse(guid.ToString(), out var _));
            }
        }
    }
}