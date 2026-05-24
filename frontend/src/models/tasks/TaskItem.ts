import type { BoardColumn } from "../columns/BoardColumn";

export type TaskItem = BoardColumn["taskItems"][0];

export type TaskItemPriority = TaskItem["priority"];
