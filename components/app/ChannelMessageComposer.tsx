"use client";

import { useEffect, useRef, useState } from "react";

import { useSendMessage } from "@/lib/hooks/useSendMessage";
import { useChatHub } from "@/lib/hooks/useChatHub";
import { useWorkspaceBySlug } from "@/lib/hooks/useWorkspaceBySlug";

export default function ChannelMessageComposer({
  workspaceSlug,
  channelId,
}: Readonly<{
  workspaceSlug: string;
  channelId: string;
}>) {
  const workspaceQuery = useWorkspaceBySlug(workspaceSlug);
  const workspaceId =
    workspaceQuery.status === "success" ? workspaceQuery.data.id : null;

  const send = useSendMessage(workspaceId, channelId);
  const [value, setValue] = useState("");
  const { connection, isConnected } = useChatHub();
  const stopTimer = useRef<number | null>(null);
  const typingActive = useRef(false);

  const typingStart = () => {
    if (!connection || !isConnected) return;
    if (typingActive.current) return;
    typingActive.current = true;
    connection.invoke("TypingStart", channelId).catch(() => {});
  };

  const typingStop = () => {
    if (!connection || !isConnected) return;
    if (!typingActive.current) return;
    typingActive.current = false;
    connection.invoke("TypingStop", channelId).catch(() => {});
  };

  useEffect(() => {
    return () => {
      if (stopTimer.current) window.clearTimeout(stopTimer.current);
      typingStop();
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [channelId]);

  return (
    <form
      className="border-t border-black/10 bg-white p-4 dark:border-white/10 dark:bg-black"
      onSubmit={(e) => {
        e.preventDefault();
        const content = value.trim();
        if (!content) return;
        typingStop();
        send.mutate({ content, contentFormat: "plain" });
        setValue("");
      }}
    >
      <div className="flex gap-3">
        <input
          value={value}
          onChange={(e) => {
            const next = e.target.value;
            setValue(next);
            if (next.trim()) typingStart();
            if (stopTimer.current) window.clearTimeout(stopTimer.current);
            stopTimer.current = window.setTimeout(() => {
              typingStop();
            }, 1200);
          }}
          placeholder="Message #channel"
          onBlur={() => typingStop()}
          className="h-11 flex-1 rounded-2xl border border-black/10 bg-white px-4 text-sm text-foreground outline-none placeholder:text-zinc-400 focus:border-black/20 dark:border-white/10 dark:bg-black dark:focus:border-white/20"
        />
        <button
          type="submit"
          disabled={send.isPending || workspaceId === null}
          className="inline-flex h-11 items-center justify-center rounded-full bg-foreground px-5 text-sm font-semibold text-background hover:opacity-90 disabled:opacity-50"
        >
          Send
        </button>
      </div>
      {send.isError ? (
        <div className="mt-2 text-xs text-zinc-600 dark:text-zinc-400">
          {send.error instanceof Error ? send.error.message : "Send failed"}
        </div>
      ) : null}
    </form>
  );
}
