"use client";

import { usePresenceStore } from "@/stores/presenceStore";

export default function PresenceDot({
  userId,
}: Readonly<{
  userId: string;
}>) {
  const presence = usePresenceStore((s) => s.presenceByUserId[userId] ?? "offline");
  const color =
    presence === "online"
      ? "bg-emerald-500"
      : presence === "away"
        ? "bg-amber-500"
        : "bg-zinc-400";

  return <span className={`inline-block size-2 rounded-full ${color}`} />;
}

