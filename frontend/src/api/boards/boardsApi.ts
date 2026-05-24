import type { AxiosInstance } from "axios";

import type {
  CreateBoardRequest,
  BoardSummaryResponse,
  UpdateBoardRequest,
} from "./boardsDtos";
import type { BoardSummary } from "../../models/boards/BoardSummary";
import { toBoardSummary } from "./boardsMapper";

export class BoardsApi {
  private readonly instance: AxiosInstance;

  public constructor(instance: AxiosInstance) {
    this.instance = instance;
  }

  /** ボードを作成する */
  public async createBoard(request: CreateBoardRequest): Promise<void> {
    const url = "/api/boards";
    await this.instance.post(url, {
      ...request,
    });
  }

  /** ボードの一覧を取得する */
  public async getBoards(): Promise<BoardSummary[]> {
    const url = "/api/boards";
    const response = await this.instance.get<BoardSummaryResponse[]>(url);

    return response.data.map((x) => toBoardSummary(x));
  }

  /** ボードを更新する */
  public async updateBoard(
    boardId: string,
    request: UpdateBoardRequest,
  ): Promise<void> {
    const url = `/api/boards/${boardId}`;
    await this.instance.put(url, {
      ...request,
    });
  }

  /** ボードを削除する */
  public async deleteBoard(boardId: string): Promise<void> {
    const url = `/api/boards/${boardId}`;
    await this.instance.delete(url);
  }
}
