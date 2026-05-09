import type { CurrentUser } from "../../models/users/CurrentUser";
import type { CurrentUserResponse } from "./usersDtos";

export function toCurrentUser(dto: CurrentUserResponse): CurrentUser {
  return {
    tenantId: dto.tenantId,
    userId: dto.userId,
    email: dto.email,
    roleName: dto.roleName,
    roleLevel: dto.roleLevel,
  };
}
