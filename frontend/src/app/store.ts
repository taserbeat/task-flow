import {
  configureStore,
  type Action,
  type ThunkAction,
} from "@reduxjs/toolkit";

import profileReducer from "../features/profile/profileSlice";

/** アプリ全体で管理するステートのReduxストア */
export const store = configureStore({
  reducer: {
    profile: profileReducer,
  },
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk<ReturnType = void> = ThunkAction<
  ReturnType,
  RootState,
  unknown,
  Action<string>
>;
