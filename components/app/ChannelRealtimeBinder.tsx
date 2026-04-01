"use client";

import { useEffect, useMemo, useState } from "react";
import { useQueryClient, type InfiniteData } from "@tanstack/react-query";
import { useSession } from "next-auth/react";

import { useChatHub } from "@/lib/hooks/useChatHub";
import TypingIndicator from "@/components/app/TypingIndicator";
import type { ChannelMessageDto } from "@/lib/hooks/useChannelMessages";
import type { ChannelMessagePageDto } from "@/lib/hooks/useChannelMessagesInfinite";

type TypingEventPayload = { channelId?: string; userId?: string | null };
type MessageNewPayload = {
  messageId?: string;
  workspaceId?: string;
  channelId?: string;
  senderId?: string;
  senderEmail?: string;
  senderName?: string | null;
  createdAt?: string;
  content?: string;
  contentFormat?: string;
};
type MessageEditedPayload = {
  messageId?: string;
  workspaceId?: string;
  channelId?: string;
  editorUserId?: string;
  editedAt?: string;
  content?: string;
  contentFormat?: string;
};
type MessageDeletedPayload = {
  messageId?: string;
  workspaceId?: string;
  channelId?: string;
  deleterUserId?: string;
  deletedAt?: string;
};
type ReactionPayload = {
  messageId?: string;
  workspaceId?: string;
  channelId?: string;
  userId?: string;
  emoji?: string;
  count?: number;
};

export default function ChannelRealtimeBinder({
  channelId,
}: Readonly<{
  channelId: string;
}>) {
  const { connection, isConnected } = useChatHub();
  const { data: session } = useSession();
  const meId = session?.user?.id ?? "";
  const [isTyping, setIsTyping] = useState(false);
  const queryClient = useQueryClient();

  const groupChannelId = useMemo(() => channelId, [channelId]);

  useEffect(() => {
    if (!connection || !isConnected) return;
    let cancelled = false;

    const join = () =>
      connection
        .invoke("JoinChannel", groupChannelId)
        .catch(() => {})
        .finally(() => {});

    join();

    const onReconnected = () => {
      if (cancelled) return;
      join();
    };

    connection.onreconnected(onReconnected);

    const refreshMessages = async (workspaceId?: string) => {
      if (!workspaceId) return;
      await queryClient.invalidateQueries({
        queryKey: ["channelMessages", workspaceId, groupChannelId],
      });
      await queryClient.invalidateQueries({
        queryKey: ["workspaceChannels", workspaceId],
      });
    };

    const upsertMessage = (payload: MessageNewPayload) => {
      if (!payload.workspaceId || !payload.channelId || !payload.messageId) return;
      const key = ["channelMessages", payload.workspaceId, groupChannelId] as const;
      queryClient.setQueryData<InfiniteData<ChannelMessagePageDto> | undefined>(key, (prev) => {
        const next: ChannelMessageDto = {
          id: payload.messageId!,
          senderId: payload.senderId ?? "unknown",
          senderEmail: payload.senderEmail ?? "unknown",
          senderName: payload.senderName ?? null,
          content: payload.content ?? "",
          contentFormat: payload.contentFormat ?? "plain",
          type: "text",
          threadParentId: null,
          createdAt: payload.createdAt ?? new Date().toISOString(),
          reactions: [],
        };

        if (!prev) {
          return {
            pageParams: [null],
            pages: [{ items: [next], nextCursorMessageId: null }],
          };
        }

        const pages = [...prev.pages];
        const first = pages[0];
        if (first.items.some((m) => m.id === next.id)) return prev;
        pages[0] = { ...first, items: [...first.items, next] };
        return { ...prev, pages };
      });
    };

    const onTypingStart = (payload: TypingEventPayload) => {
      if (cancelled) return;
      if (payload?.channelId !== groupChannelId) return;
      setIsTyping(true);
    };
    const onTypingStop = (payload: TypingEventPayload) => {
      if (cancelled) return;
      if (payload?.channelId !== groupChannelId) return;
      setIsTyping(false);
    };

    const onMessageNew = (payload: MessageNewPayload) => {
      if (cancelled) return;
      if (payload?.channelId !== groupChannelId) return;
      upsertMessage(payload);
      void queryClient.invalidateQueries({
        queryKey: ["workspaceChannels", payload.workspaceId],
      });
    };

    const onMessageEdited = (payload: MessageEditedPayload) => {
      if (cancelled) return;
      if (payload?.channelId !== groupChannelId) return;
      if (payload.workspaceId && payload.messageId) {
        const key = ["channelMessages", payload.workspaceId, groupChannelId] as const;
        queryClient.setQueryData<InfiniteData<ChannelMessagePageDto> | undefined>(key, (prev) => {
          if (!prev) return prev;
          return {
            ...prev,
            pages: prev.pages.map((p) => ({
              ...p,
              items: p.items.map((m) =>
                m.id === payload.messageId
                  ? {
                      ...m,
                      content: payload.content ?? m.content,
                      contentFormat: payload.contentFormat ?? m.contentFormat,
                    }
                  : m,
              ),
            })),
          };
        });
      }
    };

    const onMessageDeleted = (payload: MessageDeletedPayload) => {
      if (cancelled) return;
      if (payload?.channelId !== groupChannelId) return;
      if (payload.workspaceId && payload.messageId) {
        const key = ["channelMessages", payload.workspaceId, groupChannelId] as const;
        queryClient.setQueryData<InfiniteData<ChannelMessagePageDto> | undefined>(key, (prev) => {
          if (!prev) return prev;
          return {
            ...prev,
            pages: prev.pages.map((p) => ({
              ...p,
              items: p.items.filter((m) => m.id !== payload.messageId),
            })),
          };
        });
      }
      void refreshMessages(payload.workspaceId);
    };

    const onReactionAdded = (payload: ReactionPayload) => {
      if (cancelled) return;
      if (payload?.channelId !== groupChannelId) return;
      if (payload.workspaceId && payload.messageId && payload.emoji) {
        const key = ["channelMessages", payload.workspaceId, groupChannelId] as const;
        queryClient.setQueryData<InfiniteData<ChannelMessagePageDto> | undefined>(key, (prev) => {
          if (!prev) return prev;
          return {
            ...prev,
            pages: prev.pages.map((p) => ({
              ...p,
              items: p.items.map((m) => {
                if (m.id !== payload.messageId) return m;
                const existing = m.reactions?.find((r) => r.emoji === payload.emoji);
                const nextCount = payload.count ?? (existing ? existing.count + 1 : 1);
                const reactions = (m.reactions ?? []).filter((r) => r.emoji !== payload.emoji);
                reactions.push({
                  emoji: payload.emoji,
                  count: nextCount,
                  reactedByMe:
                    payload.userId && payload.userId === meId
                      ? true
                      : (existing?.reactedByMe ?? false),
                });
                return { ...m, reactions };
              }),
            })),
          };
        });
      } else {
        void refreshMessages(payload.workspaceId);
      }
    };

    const onReactionRemoved = (payload: ReactionPayload) => {
      if (cancelled) return;
      if (payload?.channelId !== groupChannelId) return;
      if (payload.workspaceId && payload.messageId && payload.emoji) {
        const key = ["channelMessages", payload.workspaceId, groupChannelId] as const;
        queryClient.setQueryData<InfiniteData<ChannelMessagePageDto> | undefined>(key, (prev) => {
          if (!prev) return prev;
          return {
            ...prev,
            pages: prev.pages.map((p) => ({
              ...p,
              items: p.items.map((m) => {
                if (m.id !== payload.messageId) return m;
                const existing = m.reactions?.find((r) => r.emoji === payload.emoji);
                const reactions = (m.reactions ?? []).filter((r) => r.emoji !== payload.emoji);
                const nextCount = payload.count ?? 0;
                if (nextCount > 0) {
                  reactions.push({
                    emoji: payload.emoji,
                    count: nextCount,
                    reactedByMe:
                      payload.userId && payload.userId === meId
                        ? false
                        : (existing?.reactedByMe ?? false),
                  });
                }
                return { ...m, reactions };
              }),
            })),
          };
        });
      } else {
        void refreshMessages(payload.workspaceId);
      }
    };

    connection.on("typing:start", onTypingStart);
    connection.on("typing:stop", onTypingStop);
    connection.on("message:new", onMessageNew);
    connection.on("message:edited", onMessageEdited);
    connection.on("message:deleted", onMessageDeleted);
    connection.on("reaction:added", onReactionAdded);
    connection.on("reaction:removed", onReactionRemoved);

    return () => {
      cancelled = true;
      connection.offreconnected(onReconnected);
      connection.off("typing:start", onTypingStart);
      connection.off("typing:stop", onTypingStop);
      connection.off("message:new", onMessageNew);
      connection.off("message:edited", onMessageEdited);
      connection.off("message:deleted", onMessageDeleted);
      connection.off("reaction:added", onReactionAdded);
      connection.off("reaction:removed", onReactionRemoved);
      connection.invoke("LeaveChannel", groupChannelId).catch(() => {});
    };
  }, [connection, isConnected, groupChannelId, queryClient, meId]);

  return <TypingIndicator isTyping={isTyping} />;
}
