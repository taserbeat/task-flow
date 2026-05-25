using Domain.Entities.BoardColumns;
using Xunit;

namespace Domain.UnitTests.Entities.BoardColumns
{
    /// <summary>
    /// <see cref="BoardColumnPosition"/>のテスト
    /// </summary>
    public class BoardColumnPositionTests
    {
        [Fact(DisplayName = "位置の生成に成功する")]
        public void Constructor_Should_Success()
        {
            var position = new BoardColumnPosition(100);
            Assert.Equal(100, position.Value);
        }

        [Fact(DisplayName = "比較演算子が正しく動作する")]
        public void ComparisonOperators_Should_Work()
        {
            var pos1 = new BoardColumnPosition(100);
            var pos2 = new BoardColumnPosition(200);

            Assert.True(pos1 < pos2);
            Assert.True(pos2 > pos1);
            Assert.True(pos1 <= pos2);
            Assert.True(pos2 >= pos1);
        }

        [Fact(DisplayName = "次の位置を取得できる")]
        public void NewNextPosition_Should_Return_Next()
        {
            var position = new BoardColumnPosition(100);
            var next = position.NewNextPosition();
            Assert.Equal(200, next.Value);
        }

        [Fact(DisplayName = "前の位置を取得できる")]
        public void NewPreviousPosition_Should_Return_Previous()
        {
            var position = new BoardColumnPosition(100);
            var previous = position.NewPreviousPosition();
            Assert.Equal(50, previous.Value);
        }

        [Fact(DisplayName = "中間の位置を取得できる")]
        public void NewMiddlePosition_Should_Return_Middle()
        {
            var low = new BoardColumnPosition(100);
            var high = new BoardColumnPosition(200);
            var middle = BoardColumnPosition.NewMiddlePosition(low, high);
            Assert.Equal(150, middle.Value);
        }

        [Fact(DisplayName = "初期位置を取得できる")]
        public void NewInitPosition_Should_Return_Init()
        {
            var init = BoardColumnPosition.NewInitPosition();
            Assert.Equal(100, init.Value);
        }
    }
}
