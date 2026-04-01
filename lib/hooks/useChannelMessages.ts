"use client";

import { useQuery } from "@tanstack/react-query";

import { api } from "@/lib/api";

export type ChannelMessageDto = {
  id: string;
  senderId: string;
  senderEmail: string;
  senderName: string | null;
  content: string;
  contentFormat: string;
  type: string;
  threadParentId: string | null;
  createdAt: string;
  reactions: { emoji: string; count: number; reactedByMe: boolean }[];
};

export function useChannelMessages(workspaceId: string | null, channelId: string) {
  return useQuery({
    queryKey: ["channelMessages", workspaceId, channelId],
    enabled: Boolean(workspaceId) && Boolean(channelId),
    queryFn: () =>
      api.get<ChannelMessageDto[]>(
        `/workspaces/${workspaceId}/channels/${channelId}/messages?limit=50`,
      ),
  });
}
