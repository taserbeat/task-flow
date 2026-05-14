import type React from "react";
import {
  RectangleStackIcon,
  UserIcon,
  BuildingOfficeIcon,
} from "@heroicons/react/24/solid";

import type { RoleName } from "../models/roles/Role";
import TaskIndexPage from "../components/pages/tasks/TaskIndexPage";
import UserIndexPage from "../components/pages/users/UserIndexPage";
import UserNewPage from "../components/pages/users/UserNewPage";
import UserEditPage from "../components/pages/users/UserEditPage";
import TenantIndexPage from "../components/pages/tenants/TenantIndexPage";

/** ルーティング設定 */
type RouteConfig = {
  /** URLパス */
  path: string;

  /** Reactコンポーネント */
  element: React.ReactNode;

  /** ラベル */
  label?: string;

  /** アイコン */
  icon?: React.ReactNode;

  /** ネストするルーティング設定 */
  children?: RouteConfig[];

  /** 画面表示するロール名 */
  roles?: RoleName[];
};

/**
 * アプリケーションのルーティング設定
 *
 * NOTE:
 * アイコンは下記サイトを参照して取得できる
 * https://heroicons.com/solid
 */
export const routes: RouteConfig[] = [
  {
    path: "/tasks",
    element: <TaskIndexPage />,
    label: "タスク",
    icon: <RectangleStackIcon className="w-5 h-5" />,
    roles: ["SystemAdmin", "Admin", "User"],
    children: [],
  },
  {
    path: "/users",
    element: <UserIndexPage />,
    label: "ユーザー管理",
    icon: <UserIcon className="w-5 h-5" />,
    roles: ["SystemAdmin", "Admin"],
    children: [
      {
        path: "/users/new",
        element: <UserNewPage />,
      },
      {
        path: "/users/:userId/edit",
        element: <UserEditPage />,
      },
    ],
  },
  {
    path: "/tenants",
    element: <TenantIndexPage />,
    label: "テナント管理",
    icon: <BuildingOfficeIcon className="w-5 h-5" />,
    roles: ["SystemAdmin"],
    children: [],
  },
];
