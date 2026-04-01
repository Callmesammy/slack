import { create } from "zustand";

type Presence = "online" | "offline" | "away";

type PresenceState = {
  presenceByUserId: Record<string, Presence>;
  setPresence: (userId: string, presence: Presence) => void;
  clearPresence: () => void;
};

export const usePresenceStore = create<PresenceState>((set) => ({
  presenceByUserId: {},
  setPresence: (userId, presence) =>
    set((state) => ({
      presenceByUserId: { ...state.presenceByUserId, [userId]: presence },
    })),
  clearPresence: () => set({ presenceByUserId: {} }),
}));

