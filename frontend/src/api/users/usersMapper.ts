import type { CurrentUser } from "../../models/users/CurrentUser";
import type { UserDetail } from "../../models/users/UserDetail";
import type { UserSummary } from "../../models/users/UserSummary";
import type {
  CurrentUserResponse,
  UserDetailResponse,
  UserSummaryResponse,
} from "./usersDtos";

export function toCurrentUser(dto: CurrentUserResponse): CurrentUser {
  return dto;
}

export function toUserSummary(dto: UserSummaryResponse): UserSummary {
  return dto;
}

export function toUserDetail(dto: UserDetailResponse): UserDetail {
  return dto;
}
