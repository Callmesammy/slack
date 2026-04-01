"use client";

import { useQuery } from "@tanstack/react-query";

import { api } from "@/lib/api";

export type WorkspaceBySlugDto = {
  id: string;
  name: string;
  slug: string;
};

export function useWorkspaceBySlug(slug: string) {
  return useQuery({
    queryKey: ["workspaceBySlug", slug],
    queryFn: () => api.get<WorkspaceBySlugDto>(`/workspaces/lookup/by-slug/${slug}`),
    enabled: Boolean(slug),
  });
}

