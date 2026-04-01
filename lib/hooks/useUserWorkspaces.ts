"use client";

import { useQuery } from "@tanstack/react-query";

import { api } from "@/lib/api";

export type UserWorkspaceDto = {
  id: string;
  name: string;
  slug: string;
  role: string;
  plan: string;
};

export function useUserWorkspaces() {
  return useQuery({
    queryKey: ["workspaces"],
    queryFn: () => api.get<UserWorkspaceDto[]>("/workspaces"),
  });
}

