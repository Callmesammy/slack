import { create } from "zustand";

type MessageState = {
  activeChannelId: string | null;
  activeThreadId: string | null;
  setActiveChannelId: (id: string | null) => void;
  setActiveThreadId: (id: string | null) => void;
};

export const useMessageStore = create<MessageState>((set) => ({
  activeChannelId: null,
  activeThreadId: null,
  setActiveChannelId: (id) => set({ activeChannelId: id }),
  setActiveThreadId: (id) => set({ activeThreadId: id }),
}));

