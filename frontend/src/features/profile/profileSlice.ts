import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import type { ConnectionStatus } from "../../api/common/connectionStatus";
import type { CurrentUser } from "../../models/users/CurrentUser";
import type { HttpError } from "../../api/common/httpError";
import type { AppDispatch, RootState } from "../../app/store";
import { apiClient } from "../../api/clients/ApiClient";

/** 自身のユーザー情報のステート */
export interface ProfileState {
  /** 通信状態 */
  connectionStatus: ConnectionStatus;

  /** ユーザー情報 */
  userInfo: CurrentUser | undefined;

  /** HTTPエラー */
  error: HttpError | undefined;
}

/** 初期値 */
const initialState: ProfileState = {
  connectionStatus: "idle",
  userInfo: undefined,
  error: undefined,
};

const profileSlice = createSlice({
  name: "profile",
  initialState: initialState,
  reducers: {},
  extraReducers(builder) {
    /** 自身のユーザー情報の取得: 開始 */
    builder.addCase(getCurrentUser.pending, (state) => {
      state.connectionStatus = "loading";
    });

    /** 自身のユーザー情報の取得: 成功 */
    builder.addCase(getCurrentUser.fulfilled, (state, action) => {
      state.connectionStatus = "succeeded";
      state.userInfo = action.payload;
      state.error = undefined;
    });

    /** 自身のユーザー情報の取得: 失敗 */
    builder.addCase(getCurrentUser.rejected, (state, action) => {
      state.connectionStatus = "failed";
      state.userInfo = undefined;
      state.error = action.payload;
    });
  },
});

//#region AsyncThunk

/** 自身のユーザー情報を取得する */
export const getCurrentUser = createAsyncThunk<
  CurrentUser,
  void,
  { rejectValue: HttpError; dispatch: AppDispatch; state: RootState }
>("profile/getCurrentUser", async (_, thunk) => {
  try {
    const response = await apiClient.users.getCurrentUser();
    return response;
  } catch (e) {
    const error = await apiClient.parseHttpError(e);
    return thunk.rejectWithValue(error);
  }
});

//#endregion

//#region Actions

export const {} = profileSlice.actions;

//#endregion

export default profileSlice.reducer;
