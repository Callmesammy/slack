import { create } from "zustand";

type Theme = "light" | "dark" | "system";

type UiState = {
  isSidebarOpen: boolean;
  theme: Theme;
  setSidebarOpen: (open: boolean) => void;
  setTheme: (theme: Theme) => void;
};

export const useUiStore = create<UiState>((set) => ({
  isSidebarOpen: true,
  theme: "system",
  setSidebarOpen: (open) => set({ isSidebarOpen: open }),
  setTheme: (theme) => set({ theme }),
}));

