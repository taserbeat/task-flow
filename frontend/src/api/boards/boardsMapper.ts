import type { BoardSummary } from "../../models/boards/BoardSummary";
import type { BoardSummaryResponse } from "./boardsDtos";

export function toBoardSummary(dto: BoardSummaryResponse): BoardSummary {
  return dto;
}
