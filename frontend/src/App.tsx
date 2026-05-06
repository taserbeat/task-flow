import { useState } from "react";

import "./App.css";

function App() {
  const [count, setCount] = useState(0);

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
    </div>
  );
}

export default App;
