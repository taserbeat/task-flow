import type { components } from "../generated/schema";

/** ユーザーの作成リクエスト */
export type CreateUserRequest = components["schemas"]["CreateUserRequest"];

/** ユーザーの更新リクエスト */
export type UpdateUserRequest = components["schemas"]["UpdateUserRequest"];

/** ユーザー一覧の取得レスポンス */
export type UserSummaryResponse = components["schemas"]["UserSummaryResponse"];

/** 自身のユーザー情報の取得レスポンス */
export type CurrentUserResponse = components["schemas"]["CurrentUserResponse"];

/** ユーザー詳細の取得レスポンス */
export type UserDetailResponse = components["schemas"]["UserDetailResponse"];
