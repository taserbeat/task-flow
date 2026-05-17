/**
 * 日時を「YYYY/MM/DD HH:mm:ss」形式でフォーマット
 * @param dateString - 日時文字列またはDateオブジェクト
 * @returns フォーマットされた日時文字列
 */
export const formatDateTime = (dateString: string | Date): string => {
  const date =
    typeof dateString === "string" ? new Date(dateString) : dateString;
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");
  const seconds = String(date.getSeconds()).padStart(2, "0");

  return `${year}/${month}/${day} ${hours}:${minutes}:${seconds}`;
};

/**
 * 日付を「YYYY/MM/DD」形式でフォーマット
 * @param dateString - 日時文字列またはDateオブジェクト
 * @returns フォーマットされた日付文字列
 */
export const formatDate = (dateString: string | Date): string => {
  const date =
    typeof dateString === "string" ? new Date(dateString) : dateString;
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");

  return `${year}/${month}/${day}`;
};

/**
 * 時刻を「HH:mm:ss」形式でフォーマット
 * @param dateString - 日時文字列またはDateオブジェクト
 * @returns フォーマットされた時刻文字列
 */
export const formatTime = (dateString: string | Date): string => {
  const date =
    typeof dateString === "string" ? new Date(dateString) : dateString;
  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");
  const seconds = String(date.getSeconds()).padStart(2, "0");

  return `${hours}:${minutes}:${seconds}`;
};
