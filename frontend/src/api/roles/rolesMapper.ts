import type { Role } from "../../models/roles/Role";
import type { RoleDetailResponse } from "./rolesDtos";

export function toRole(dto: RoleDetailResponse): Role {
  return dto;
}
