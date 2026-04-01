"use client";

import PresenceDot from "@/components/app/PresenceDot";
import { useChannelMembers } from "@/lib/hooks/useChannelMembers";
import { useWorkspaceBySlug } from "@/lib/hooks/useWorkspaceBySlug";

export default function ChannelMemberList({
  workspaceSlug,
  channelId,
}: Readonly<{
  workspaceSlug: string;
  channelId: string;
}>) {
  const workspaceQuery = useWorkspaceBySlug(workspaceSlug);
  const workspaceId =
    workspaceQuery.status === "success" ? workspaceQuery.data.id : null;

  const members = useChannelMembers(workspaceId, channelId);

  if (members.status === "pending") {
    return (
      <div className="px-6 py-3 text-xs text-zinc-600 dark:text-zinc-400">
        Loading members…
      </div>
    );
  }

  if (members.status === "error") {
    return (
      <div className="px-6 py-3 text-xs text-zinc-600 dark:text-zinc-400">
        Failed to load members.
      </div>
    );
  }

  return (
    <div className="px-4 py-3">
      <div className="px-2 text-xs font-semibold text-zinc-600 dark:text-zinc-400">
        Members ({members.data.length})
      </div>
      <div className="mt-2 grid gap-1">
        {members.data.map((m) => (
          <div
            key={m.userId}
            className="flex items-center gap-2 rounded-lg px-2 py-1 text-sm text-foreground hover:bg-black/[.04] dark:hover:bg-white/[.06]"
            title={m.email}
          >
            <PresenceDot userId={m.userId} />
            <div className="min-w-0 flex-1 truncate">
              {m.name ?? m.email}
            </div>
            <div className="text-xs text-zinc-500">{m.role}</div>
          </div>
        ))}
      </div>
    </div>
  );
}

