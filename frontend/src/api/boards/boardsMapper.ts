import type { BoardDetail } from "../../models/boards/BoardDetail";
import type { BoardSummary } from "../../models/boards/BoardSummary";
import type { BoardDetailResponse, BoardSummaryResponse } from "./boardsDtos";

export function toBoardSummary(dto: BoardSummaryResponse): BoardSummary {
  return dto;
}

export function toBoardDetail(dto: BoardDetailResponse): BoardDetail {
  return dto;
}
