import { useState, useEffect } from "react";
import type { UserSummary } from "../../../models/users/UserSummary";
import { apiClient } from "../../../api/clients/ApiClient";
import { useAppSelector } from "../../../app/hook";

/** ユーザー一覧ページ */
const UserIndexPage = () => {
  const [users, setUsers] = useState<UserSummary[]>([]);
  const [selectedUser, setSelectedUser] = useState<UserSummary | null>(null);
  const currentUser = useAppSelector((root) => root.profile.userInfo?.user);

  /** ユーザー一覧取得APIを呼び出し、ユーザー一覧のステートを更新する */
  const getUsers = async () => {
    try {
      const response = await apiClient.users.getUsers();
      setUsers(response);
    } catch (e) {
      setUsers([]);
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "ユーザー一覧の取得に失敗しました。");
    }
  };

  useEffect(() => {
    const initLoad = async () => {
      await getUsers();
    };

    initLoad();
  }, []);

  /** 新規ボタンのクリック処理 */
  const handleNew = () => {
    // TODO: 新規作成ボタンへの遷移
  };

  /** 編集ボタンのクリック処理 */
  const handleEdit = () => {
    if (!selectedUser) return;
    // TODO: 編集画面への遷移またはモーダル表示
  };

  /** 削除ボタンのクリック処理 */
  const handleDelete = async () => {
    if (
      !selectedUser ||
      selectedUser.id === currentUser?.id ||
      !confirm("選択したユーザーを削除しますか？")
    )
      return;

    try {
      // ユーザー削除APIを呼び出し
      await apiClient.users.deleteUser(selectedUser.id);

      // 削除成功後に一覧を再読み込み
      await getUsers();
      setSelectedUser(null);
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "ユーザーの削除に失敗しました。");
    }
  };

  const isEditDisabled =
    !selectedUser ||
    !currentUser ||
    selectedUser.role.level > currentUser.role.level;

  const isDeleteDisabled =
    !selectedUser ||
    !currentUser ||
    selectedUser.id === currentUser.id ||
    selectedUser.role.level > currentUser.role.level;

  return (
    <div className="p-6 max-w-6xl mx-auto">
      <div className="mb-8">
        <h1 className="text-3xl font-semibold text-gray-900 mb-2">
          ユーザー管理
        </h1>
      </div>

      <div className="mb-4 flex gap-3">
        <button
          onClick={handleNew}
          className="px-4 py-2 rounded-md font-medium transition-colors bg-green-600 text-white hover:bg-green-700 cursor-pointer"
        >
          新規
        </button>

        <button
          onClick={handleEdit}
          disabled={isEditDisabled}
          className={`px-4 py-2 rounded-md font-medium transition-colors ${
            isEditDisabled
              ? "bg-gray-200 text-gray-400 cursor-not-allowed"
              : "bg-blue-600 text-white hover:bg-blue-700 cursor-pointer"
          }`}
        >
          編集
        </button>

        <button
          onClick={handleDelete}
          disabled={isDeleteDisabled}
          className={`px-4 py-2 rounded-md font-medium transition-colors ${
            isDeleteDisabled
              ? "bg-gray-200 text-gray-400 cursor-not-allowed"
              : "bg-red-600 text-white hover:bg-red-700 cursor-pointer"
          }`}
        >
          削除
        </button>
      </div>

      <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden h-96">
        <div className="sticky top-0 bg-white z-10">
          <table className="w-full">
            <thead>
              <tr className="bg-gray-50 border-b border-gray-200">
                <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900">
                  氏名
                </th>

                <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900">
                  メールアドレス
                </th>

                <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900">
                  ロール
                </th>
              </tr>
            </thead>
          </table>
        </div>

        <div className="overflow-y-auto h-full">
          <table className="w-full">
            <tbody className="divide-y divide-gray-100">
              {users.map((user) => (
                <tr
                  key={user.id}
                  onClick={() => setSelectedUser(user)}
                  className={`cursor-pointer transition-colors border-b border-gray-200 ${
                    selectedUser?.id === user.id
                      ? "bg-blue-50"
                      : "hover:bg-gray-50"
                  }`}
                >
                  <td className="px-4 py-3 text-sm font-medium text-gray-900">
                    {user.username}
                    {user.id === currentUser?.id && (
                      <span className="ml-2 px-2 py-1 text-xs bg-blue-100 text-blue-800 rounded-full">
                        自分
                      </span>
                    )}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-600">
                    {user.email}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-600">
                    {user.role.label}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {users.length === 0 && (
            <div className="text-center py-12 text-gray-500">
              ユーザーが見つかりません
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default UserIndexPage;
