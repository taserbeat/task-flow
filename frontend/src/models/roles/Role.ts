import type { CurrentUserResponse } from "../../api/users/usersDtos";

/** ロール名 */
export type RoleName = CurrentUserResponse["user"]["role"]["name"];
