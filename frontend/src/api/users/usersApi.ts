import type { AxiosInstance } from "axios";
import type { CurrentUser } from "../../models/users/CurrentUser";
import type { CurrentUserResponse } from "./usersDtos";
import { toCurrentUser } from "./usersMapper";

export class UsersApi {
  private readonly instance: AxiosInstance;

  public constructor(instance: AxiosInstance) {
    this.instance = instance;
  }

  public async getCurrentUser(): Promise<CurrentUser> {
    const url = "/api/users/me";
    const response = await this.instance.get<CurrentUserResponse>(url);

    return toCurrentUser(response.data);
  }
}
