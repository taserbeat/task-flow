import { useParams } from "react-router-dom";

/** ユーザー編集ページ */
const UserEditPage = () => {
  const { userId } = useParams();

  return (
    <div>
      <h2>#UserDetailPage</h2>

      <div>userId: {userId}</div>
    </div>
  );
};

export default UserEditPage;
