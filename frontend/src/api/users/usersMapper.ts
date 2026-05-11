import type { CurrentUser } from "../../models/users/CurrentUser";
import type { CurrentUserResponse } from "./usersDtos";

export function toCurrentUser(dto: CurrentUserResponse): CurrentUser {
  return dto;
}
