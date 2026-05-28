import { useState, useEffect, useMemo } from "react";

import type { TenantDetail } from "../../../models/tenants/TenantDetail";
import type { Role } from "../../../models/roles/Role";
import { useAppSelector } from "../../../app/hook";

interface TenantFormProps {
  initialData?: TenantDetail;
  roles?: Role[];
  onSubmit: (formData: {
    name: string;
    initUser?: {
      email: string;
      lastName: string;
      firstName: string;
      password: string;
      roleId: string;
    };
  }) => Promise<void>;
  isLoading?: boolean;
}

/** テナント作成・編集フォーム */
const TenantForm = ({
  initialData,
  roles = [],
  onSubmit,
  isLoading = false,
}: TenantFormProps) => {
  const currentUser = useAppSelector((root) => root.profile.userInfo);

  const [formData, setFormData] = useState({
    name: "",
  });

  const [userFormData, setUserFormData] = useState({
    email: "",
    lastName: "",
    firstName: "",
    password: "",
    roleId: "",
  });

  const isNewTenant = !initialData;

  // 管理者以上かつ自身のロール以下のロールをフィルタリング（新規テナント作成時の初期ユーザー用）
  const selectableRoles = useMemo(() => {
    if (!currentUser) return [];

    return roles.filter((role) => {
      // 管理者以上（Admin, SystemAdmin）かつ自身のロール以下の権限レベル
      const isAdminOrAbove =
        role.name === "Admin" || role.name === "SystemAdmin";
      const isWithinUserLevel = role.level <= currentUser.user.role.level;

      return isAdminOrAbove && isWithinUserLevel;
    });
  }, [roles, currentUser]);

  useEffect(() => {
    if (initialData) {
      setFormData({
        name: initialData.name || "",
      });
    }
  }, [initialData]);

  // 編集時の変更検知
  const hasChanges = useMemo(() => {
    if (!initialData) return true; // 新規作成時は常にtrue

    return formData.name !== (initialData.name || "");
  }, [formData, initialData]);

  // 必須項目の入力チェック
  const isFormValid = useMemo(() => {
    const tenantValid = formData.name.trim() !== "";

    if (!isNewTenant) {
      return tenantValid;
    }

    // 新規作成時は初期ユーザー情報も必須
    const hasUserName =
      userFormData.lastName.trim() !== "" ||
      userFormData.firstName.trim() !== "";
    const userValid =
      userFormData.email.trim() !== "" &&
      hasUserName &&
      userFormData.password.trim() !== "" &&
      userFormData.roleId !== "";

    return tenantValid && userValid;
  }, [formData, userFormData, isNewTenant]);

  const isSubmitDisabled = isLoading || !hasChanges || !isFormValid;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const submitData: any = {
      name: formData.name,
    };

    if (isNewTenant) {
      submitData.initUser = userFormData;
    }

    await onSubmit(submitData);
  };

  const handleTenantChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData((prev) => ({
      ...prev,
      [e.target.name]: e.target.value,
    }));
  };

  const handleUserChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>,
  ) => {
    setUserFormData((prev) => ({
      ...prev,
      [e.target.name]: e.target.value,
    }));
  };

  return (
    <div className="p-6 max-w-2xl mx-auto">
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-200">
          <h2 className="text-xl font-semibold text-gray-900">
            {initialData ? "テナント情報編集" : "新規テナント作成"}
          </h2>
        </div>

        <form onSubmit={handleSubmit} className="p-6 space-y-8">
          {/* テナント情報 */}
          <div className="space-y-4">
            <h3 className="text-lg font-medium text-gray-900">テナント情報</h3>

            <div>
              <label
                htmlFor="name"
                className="block text-sm font-medium text-gray-700 mb-2"
              >
                テナント名
              </label>
              <input
                id="name"
                name="name"
                type="text"
                value={formData.name}
                onChange={handleTenantChange}
                required
                className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
                placeholder="テナント名を入力"
              />
            </div>
          </div>

          {/* 初期ユーザー情報（新規作成時のみ） */}
          {isNewTenant && (
            <div className="space-y-4 border-t border-gray-200 pt-6">
              <h3 className="text-lg font-medium text-gray-900">
                初期管理者ユーザー
              </h3>

              <div>
                <label
                  htmlFor="email"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  メールアドレス
                </label>
                <input
                  id="email"
                  name="email"
                  type="email"
                  value={userFormData.email}
                  onChange={handleUserChange}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
                  placeholder="example@company.com"
                />
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label
                    htmlFor="lastName"
                    className="block text-sm font-medium text-gray-700 mb-2"
                  >
                    姓
                  </label>

                  <input
                    id="lastName"
                    name="lastName"
                    type="text"
                    value={userFormData.lastName}
                    onChange={handleUserChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
                    placeholder="田中"
                  />
                </div>

                <div>
                  <label
                    htmlFor="firstName"
                    className="block text-sm font-medium text-gray-700 mb-2"
                  >
                    名
                  </label>

                  <input
                    id="firstName"
                    name="firstName"
                    type="text"
                    value={userFormData.firstName}
                    onChange={handleUserChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
                    placeholder="太郎"
                  />
                </div>
              </div>

              <div>
                <label
                  htmlFor="roleId"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  ロール
                </label>

                <select
                  id="roleId"
                  name="roleId"
                  value={userFormData.roleId}
                  onChange={handleUserChange}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors bg-white"
                >
                  <option value="">選択してください</option>
                  {selectableRoles.map((role) => (
                    <option key={role.id} value={role.id}>
                      {role.label}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label
                  htmlFor="password"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  パスワード
                </label>

                <input
                  id="password"
                  name="password"
                  type="password"
                  value={userFormData.password}
                  onChange={handleUserChange}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
                  placeholder="パスワードを入力"
                />
              </div>
            </div>
          )}

          <div className="flex justify-end pt-4 border-t border-gray-200">
            <button
              type="submit"
              disabled={isSubmitDisabled}
              className={`px-6 py-2 rounded-md font-medium transition-colors ${
                isSubmitDisabled
                  ? "bg-gray-200 text-gray-400 cursor-not-allowed"
                  : "bg-blue-600 text-white hover:bg-blue-700 cursor-pointer"
              }`}
            >
              {isLoading ? "保存中..." : initialData ? "更新" : "作成"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default TenantForm;
