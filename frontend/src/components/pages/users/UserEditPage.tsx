import { useParams, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";

import UserForm from "../../ui/forms/UserForm";
import BackButton from "../../ui/buttons/BackButton";
import type { UserDetail } from "../../../models/users/UserDetail";
import type { Role } from "../../../models/roles/Role";
import { apiClient } from "../../../api/clients/ApiClient";

/** ユーザー編集ページ */
const UserEditPage = () => {
  const { userId } = useParams();
  const navigate = useNavigate();
  const [editUer, setEditUser] = useState<UserDetail | null>(null);
  const [roles, setRoles] = useState<Role[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const initLoad = async () => {
      setIsLoading(true);

      try {
        const [rolesData, userData] = await Promise.all([
          apiClient.roles.getRoles(),
          userId ? apiClient.users.getUser(userId) : null,
        ]);

        setRoles(rolesData);
        if (userData) {
          setEditUser(userData);
        }
      } catch (e) {
        const error = await apiClient.parseHttpError(e);
        alert(error.response?.title ?? "ユーザー情報の取得に失敗しました。");
      } finally {
        setIsLoading(false);
      }
    };

    initLoad();
  }, [userId]);

  const handleSubmit = async (formData: {
    email: string;
    lastName: string;
    firstName: string;
    password: string;
    roleId: string;
  }) => {
    if (!userId || !editUer) return;

    setIsLoading(true);

    try {
      // 変更のあったパラメータのみを抽出
      const updateRequest: Record<string, any> = {};

      if (formData.email !== (editUer.email || "")) {
        updateRequest.email = formData.email;
      }

      if (formData.lastName !== (editUer.lastName || "")) {
        updateRequest.lastName = formData.lastName;
      }

      if (formData.firstName !== (editUer.firstName || "")) {
        updateRequest.firstName = formData.firstName;
      }

      if (formData.roleId !== editUer.role.id) {
        updateRequest.roleId = formData.roleId;
      }

      if (formData.password.trim() !== "") {
        updateRequest.password = formData.password;
      }

      await apiClient.users.updateUser(userId, updateRequest);

      alert("ユーザー情報を更新しました。");
      navigate("/users");
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "ユーザー情報の更新に失敗しました。");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div>
      <div className="mb-4">
        <BackButton />
      </div>
      <UserForm
        initialData={editUer || undefined}
        roles={roles}
        onSubmit={handleSubmit}
        isLoading={isLoading}
      />
    </div>
  );
};

export default UserEditPage;
