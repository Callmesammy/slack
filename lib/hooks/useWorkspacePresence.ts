"use client";

import { useEffect, useRef } from "react";

import { useChatHub } from "@/lib/hooks/useChatHub";

export function useWorkspacePresence(workspaceId: string | null) {
  const { connection, isConnected } = useChatHub();
  const timer = useRef<number | null>(null);

  useEffect(() => {
    if (!connection || !isConnected || !workspaceId) return;
    let cancelled = false;

    const join = () => connection.invoke("JoinWorkspace", workspaceId).catch(() => {});
    join();

    const onReconnected = () => {
      if (cancelled) return;
      join();
      connection.invoke("PresenceHeartbeat", workspaceId).catch(() => {});
    };

    connection.onreconnected(onReconnected);

    const heartbeat = () => {
      if (cancelled) return;
      connection.invoke("PresenceHeartbeat", workspaceId).catch(() => {});
    };

    heartbeat();
    timer.current = window.setInterval(heartbeat, 30_000);

    return () => {
      cancelled = true;
      connection.offreconnected(onReconnected);
      if (timer.current) window.clearInterval(timer.current);
      timer.current = null;
      connection.invoke("LeaveWorkspace", workspaceId).catch(() => {});
    };
  }, [connection, isConnected, workspaceId]);
}
