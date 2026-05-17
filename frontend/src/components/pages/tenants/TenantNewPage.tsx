import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

import TenantForm from "../../ui/forms/TenantForm";
import BackButton from "../../ui/buttons/BackButton";
import type { Role } from "../../../models/roles/Role";
import { apiClient } from "../../../api/clients/ApiClient";

/** テナントの新規作成ページ */
const TenantNewPage = () => {
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
    name: string;
    initUser?: {
      email: string;
      lastName: string;
      firstName: string;
      password: string;
      roleId: string;
    };
  }) => {
    setIsLoading(true);

    try {
      await apiClient.tenants.createTenant({
        name: formData.name,
        initUser: formData.initUser!,
      });

      navigate("/tenants");
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "テナントの作成に失敗しました。");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div>
      <div className="mb-4">
        <BackButton />
      </div>

      <TenantForm roles={roles} onSubmit={handleSubmit} isLoading={isLoading} />
    </div>
  );
};

export default TenantNewPage;
