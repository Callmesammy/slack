"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";

import { api } from "@/lib/api";

export function useAddReaction(
  workspaceId: string | null,
  channelId: string,
  messageId: string,
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (emoji: string) => {
      if (!workspaceId) throw new Error("Missing workspaceId.");
      return api.post<{ messageId: string; emoji: string }>(
        `/workspaces/${workspaceId}/channels/${channelId}/messages/${messageId}/reactions`,
        { emoji },
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["channelMessages", workspaceId, channelId],
      });
    },
  });
}

export function useRemoveReaction(
  workspaceId: string | null,
  channelId: string,
  messageId: string,
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (emoji: string) => {
      if (!workspaceId) throw new Error("Missing workspaceId.");
      return api.delete<{ messageId: string; emoji: string }>(
        `/workspaces/${workspaceId}/channels/${channelId}/messages/${messageId}/reactions`,
        { emoji },
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["channelMessages", workspaceId, channelId],
      });
    },
  });
}

