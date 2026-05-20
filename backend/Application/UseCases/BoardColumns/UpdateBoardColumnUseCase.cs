using Application.Repositories;
using Application.Services;
using Domain.Entities.BoardColumns;
using Domain.Entities.Boards;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;

namespace Application.UseCases.BoardColumns
{
    /// <summary>
    /// ボード列の更新ユースケース
    /// </summary>
    public class UpdateBoardColumnUseCase
    {
        private readonly TimeProvider _timeProvider;
        private readonly IBoardColumnRepository _boardColumnRepository;
        private readonly IUnitOfWork _uow;
        private readonly IBoardService _boardService;
        private readonly IExceptionService _exceptionService;

        public UpdateBoardColumnUseCase(TimeProvider timeProvider, IBoardColumnRepository boardColumnRepository, IUnitOfWork uow, IBoardService boardService, IExceptionService exceptionService)
        {
            _timeProvider = timeProvider;
            _boardColumnRepository = boardColumnRepository;
            _uow = uow;
            _boardService = boardService;
            _exceptionService = exceptionService;
        }

        public async Task ExecuteAsync(TenantId tenantId, UserId actorId, BoardId targetBoardId, BoardColumnId targetColumnId, UpdateBoardColumnParam param)
        {
            var targetColumnEm = await _boardColumnRepository.GetByIdAsync(tenantId, targetColumnId);
            if (targetColumnEm is null)
            {
                throw new AppNotFoundException("指定の列は存在しません。");
            }

            var now = _timeProvider.GetUtcNow();

            // ボード列名
            if (!string.IsNullOrWhiteSpace(param.Name))
            {
                var newName = new BoardColumnName(param.Name);
                targetColumnEm.ChangeName(newName, now, actorId);
            }

            // 位置
            if (param.PreviousColumnId != null || param.NextColumnId != null)
            {
                #region 位置変更の検証

                if (param.PreviousColumnId == param.NextColumnId)
                {
                    // prev == next は位置変更できない
                    throw new AppValidateException("前後の列が同じため位置を変更できません。");
                }

                if (targetColumnId.Value == param.PreviousColumnId)
                {
                    // 自分自身を前後にあると指定することはできない
                    throw new AppValidateException("自身の列を前にある列として指定することはできません。");
                }

                if (targetColumnId.Value == param.NextColumnId)
                {
                    // 自分自身を前後にあると指定することはできない
                    throw new AppValidateException("自身の列を後ろにある列として指定することはできません。");
                }

                var prevColumnId = BoardColumnId.New(param.PreviousColumnId);
                var nextColumnId = BoardColumnId.New(param.NextColumnId);

                // 前後の列情報を取得
                var prevColumnEm = param.PreviousColumnId is null ? null : await _boardColumnRepository.GetByIdAsync(tenantId, prevColumnId);
                var nextColumnEm = param.NextColumnId is null ? null : await _boardColumnRepository.GetByIdAsync(tenantId, nextColumnId);

                // 既に採番が必要と判明している場合は採番する
                if (prevColumnEm is not null && nextColumnEm is not null)
                {
                    var middlePosition = BoardColumnPosition.NewMiddlePosition(prevColumnEm.Position, nextColumnEm.Position);
                    if (prevColumnEm.Position == middlePosition || nextColumnEm.Position == middlePosition)
                    {
                        // 採番
                        await _boardService.RebalanceAsync(tenantId, targetBoardId);

                        // 採番後の列情報を再取得
                        prevColumnEm = await _boardColumnRepository.GetByIdAsync(tenantId, prevColumnId);
                        nextColumnEm = await _boardColumnRepository.GetByIdAsync(tenantId, nextColumnId);
                    }
                }

                if (prevColumnEm is null && nextColumnEm is null)
                {
                    // 両方存在しない列の場合は位置変更できない
                    throw new AppValidateException("前後の列が不明のため位置を変更できません。");
                }

                if (prevColumnEm is not null && prevColumnEm.BoardId != targetBoardId)
                {
                    // 同一ボードでないなら位置変更できない
                    throw new AppValidateException("前の列が異なるボードのため位置を変更できません。");
                }

                if (nextColumnEm is not null && nextColumnEm.BoardId != targetBoardId)
                {
                    // 同一ボードでないなら位置変更できない
                    throw new AppValidateException("後ろの列が異なるボードのため位置を変更できません。");
                }

                // 先頭位置に変更する場合、指定の列が先頭位置であったのかチェック
                if (prevColumnEm is null && nextColumnEm is not null)
                {
                    var firstPosition = await _boardColumnRepository.GetFirstPositionAsync(tenantId, nextColumnEm.BoardId);
                    if (firstPosition is null || nextColumnEm.Position != firstPosition)
                    {
                        // 先頭位置ではない列なので位置変更できない
                        throw new AppValidateException("先頭位置ではないため位置を変更できません。");
                    }
                }

                // 2つの列の間に位置変更する場合、前後2つが連続であるかをチェック (順序チェックも含む)
                if (prevColumnEm is not null && nextColumnEm is not null)
                {
                    var rangeCount = await _boardColumnRepository.CountPositionRangeAsync(tenantId, targetBoardId, prevColumnEm.Position, nextColumnEm.Position);
                    if (rangeCount != 2)
                    {
                        // 前後の列が非連続、または、順序が間違いなので位置変更できない
                        throw new AppValidateException("前後の列が連続していないため位置を変更できません。");
                    }
                }

                // 最終位置に変更する場合、指定の列が最終位置であったのかチェック
                if (prevColumnEm is not null && nextColumnEm is null)
                {
                    var lastPosition = await _boardColumnRepository.GetLastPositionAsync(tenantId, prevColumnEm.BoardId);
                    if (lastPosition is null || prevColumnEm.Position != lastPosition)
                    {
                        // 最終位置ではない列なので位置変更できない
                        throw new AppValidateException("最終位置ではないため位置を変更できません。");
                    }
                }

                #endregion

                #region 位置の更新

                ChangePosition(targetColumnEm, now, actorId, prevColumnEm, nextColumnEm);

                try
                {
                    await _uow.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    if (!_exceptionService.IsUniqueConstraintViolation(ex))
                    {
                        // ユニーク制約以外の例外はそのままスロー
                        throw;
                    }

                    // 採番してリトライ
                    await _boardService.RebalanceAsync(tenantId, targetBoardId);

                    prevColumnEm = param.PreviousColumnId is null ? null : await _boardColumnRepository.GetByIdAsync(tenantId, prevColumnId);
                    nextColumnEm = param.NextColumnId is null ? null : await _boardColumnRepository.GetByIdAsync(tenantId, nextColumnId);

                    ChangePosition(targetColumnEm, now, actorId, prevColumnEm, nextColumnEm);

                    await _uow.SaveChangesAsync();
                }

                #endregion
            }
        }

        private void ChangePosition(BoardColumnEm target, DateTimeOffset now, UserId actorId, BoardColumnEm? prev, BoardColumnEm? next)
        {
            // 先頭の位置に変更
            if (prev is null && next is not null)
            {
                var newPosition = next.Position.NewPreviousPosition();
                target.ChangePosition(newPosition, now, actorId);
            }

            // 中間の位置に変更
            if (prev is not null && next is not null)
            {
                var newPosition = BoardColumnPosition.NewMiddlePosition(prev.Position, next.Position);
                target.ChangePosition(newPosition, now, actorId);
            }

            // 末尾の位置に変更
            if (prev is not null && next is null)
            {
                var newPosition = prev.Position.NewNextPosition();
                target.ChangePosition(newPosition, now, actorId);
            }
        }
    }

    /// <summary>
    /// ボード列更新ユースケースのパラメータ
    /// </summary>
    /// <value></value>
    public record UpdateBoardColumnParam
    {
        /// <summary>
        /// ボード名
        /// </summary>
        /// <value></value>
        public string? Name { get; init; }

        /// <summary>
        /// 変更後の位置の1つ前のボード列ID (位置変更がある場合のみ)
        /// </summary>
        /// <value></value>
        public Guid? PreviousColumnId { get; init; }

        /// <summary>
        /// 変更後の位置の1つ後のボード列ID (位置変更がある場合のみ)
        /// </summary>
        /// <value></value>
        public Guid? NextColumnId { get; init; }
    }
}