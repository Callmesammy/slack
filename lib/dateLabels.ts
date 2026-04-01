export function getDayKey(iso: string) {
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return "unknown";
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`;
}

export function formatDayLabel(iso: string) {
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return "Unknown";

  const now = new Date();
  const todayKey = getDayKey(now.toISOString());
  const dayKey = getDayKey(iso);

  if (dayKey === todayKey) return "Today";

  const yesterday = new Date(now);
  yesterday.setDate(now.getDate() - 1);
  const yesterdayKey = getDayKey(yesterday.toISOString());
  if (dayKey === yesterdayKey) return "Yesterday";

  return d.toLocaleDateString(undefined, { weekday: "short", month: "short", day: "numeric" });
}

