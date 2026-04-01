"use client";

import { useEffect } from "react";

import { useChatHub } from "@/lib/hooks/useChatHub";
import { useWorkspaceBySlug } from "@/lib/hooks/useWorkspaceBySlug";
import { useWorkspacePresence } from "@/lib/hooks/useWorkspacePresence";
import { usePresenceStore } from "@/stores/presenceStore";

export default function WorkspacePresenceBinder({
  workspaceSlug,
}: Readonly<{
  workspaceSlug: string;
}>) {
  const workspaceQuery = useWorkspaceBySlug(workspaceSlug);
  const workspaceId =
    workspaceQuery.status === "success" ? workspaceQuery.data.id : null;

  useWorkspacePresence(workspaceId);

  const { connection, isConnected } = useChatHub();
  const setPresence = usePresenceStore((s) => s.setPresence);
  const clearPresence = usePresenceStore((s) => s.clearPresence);

  useEffect(() => {
    if (!connection || !isConnected || !workspaceId) return;

    const onOnline = (payload: { workspaceId?: string; userId?: string }) => {
      if (payload?.workspaceId !== workspaceId) return;
      if (!payload?.userId) return;
      setPresence(payload.userId, "online");
    };

    const onOffline = (payload: { workspaceId?: string; userId?: string }) => {
      if (payload?.workspaceId !== workspaceId) return;
      if (!payload?.userId) return;
      setPresence(payload.userId, "offline");
    };

    connection.on("presence:online", onOnline);
    connection.on("presence:offline", onOffline);

    return () => {
      connection.off("presence:online", onOnline);
      connection.off("presence:offline", onOffline);
      clearPresence();
    };
  }, [connection, isConnected, workspaceId, setPresence, clearPresence]);

  return null;
}
