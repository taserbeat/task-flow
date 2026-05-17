import type { components } from "../generated/schema";

/** テナントの作成リクエスト */
export type CreateTenantRequest = components["schemas"]["CreateTenantRequest"];

/** テナントの更新リクエスト */
export type UpdateTenantRequest = components["schemas"]["UpdateTenantRequest"];

/** テナント一覧の取得レスポンス */
export type TenantSummaryResponse =
  components["schemas"]["TenantSummaryResponse"];

/** テナント詳細の取得レスポンス */
export type TenantDetailResponse =
  components["schemas"]["TenantDetailResponse"];
