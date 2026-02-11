"use client";

import { useEffect, useRef } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { HubConnectionState } from "@microsoft/signalr";
import {
  createConnection,
  onCommentCreated,
  offCommentCreated,
  resetConnection,
} from "@/lib/signalr";

export function useSignalR() {
  const queryClient = useQueryClient();
  const connectionRef = useRef<ReturnType<typeof createConnection> | null>(
    null
  );

  useEffect(() => {
    const connection = createConnection();
    connectionRef.current = connection;

    onCommentCreated(connection, () => {
      queryClient.invalidateQueries({ queryKey: ["comments"] });
    });

    if (connection.state === HubConnectionState.Disconnected) {
      connection.start().catch((err) => {
        console.error("SignalR connection error:", err);
      });
    }

    return () => {
      offCommentCreated(connection);
      if (connection.state === HubConnectionState.Connected) {
        connection.stop().then(() => {
          resetConnection();
        });
      }
    };
  }, [queryClient]);

  return connectionRef;
}
