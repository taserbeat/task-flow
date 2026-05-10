import {
  useDispatch,
  useSelector,
  type TypedUseSelectorHook,
} from "react-redux";
import type { RootState, AppDispatch } from "./store";

/** Reduxストアにアクションを通知するDispatcher */
export const useAppDispatch = () => useDispatch<AppDispatch>();

/** Reduxストアのステートを取得するSelector */
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;
