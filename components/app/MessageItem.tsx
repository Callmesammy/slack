"use client";

import { useMemo, useState } from "react";
import { useSession } from "next-auth/react";

import type { ChannelMessageDto } from "@/lib/hooks/useChannelMessages";
import { useEditMessage, useDeleteMessage } from "@/lib/hooks/useEditDeleteMessage";
import { useAddReaction, useRemoveReaction } from "@/lib/hooks/useMessageReactions";

export default function MessageItem({
  workspaceId,
  channelId,
  message,
}: Readonly<{
  workspaceId: string | null;
  channelId: string;
  message: ChannelMessageDto;
}>) {
  const { data: session } = useSession();
  const meId = session?.user?.id ?? "";
  const isMine = message.senderId === meId;
  const isOptimistic = message.id.startsWith("optimistic:");

  const [isEditing, setIsEditing] = useState(false);
  const [draft, setDraft] = useState(message.content);

  const edit = useEditMessage(workspaceId, channelId);
  const del = useDeleteMessage(workspaceId, channelId);
  const addReaction = useAddReaction(workspaceId, channelId, message.id);
  const removeReaction = useRemoveReaction(workspaceId, channelId, message.id);

  const createdLabel = useMemo(() => {
    try {
      return new Date(message.createdAt).toLocaleString();
    } catch {
      return message.createdAt;
    }
  }, [message.createdAt]);

  return (
    <div className="rounded-2xl bg-[color:var(--surface-2)] p-4">
      <div className="flex items-start justify-between gap-3">
        <div className="text-xs text-zinc-600 dark:text-zinc-400">
          <span className="font-medium text-foreground">
            {message.senderName ?? message.senderEmail}
          </span>
          <span className="mx-2">·</span>
          <span className="font-mono">{createdLabel}</span>
          {isOptimistic ? (
            <>
              <span className="mx-2">·</span>
                <span className="rounded-full bg-amber-500/15 px-2 py-0.5 text-[10px] font-semibold text-amber-700 dark:text-amber-300">
                  Sending…
                </span>
              </>
            ) : null}
          </div>
        <div className="flex items-center gap-2">
          <button
            type="button"
            disabled={!workspaceId || isOptimistic || addReaction.isPending}
            onClick={() => addReaction.mutate("👍")}
            className="rounded-full border border-black/10 bg-white px-3 py-1 text-xs text-foreground hover:bg-black/[.04] disabled:opacity-50 dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]"
          >
            👍
          </button>
          <button
            type="button"
            disabled={!workspaceId || isOptimistic || removeReaction.isPending}
            onClick={() => removeReaction.mutate("👍")}
            className="rounded-full border border-black/10 bg-white px-3 py-1 text-xs text-foreground hover:bg-black/[.04] disabled:opacity-50 dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]"
          >
            −👍
          </button>
          {isMine ? (
            <>
              <button
                type="button"
                disabled={!workspaceId || isOptimistic || del.isPending}
                onClick={() => del.mutate({ messageId: message.id })}
                className="rounded-full border border-black/10 bg-white px-3 py-1 text-xs text-foreground hover:bg-black/[.04] disabled:opacity-50 dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]"
              >
                Delete
              </button>
              <button
                type="button"
                disabled={!workspaceId || isOptimistic}
                onClick={() => {
                  setDraft(message.content);
                  setIsEditing((v) => !v);
                }}
                className="rounded-full border border-black/10 bg-white px-3 py-1 text-xs text-foreground hover:bg-black/[.04] disabled:opacity-50 dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]"
              >
                {isEditing ? "Cancel" : "Edit"}
              </button>
            </>
          ) : null}
        </div>
      </div>

      {isEditing ? (
        <form
          className="mt-2 flex gap-2"
          onSubmit={(e) => {
            e.preventDefault();
            const content = draft.trim();
            if (!content) return;
            edit.mutate(
              { messageId: message.id, content, contentFormat: "plain" },
              {
                onSuccess: () => {
                  setIsEditing(false);
                },
              },
            );
          }}
        >
          <input
            value={draft}
            onChange={(e) => setDraft(e.target.value)}
            className="h-10 flex-1 rounded-xl border border-black/10 bg-white px-3 text-sm text-foreground outline-none placeholder:text-zinc-400 focus:border-black/20 dark:border-white/10 dark:bg-black dark:focus:border-white/20"
          />
          <button
            type="submit"
            disabled={!workspaceId || edit.isPending}
            className="inline-flex h-10 items-center justify-center rounded-xl bg-foreground px-4 text-sm font-semibold text-background disabled:opacity-50"
          >
            Save
          </button>
        </form>
      ) : (
        <div className="mt-1 text-sm text-foreground">{message.content}</div>
      )}

      {message.reactions?.length ? (
        <div className="mt-3 flex flex-wrap gap-2">
          {message.reactions.map((r) => (
            <button
              key={`${r.emoji}`}
              type="button"
              disabled={!workspaceId || addReaction.isPending || removeReaction.isPending}
              onClick={() => {
                if (r.reactedByMe) {
                  removeReaction.mutate(r.emoji);
                  return;
                }
                addReaction.mutate(r.emoji);
              }}
              className={[
                "inline-flex items-center gap-1 rounded-full border px-3 py-1 text-xs",
                r.reactedByMe
                  ? "border-emerald-500/40 bg-emerald-500/10 text-foreground"
                  : "border-black/10 bg-white text-foreground hover:bg-black/[.04] dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]",
              ].join(" ")}
            >
              <span>{r.emoji}</span>
              <span className="tabular-nums text-zinc-600 dark:text-zinc-400">
                {r.count}
              </span>
            </button>
          ))}
        </div>
      ) : null}
    </div>
  );
}
