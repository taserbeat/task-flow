/** HTTPエラー */
export type HttpError = {
  /** HTTPエラーのステータス */
  status: HttpErrorStatus | undefined;

  /** エラーレスポンス */
  response: ErrorResponse | undefined;
};

/** HTTP通信エラーのステータス */
export type HttpErrorStatus =
  | "BadRequest" // 400
  | "Unauthorized" // 401
  | "Forbidden" // 403
  | "NotFound" // 404
  | "InternalServerError" // 500
  | "Cancelled" // 通信キャンセル
  | "Unknown"; // その他

/** HTTPエラーのレスポンス */
export type ErrorResponse = {
  /** メッセージ */
  title: string;

  /** ステータスコード */
  status: number;
};
