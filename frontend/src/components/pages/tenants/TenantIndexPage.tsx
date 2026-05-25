import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

import type { TenantSummary } from "../../../models/tenants/TenantSummary";
import { apiClient } from "../../../api/clients/ApiClient";
import { useAppSelector } from "../../../app/hook";
import { formatDateTime } from "../../../helpers/dateUtils";

/** テナント一覧ページ */
const TenantIndexPage = () => {
  const navigate = useNavigate();
  const [tenants, setTenants] = useState<TenantSummary[]>([]);
  const [selectedTenant, setSelectedTenant] = useState<TenantSummary | null>(
    null,
  );
  const currentUser = useAppSelector((root) => root.profile.userInfo);

  /** テナント一覧取得APIを呼び出し、テナント一覧のステートを更新する */
  const getTenants = async () => {
    try {
      const response = await apiClient.tenants.getTenants();
      setTenants(response);
    } catch (e) {
      setTenants([]);
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "テナント一覧の取得に失敗しました。");
    }
  };

  useEffect(() => {
    const initLoad = async () => {
      await getTenants();
    };

    initLoad();
  }, []);

  /** 新規ボタンのクリック処理 */
  const handleNew = () => {
    navigate("/tenants/new");
  };

  /** 編集ボタンのクリック処理 */
  const handleEdit = () => {
    if (!selectedTenant) return;

    navigate(`/tenants/${selectedTenant.id}/edit`);
  };

  /** 削除ボタンのクリック処理 */
  const handleDelete = async () => {
    if (
      !selectedTenant ||
      selectedTenant.id === currentUser?.tenant.id ||
      !confirm("選択したテナントを削除しますか？")
    )
      return;

    try {
      // テナント削除APIを呼び出し
      await apiClient.tenants.deleteTenant(selectedTenant.id);

      // 削除成功後に一覧を再読み込み
      await getTenants();
      setSelectedTenant(null);
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "テナントの削除に失敗しました。");
    }
  };

  /** 行クリック処理 */
  const handleRowClick = (tenant: TenantSummary) => {
    setSelectedTenant(tenant);
  };

  /** 行ダブルクリック処理 */
  const handleRowDoubleClick = () => {
    // 編集ボタンと同じ処理を呼び出す
    handleEdit();
  };

  const isDeleteDisabled =
    !selectedTenant ||
    !currentUser ||
    selectedTenant.id === currentUser.tenant.id;

  return (
    <div className="p-6 max-w-6xl mx-auto">
      <div className="mb-8">
        <h1 className="text-3xl font-semibold text-gray-900 mb-2">
          テナント管理
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
          disabled={!selectedTenant}
          className={`px-4 py-2 rounded-md font-medium transition-colors ${
            !selectedTenant
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
                  テナント名
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
              {tenants.map((tenant) => (
                <tr
                  key={tenant.id}
                  onClick={() => handleRowClick(tenant)}
                  onDoubleClick={handleRowDoubleClick}
                  className={`cursor-pointer transition-colors border-b border-gray-200 ${
                    selectedTenant?.id === tenant.id
                      ? "bg-blue-50"
                      : "hover:bg-gray-50"
                  }`}
                >
                  <td className="px-4 py-3 text-sm font-medium text-gray-900">
                    {tenant.name}
                    {tenant.id === currentUser?.tenant.id && (
                      <span className="ml-2 px-2 py-1 text-xs bg-blue-100 text-blue-800 rounded-full">
                        所属
                      </span>
                    )}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-600">
                    {formatDateTime(tenant.createdAt)}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-600">
                    {formatDateTime(tenant.updatedAt)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {tenants.length === 0 && (
            <div className="text-center py-12 text-gray-500">
              テナントが見つかりません
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default TenantIndexPage;
