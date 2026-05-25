import { useState, useEffect, useMemo } from "react";
import { useNavigate } from "react-router-dom";

import type { BoardSummary } from "../../../models/boards/BoardSummary";
import { apiClient } from "../../../api/clients/ApiClient";
import { formatDateTime } from "../../../helpers/dateUtils";
import { useAppSelector } from "../../../app/hook";
import type { Role } from "../../../models/roles/Role";

/** ボード一覧ページ */
const BoardIndexPage = () => {
  const navigate = useNavigate();
  const [boards, setBoards] = useState<BoardSummary[]>([]);
  const [selectedBoard, setSelectedBoard] = useState<BoardSummary | null>(null);
  const [roles, setRoles] = useState<Role[]>([]);

  const userInfo = useAppSelector((state) => state.profile.userInfo);

  const isAdmin = useMemo(() => {
    if (!userInfo) return false;

    const adminRole = roles.find((role) => role.name === "Admin");
    if (!adminRole) return false;

    return userInfo.user.role.level >= adminRole.level;
  }, [userInfo, roles]);

  /** ボード一覧取得APIを呼び出し、ボード一覧のステートを更新する */
  const getBoards = async () => {
    try {
      const response = await apiClient.boards.getBoards();
      setBoards(response);
    } catch (e) {
      setBoards([]);
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "ボード一覧の取得に失敗しました。");
    }
  };

  /** ロール一覧取得APIを呼び出し、ロール一覧のステートを更新する */
  const getRoles = async () => {
    try {
      const response = await apiClient.roles.getRoles();
      setRoles(response);
    } catch (e) {
      setRoles([]);
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "ロール一覧の取得に失敗しました。");
    }
  };

  useEffect(() => {
    const initLoad = async () => {
      await getBoards();
      await getRoles();
    };

    initLoad();
  }, []);

  /** 新規ボタンのクリック処理 */
  const handleNew = () => {
    navigate("/boards/new");
  };

  /** 編集ボタンのクリック処理 */
  const handleEdit = () => {
    if (!selectedBoard) return;

    navigate(`/boards/${selectedBoard.id}/edit`);
  };

  /** 削除ボタンのクリック処理 */
  const handleDelete = async () => {
    if (!selectedBoard || !confirm("選択したボードを削除しますか？")) return;

    try {
      // ボード削除APIを呼び出し
      await apiClient.boards.deleteBoard(selectedBoard.id);

      // 削除成功後に一覧を再読み込み
      await getBoards();
      setSelectedBoard(null);
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "ボードの削除に失敗しました。");
    }
  };

  /** 行クリック処理 */
  const handleRowClick = (board: BoardSummary) => {
    setSelectedBoard(board);
  };

  /** 行ダブルクリック処理 */
  const handleRowDoubleClick = () => {
    // 編集ボタンと同じ処理を呼び出す
    handleEdit();
  };

  return (
    <div className="p-6 max-w-6xl mx-auto">
      <div className="mb-8">
        <h1 className="text-3xl font-semibold text-gray-900 mb-2">ボード</h1>
      </div>

      <div className="mb-4 flex gap-3">
        {isAdmin && (
          <button
            onClick={handleNew}
            className="px-4 py-2 rounded-md font-medium transition-colors bg-green-600 text-white hover:bg-green-700 cursor-pointer"
          >
            新規
          </button>
        )}

        <button
          onClick={handleEdit}
          disabled={!selectedBoard}
          className={`px-4 py-2 rounded-md font-medium transition-colors ${
            !selectedBoard
              ? "bg-gray-200 text-gray-400 cursor-not-allowed"
              : "bg-blue-600 text-white hover:bg-blue-700 cursor-pointer"
          }`}
        >
          編集
        </button>

        {isAdmin && (
          <button
            onClick={handleDelete}
            disabled={!selectedBoard}
            className={`px-4 py-2 rounded-md font-medium transition-colors ${
              !selectedBoard
                ? "bg-gray-200 text-gray-400 cursor-not-allowed"
                : "bg-red-600 text-white hover:bg-red-700 cursor-pointer"
            }`}
          >
            削除
          </button>
        )}
      </div>

      <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden h-96">
        <div className="sticky top-0 bg-white z-10">
          <table className="w-full">
            <thead>
              <tr className="bg-gray-50 border-b border-gray-200">
                <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900">
                  ボード名
                </th>

                <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900">
                  作成日時
                </th>

                <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900">
                  更新日時
                </th>
              </tr>
            </thead>
          </table>
        </div>

        <div className="overflow-y-auto h-full">
          <table className="w-full">
            <tbody className="divide-y divide-gray-100">
              {boards.map((board) => (
                <tr
                  key={board.id}
                  onClick={() => handleRowClick(board)}
                  onDoubleClick={handleRowDoubleClick}
                  className={`cursor-pointer transition-colors border-b border-gray-200 ${
                    selectedBoard?.id === board.id
                      ? "bg-blue-50"
                      : "hover:bg-gray-50"
                  }`}
                >
                  <td className="px-4 py-3 text-sm font-medium text-gray-900">
                    {board.name}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-600">
                    {formatDateTime(board.createdAt)}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-600">
                    {formatDateTime(board.updatedAt)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {boards.length === 0 && (
            <div className="text-center py-12 text-gray-500">
              ボードが見つかりません
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default BoardIndexPage;
