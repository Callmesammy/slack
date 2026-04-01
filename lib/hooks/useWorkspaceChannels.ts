"use client";

import { useQuery } from "@tanstack/react-query";

import { api } from "@/lib/api";

export type WorkspaceChannelDto = {
  id: string;
  name: string | null;
  type: string;
  isArchived: boolean;
  memberCount: number;
  lastMessageAt: string | null;
};

export function useWorkspaceChannels(workspaceId: string | null) {
  return useQuery({
    queryKey: ["workspaceChannels", workspaceId],
    enabled: Boolean(workspaceId),
    queryFn: () =>
      api.get<WorkspaceChannelDto[]>(`/workspaces/${workspaceId}/channels`),
  });
}

