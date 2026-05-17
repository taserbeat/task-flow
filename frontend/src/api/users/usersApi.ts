import type { AxiosInstance } from "axios";

import type { CurrentUser } from "../../models/users/CurrentUser";
import type {
  CreateUserRequest,
  CurrentUserResponse,
  UpdateUserRequest,
  UserDetailResponse,
  UserSummaryResponse,
} from "./usersDtos";
import { toCurrentUser, toUserDetail, toUserSummary } from "./usersMapper";
import type { UserDetail } from "../../models/users/UserDetail";
import type { UserSummary } from "../../models/users/UserSummary";

export class UsersApi {
  private readonly instance: AxiosInstance;

  public constructor(instance: AxiosInstance) {
    this.instance = instance;
  }

  /** 自身のユーザー情報を取得する */
  public async getCurrentUser(): Promise<CurrentUser> {
    const url = "/api/users/me";
    const response = await this.instance.get<CurrentUserResponse>(url);

    return toCurrentUser(response.data);
  }

  /** ユーザーを作成する */
  public async createUser(request: CreateUserRequest): Promise<void> {
    const url = "/api/users";
    await this.instance.post(url, {
      ...request,
    });
  }

  /** ユーザーの一覧を取得する */
  public async getUsers(): Promise<UserSummary[]> {
    const url = "/api/users";
    const response = await this.instance.get<UserSummaryResponse[]>(url);

    return response.data.map((x) => toUserSummary(x));
  }

  /** ユーザーの詳細情報を取得する */
  public async getUser(userId: string): Promise<UserDetail> {
    const url = `/api/users/${userId}`;
    const response = await this.instance.get<UserDetailResponse>(url);

    return toUserDetail(response.data);
  }

  /** ユーザーを更新する */
  public async updateUser(
    userId: string,
    request: UpdateUserRequest,
  ): Promise<void> {
    const url = `/api/users/${userId}`;
    await this.instance.put(url, {
      ...request,
    });
  }

  /** ユーザーを削除する */
  public async deleteUser(userId: string): Promise<void> {
    const url = `/api/users/${userId}`;
    await this.instance.delete(url);
  }
}
