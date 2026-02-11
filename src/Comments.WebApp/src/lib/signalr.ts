import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";
import type { Comment } from "./types";

let connection: HubConnection | null = null;

export function createConnection(): HubConnection {
  if (connection) {
    return connection;
  }

  connection = new HubConnectionBuilder()
    .withUrl("/hubs/comments")
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();

  return connection;
}

export function onCommentCreated(
  hubConnection: HubConnection,
  callback: (comment: Comment) => void
): void {
  hubConnection.on("CommentCreated", callback);
}

export function offCommentCreated(hubConnection: HubConnection): void {
  hubConnection.off("CommentCreated");
}

export function getConnection(): HubConnection | null {
  return connection;
}

export function resetConnection(): void {
  connection = null;
}
