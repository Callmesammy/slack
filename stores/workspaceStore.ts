import { create } from "zustand";

type WorkspaceState = {
  activeWorkspaceSlug: string | null;
  setActiveWorkspaceSlug: (slug: string | null) => void;
};

export const useWorkspaceStore = create<WorkspaceState>((set) => ({
  activeWorkspaceSlug: null,
  setActiveWorkspaceSlug: (slug) => set({ activeWorkspaceSlug: slug }),
}));

