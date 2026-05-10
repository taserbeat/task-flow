import { useParams } from "react-router-dom";

/** ユーザー詳細ページ */
const UserDetailPage = () => {
  const { userId } = useParams();

  return (
    <div>
      <h2>#UserDetailPage</h2>

      <div>userId: {userId}</div>
    </div>
  );
};

export default UserDetailPage;
