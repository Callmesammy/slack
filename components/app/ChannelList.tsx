"use client";

import Link from "next/link";

import { useWorkspaceBySlug } from "@/lib/hooks/useWorkspaceBySlug";
import { useWorkspaceChannels } from "@/lib/hooks/useWorkspaceChannels";

export default function ChannelList({
  workspaceSlug,
}: Readonly<{
  workspaceSlug: string;
}>) {
  const workspaceQuery = useWorkspaceBySlug(workspaceSlug);
  const workspaceId =
    workspaceQuery.status === "success" ? workspaceQuery.data.id : null;
  const channelsQuery = useWorkspaceChannels(workspaceId);

  return (
    <div className="mt-6">
      <div className="text-xs font-semibold text-zinc-600 dark:text-zinc-400">
        Channels
      </div>

      <div className="mt-2 grid gap-1">
        {workspaceQuery.status === "pending" ? (
          <div className="text-xs text-zinc-600 dark:text-zinc-400">Loading…</div>
        ) : null}
        {workspaceQuery.status === "error" ? (
          <div className="text-xs text-zinc-600 dark:text-zinc-400">
            Failed to load workspace.
          </div>
        ) : null}

        {channelsQuery.status === "pending" && workspaceId ? (
          <div className="text-xs text-zinc-600 dark:text-zinc-400">Loading…</div>
        ) : null}
        {channelsQuery.status === "error" ? (
          <div className="text-xs text-zinc-600 dark:text-zinc-400">
            Failed to load channels.
          </div>
        ) : null}

        {channelsQuery.status === "success" ? (
          <div className="grid gap-1">
            {channelsQuery.data
              .filter((c) => c.type === "public" || c.type === "private")
              .map((c) => (
                <Link
                  key={c.id}
                  href={`/app/${workspaceSlug}/channel/${c.id}`}
                  className="rounded-lg px-2 py-1 text-sm text-zinc-700 hover:bg-black/[.04] hover:text-foreground dark:text-zinc-300 dark:hover:bg-white/[.06]"
                >
                  #{c.name ?? "channel"}
                </Link>
              ))}
            {channelsQuery.data.length === 0 ? (
              <div className="text-xs text-zinc-600 dark:text-zinc-400">
                No channels.
              </div>
            ) : null}
          </div>
        ) : null}
      </div>
    </div>
  );
}

