"use client";

import { useQuery } from "@tanstack/react-query";

import { api } from "@/lib/api";

export type ChannelMemberDto = {
  userId: string;
  email: string;
  name: string | null;
  role: string;
  joinedAt: string;
};

export function useChannelMembers(
  workspaceId: string | null,
  channelId: string,
) {
  return useQuery({
    queryKey: ["channelMembers", workspaceId, channelId],
    enabled: Boolean(workspaceId) && Boolean(channelId),
    queryFn: () =>
      api.get<ChannelMemberDto[]>(
        `/workspaces/${workspaceId}/channels/${channelId}/members`,
      ),
  });
}

