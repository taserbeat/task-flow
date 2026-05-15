import type { RoleDetailResponse } from "../../api/roles/rolesDtos";

/** ロール */
export type Role = RoleDetailResponse;

/** ロール名 */
export type RoleName = RoleDetailResponse["name"];
