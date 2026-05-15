import { useNavigate } from "react-router-dom";

interface BackButtonProps {
  /** ボタンのテキスト（デフォルト: "← 戻る"） */
  text?: string;
  /** 追加のCSSクラス */
  className?: string;
  /** カスタムの戻る処理（デフォルト: navigate(-1)） */
  onBack?: () => void;
}

/** 戻るボタンコンポーネント */
const BackButton = ({
  text = "← 戻る",
  className = "",
  onBack,
}: BackButtonProps) => {
  const navigate = useNavigate();

  const handleClick = () => {
    if (onBack) {
      onBack();
    } else {
      navigate(-1);
    }
  };

  return (
    <button
      onClick={handleClick}
      className={`px-4 py-2 text-sm font-medium text-gray-600 bg-white border border-gray-300 rounded-md hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors cursor-pointer ${className}`}
    >
      {text}
    </button>
  );
};

export default BackButton;
