import type { AxiosInstance } from "axios";

import type {
  CreateBoardRequest,
  BoardSummaryResponse,
  BoardDetailResponse,
  UpdateBoardRequest,
  CreateBoardColumnRequest,
  UpdateBoardColumnRequest,
  CreateTaskItemRequest,
  UpdateTaskItemRequest,
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

  /** ボードの詳細を取得する */
  public async getBoard(boardId: string): Promise<BoardDetailResponse> {
    const url = `/api/boards/${boardId}`;
    const response = await this.instance.get<BoardDetailResponse>(url);
    return response.data;
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

  /** 列を作成する */
  public async createBoardColumn(
    boardId: string,
    request: CreateBoardColumnRequest,
  ): Promise<void> {
    const url = `/api/boards/${boardId}/columns`;
    await this.instance.post(url, {
      ...request,
    });
  }

  /** 列を更新する */
  public async updateBoardColumn(
    boardId: string,
    columnId: string,
    request: UpdateBoardColumnRequest,
  ): Promise<void> {
    const url = `/api/boards/${boardId}/columns/${columnId}`;
    await this.instance.put(url, {
      ...request,
    });
  }

  /** 列を削除する */
  public async deleteBoardColumn(
    boardId: string,
    columnId: string,
  ): Promise<void> {
    const url = `/api/boards/${boardId}/columns/${columnId}`;
    await this.instance.delete(url);
  }

  /** タスクを作成する */
  public async createTaskItem(
    boardId: string,
    columnId: string,
    request: CreateTaskItemRequest,
  ): Promise<void> {
    const url = `/api/boards/${boardId}/columns/${columnId}/tasks`;
    await this.instance.post(url, {
      ...request,
    });
  }

  /** タスクを更新する */
  public async updateTaskItem(
    boardId: string,
    columnId: string,
    taskId: string,
    request: UpdateTaskItemRequest,
  ): Promise<void> {
    const url = `/api/boards/${boardId}/columns/${columnId}/tasks/${taskId}`;
    await this.instance.put(url, {
      ...request,
    });
  }

  /** タスクを削除する */
  public async deleteTaskItem(
    boardId: string,
    columnId: string,
    taskId: string,
  ): Promise<void> {
    const url = `/api/boards/${boardId}/columns/${columnId}/tasks/${taskId}`;
    await this.instance.delete(url);
  }
}
