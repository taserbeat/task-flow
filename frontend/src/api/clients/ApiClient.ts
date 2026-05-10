import type { AxiosInstance } from "axios";
import { UsersApi } from "../users/usersApi";
import { axiosInstance } from "./axios";
import type { ErrorResponse, HttpError } from "../common/httpError";
import axios from "axios";

export class ApiClient {
  private readonly instance: AxiosInstance;

  users: UsersApi;

  constructor(instance: AxiosInstance) {
    this.instance = instance;

    this.users = new UsersApi(this.instance);
  }

  /** HTTPエラーをパースする */
  public async parseHttpError(e: unknown): Promise<HttpError> {
    if (!axios.isAxiosError<ErrorResponse>(e) || e.response === undefined) {
      // axiosのエラーではない場合
      return { status: "Unknown", response: undefined };
    }

    if (axios.isCancel(e)) {
      // 通信が明示的にキャンセルされた場合
      return { status: "Cancelled", response: undefined };
    }

    let errorResponse: ErrorResponse | undefined = undefined;

    if (e.response.data instanceof Blob) {
      try {
        const text = await e.response.data.text();
        errorResponse = JSON.parse(text) as ErrorResponse;
      } catch {
        errorResponse = undefined;
      }
    } else if (
      typeof e.response.data === "object" &&
      e.response.data !== null
    ) {
      errorResponse = e.response.data as ErrorResponse;
    }

    switch (e.response.status) {
      case 400:
        return { status: "BadRequest", response: errorResponse };

      case 401:
        return { status: "Unauthorized", response: errorResponse };

      case 403:
        return { status: "Forbidden", response: errorResponse };

      case 404:
        return { status: "NotFound", response: errorResponse };

      case 500:
        return {
          status: "InternalServerError",
          response: errorResponse,
        };

      default:
        break;
    }

    return { status: "Unknown", response: e.response.data };
  }
}

export const apiClient = new ApiClient(axiosInstance);
