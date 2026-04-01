"use client";

import { useEffect, useMemo } from "react";
import { useRouter } from "next/navigation";

import { useWorkspaceBySlug } from "@/lib/hooks/useWorkspaceBySlug";
import { useWorkspaceChannels } from "@/lib/hooks/useWorkspaceChannels";

export default function WorkspaceHomeRedirect({
  workspaceSlug,
}: Readonly<{
  workspaceSlug: string;
}>) {
  const router = useRouter();
  const workspaceQuery = useWorkspaceBySlug(workspaceSlug);
  const workspaceId =
    workspaceQuery.status === "success" ? workspaceQuery.data.id : null;

  const channelsQuery = useWorkspaceChannels(workspaceId);

  const targetChannelId = useMemo(() => {
    if (channelsQuery.status !== "success") return null;
    const channels = channelsQuery.data.filter((c) => !c.isArchived);
    const general = channels.find((c) => (c.name ?? "").toLowerCase() === "general");
    return (general ?? channels[0] ?? null)?.id ?? null;
  }, [channelsQuery.status, channelsQuery.data]);

  useEffect(() => {
    if (!targetChannelId) return;
    router.replace(`/app/${workspaceSlug}/channel/${targetChannelId}`);
  }, [router, targetChannelId, workspaceSlug]);

  return (
    <main className="flex flex-1 items-center justify-center px-6 py-16">
      <div className="w-full max-w-xl">
        <h1 className="text-xl font-semibold tracking-tight text-foreground">
          Opening workspace…
        </h1>
        <p className="mt-2 text-sm text-zinc-600 dark:text-zinc-400">
          Redirecting to your channels.
        </p>
      </div>
    </main>
  );
}

