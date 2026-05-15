import type { AxiosInstance } from "axios";
import type { Role } from "../../models/roles/Role";
import { toRole } from "./rolesMapper";
import type { RoleDetailResponse } from "./rolesDtos";

export class RolesApi {
  private readonly instance: AxiosInstance;

  public constructor(instance: AxiosInstance) {
    this.instance = instance;
  }

  /** ロール一覧を取得する */
  public async getRoles(): Promise<Role[]> {
    const url = "/api/roles";
    const response = await this.instance.get<RoleDetailResponse[]>(url);

    return response.data.map((x) => toRole(x));
  }
}
