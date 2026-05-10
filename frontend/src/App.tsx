import { useEffect } from "react";
import {
  BrowserRouter,
  Outlet,
  Route,
  Routes,
  useNavigate,
} from "react-router-dom";

import { useAppDispatch, useAppSelector } from "./app/hook";
import { getCurrentUser } from "./features/profile/profileSlice";
import { routes } from "./routers/routeConfig";
import MainTemplate from "./components/templates/MainTemplate";
import ProtectedRoute from "./routers/ProtectedRoute";

function App() {
  const dispatch = useAppDispatch();
  const roleName = useAppSelector((root) => root.profile.userInfo?.roleName);

  useEffect(() => {
    const initLoad = async () => {
      await dispatch(getCurrentUser());
    };

    initLoad();
  }, [dispatch]);

  return (
    <BrowserRouter>
      <Routes>
        <Route
          element={
            <MainTemplate>
              <Outlet />
            </MainTemplate>
          }
        >
          {routes
            .filter(
              (route) =>
                roleName && route.roles && route.roles.includes(roleName),
            )
            .map((route) => (
              <Route
                key={route.path}
                path={route.path}
                element={
                  <ProtectedRoute roles={route.roles}>
                    <Outlet />
                  </ProtectedRoute>
                }
              >
                {/* 親ルート自体のコンテンツ（index ルート） */}
                <Route
                  index
                  element={
                    <ProtectedRoute roles={route.roles}>
                      {route.element}
                    </ProtectedRoute>
                  }
                />

                {/* 子ルートがある場合は親と同じロールでネストルートを設定 */}
                {route.children?.map((childRoute) => (
                  <Route
                    key={childRoute.path}
                    path={childRoute.path}
                    element={
                      <ProtectedRoute roles={route.roles}>
                        {childRoute.element}
                      </ProtectedRoute>
                    }
                  />
                ))}
              </Route>
            ))}
        </Route>

        {/* 未定義のURLパスに来た場合はデフォルトのリダイレクト処理を行う */}
        <Route path="*" element={<DefaultRedirect />} />
      </Routes>
    </BrowserRouter>
  );
}

/** デフォルトのリダイレクト処理を行うコンポーネント */
const DefaultRedirect = () => {
  const roleName = useAppSelector((state) => state.profile.userInfo?.roleName);
  const navigate = useNavigate();

  useEffect(() => {
    if (roleName) {
      const accesibleRoutes = routes.filter(
        (route) => route.roles && route.roles.includes(roleName),
      );

      if (accesibleRoutes.length > 0) {
        navigate(accesibleRoutes[0].path, { replace: true });
        return;
      }

      // アクセス可能なメニューが無い場合はルートパスへ
      navigate("/", { replace: true });
    }
  }, [roleName, navigate]);

  return <></>;
};

export default App;
