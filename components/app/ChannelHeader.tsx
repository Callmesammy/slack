"use client";

import { useSession } from "next-auth/react";

import { useChannelMembers } from "@/lib/hooks/useChannelMembers";
import { useJoinChannel, useLeaveChannel } from "@/lib/hooks/useJoinLeaveChannel";
import { useWorkspaceBySlug } from "@/lib/hooks/useWorkspaceBySlug";

export default function ChannelHeader({
  workspaceSlug,
  channelId,
}: Readonly<{
  workspaceSlug: string;
  channelId: string;
}>) {
  const { data: session } = useSession();
  const meId = session?.user?.id ?? "";

  const workspaceQuery = useWorkspaceBySlug(workspaceSlug);
  const workspaceId =
    workspaceQuery.status === "success" ? workspaceQuery.data.id : null;

  const members = useChannelMembers(workspaceId, channelId);
  const isMember =
    members.status === "success"
      ? members.data.some((m) => m.userId === meId)
      : false;

  const join = useJoinChannel(workspaceId, channelId);
  const leave = useLeaveChannel(workspaceId, channelId);

  const isBusy = join.isPending || leave.isPending;

  return (
    <div className="flex items-start justify-between gap-4 border-b border-black/10 bg-white px-6 py-4 dark:border-white/10 dark:bg-black">
      <div className="min-w-0">
        <h1 className="truncate text-sm font-semibold text-foreground">Channel</h1>
        <p className="mt-1 truncate text-xs text-zinc-600 dark:text-zinc-400">
          {channelId}
        </p>
      </div>

      <div className="flex items-center gap-2">
        {members.status === "success" ? (
          <div className="hidden text-xs text-zinc-600 dark:text-zinc-400 sm:block">
            {members.data.length} member{members.data.length === 1 ? "" : "s"}
          </div>
        ) : null}

        <button
          type="button"
          disabled={!workspaceId || isBusy}
          onClick={() => {
            if (!workspaceId) return;
            if (isMember) {
              leave.mutate();
              return;
            }
            join.mutate();
          }}
          className="inline-flex h-9 items-center justify-center rounded-full border border-black/10 bg-white px-4 text-sm font-medium text-foreground hover:bg-black/[.04] disabled:opacity-50 dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]"
        >
          {isMember ? "Leave" : "Join"}
        </button>
      </div>
    </div>
  );
}

