"use client";

import { useMutation, useQueryClient, type InfiniteData } from "@tanstack/react-query";
import { useSession } from "next-auth/react";

import { api } from "@/lib/api";
import type { ChannelMessageDto } from "@/lib/hooks/useChannelMessages";
import type { ChannelMessagePageDto } from "@/lib/hooks/useChannelMessagesInfinite";

export function useSendMessage(workspaceId: string | null, channelId: string) {
  const queryClient = useQueryClient();
  const { data: session } = useSession();
  const meId = session?.user?.id ?? "me";

  return useMutation({
    mutationFn: async (payload: { content: string; contentFormat?: string }) => {
      if (!workspaceId) throw new Error("Missing workspaceId.");
      return api.post<{
        messageId: string;
        workspaceId: string;
        channelId: string;
        senderId: string;
        createdAt: string;
        content: string;
        contentFormat: string;
      }>(
        `/workspaces/${workspaceId}/channels/${channelId}/messages/send`,
        {
          content: payload.content,
          contentFormat: payload.contentFormat ?? "plain",
        },
      );
    },
    onMutate: async (payload) => {
      if (!workspaceId) return { optimisticId: null as string | null };

      await queryClient.cancelQueries({
        queryKey: ["channelMessages", workspaceId, channelId],
      });

      const optimisticId = `optimistic:${crypto.randomUUID()}`;
      const optimisticMessage: ChannelMessageDto = {
        id: optimisticId,
        senderId: meId,
        senderEmail: session?.user?.email ?? "me",
        senderName: session?.user?.name ?? null,
        content: payload.content,
        contentFormat: payload.contentFormat ?? "plain",
        type: "text",
        threadParentId: null,
        createdAt: new Date().toISOString(),
        reactions: [],
      };

      queryClient.setQueryData<InfiniteData<ChannelMessagePageDto>>(
        ["channelMessages", workspaceId, channelId],
        (prev) => {
          if (!prev) {
            return {
              pageParams: [null],
              pages: [{ items: [optimisticMessage], nextCursorMessageId: null }],
            };
          }

          const pages = [...prev.pages];
          pages[0] = {
            ...pages[0],
            items: [...pages[0].items, optimisticMessage],
          };

          return { ...prev, pages };
        },
      );

      return { optimisticId };
    },
    onError: async (_err, _payload, context) => {
      if (!workspaceId) return;
      if (!context?.optimisticId) return;
      queryClient.setQueryData<InfiniteData<ChannelMessagePageDto>>(
        ["channelMessages", workspaceId, channelId],
        (prev) => {
          if (!prev) return prev;
          return {
            ...prev,
            pages: prev.pages.map((p, idx) =>
              idx === 0 ? { ...p, items: p.items.filter((m) => m.id !== context.optimisticId) } : p,
            ),
          };
        },
      );
    },
    onSuccess: async (data, _payload, context) => {
      if (!workspaceId) return;

      if (context?.optimisticId) {
        queryClient.setQueryData<InfiniteData<ChannelMessagePageDto>>(
          ["channelMessages", workspaceId, channelId],
          (prev) => {
            if (!prev) return prev;
            return {
              ...prev,
              pages: prev.pages.map((p, idx) =>
                idx === 0
                  ? {
                      ...p,
                      items: p.items.map((m) =>
                        m.id === context.optimisticId
                          ? {
                              ...m,
                              id: data.messageId,
                              senderId: data.senderId,
                              createdAt: data.createdAt,
                              content: data.content,
                              contentFormat: data.contentFormat,
                            }
                          : m,
                      ),
                    }
                  : p,
              ),
            };
          },
        );
      }

      await queryClient.invalidateQueries({
        queryKey: ["workspaceChannels", workspaceId],
      });
    },
  });
}
