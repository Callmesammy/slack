"use client";

import { useInfiniteQuery } from "@tanstack/react-query";

import { api } from "@/lib/api";
import type { ChannelMessageDto } from "@/lib/hooks/useChannelMessages";

export type ChannelMessagePageDto = {
  items: ChannelMessageDto[];
  nextCursorMessageId: string | null;
};

export function useChannelMessagesInfinite(
  workspaceId: string | null,
  channelId: string,
) {
  return useInfiniteQuery({
    queryKey: ["channelMessages", workspaceId, channelId],
    enabled: Boolean(workspaceId) && Boolean(channelId),
    initialPageParam: null as string | null,
    queryFn: ({ pageParam }) =>
      api.get<ChannelMessagePageDto>(
        `/workspaces/${workspaceId}/channels/${channelId}/messages/page?limit=50${
          pageParam ? `&cursorMessageId=${encodeURIComponent(pageParam)}` : ""
        }`,
      ),
    getNextPageParam: (lastPage) => lastPage.nextCursorMessageId ?? undefined,
  });
}

