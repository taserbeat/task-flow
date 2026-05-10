import type React from "react";

import { useAppSelector } from "../app/hook";
import type { RoleName } from "../models/roles/Role";

interface ProtectedRoute {
  roles?: RoleName[];
  children: React.ReactNode;
}

/** ルーティングの認可保護 */
const ProtectedRoute: React.FC<ProtectedRoute> = ({ roles, children }) => {
  const profile = useAppSelector((root) => root.profile);

  if (
    profile.connectionStatus === "idle" ||
    profile.connectionStatus === "loading" ||
    !profile.userInfo
  ) {
    // 自身のユーザー情報を取得するまでは空のコンポーネント
    return <></>;
  }

  const canAccess = roles && roles.includes(profile.userInfo?.roleName);
  if (!canAccess) {
    // アクセスできない場合は空のコンポーネント
    return <></>;
  }

  return children;
};

export default ProtectedRoute;
