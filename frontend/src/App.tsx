import { useEffect, useState } from "react";

import { useAppDispatch } from "./app/hook";
import { getCurrentUser } from "./features/profile/profileSlice";

function App() {
  const dispatch = useAppDispatch();
  const [count, setCount] = useState(0);

  useEffect(() => {
    const initLoad = async () => {
      await dispatch(getCurrentUser());
    };

    initLoad();
  }, [dispatch]);

  return (
    <div>
      <p>{count}</p>

      <button
        onClick={() => {
          setCount((prev) => prev + 1);
        }}
      >
        +1
      </button>

      <button
        onClick={() => {
          setCount((prev) => prev - 1);
        }}
      >
        -1
      </button>

      <div style={{ margin: "1rem 0 0 0" }}>
        <a className="link logout" href="/auth/logout">
          ログアウト
        </a>
      </div>
    </div>
  );
}

export default App;
