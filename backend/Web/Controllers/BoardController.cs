using Application.Contexts;
using Application.UseCases.BoardColumns;
using Application.UseCases.Boards;
using Domain.Entities.BoardColumns;
using Domain.Entities.Boards;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Common.Constants;
using Web.Dtos.BoardColumns.CreateBoardColumn;
using Web.Dtos.BoardColumns.UpdateBoardColumn;
using Web.Dtos.Boards.CreateBoard;
using Web.Dtos.Boards.GetBoard;
using Web.Dtos.Boards.UpdateBoard;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/boards")]
    [Tags("ボード")]
    public class BoardController : ControllerBase
    {
        private readonly IUserContext _userContext;

        private readonly CreateBoardUseCase _createBoardUseCase;
        private readonly GetBoardsUseCase _getBoardsUseCase;
        private readonly GetBoardUseCase _getBoardUseCase;
        private readonly UpdateBoardUseCase _updateBoardUseCase;
        private readonly DeleteBoardUseCase _deleteBoardUseCase;

        private readonly CreateBoardColumnUseCase _createBoardColumnUseCase;
        private readonly UpdateBoardColumnUseCase _updateBoardColumnUseCase;
        private readonly DeleteBoardColumnUseCase _deleteBoardColumnUseCase;

        public BoardController(IUserContext userContext, CreateBoardUseCase createBoardUseCase, GetBoardsUseCase getBoardsUseCase, GetBoardUseCase getBoardUseCase, UpdateBoardUseCase updateBoardUseCase, DeleteBoardUseCase deleteBoardUseCase, CreateBoardColumnUseCase createBoardColumnUseCase, UpdateBoardColumnUseCase updateBoardColumnUseCase, DeleteBoardColumnUseCase deleteBoardColumnUseCase)
        {
            _userContext = userContext;
            _createBoardUseCase = createBoardUseCase;
            _getBoardsUseCase = getBoardsUseCase;
            _getBoardUseCase = getBoardUseCase;
            _updateBoardUseCase = updateBoardUseCase;
            _deleteBoardUseCase = deleteBoardUseCase;
            _createBoardColumnUseCase = createBoardColumnUseCase;
            _updateBoardColumnUseCase = updateBoardColumnUseCase;
            _deleteBoardColumnUseCase = deleteBoardColumnUseCase;
        }

        #region ボード

        /// <summary>
        /// ボードの作成
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">リクエストが不正</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="403">権限エラー</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpPost]
        [Route("")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizePolicyNames.RequireAdmin)]
        public async Task<IActionResult> CreateBoard([FromBody] CreateBoardRequest request)
        {
            var param = new CreateBoardParam
            {
                Name = request.Name,
            };

            await _createBoardUseCase.ExecuteAsync(_userContext.TenantId, _userContext.UserId, param);

            return Ok();
        }

        /// <summary>
        /// ボード一覧の取得
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpGet]
        [Route("")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<BoardSummaryResponse>>> GetBoards()
        {
            var boardEms = await _getBoardsUseCase.ExecuteAsync(_userContext.TenantId);
            var boards = boardEms.Select(x => BoardSummaryResponse.FromEntity(x));

            return Ok(boards);
        }

        /// <summary>
        /// ボードの取得
        /// </summary>
        /// <param name="boardId">取得対象のボードID</param>
        /// <returns></returns>
        /// <response code="401">未認証エラー</response>
        /// <response code="404">存在しない</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpGet]
        [Route("{boardId}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult<BoardDetailResponse>> GetBoard([FromRoute] Guid boardId)
        {
            var boardEm = await _getBoardUseCase.ExecuteAsync(_userContext.TenantId, BoardId.New(boardId));
            if (boardEm is null)
            {
                return NotFound();
            }

            return BoardDetailResponse.FromEntity(boardEm);
        }

        /// <summary>
        /// ボードの更新
        /// </summary>
        /// <param name="boardId">更新対象のボードID</param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">リクエストが不正</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="403">権限エラー</response>
        /// <response code="404">存在しない</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpPut]
        [Route("{boardId}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizePolicyNames.RequireAdmin)]
        public async Task<IActionResult> UpdateBoard([FromRoute] Guid boardId, UpdateBoardRequest request)
        {
            var param = new UpdateBoardParam
            {
                Name = request.Name,
            };

            await _updateBoardUseCase.ExecuteAsync(_userContext.TenantId, _userContext.UserId, BoardId.New(boardId), param);

            return Ok();
        }

        /// <summary>
        /// ボードの削除
        /// </summary>
        /// <param name="boardId">削除対象のボードID</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">リクエストが不正</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="403">権限エラー</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpDelete]
        [Route("{boardId}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizePolicyNames.RequireAdmin)]
        public async Task<IActionResult> DeleteBoard([FromRoute] Guid boardId)
        {
            await _deleteBoardUseCase.ExecuteAsync(_userContext.TenantId, BoardId.New(boardId));

            return Ok();
        }

        #endregion

        #region ボード列

        /// <summary>
        /// ボード列の作成
        /// </summary>
        /// <param name="boardId">ボードID</param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">リクエストが不正</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="403">権限エラー</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpPost]
        [Route("{boardId}/columns")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateBoardColumn([FromRoute] Guid boardId, CreateBoardColumnRequest request)
        {
            var param = new CreateBoardColumnParam
            {
                BoardId = boardId,
                Name = request.Name,
            };

            await _createBoardColumnUseCase.ExecuteAsync(_userContext.TenantId, _userContext.UserId, param);

            return Ok();
        }

        /// <summary>
        /// ボード列の更新
        /// </summary>
        /// <param name="boardId">ボードID</param>
        /// <param name="columnId">更新対象のボード列ID</param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">リクエストが不正</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="403">権限エラー</response>
        /// <response code="404">存在しない</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpPut]
        [Route("{boardId}/columns/{columnId}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateBoardColumn([FromRoute] Guid boardId, [FromRoute] Guid columnId, UpdateBoardColumnRequest request)
        {
            var param = new UpdateBoardColumnParam
            {
                Name = request.Name,
                PreviousColumnId = request.PreviousColumnId,
                NextColumnId = request.NextColumnId,
            };

            await _updateBoardColumnUseCase.ExecuteAsync(_userContext.TenantId, _userContext.UserId, BoardId.New(boardId), BoardColumnId.New(columnId), param);

            return Ok();
        }

        /// <summary>
        /// ボード列の削除
        /// </summary>
        /// <param name="boardId">ボードID</param>
        /// <param name="columnId">削除対象のボード列ID</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">リクエストが不正</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="403">権限エラー</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpDelete]
        [Route("{boardId}/columns/{columnId}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteBoardColumn([FromRoute] Guid boardId, [FromRoute] Guid columnId)
        {
            await _deleteBoardColumnUseCase.ExecuteAsync(_userContext.TenantId, BoardId.New(boardId), BoardColumnId.New(columnId));

            return Ok();
        }

        #endregion
    }
}