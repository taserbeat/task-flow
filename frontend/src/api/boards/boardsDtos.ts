import type { components } from "../generated/schema";

/** ボードの作成リクエスト */
export type CreateBoardRequest = components["schemas"]["CreateBoardRequest"];

/** ボード一覧の取得レスポンス */
export type BoardSummaryResponse =
  components["schemas"]["BoardSummaryResponse"];

/** ボード詳細の取得レスポンス */
export type BoardDetailResponse = components["schemas"]["BoardDetailResponse"];

/** ボードの更新リクエスト */
export type UpdateBoardRequest = components["schemas"]["UpdateBoardRequest"];

/** 列の作成リクエスト */
export type CreateBoardColumnRequest =
  components["schemas"]["CreateBoardColumnRequest"];

/** 列の更新リクエスト */
export type UpdateBoardColumnRequest =
  components["schemas"]["UpdateBoardColumnRequest"];

/** タスクの作成リクエスト */
export type CreateTaskItemRequest =
  components["schemas"]["CreateTaskItemRequest"];

/** タスクの更新リクエスト */
export type UpdateTaskItemRequest =
  components["schemas"]["UpdateTaskItemRequest"];
