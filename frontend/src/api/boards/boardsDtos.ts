import type { components } from "../generated/schema";

/** ボードの作成リクエスト */
export type CreateBoardRequest = components["schemas"]["CreateBoardRequest"];

/** ボードの更新リクエスト */
export type UpdateBoardRequest = components["schemas"]["UpdateBoardRequest"];

/** ボード一覧の取得レスポンス */
export type BoardSummaryResponse =
  components["schemas"]["BoardSummaryResponse"];

/** ボード詳細の取得レスポンス */
export type BoardDetailResponse = components["schemas"]["BoardDetailResponse"];
