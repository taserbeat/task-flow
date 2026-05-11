import type React from "react";
import type { ReactNode } from "react";

import { useAppSelector } from "../../app/hook";
import logo from "../../assets/logo.svg";
import Sidebar from "../ui/sidebar/Sidebar";
import { NavLink } from "react-router-dom";

/** メインテンプレートのProps */
interface MainTemplateProps {
  /** コンテンツ */
  children?: ReactNode;
}

/** メインテンプレート */
const MainTemplate: React.FC<MainTemplateProps> = ({ children }) => {
  const userInfo = useAppSelector((state) => state.profile.userInfo);

  return (
    <div className="flex flex-col h-screen  text-black">
      {/* フッター */}
      <footer className="h-10 bg-slate-800 text-white flex items-center px-4 gap-4">
        <NavLink to="/">
          <div className="flex">
            <img src={logo} alt="logo" className="h-6 w-6" />
            <h1 className="font-bold ml-1">TaskFlow</h1>
          </div>
        </NavLink>

        <div className="flex ml-16">
          {/* TODO: テナント名に変更 */}
          <span className="ml-2 mr-4">テナント: {userInfo?.tenant.id}</span>

          {/* TODO: 氏名に変更 */}
          <span>ユーザー: {userInfo?.user.email}</span>
        </div>

        <a
          href="/auth/logout"
          className="h-full flex items-center px-4 ml-auto hover:bg-slate-700"
        >
          ログアウト
        </a>
      </footer>

      {/* Body */}
      <div className="flex flex-1 overflow-hidden">
        {/* サイドメニュー */}
        <div className="w-50 text-white bg-slate-800 flex flex-col">
          <Sidebar />

          <div className="mt-auto flex justify-center pr-2 pb-2">
            <span className="text-sm">ver. {__APP_VERSION__}</span>
          </div>
        </div>

        {/* ページコンテンツ */}
        <div className="flex-1 overflow-auto p-2 bg-gray-100 ">{children}</div>
      </div>
    </div>
  );
};

export default MainTemplate;
