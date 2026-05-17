import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";

import TenantForm from "../../ui/forms/TenantForm";
import BackButton from "../../ui/buttons/BackButton";
import type { TenantDetail } from "../../../models/tenants/TenantDetail";
import type { Role } from "../../../models/roles/Role";
import { apiClient } from "../../../api/clients/ApiClient";
import { useAppDispatch, useAppSelector } from "../../../app/hook";
import { getCurrentUser } from "../../../features/profile/profileSlice";

/** テナントの編集ページ */
const TenantEditPage = () => {
  const { tenantId } = useParams();
  const navigate = useNavigate();

  const currentTenant = useAppSelector((root) => root.profile.userInfo?.tenant);
  const dispatch = useAppDispatch();

  const [tenant, setTenant] = useState<TenantDetail | null>(null);
  const [roles, setRoles] = useState<Role[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const initLoad = async () => {
      if (!tenantId) return;

      setIsLoading(true);

      try {
        const [tenantData, rolesData] = await Promise.all([
          apiClient.tenants.getTenant(tenantId),
          apiClient.roles.getRoles(),
        ]);
        setTenant(tenantData);
        setRoles(rolesData);
      } catch (e) {
        const error = await apiClient.parseHttpError(e);
        alert(error.response?.title ?? "テナント情報の取得に失敗しました。");
        navigate("/tenants");
      } finally {
        setIsLoading(false);
      }
    };

    initLoad();
  }, [tenantId, navigate]);

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
    if (!tenantId) return;

    setIsLoading(true);

    try {
      await apiClient.tenants.updateTenant(tenantId, {
        name: formData.name,
      });

      if (tenantId === currentTenant?.id) {
        await dispatch(getCurrentUser());
      }

      navigate("/tenants");
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "テナント情報の更新に失敗しました。");
    } finally {
      setIsLoading(false);
    }
  };

  if (!tenant) {
    return (
      <div className="p-6 max-w-2xl mx-auto">
        <div className="mb-4">
          <BackButton />
        </div>
        <div className="text-center py-12 text-gray-500">読み込み中...</div>
      </div>
    );
  }

  return (
    <div>
      <div className="mb-4">
        <BackButton />
      </div>

      <TenantForm
        initialData={tenant}
        roles={roles}
        onSubmit={handleSubmit}
        isLoading={isLoading}
      />
    </div>
  );
};

export default TenantEditPage;
