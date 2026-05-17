import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

import UserForm from "../../ui/forms/UserForm";
import BackButton from "../../ui/buttons/BackButton";
import type { Role } from "../../../models/roles/Role";
import { apiClient } from "../../../api/clients/ApiClient";

/** ユーザーの新規作成ページ */
const UserNewPage = () => {
  const navigate = useNavigate();
  const [roles, setRoles] = useState<Role[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const initLoad = async () => {
      setIsLoading(true);

      try {
        const rolesData = await apiClient.roles.getRoles();
        setRoles(rolesData);
      } catch (e) {
        const error = await apiClient.parseHttpError(e);
        alert(error.response?.title ?? "ロール情報の取得に失敗しました。");
      } finally {
        setIsLoading(false);
      }
    };

    initLoad();
  }, []);

  const handleSubmit = async (formData: {
    email: string;
    lastName: string;
    firstName: string;
    password: string;
    roleId: string;
  }) => {
    setIsLoading(true);

    try {
      await apiClient.users.createUser({
        email: formData.email,
        lastName: formData.lastName,
        firstName: formData.firstName,
        password: formData.password,
        roleId: formData.roleId,
      });

      navigate("/users");
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "ユーザーの作成に失敗しました。");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div>
      <div className="mb-4">
        <BackButton />
      </div>
      <UserForm roles={roles} onSubmit={handleSubmit} isLoading={isLoading} />
    </div>
  );
};

export default UserNewPage;
