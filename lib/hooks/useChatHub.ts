"use client";

import { useEffect, useMemo, useState } from "react";
import type { HubConnection } from "@microsoft/signalr";
import { useSession } from "next-auth/react";

import { ensureChatHubStarted, getChatHubConnection } from "@/lib/signalr";

export function useChatHub() {
  const { data: session, status } = useSession();
  const token = session?.apiJwt ?? "";

  const connection = useMemo<HubConnection | null>(() => {
    if (!token) return null;
    return getChatHubConnection(token);
  }, [token]);

  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    if (!connection) return;
    let cancelled = false;

    const onReconnected = () => {
      if (!cancelled) setIsConnected(true);
    };
    const onClose = () => {
      if (!cancelled) setIsConnected(false);
    };

    connection.onreconnected(onReconnected);
    connection.onclose(onClose);

    ensureChatHubStarted(connection)
      .then(() => {
        if (!cancelled) setIsConnected(true);
      })
      .catch(() => {
        if (!cancelled) setIsConnected(false);
      });

    return () => {
      cancelled = true;
      connection.offreconnected(onReconnected);
      connection.offclose(onClose);
    };
  }, [connection]);

  return {
    status,
    connection,
    isConnected,
  };
}

