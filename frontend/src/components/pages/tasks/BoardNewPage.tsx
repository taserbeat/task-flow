import { useState } from "react";
import { useNavigate } from "react-router-dom";

import BoardForm from "../../ui/forms/BoardForm";
import BackButton from "../../ui/buttons/BackButton";
import { apiClient } from "../../../api/clients/ApiClient";

/** ボードの新規作成ページ */
const BoardNewPage = () => {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (formData: { name: string }) => {
    setIsLoading(true);

    try {
      await apiClient.boards.createBoard({
        name: formData.name,
      });

      navigate("/boards");
    } catch (e) {
      const error = await apiClient.parseHttpError(e);
      alert(error.response?.title ?? "ボードの作成に失敗しました。");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div>
      <div className="mb-4">
        <BackButton />
      </div>

      <BoardForm onSubmit={handleSubmit} isLoading={isLoading} />
    </div>
  );
};

export default BoardNewPage;
