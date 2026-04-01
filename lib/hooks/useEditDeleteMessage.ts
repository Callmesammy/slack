"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";

import { api } from "@/lib/api";

export function useEditMessage(
  workspaceId: string | null,
  channelId: string,
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (payload: {
      messageId: string;
      content: string;
      contentFormat?: string;
    }) => {
      if (!workspaceId) throw new Error("Missing workspaceId.");
      return api.patch<{ messageId: string }>(
        `/workspaces/${workspaceId}/channels/${channelId}/messages/${payload.messageId}`,
        {
          content: payload.content,
          contentFormat: payload.contentFormat ?? "plain",
        },
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["channelMessages", workspaceId, channelId],
      });
    },
  });
}

export function useDeleteMessage(
  workspaceId: string | null,
  channelId: string,
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (payload: { messageId: string }) => {
      if (!workspaceId) throw new Error("Missing workspaceId.");
      return api.delete<{ messageId: string }>(
        `/workspaces/${workspaceId}/channels/${channelId}/messages/${payload.messageId}`,
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["channelMessages", workspaceId, channelId],
      });
      await queryClient.invalidateQueries({
        queryKey: ["workspaceChannels", workspaceId],
      });
    },
  });
}

