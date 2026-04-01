"use client";

import { useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { api } from "@/lib/api";

type ChannelDto = {
  id: string;
  name: string | null;
  type: string;
  isArchived: boolean;
  memberCount: number;
  lastMessageAt: string | null;
};

type MessageDto = {
  id: string;
  senderId: string;
  content: string;
  contentFormat: string;
  type: string;
  threadParentId: string | null;
  createdAt: string;
};

export default function ApiPlayground() {
  const queryClient = useQueryClient();

  const [workspaceId, setWorkspaceId] = useState("");
  const [channelId, setChannelId] = useState("");
  const [message, setMessage] = useState("");

  const workspaceKey = useMemo(
    () => (workspaceId.trim() ? workspaceId.trim() : null),
    [workspaceId],
  );
  const channelKey = useMemo(
    () => (channelId.trim() ? channelId.trim() : null),
    [channelId],
  );

  const channelsQuery = useQuery({
    queryKey: ["channels", workspaceKey],
    enabled: workspaceKey !== null,
    queryFn: () =>
      api.get<ChannelDto[]>(`/workspaces/${workspaceKey}/channels`),
  });

  const messagesQuery = useQuery({
    queryKey: ["messages", workspaceKey, channelKey],
    enabled: workspaceKey !== null && channelKey !== null,
    queryFn: () =>
      api.get<MessageDto[]>(
        `/workspaces/${workspaceKey}/channels/${channelKey}/messages?limit=50`,
      ),
  });

  const sendMessageMutation = useMutation({
    mutationFn: async () => {
      if (!workspaceKey || !channelKey) {
        throw new Error("WorkspaceId and ChannelId are required.");
      }
      if (!message.trim()) throw new Error("Message is empty.");
      return api.post<{ messageId: string }>(
        `/workspaces/${workspaceKey}/channels/${channelKey}/messages/send`,
        { content: message.trim(), contentFormat: "plain" },
      );
    },
    onSuccess: async () => {
      setMessage("");
      await queryClient.invalidateQueries({
        queryKey: ["messages", workspaceKey, channelKey],
      });
    },
  });

  return (
    <section className="rounded-2xl border border-black/10 bg-white p-5 dark:border-white/10 dark:bg-black">
      <h2 className="text-sm font-semibold text-foreground">API playground</h2>
      <p className="mt-1 text-xs text-zinc-600 dark:text-zinc-400">
        Uses your authenticated JWT to call channels/messages endpoints.
      </p>

      <div className="mt-4 grid gap-3">
        <label className="grid gap-2">
          <span className="text-xs font-medium text-foreground">
            Workspace ID (UUID)
          </span>
          <input
            value={workspaceId}
            onChange={(e) => setWorkspaceId(e.target.value)}
            placeholder="e.g. 0f9c...-...."
            className="h-11 rounded-2xl border border-black/10 bg-white px-4 text-sm text-foreground outline-none placeholder:text-zinc-400 focus:border-black/20 dark:border-white/10 dark:bg-black dark:focus:border-white/20"
          />
        </label>

        <label className="grid gap-2">
          <span className="text-xs font-medium text-foreground">
            Channel ID (UUID)
          </span>
          <input
            value={channelId}
            onChange={(e) => setChannelId(e.target.value)}
            placeholder="e.g. 7b2a...-...."
            className="h-11 rounded-2xl border border-black/10 bg-white px-4 text-sm text-foreground outline-none placeholder:text-zinc-400 focus:border-black/20 dark:border-white/10 dark:bg-black dark:focus:border-white/20"
          />
        </label>
      </div>

      <div className="mt-5 grid gap-3 md:grid-cols-2">
        <div className="rounded-xl bg-[color:var(--surface-2)] p-4">
          <div className="flex items-center justify-between">
            <div className="text-xs font-semibold text-foreground">Channels</div>
            <button
              type="button"
              className="rounded-full border border-black/10 bg-white px-3 py-1 text-xs font-semibold text-foreground hover:bg-black/[.04] dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]"
              onClick={() => channelsQuery.refetch()}
              disabled={workspaceKey === null || channelsQuery.isFetching}
            >
              Refresh
            </button>
          </div>

          <div className="mt-3 text-sm text-zinc-700 dark:text-zinc-300">
            {channelsQuery.status === "pending" && workspaceKey ? (
              <div>Loading…</div>
            ) : null}
            {channelsQuery.status === "error" ? (
              <div className="text-xs">
                {channelsQuery.error instanceof Error
                  ? channelsQuery.error.message
                  : "Request failed"}
              </div>
            ) : null}
            {channelsQuery.status === "success" ? (
              <ul className="grid gap-2">
                {channelsQuery.data.map((c) => (
                  <li key={c.id} className="rounded-lg bg-white p-3 dark:bg-black">
                    <div className="text-xs font-semibold text-foreground">
                      {c.name ? `#${c.name}` : c.type}
                    </div>
                    <div className="mt-1 font-mono text-[11px] opacity-80">
                      {c.id}
                    </div>
                  </li>
                ))}
                {channelsQuery.data.length === 0 ? (
                  <li className="text-xs opacity-80">No channels returned.</li>
                ) : null}
              </ul>
            ) : null}
          </div>
        </div>

        <div className="rounded-xl bg-[color:var(--surface-2)] p-4">
          <div className="text-xs font-semibold text-foreground">Messages</div>

          <div className="mt-3 text-sm text-zinc-700 dark:text-zinc-300">
            {messagesQuery.status === "pending" &&
            workspaceKey &&
            channelKey ? (
              <div>Loading…</div>
            ) : null}
            {messagesQuery.status === "error" ? (
              <div className="text-xs">
                {messagesQuery.error instanceof Error
                  ? messagesQuery.error.message
                  : "Request failed"}
              </div>
            ) : null}
            {messagesQuery.status === "success" ? (
              <ul className="grid gap-2">
                {messagesQuery.data.map((m) => (
                  <li key={m.id} className="rounded-lg bg-white p-3 dark:bg-black">
                    <div className="text-xs text-zinc-600 dark:text-zinc-400">
                      <span className="font-mono">{m.senderId}</span>
                      <span className="mx-2">·</span>
                      <span className="font-mono">{m.createdAt}</span>
                    </div>
                    <div className="mt-1 text-sm text-foreground">
                      {m.content}
                    </div>
                  </li>
                ))}
                {messagesQuery.data.length === 0 ? (
                  <li className="text-xs opacity-80">No messages returned.</li>
                ) : null}
              </ul>
            ) : null}
          </div>

          <div className="mt-4 grid gap-2">
            <textarea
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              placeholder="Type a message…"
              rows={3}
              className="w-full resize-none rounded-2xl border border-black/10 bg-white px-4 py-3 text-sm text-foreground outline-none placeholder:text-zinc-400 focus:border-black/20 dark:border-white/10 dark:bg-black dark:focus:border-white/20"
            />
            <button
              type="button"
              onClick={() => sendMessageMutation.mutate()}
              disabled={sendMessageMutation.isPending}
              className="inline-flex h-11 items-center justify-center rounded-full bg-foreground px-4 text-sm font-semibold text-background hover:opacity-90 disabled:opacity-50"
            >
              Send message
            </button>
            {sendMessageMutation.isError ? (
              <div className="text-xs text-zinc-600 dark:text-zinc-400">
                {sendMessageMutation.error instanceof Error
                  ? sendMessageMutation.error.message
                  : "Send failed"}
              </div>
            ) : null}
          </div>
        </div>
      </div>
    </section>
  );
}

