import { createSlice } from "@reduxjs/toolkit";

/** プロフィールの状態 */
export interface ProfileState {}

/** 初期値 */
const initialState: ProfileState = {};

const profileSlice = createSlice({
  name: "profile",
  initialState: initialState,
  reducers: {},
});

//#region AsyncThunk

// Profile情報を取得するAsyncThunkを実装する

//#endregion

//#region Actions

// TODO: Actionをexportする

//#endregion

export default profileSlice.reducer;
