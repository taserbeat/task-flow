import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import type {
  BoardDetailResponse,
  CreateBoardColumnRequest,
  UpdateBoardColumnRequest,
  CreateTaskItemRequest,
  UpdateTaskItemRequest,
} from "../../../api/boards/boardsDtos";
import { apiClient } from "../../../api/clients/ApiClient";
import type { UserSummary } from "../../../models/users/UserSummary";

type Column = BoardDetailResponse["columns"][0];
type Task = Column["taskItems"][0];

/** ボードの編集ページ */
const BoardEditPage = () => {
  const { boardId } = useParams<{ boardId: string }>();
  const [board, setBoard] = useState<BoardDetailResponse | null>(null);
  const [editingBoardName, setEditingBoardName] = useState(false);
  const [boardName, setBoardName] = useState("");
  const [editingColumnId, setEditingColumnId] = useState<string | null>(null);
  const [columnName, setColumnName] = useState("");
  const [editingTaskId, setEditingTaskId] = useState<string | null>(null);
  const [taskForm, setTaskForm] = useState({
    title: "",
    description: "",
    priority: "Medium" as "Low" | "Medium" | "High",
    dueDate: "",
    assigneeId: "" as string | null,
  });
  const [draggedTask, setDraggedTask] = useState<{
    task: Task;
    columnId: string;
  } | null>(null);
  const [users, setUsers] = useState<UserSummary[]>([]);

  /** ボードを読み込む */
  const loadBoard = async () => {
    if (!boardId) return;
    try {
      const response = await apiClient.boards.getBoard(boardId);
      setBoard(response);
      setBoardName(response.name);
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "ボードの取得に失敗しました。");
    }
  };

  /** ユーザー一覧を読み込む */
  const loadUsers = async () => {
    try {
      const response = await apiClient.users.getUsers();
      setUsers(response);
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "ユーザー一覧の取得に失敗しました。");
    }
  };

  useEffect(() => {
    loadBoard();
    loadUsers();
  }, [boardId]);

  /** ボード名を更新する */
  const handleUpdateBoardName = async () => {
    if (!boardId || !boardName.trim()) return;
    try {
      await apiClient.boards.updateBoard(boardId, { name: boardName });
      await loadBoard();
      setEditingBoardName(false);
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "ボード名の更新に失敗しました。");
    }
  };

  /** 列を作成する */
  const handleCreateColumn = async () => {
    if (!boardId) return;
    const name = prompt("列名を入力してください");
    if (!name?.trim()) return;
    try {
      const request: CreateBoardColumnRequest = { name };
      await apiClient.boards.createBoardColumn(boardId, request);
      await loadBoard();
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "列の作成に失敗しました。");
    }
  };

  /** 列を更新する */
  const handleUpdateColumn = async (columnId: string) => {
    if (!boardId || !columnName.trim()) return;
    try {
      const request: UpdateBoardColumnRequest = { name: columnName };
      await apiClient.boards.updateBoardColumn(boardId, columnId, request);
      await loadBoard();
      setEditingColumnId(null);
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "列名の更新に失敗しました。");
    }
  };

  /** 列を削除する */
  const handleDeleteColumn = async (columnId: string) => {
    if (!boardId || !confirm("この列を削除しますか？")) return;
    try {
      await apiClient.boards.deleteBoardColumn(boardId, columnId);
      await loadBoard();
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "列の削除に失敗しました。");
    }
  };

  /** 列を移動する */
  const handleMoveColumn = async (
    columnId: string,
    direction: "left" | "right",
  ) => {
    if (!boardId || !board) return;
    const columns = board.columns;
    const index = columns.findIndex((c) => c.id === columnId);
    if (index === -1) return;
    if (direction === "left" && index === 0) return;
    if (direction === "right" && index === columns.length - 1) return;

    let previousColumnId: string | null = null;
    let nextColumnId: string | null = null;

    if (direction === "left") {
      // 左に移動: index-1の位置に挿入
      previousColumnId = index >= 2 ? columns[index - 2].id : null;
      nextColumnId = columns[index - 1].id;
    } else {
      // 右に移動: index+1の位置に挿入
      previousColumnId = columns[index + 1].id;
      nextColumnId = index + 2 < columns.length ? columns[index + 2].id : null;
    }

    try {
      const request: UpdateBoardColumnRequest = {
        previousColumnId,
        nextColumnId,
      };
      await apiClient.boards.updateBoardColumn(boardId, columnId, request);
      await loadBoard();
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "列の移動に失敗しました。");
    }
  };

  /** タスクを作成する */
  const handleCreateTask = async (columnId: string) => {
    if (!boardId) return;
    setEditingTaskId(`new-${columnId}`);
    setTaskForm({
      title: "",
      description: "",
      priority: "Medium",
      dueDate: "",
      assigneeId: null,
    });
  };

  /** タスクを保存する */
  const handleSaveTask = async (columnId: string, taskId: string | null) => {
    if (!boardId || !taskForm.title.trim()) return;
    try {
      if (taskId && !taskId.startsWith("new-")) {
        const request: UpdateTaskItemRequest = {
          title: taskForm.title,
          description: taskForm.description,
          priority: taskForm.priority,
          dueDate: taskForm.dueDate || null,
          assigneeId: taskForm.assigneeId || null,
          isReleaseAssignee: taskForm.assigneeId === null,
        };
        await apiClient.boards.updateTaskItem(
          boardId,
          columnId,
          taskId,
          request,
        );
      } else {
        const request: CreateTaskItemRequest = {
          title: taskForm.title,
          description: taskForm.description,
          priority: taskForm.priority,
          dueDate: taskForm.dueDate || null,
          assigneeId: taskForm.assigneeId || null,
        };
        await apiClient.boards.createTaskItem(boardId, columnId, request);
      }
      await loadBoard();
      setEditingTaskId(null);
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "タスクの保存に失敗しました。");
    }
  };

  /** タスクを削除する */
  const handleDeleteTask = async (columnId: string, taskId: string) => {
    if (!boardId || !confirm("このタスクを削除しますか？")) return;
    try {
      await apiClient.boards.deleteTaskItem(boardId, columnId, taskId);
      await loadBoard();
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "タスクの削除に失敗しました。");
    }
  };

  /** タスクを移動する */
  const handleMoveTask = async (
    columnId: string,
    taskId: string,
    direction: "up" | "down",
  ) => {
    if (!boardId || !board) return;
    const column = board.columns.find((c) => c.id === columnId);
    if (!column) return;
    const tasks = column.taskItems;
    const index = tasks.findIndex((t) => t.id === taskId);
    if (index === -1) return;
    if (direction === "up" && index === 0) return;
    if (direction === "down" && index === tasks.length - 1) return;

    let previousTaskItemId: string | null = null;
    let nextTaskItemId: string | null = null;

    if (direction === "up") {
      // 上に移動: index-1の位置に挿入
      previousTaskItemId = index >= 2 ? tasks[index - 2].id : null;
      nextTaskItemId = tasks[index - 1].id;
    } else {
      // 下に移動: index+1の位置に挿入
      previousTaskItemId = tasks[index + 1].id;
      nextTaskItemId = index + 2 < tasks.length ? tasks[index + 2].id : null;
    }

    try {
      const request: UpdateTaskItemRequest = {
        previousTaskItemId,
        nextTaskItemId,
      };
      await apiClient.boards.updateTaskItem(boardId, columnId, taskId, request);
      await loadBoard();
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "タスクの移動に失敗しました。");
    }
  };

  /** ドラッグ開始時に呼び出される */
  const handleDragStart = (task: Task, columnId: string) => {
    setDraggedTask({ task, columnId });
  };

  /** ドロップ時に呼び出される */
  const handleDrop = async (targetColumnId: string) => {
    if (!boardId || !draggedTask) return;
    if (draggedTask.columnId === targetColumnId) {
      setDraggedTask(null);
      return;
    }

    try {
      const request: UpdateTaskItemRequest = { boardColumnId: targetColumnId };
      await apiClient.boards.updateTaskItem(
        boardId,
        draggedTask.columnId,
        draggedTask.task.id,
        request,
      );
      await loadBoard();
      setDraggedTask(null);
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "タスクの移動に失敗しました。");
      setDraggedTask(null);
    }
  };

  if (!board) {
    return <div className="p-6">読み込み中...</div>;
  }

  return (
    <div className="p-6 h-full flex flex-col overflow-hidden">
      <div className="mb-4 flex items-center gap-3 shrink-0">
        {editingBoardName ? (
          <>
            <input
              type="text"
              value={boardName}
              onChange={(e) => setBoardName(e.target.value)}
              className="text-2xl font-bold border-b-2 border-blue-500 outline-none"
              autoFocus
            />
            <button
              onClick={handleUpdateBoardName}
              className="px-3 py-1 text-sm bg-blue-600 text-white hover:bg-blue-700 rounded cursor-pointer"
            >
              保存
            </button>
            <button
              onClick={() => {
                setEditingBoardName(false);
                setBoardName(board.name);
              }}
              className="px-3 py-1 text-sm bg-gray-200 hover:bg-gray-300 rounded cursor-pointer"
            >
              キャンセル
            </button>
          </>
        ) : (
          <>
            <h1 className="text-2xl font-bold">{board.name}</h1>
            <button
              onClick={() => setEditingBoardName(true)}
              className="px-3 py-1 text-sm bg-blue-100 hover:bg-blue-200 rounded cursor-pointer"
            >
              編集
            </button>
          </>
        )}
      </div>

      <div className="flex-1 overflow-x-auto overflow-y-hidden min-h-0">
        <div className="flex gap-4 h-full">
          {/* 列リスト */}
          {board.columns.map((column, colIndex) => (
            <div
              key={column.id}
              className="shrink-0 w-80 bg-gray-100 rounded-lg p-4 flex flex-col h-full"
              onDragOver={(e) => e.preventDefault()}
              onDrop={() => handleDrop(column.id)}
            >
              <div className="mb-3">
                {editingColumnId === column.id ? (
                  <div className="flex gap-2">
                    <input
                      type="text"
                      value={columnName}
                      onChange={(e) => setColumnName(e.target.value)}
                      className="flex-1 px-2 py-1 border rounded"
                      autoFocus
                    />

                    <button
                      onClick={() => handleUpdateColumn(column.id)}
                      className="px-2 py-1 text-sm bg-blue-600 text-white rounded cursor-pointer"
                    >
                      保存
                    </button>

                    <button
                      onClick={() => setEditingColumnId(null)}
                      className="px-2 py-1 text-sm bg-gray-300 rounded cursor-pointer"
                    >
                      ×
                    </button>
                  </div>
                ) : (
                  <div className="flex items-center justify-between">
                    <h2 className="font-semibold text-lg">{column.name}</h2>
                    <div className="flex gap-1">
                      <button
                        onClick={() => handleMoveColumn(column.id, "left")}
                        disabled={colIndex === 0}
                        className="px-2 py-1 text-xs bg-gray-200 hover:bg-gray-300 rounded disabled:opacity-30 cursor-pointer disabled:cursor-not-allowed"
                      >
                        ←
                      </button>
                      <button
                        onClick={() => handleMoveColumn(column.id, "right")}
                        disabled={colIndex === board.columns.length - 1}
                        className="px-2 py-1 text-xs bg-gray-200 hover:bg-gray-300 rounded disabled:opacity-30 cursor-pointer disabled:cursor-not-allowed"
                      >
                        →
                      </button>
                      <button
                        onClick={() => {
                          setEditingColumnId(column.id);
                          setColumnName(column.name);
                        }}
                        className="px-2 py-1 text-xs bg-blue-100 hover:bg-blue-200 rounded cursor-pointer"
                      >
                        編集
                      </button>
                      <button
                        onClick={() => handleDeleteColumn(column.id)}
                        className="px-2 py-1 text-xs bg-red-100 hover:bg-red-200 rounded cursor-pointer"
                      >
                        削除
                      </button>
                    </div>
                  </div>
                )}
              </div>

              {/* タスクリスト */}
              <div className="flex-1 overflow-y-auto space-y-2 min-h-0">
                {column.taskItems.map((task, taskIndex) => (
                  <div key={task.id}>
                    {editingTaskId === task.id ? (
                      <div className="bg-white p-3 rounded shadow space-y-2">
                        <input
                          type="text"
                          placeholder="タイトル"
                          value={taskForm.title}
                          onChange={(e) =>
                            setTaskForm({ ...taskForm, title: e.target.value })
                          }
                          className="w-full px-2 py-1 border rounded"
                        />
                        <textarea
                          placeholder="説明"
                          value={taskForm.description}
                          onChange={(e) =>
                            setTaskForm({
                              ...taskForm,
                              description: e.target.value,
                            })
                          }
                          className="w-full px-2 py-1 border rounded text-sm"
                          rows={3}
                        />
                        <select
                          value={taskForm.priority}
                          onChange={(e) =>
                            setTaskForm({
                              ...taskForm,
                              priority: e.target.value as
                                | "Low"
                                | "Medium"
                                | "High",
                            })
                          }
                          className="w-full px-2 py-1 border rounded text-sm"
                        >
                          <option value="Low">Low</option>
                          <option value="Medium">Medium</option>
                          <option value="High">High</option>
                        </select>
                        <input
                          type="date"
                          value={taskForm.dueDate}
                          onChange={(e) =>
                            setTaskForm({
                              ...taskForm,
                              dueDate: e.target.value,
                            })
                          }
                          className="w-full px-2 py-1 border rounded text-sm"
                        />
                        <select
                          value={taskForm.assigneeId || ""}
                          onChange={(e) =>
                            setTaskForm({
                              ...taskForm,
                              assigneeId: e.target.value || null,
                            })
                          }
                          className="w-full px-2 py-1 border rounded text-sm"
                        >
                          <option value="">担当者なし</option>
                          {users.map((user) => (
                            <option key={user.id} value={user.id}>
                              {user.username}
                            </option>
                          ))}
                        </select>
                        <div className="flex gap-2">
                          <button
                            onClick={() => handleSaveTask(column.id, task.id)}
                            className="flex-1 px-2 py-1 text-sm bg-blue-600 text-white rounded cursor-pointer"
                          >
                            保存
                          </button>
                          <button
                            onClick={() => setEditingTaskId(null)}
                            className="flex-1 px-2 py-1 text-sm bg-gray-300 rounded cursor-pointer"
                          >
                            キャンセル
                          </button>
                        </div>
                      </div>
                    ) : (
                      <div
                        draggable
                        onDragStart={() => handleDragStart(task, column.id)}
                        className="bg-white p-3 rounded shadow cursor-move hover:shadow-md"
                      >
                        <div className="font-medium mb-1">{task.title}</div>
                        {task.description && (
                          <div className="text-sm text-gray-600 mb-2">
                            {task.description}
                          </div>
                        )}
                        <div className="flex items-center justify-between text-xs mb-2">
                          <span
                            className={`px-2 py-1 rounded ${
                              task.priority === "High"
                                ? "bg-red-100 text-red-700"
                                : task.priority === "Medium"
                                  ? "bg-yellow-100 text-yellow-700"
                                  : "bg-green-100 text-green-700"
                            }`}
                          >
                            {task.priority}
                          </span>
                          {task.dueDate && (
                            <span className="text-gray-500">
                              {new Date(task.dueDate).toLocaleDateString()}
                            </span>
                          )}
                        </div>
                        {task.assigneeId && (
                          <div className="text-xs text-gray-600 mb-2">
                            担当:{" "}
                            {users.find((u) => u.id === task.assigneeId)
                              ?.username || "不明"}
                          </div>
                        )}
                        <div className="flex gap-1 mt-2">
                          <button
                            onClick={() =>
                              handleMoveTask(column.id, task.id, "up")
                            }
                            disabled={taskIndex === 0}
                            className="px-2 py-1 text-xs bg-gray-100 hover:bg-gray-200 rounded disabled:opacity-30 cursor-pointer disabled:cursor-not-allowed"
                          >
                            ↑
                          </button>
                          <button
                            onClick={() =>
                              handleMoveTask(column.id, task.id, "down")
                            }
                            disabled={taskIndex === column.taskItems.length - 1}
                            className="px-2 py-1 text-xs bg-gray-100 hover:bg-gray-200 rounded disabled:opacity-30 cursor-pointer disabled:cursor-not-allowed"
                          >
                            ↓
                          </button>
                          <button
                            onClick={() => {
                              setEditingTaskId(task.id);
                              setTaskForm({
                                title: task.title,
                                description: task.description,
                                priority: task.priority,
                                dueDate: task.dueDate
                                  ? new Date(task.dueDate)
                                      .toISOString()
                                      .split("T")[0]
                                  : "",
                                assigneeId: task.assigneeId || null,
                              });
                            }}
                            className="px-2 py-1 text-xs bg-blue-100 hover:bg-blue-200 rounded cursor-pointer"
                          >
                            編集
                          </button>
                          <button
                            onClick={() => handleDeleteTask(column.id, task.id)}
                            className="px-2 py-1 text-xs bg-red-100 hover:bg-red-200 rounded cursor-pointer"
                          >
                            削除
                          </button>
                        </div>
                      </div>
                    )}
                  </div>
                ))}

                {editingTaskId === `new-${column.id}` && (
                  <div className="bg-white p-3 rounded shadow space-y-2">
                    <input
                      type="text"
                      placeholder="タイトル"
                      value={taskForm.title}
                      onChange={(e) =>
                        setTaskForm({ ...taskForm, title: e.target.value })
                      }
                      className="w-full px-2 py-1 border rounded"
                      autoFocus
                    />
                    <textarea
                      placeholder="説明"
                      value={taskForm.description}
                      onChange={(e) =>
                        setTaskForm({
                          ...taskForm,
                          description: e.target.value,
                        })
                      }
                      className="w-full px-2 py-1 border rounded text-sm"
                      rows={3}
                    />
                    <select
                      value={taskForm.priority}
                      onChange={(e) =>
                        setTaskForm({
                          ...taskForm,
                          priority: e.target.value as "Low" | "Medium" | "High",
                        })
                      }
                      className="w-full px-2 py-1 border rounded text-sm"
                    >
                      <option value="Low">Low</option>
                      <option value="Medium">Medium</option>
                      <option value="High">High</option>
                    </select>
                    <input
                      type="date"
                      value={taskForm.dueDate}
                      onChange={(e) =>
                        setTaskForm({ ...taskForm, dueDate: e.target.value })
                      }
                      className="w-full px-2 py-1 border rounded text-sm"
                    />
                    <select
                      value={taskForm.assigneeId || ""}
                      onChange={(e) =>
                        setTaskForm({
                          ...taskForm,
                          assigneeId: e.target.value || null,
                        })
                      }
                      className="w-full px-2 py-1 border rounded text-sm"
                    >
                      <option value="">担当者なし</option>
                      {users.map((user) => (
                        <option key={user.id} value={user.id}>
                          {user.username}
                        </option>
                      ))}
                    </select>
                    <div className="flex gap-2">
                      <button
                        onClick={() => handleSaveTask(column.id, null)}
                        className="flex-1 px-2 py-1 text-sm bg-blue-600 text-white rounded cursor-pointer"
                      >
                        作成
                      </button>
                      <button
                        onClick={() => setEditingTaskId(null)}
                        className="flex-1 px-2 py-1 text-sm bg-gray-300 rounded cursor-pointer"
                      >
                        キャンセル
                      </button>
                    </div>
                  </div>
                )}
              </div>

              <button
                onClick={() => handleCreateTask(column.id)}
                className="mt-3 w-full px-3 py-2 text-sm bg-blue-600 text-white hover:bg-blue-700 rounded cursor-pointer"
              >
                + タスクを追加
              </button>
            </div>
          ))}

          <button
            onClick={handleCreateColumn}
            className="shrink-0 w-80 h-32 bg-gray-200 hover:bg-gray-300 rounded-lg flex items-center justify-center text-gray-600 font-medium cursor-pointer"
          >
            + 列を追加
          </button>
        </div>
      </div>
    </div>
  );
};

export default BoardEditPage;
