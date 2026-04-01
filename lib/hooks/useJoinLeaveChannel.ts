"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";

import { api } from "@/lib/api";

export function useJoinChannel(
  workspaceId: string | null,
  channelId: string,
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async () => {
      if (!workspaceId) throw new Error("Missing workspaceId.");
      return api.post<{ channelId: string }>(
        `/workspaces/${workspaceId}/channels/${channelId}/join`,
        {},
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["channelMembers", workspaceId, channelId],
      });
      await queryClient.invalidateQueries({
        queryKey: ["workspaceChannels", workspaceId],
      });
    },
  });
}

export function useLeaveChannel(
  workspaceId: string | null,
  channelId: string,
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async () => {
      if (!workspaceId) throw new Error("Missing workspaceId.");
      return api.post<{ channelId: string }>(
        `/workspaces/${workspaceId}/channels/${channelId}/leave`,
        {},
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["channelMembers", workspaceId, channelId],
      });
      await queryClient.invalidateQueries({
        queryKey: ["workspaceChannels", workspaceId],
      });
    },
  });
}

