import React from "react";
import { NavLink } from "react-router-dom";

import { useAppSelector } from "../../../app/hook";
import { routes } from "../../../routers/routeConfig";

/** サイドバー */
const Sidebar: React.FC = () => {
  const roleName = useAppSelector(
    (root) => root.profile.userInfo?.user.role.name,
  );

  if (!roleName) {
    return <></>;
  }

  return (
    <ul className="">
      {routes
        .filter((route) => route.roles && route.roles.includes(roleName))
        .map((route) => {
          return (
            <li key={route.path} className="">
              <NavLink
                to={route.path}
                className="flex items-center w-full h-10 tex2t-xl hover:bg-emerald-400 px-4 gap-3"
              >
                {route.icon && <span className="shrink-0">{route.icon}</span>}

                <span>{route.label}</span>
              </NavLink>
            </li>
          );
        })}
    </ul>
  );
};

export default Sidebar;
