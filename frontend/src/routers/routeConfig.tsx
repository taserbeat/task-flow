import type React from "react";
import {
  RectangleStackIcon,
  UserIcon,
  BuildingOfficeIcon,
} from "@heroicons/react/24/solid";

import type { RoleName } from "../models/roles/Role";
import BoardIndexPage from "../components/pages/tasks/BoardIndexPage";
import BoardNewPage from "../components/pages/tasks/BoardNewPage";
import UserIndexPage from "../components/pages/users/UserIndexPage";
import UserNewPage from "../components/pages/users/UserNewPage";
import UserEditPage from "../components/pages/users/UserEditPage";
import TenantIndexPage from "../components/pages/tenants/TenantIndexPage";
import TenantNewPage from "../components/pages/tenants/TenantNewPage";
import TenantEditPage from "../components/pages/tenants/TenantEditPage";
import BoardEditPage from "../components/pages/tasks/BoardEditPage";

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
    path: "/boards",
    element: <BoardIndexPage />,
    label: "ボード",
    icon: <RectangleStackIcon className="w-5 h-5" />,
    roles: ["SystemAdmin", "Admin", "User"],
    children: [
      {
        path: "/boards/new",
        element: <BoardNewPage />,
      },
      {
        path: "/boards/:boardId/edit",
        element: <BoardEditPage />,
      },
    ],
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
    children: [
      {
        path: "/tenants/new",
        element: <TenantNewPage />,
      },
      {
        path: "/tenants/:tenantId/edit",
        element: <TenantEditPage />,
      },
    ],
  },
];
