using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities.BoardColumns;
using Domain.Entities.Boards;
using Domain.Entities.TaskItems;
using Microsoft.EntityFrameworkCore;
using Web.Dtos.BoardColumns.CreateBoardColumn;
using Web.Dtos.BoardColumns.UpdateBoardColumn;
using Web.Dtos.Boards.CreateBoard;
using Web.Dtos.Boards.GetBoard;
using Web.Dtos.Boards.UpdateBoard;
using Web.Dtos.TaskItems.CreateTaskItem;
using Web.Dtos.TaskItems.UpdateTaskItem;
using Web.IntegrationTests.Collections;
using Web.IntegrationTests.Environments;
using Web.IntegrationTests.Extensions;
using Web.IntegrationTests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Web.IntegrationTests.Controllers
{
    /// <summary>
    /// BoardControllerのインテグレーションテスト
    /// </summary>
    [Collection(CollectionDefinitionNames.Integration)]
    public class BoardControllerTests
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly ITestOutputHelper _outputHelper;
        private readonly TestEnvironment _env;

        public BoardControllerTests(ITestOutputHelper outputHelper, TestEnvironment env)
        {
            _outputHelper = outputHelper;
            _env = env;
        }

        #region ボード

        [Fact(DisplayName = "管理者権限でボードを作成できる(200)")]
        public async Task CreateBoard_AsAdmin_ReturnsOk()
        {
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForAdminUser();
            var request = new CreateBoardRequest
            {
                Name = "プロジェクトボード",
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async client =>
            {
                var response = await client.PostAsJsonAsync("/api/boards", request);

                response.IsOk();
            });

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            var createdBoards = await dbContext.Boards.ToListAsync();
            Assert.Contains(createdBoards, b => b.Name.Value == request.Name);
        }

        [Fact(DisplayName = "一般ユーザー権限はボードを作成できない(403)")]
        public async Task CreateBoard_AsGeneralUser_ReturnsForbidden()
        {
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForSampleUser();
            var request = new CreateBoardRequest
            {
                Name = "プロジェクトボード",
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async client =>
            {
                var response = await client.PostAsJsonAsync("/api/boards", request);

                response.IsForbidden();
            });
        }

        [Fact(DisplayName = "認証ユーザーはボード一覧を取得できる(200)")]
        public async Task GetBoards_ReturnsBoardList()
        {
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var now = _env.Factory.TimeProvider.GetUtcNow();
            var boardEms = new[]
            {
                BoardEm.Create(BoardId.New(), _env.DefaultTenant.Id, new BoardName("ボードA"), now, now, _env.AdminUser.Id, _env.AdminUser.Id),
                BoardEm.Create(BoardId.New(), _env.DefaultTenant.Id, new BoardName("ボードB"), now, now, _env.AdminUser.Id, _env.AdminUser.Id),
            };

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            await dbContext.Boards.AddRangeAsync(boardEms);
            await dbContext.SaveChangesAsync();

            var claims = _env.GetClaimsForSampleUser();

            await _env.Factory.RunWithAuthenticationAsync(claims, async client =>
            {
                var response = await client.GetAsync("/api/boards");

                response.IsOk();

                var responseData = await response.Content.ReadFromJsonAsync<List<BoardSummaryResponse>>();
                Assert.NotNull(responseData);
                Assert.Equal(2, responseData!.Count);
                Assert.Contains(responseData, x => x.Name == "ボードA");
                Assert.Contains(responseData, x => x.Name == "ボードB");
            });
        }

        [Fact(DisplayName = "ボードの詳細を取得できる(200)")]
        public async Task GetBoard_ReturnsBoardDetail()
        {
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var now = _env.Factory.TimeProvider.GetUtcNow();
            var boardId = BoardId.New();
            var boardEm = BoardEm.Create(boardId, _env.DefaultTenant.Id, new BoardName("詳細ボード"), now, now, _env.AdminUser.Id, _env.AdminUser.Id);
            var todoColumnId = BoardColumnId.New();
            var doneColumnId = BoardColumnId.New();
            var columnEms = new[]
            {
                BoardColumnEm.Create(todoColumnId, _env.DefaultTenant.Id, boardId, new BoardColumnName("ToDo"), BoardColumnPosition.NewInitPosition(), now, now, _env.AdminUser.Id, _env.AdminUser.Id),
                BoardColumnEm.Create(doneColumnId, _env.DefaultTenant.Id, boardId, new BoardColumnName("Done"), new BoardColumnPosition(200), now, now, _env.AdminUser.Id, _env.AdminUser.Id),
            };

            var taskEms = new[]
            {
                TaskItemEm.Create(TaskItemId.New(), _env.DefaultTenant.Id, todoColumnId, _env.SampleUser.Id, new TaskItemTitle("タスク1"), new TaskItemDescription("説明1"), TaskItemPriorityEnum.Low, now.AddDays(1), TaskItemPosition.NewInitPosition(), now, now, _env.AdminUser.Id, _env.AdminUser.Id),
                TaskItemEm.Create(TaskItemId.New(), _env.DefaultTenant.Id, doneColumnId, _env.SampleUser.Id, new TaskItemTitle("タスク2"), new TaskItemDescription("説明2"), TaskItemPriorityEnum.High, now.AddDays(2), new TaskItemPosition(200), now, now, _env.AdminUser.Id, _env.AdminUser.Id),
            };

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            await dbContext.Boards.AddAsync(boardEm);
            await dbContext.BoardColumns.AddRangeAsync(columnEms);
            await dbContext.TaskItems.AddRangeAsync(taskEms);
            await dbContext.SaveChangesAsync();

            var claims = _env.GetClaimsForSampleUser();

            await _env.Factory.RunWithAuthenticationAsync(claims, async client =>
            {
                var response = await client.GetAsync($"/api/boards/{boardId.Value}");

                response.IsOk();

                var responseData = await response.Content.ReadFromJsonAsync<BoardDetailResponse>(JsonOptions);
                Assert.NotNull(responseData);
                Assert.Equal(boardId.Value, responseData!.Id);
                Assert.Equal(boardEm.Name.Value, responseData.Name);
                Assert.Equal(2, responseData.Columns.Count());
                Assert.Contains(responseData.Columns, c => c.Name == "ToDo");
                Assert.Contains(responseData.Columns, c => c.Name == "Done");

                var todoColumn = responseData.Columns.Single(c => c.Name == "ToDo");
                var doneColumn = responseData.Columns.Single(c => c.Name == "Done");
                Assert.Single(todoColumn.TaskItems);
                Assert.Equal("タスク1", todoColumn.TaskItems.Single().Title);
                Assert.Single(doneColumn.TaskItems);
                Assert.Equal("タスク2", doneColumn.TaskItems.Single().Title);
            });
        }

        [Fact(DisplayName = "認証ユーザーはボードを更新できる(200)")]
        public async Task UpdateBoard_ReturnsOk()
        {
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var now = _env.Factory.TimeProvider.GetUtcNow();
            var boardId = BoardId.New();
            var boardEm = BoardEm.Create(boardId, _env.DefaultTenant.Id, new BoardName("旧ボード"), now, now, _env.AdminUser.Id, _env.AdminUser.Id);

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            await dbContext.Boards.AddAsync(boardEm);
            await dbContext.SaveChangesAsync();

            var claims = _env.GetClaimsForSampleUser();
            var request = new UpdateBoardRequest
            {
                Name = "更新後ボード",
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async client =>
            {
                var response = await client.PutAsJsonAsync($"/api/boards/{boardId.Value}", request);

                response.IsOk();
            });

            await using var verifyContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            var updatedBoard = await verifyContext.Boards.FindAsync(boardId);
            Assert.NotNull(updatedBoard);
            Assert.Equal(request.Name, updatedBoard!.Name.Value);
        }

        [Fact(DisplayName = "管理者権限でボードを削除できる(200)")]
        public async Task DeleteBoard_AsAdmin_ReturnsOk()
        {
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var now = _env.Factory.TimeProvider.GetUtcNow();
            var boardId = BoardId.New();
            var boardEm = BoardEm.Create(boardId, _env.DefaultTenant.Id, new BoardName("削除ボード"), now, now, _env.AdminUser.Id, _env.AdminUser.Id);

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            await dbContext.Boards.AddAsync(boardEm);
            await dbContext.SaveChangesAsync();

            var claims = _env.GetClaimsForAdminUser();

            await _env.Factory.RunWithAuthenticationAsync(claims, async client =>
            {
                var response = await client.DeleteAsync($"/api/boards/{boardId.Value}");

                response.IsOk();
            });

            await using var verifyContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            var deletedBoard = await verifyContext.Boards.FindAsync(boardId);
            Assert.Null(deletedBoard);
        }

        #endregion

        #region ボード列

        [Fact(DisplayName = "認証ユーザーはボード列を作成できる(200)")]
        public async Task CreateBoardColumn_ReturnsOk()
        {
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var now = _env.Factory.TimeProvider.GetUtcNow();
            var boardId = BoardId.New();
            var boardEm = BoardEm.Create(boardId, _env.DefaultTenant.Id, new BoardName("列テストボード"), now, now, _env.AdminUser.Id, _env.AdminUser.Id);

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            await dbContext.Boards.AddAsync(boardEm);
            await dbContext.SaveChangesAsync();

            var claims = _env.GetClaimsForSampleUser();
            var request = new CreateBoardColumnRequest
            {
                Name = "ToDo",
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async client =>
            {
                var response = await client.PostAsJsonAsync($"/api/boards/{boardId.Value}/columns", request);

                response.IsOk();
            });

            var createdColumns = await dbContext.BoardColumns.ToListAsync();
            Assert.Contains(createdColumns, c => c.BoardId == boardId && c.Name.Value == request.Name);
        }

        [Fact(DisplayName = "認証ユーザーはボード列を更新できる(200)")]
        public async Task UpdateBoardColumn_ReturnsOk()
        {
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var now = _env.Factory.TimeProvider.GetUtcNow();
            var boardId = BoardId.New();
            var boardEm = BoardEm.Create(boardId, _env.DefaultTenant.Id, new BoardName("列更新ボード"), now, now, _env.AdminUser.Id, _env.AdminUser.Id);
            var columnId = BoardColumnId.New();
            var columnEm = BoardColumnEm.Create(columnId, _env.DefaultTenant.Id, boardId, new BoardColumnName("ToDo"), BoardColumnPosition.NewInitPosition(), now, now, _env.AdminUser.Id, _env.AdminUser.Id);

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            await dbContext.Boards.AddAsync(boardEm);
            await dbContext.BoardColumns.AddAsync(columnEm);
            await dbContext.SaveChangesAsync();

            var claims = _env.GetClaimsForSampleUser();
            var request = new UpdateBoardColumnRequest
            {
                Name = "進行中",
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async client =>
            {
                var response = await client.PutAsJsonAsync($"/api/boards/{boardId.Value}/columns/{columnId.Value}", request);

                response.IsOk();
            });

            await using var verifyContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            var updatedColumn = await verifyContext.BoardColumns.FindAsync(columnId);
            Assert.NotNull(updatedColumn);
            Assert.Equal(request.Name, updatedColumn!.Name.Value);
        }

        [Fact(DisplayName = "認証ユーザーはボード列を削除できる(200)")]
        public async Task DeleteBoardColumn_ReturnsOk()
        {
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var now = _env.Factory.TimeProvider.GetUtcNow();
            var boardId = BoardId.New();
            var boardEm = BoardEm.Create(boardId, _env.DefaultTenant.Id, new BoardName("列削除ボード"), now, now, _env.AdminUser.Id, _env.AdminUser.Id);
            var columnId = BoardColumnId.New();
            var columnEm = BoardColumnEm.Create(columnId, _env.DefaultTenant.Id, boardId, new BoardColumnName("ToDo"), BoardColumnPosition.NewInitPosition(), now, now, _env.AdminUser.Id, _env.AdminUser.Id);

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            await dbContext.Boards.AddAsync(boardEm);
            await dbContext.BoardColumns.AddAsync(columnEm);
            await dbContext.SaveChangesAsync();

            var claims = _env.GetClaimsForSampleUser();

            await _env.Factory.RunWithAuthenticationAsync(claims, async client =>
            {
                var response = await client.DeleteAsync($"/api/boards/{boardId.Value}/columns/{columnId.Value}");

                response.IsOk();
            });

            await using var verifyContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            var deletedColumn = await verifyContext.BoardColumns.FindAsync(columnId);
            Assert.Null(deletedColumn);
        }

        #endregion

        #region タスク

        [Fact(DisplayName = "認証ユーザーはタスクを作成できる(200)")]
        public async Task CreateTaskItem_ReturnsOk()
        {
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var now = _env.Factory.TimeProvider.GetUtcNow();
            var boardId = BoardId.New();
            var boardEm = BoardEm.Create(boardId, _env.DefaultTenant.Id, new BoardName("タスク作成ボード"), now, now, _env.AdminUser.Id, _env.AdminUser.Id);
            var columnId = BoardColumnId.New();
            var columnEm = BoardColumnEm.Create(columnId, _env.DefaultTenant.Id, boardId, new BoardColumnName("ToDo"), BoardColumnPosition.NewInitPosition(), now, now, _env.AdminUser.Id, _env.AdminUser.Id);

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            await dbContext.Boards.AddAsync(boardEm);
            await dbContext.BoardColumns.AddAsync(columnEm);
            await dbContext.SaveChangesAsync();

            var claims = _env.GetClaimsForSampleUser();
            var request = new CreateTaskItemRequest
            {
                AssigneeId = _env.SampleUser.Id.Value,
                Title = "タスク1",
                Description = "説明1",
                Priority = TaskItemPriorityEnum.Medium,
                DueDate = now.AddDays(7),
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async client =>
            {
                var response = await client.PostAsJsonAsync($"/api/boards/{boardId.Value}/columns/{columnId.Value}/tasks", request);

                response.IsOk();
            });

            var createdTasks = await dbContext.TaskItems.ToListAsync();
            Assert.Contains(createdTasks, t => t.BoardColumnId == columnId && t.Title.Value == request.Title && t.AssigneeId == _env.SampleUser.Id && t.Priority == request.Priority);
        }

        [Fact(DisplayName = "認証ユーザーはタスクを更新できる(200)")]
        public async Task UpdateTaskItem_ReturnsOk()
        {
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var now = _env.Factory.TimeProvider.GetUtcNow();
            var boardId = BoardId.New();
            var boardEm = BoardEm.Create(boardId, _env.DefaultTenant.Id, new BoardName("タスク更新ボード"), now, now, _env.AdminUser.Id, _env.AdminUser.Id);
            var columnId = BoardColumnId.New();
            var columnEm = BoardColumnEm.Create(columnId, _env.DefaultTenant.Id, boardId, new BoardColumnName("ToDo"), BoardColumnPosition.NewInitPosition(), now, now, _env.AdminUser.Id, _env.AdminUser.Id);
            var taskId = TaskItemId.New();
            var taskEm = TaskItemEm.Create(taskId, _env.DefaultTenant.Id, columnId, _env.SampleUser.Id, new TaskItemTitle("旧タスク"), new TaskItemDescription("旧説明"), TaskItemPriorityEnum.Low, now.AddDays(1), TaskItemPosition.NewInitPosition(), now, now, _env.AdminUser.Id, _env.AdminUser.Id);

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            await dbContext.Boards.AddAsync(boardEm);
            await dbContext.BoardColumns.AddAsync(columnEm);
            await dbContext.TaskItems.AddAsync(taskEm);
            await dbContext.SaveChangesAsync();

            var claims = _env.GetClaimsForSampleUser();
            var request = new UpdateTaskItemRequest
            {
                Title = "更新後タスク",
                Description = "更新後説明",
                Priority = TaskItemPriorityEnum.High,
                DueDate = now.AddDays(3),
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async client =>
            {
                var response = await client.PutAsJsonAsync($"/api/boards/{boardId.Value}/columns/{columnId.Value}/tasks/{taskId.Value}", request);

                response.IsOk();
            });

            await using var verifyContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            var updatedTask = await verifyContext.TaskItems.FindAsync(taskId);
            Assert.NotNull(updatedTask);
            Assert.Equal(request.Title, updatedTask!.Title.Value);
            Assert.Equal(request.Description, updatedTask.Description.Value);
            Assert.Equal(request.Priority, updatedTask.Priority);
            Assert.Equal(request.DueDate, updatedTask.DueDate);
        }

        [Fact(DisplayName = "認証ユーザーはタスクの位置を変更できる(200)")]
        public async Task UpdateTaskItem_PositionChange_ReturnsOk()
        {
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var now = _env.Factory.TimeProvider.GetUtcNow();
            var boardId = BoardId.New();
            var boardEm = BoardEm.Create(boardId, _env.DefaultTenant.Id, new BoardName("タスク移動ボード"), now, now, _env.AdminUser.Id, _env.AdminUser.Id);
            var columnId = BoardColumnId.New();
            var columnEm = BoardColumnEm.Create(columnId, _env.DefaultTenant.Id, boardId, new BoardColumnName("ToDo"), BoardColumnPosition.NewInitPosition(), now, now, _env.AdminUser.Id, _env.AdminUser.Id);
            var firstTaskId = TaskItemId.New();
            var secondTaskId = TaskItemId.New();
            var thirdTaskId = TaskItemId.New();
            var firstTask = TaskItemEm.Create(firstTaskId, _env.DefaultTenant.Id, columnId, _env.SampleUser.Id, new TaskItemTitle("タスクA"), new TaskItemDescription("説明A"), TaskItemPriorityEnum.Low, now.AddDays(1), TaskItemPosition.NewInitPosition(), now, now, _env.AdminUser.Id, _env.AdminUser.Id);
            var secondTask = TaskItemEm.Create(secondTaskId, _env.DefaultTenant.Id, columnId, _env.SampleUser.Id, new TaskItemTitle("タスクB"), new TaskItemDescription("説明B"), TaskItemPriorityEnum.Low, now.AddDays(2), new TaskItemPosition(200), now, now, _env.AdminUser.Id, _env.AdminUser.Id);
            var thirdTask = TaskItemEm.Create(thirdTaskId, _env.DefaultTenant.Id, columnId, _env.SampleUser.Id, new TaskItemTitle("タスクC"), new TaskItemDescription("説明C"), TaskItemPriorityEnum.Low, now.AddDays(3), new TaskItemPosition(300), now, now, _env.AdminUser.Id, _env.AdminUser.Id);

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            await dbContext.Boards.AddAsync(boardEm);
            await dbContext.BoardColumns.AddAsync(columnEm);
            await dbContext.TaskItems.AddRangeAsync(firstTask, secondTask, thirdTask);
            await dbContext.SaveChangesAsync();

            var claims = _env.GetClaimsForSampleUser();
            var request = new UpdateTaskItemRequest
            {
                PreviousTaskItemId = firstTaskId.Value,
                NextTaskItemId = secondTaskId.Value,
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async client =>
            {
                var response = await client.PutAsJsonAsync($"/api/boards/{boardId.Value}/columns/{columnId.Value}/tasks/{thirdTaskId.Value}", request);

                response.IsOk();
            });

            await using var verifyContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            var updatedTask = await verifyContext.TaskItems.FindAsync(thirdTaskId);
            Assert.NotNull(updatedTask);
            Assert.Equal(150, updatedTask!.Position.Value);
        }

        [Fact(DisplayName = "認証ユーザーはタスクの期限日を削除できる(200)")]
        public async Task UpdateTaskItem_DeleteDueDate_ReturnsOk()
        {
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var now = _env.Factory.TimeProvider.GetUtcNow();
            var boardId = BoardId.New();
            var boardEm = BoardEm.Create(boardId, _env.DefaultTenant.Id, new BoardName("期限削除ボード"), now, now, _env.AdminUser.Id, _env.AdminUser.Id);
            var columnId = BoardColumnId.New();
            var columnEm = BoardColumnEm.Create(columnId, _env.DefaultTenant.Id, boardId, new BoardColumnName("ToDo"), BoardColumnPosition.NewInitPosition(), now, now, _env.AdminUser.Id, _env.AdminUser.Id);
            var taskId = TaskItemId.New();
            var taskEm = TaskItemEm.Create(taskId, _env.DefaultTenant.Id, columnId, _env.SampleUser.Id, new TaskItemTitle("タスクD"), new TaskItemDescription("説明D"), TaskItemPriorityEnum.Medium, now.AddDays(5), TaskItemPosition.NewInitPosition(), now, now, _env.AdminUser.Id, _env.AdminUser.Id);

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            await dbContext.Boards.AddAsync(boardEm);
            await dbContext.BoardColumns.AddAsync(columnEm);
            await dbContext.TaskItems.AddAsync(taskEm);
            await dbContext.SaveChangesAsync();

            var claims = _env.GetClaimsForSampleUser();
            var request = new UpdateTaskItemRequest
            {
                IsDeleteDueDate = true,
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async client =>
            {
                var response = await client.PutAsJsonAsync($"/api/boards/{boardId.Value}/columns/{columnId.Value}/tasks/{taskId.Value}", request);

                response.IsOk();
            });

            await using var verifyContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            var updatedTask = await verifyContext.TaskItems.FindAsync(taskId);
            Assert.NotNull(updatedTask);
            Assert.Null(updatedTask!.DueDate);
        }

        [Fact(DisplayName = "認証ユーザーはタスクを削除できる(200)")]
        public async Task DeleteTaskItem_ReturnsOk()
        {
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var now = _env.Factory.TimeProvider.GetUtcNow();
            var boardId = BoardId.New();
            var boardEm = BoardEm.Create(boardId, _env.DefaultTenant.Id, new BoardName("タスク削除ボード"), now, now, _env.AdminUser.Id, _env.AdminUser.Id);
            var columnId = BoardColumnId.New();
            var columnEm = BoardColumnEm.Create(columnId, _env.DefaultTenant.Id, boardId, new BoardColumnName("ToDo"), BoardColumnPosition.NewInitPosition(), now, now, _env.AdminUser.Id, _env.AdminUser.Id);
            var taskId = TaskItemId.New();
            var taskEm = TaskItemEm.Create(taskId, _env.DefaultTenant.Id, columnId, _env.SampleUser.Id, new TaskItemTitle("削除タスク"), new TaskItemDescription("削除説明"), TaskItemPriorityEnum.Low, now.AddDays(1), TaskItemPosition.NewInitPosition(), now, now, _env.AdminUser.Id, _env.AdminUser.Id);

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            await dbContext.Boards.AddAsync(boardEm);
            await dbContext.BoardColumns.AddAsync(columnEm);
            await dbContext.TaskItems.AddAsync(taskEm);
            await dbContext.SaveChangesAsync();

            var claims = _env.GetClaimsForSampleUser();

            await _env.Factory.RunWithAuthenticationAsync(claims, async client =>
            {
                var response = await client.DeleteAsync($"/api/boards/{boardId.Value}/columns/{columnId.Value}/tasks/{taskId.Value}");

                response.IsOk();
            });

            await using var verifyContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            var deletedTask = await verifyContext.TaskItems.FindAsync(taskId);
            Assert.Null(deletedTask);
        }

        #endregion
    }
}
