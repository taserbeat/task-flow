import type React from "react";

import type { RoleName } from "../models/roles/Role";
import TaskIndexPage from "../components/pages/tasks/TaskIndexPage";
import UserIndexPage from "../components/pages/users/UserIndexPage";
import UserDetailPage from "../components/pages/users/UserDetailPage";
import TenantIndexPage from "../components/pages/tenants/TenantIndexPage";

/** ルーティング設定 */
type RouteConfig = {
  /** URLパス */
  path: string;

  /** Reactコンポーネント */
  element: React.ReactNode;

  /** ラベル */
  label?: string;

  /** ネストするルーティング設定 */
  children?: RouteConfig[];

  /** 画面表示するロール名 */
  roles?: RoleName[];
};

/** アプリケーションのルーティング設定 */
export const routes: RouteConfig[] = [
  {
    path: "/tasks",
    label: "タスク",
    element: <TaskIndexPage />,
    roles: ["SystemAdmin", "Admin", "User"],
    children: [],
  },
  {
    path: "/users",
    label: "ユーザー管理",
    element: <UserIndexPage />,
    roles: ["SystemAdmin", "Admin"],
    children: [
      {
        path: "/users/:userId",
        element: <UserDetailPage />,
      },
    ],
  },
  {
    path: "/tenants",
    label: "テナント管理",
    element: <TenantIndexPage />,
    roles: ["SystemAdmin"],
    children: [],
  },
];
