import { useState, useEffect, useMemo } from "react";

import type { UserDetail } from "../../../models/users/UserDetail";
import type { Role } from "../../../models/roles/Role";
import { useAppSelector } from "../../../app/hook";

interface UserFormProps {
  initialData?: UserDetail;
  roles: Role[];
  onSubmit: (formData: {
    email: string;
    lastName: string;
    firstName: string;
    password: string;
    roleId: string;
  }) => Promise<void>;
  isLoading?: boolean;
}

/** ユーザー作成・編集フォーム */
const UserForm = ({
  initialData,
  roles,
  onSubmit,
  isLoading = false,
}: UserFormProps) => {
  const currentUser = useAppSelector((root) => root.profile.userInfo?.user);

  const [formData, setFormData] = useState({
    email: "",
    lastName: "",
    firstName: "",
    password: "",
    roleId: "",
  });

  // 実行者のロールレベル以下のロールのみフィルタリング
  const selectableRoles = useMemo(() => {
    if (!currentUser) return [];
    return roles.filter((role) => role.level <= currentUser.role.level);
  }, [roles, currentUser]);

  useEffect(() => {
    if (initialData) {
      setFormData({
        email: initialData.email || "",
        lastName: initialData.lastName || "",
        firstName: initialData.firstName || "",
        password: "",
        roleId: initialData.role.id,
      });
    }
  }, [initialData]);

  // 編集時の変更検知
  const hasChanges = useMemo(() => {
    if (!initialData) return true; // 新規作成時は常にtrue

    return (
      formData.email !== (initialData.email || "") ||
      formData.lastName !== (initialData.lastName || "") ||
      formData.firstName !== (initialData.firstName || "") ||
      formData.roleId !== initialData.role.id ||
      formData.password !== ""
    );
  }, [formData, initialData]);

  // 必須項目の入力チェック
  const isFormValid = useMemo(() => {
    // 姓と名のどちらかが入力されているかチェック
    const hasName =
      formData.lastName.trim() !== "" || formData.firstName.trim() !== "";

    // 新規作成時はメール、姓名のどちらか、ロール、パスワードが必須
    if (!initialData) {
      return (
        formData.email.trim() !== "" &&
        hasName &&
        formData.roleId !== "" &&
        formData.password.trim() !== ""
      );
    }

    // 編集時はメール、姓名のどちらか、ロールが必須
    return formData.email.trim() !== "" && hasName && formData.roleId !== "";
  }, [formData, initialData]);

  const isSubmitDisabled = isLoading || !hasChanges || !isFormValid;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await onSubmit(formData);
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>,
  ) => {
    setFormData((prev) => ({
      ...prev,
      [e.target.name]: e.target.value,
    }));
  };

  return (
    <div className="p-6 max-w-2xl mx-auto">
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-200">
          <h2 className="text-xl font-semibold text-gray-900">
            {initialData ? "ユーザー情報編集" : "新規ユーザー作成"}
          </h2>
        </div>

        <form onSubmit={handleSubmit} className="p-6 space-y-6">
          <div className="space-y-4">
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
                value={formData.email}
                onChange={handleChange}
                required={!initialData}
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
                  value={formData.lastName}
                  onChange={handleChange}
                  required={false}
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
                  value={formData.firstName}
                  onChange={handleChange}
                  required={false}
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
                value={formData.roleId}
                onChange={handleChange}
                required={!initialData}
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
                パスワード{initialData && " (変更する場合のみ入力)"}
              </label>
              <input
                id="password"
                name="password"
                type="password"
                value={formData.password}
                onChange={handleChange}
                required={!initialData}
                className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
                placeholder={
                  initialData ? "新しいパスワードを入力" : "パスワードを入力"
                }
              />
            </div>
          </div>

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

export default UserForm;
