"use client";

import { useEffect, useMemo, useRef } from "react";
import { useVirtualizer } from "@tanstack/react-virtual";

import MessageItem from "@/components/app/MessageItem";
import { formatDayLabel, getDayKey } from "@/lib/dateLabels";
import { useChannelMessagesInfinite } from "@/lib/hooks/useChannelMessagesInfinite";
import { useWorkspaceBySlug } from "@/lib/hooks/useWorkspaceBySlug";

export default function ChannelMessageList({
  workspaceSlug,
  channelId,
}: Readonly<{
  workspaceSlug: string;
  channelId: string;
}>) {
  const parentRef = useRef<HTMLDivElement | null>(null);

  const workspaceQuery = useWorkspaceBySlug(workspaceSlug);
  const workspaceId =
    workspaceQuery.status === "success" ? workspaceQuery.data.id : null;

  const messagesQuery = useChannelMessagesInfinite(workspaceId, channelId);

  const items = useMemo(() => {
    if (messagesQuery.status !== "success") return [];
    const all = messagesQuery.data.pages.flatMap((p) => p.items);
    return [...all].reverse();
  }, [messagesQuery.data, messagesQuery.status]);

  const rows = useMemo(() => {
    const out: Array<
      | { kind: "divider"; key: string; label: string }
      | { kind: "message"; id: string; messageIndex: number }
    > = [];

    let lastDayKey: string | null = null;
    items.forEach((m, idx) => {
      const dayKey = getDayKey(m.createdAt);
      if (dayKey !== lastDayKey) {
        out.push({ kind: "divider", key: dayKey, label: formatDayLabel(m.createdAt) });
        lastDayKey = dayKey;
      }
      out.push({ kind: "message", id: m.id, messageIndex: idx });
    });

    return out;
  }, [items]);

  const rowVirtualizer = useVirtualizer({
    count: rows.length,
    getScrollElement: () => parentRef.current,
    estimateSize: (index) => (rows[index]?.kind === "divider" ? 36 : 96),
    overscan: 10,
  });

  useEffect(() => {
    if (items.length === 0) return;
    rowVirtualizer.scrollToIndex(rows.length - 1, { align: "end" });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rows.length]);

  const fetchingNextRef = useRef(false);
  useEffect(() => {
    const el = parentRef.current;
    if (!el) return;

    const hasNextPage = messagesQuery.hasNextPage;
    const isFetchingNextPage = messagesQuery.isFetchingNextPage;
    const fetchNextPage = messagesQuery.fetchNextPage;

    const onScroll = () => {
      if (!hasNextPage) return;
      if (isFetchingNextPage) return;
      if (fetchingNextRef.current) return;

      if (el.scrollTop <= 24) {
        fetchingNextRef.current = true;
        const prevHeight = el.scrollHeight;
        const prevTop = el.scrollTop;

        fetchNextPage()
          .then(() => {
            // keep viewport anchored after older messages prepend
            requestAnimationFrame(() => {
              const nextHeight = el.scrollHeight;
              el.scrollTop = prevTop + (nextHeight - prevHeight);
            });
          })
          .finally(() => {
            fetchingNextRef.current = false;
          });
      }
    };

    el.addEventListener("scroll", onScroll, { passive: true });
    return () => el.removeEventListener("scroll", onScroll);
  }, [messagesQuery.hasNextPage, messagesQuery.isFetchingNextPage, messagesQuery.fetchNextPage]);

  if (workspaceQuery.status === "pending") {
    return <div className="p-6 text-sm text-zinc-600 dark:text-zinc-400">Loading…</div>;
  }

  if (workspaceQuery.status === "error") {
    return (
      <div className="p-6 text-sm text-zinc-600 dark:text-zinc-400">
        Failed to load workspace.
      </div>
    );
  }

  return (
    <div className="flex flex-1 flex-col">
      <div className="flex-1 overflow-auto px-6 py-4" ref={parentRef}>
        {messagesQuery.status === "pending" ? (
          <div className="text-sm text-zinc-600 dark:text-zinc-400">Loading messages…</div>
        ) : null}
        {messagesQuery.status === "error" ? (
          <div className="text-sm text-zinc-600 dark:text-zinc-400">
            Failed to load messages.
          </div>
        ) : null}
        {messagesQuery.status === "success" ? (
          <div style={{ width: "100%" }}>
            {messagesQuery.hasNextPage ? (
              <div className="mb-3 flex justify-center">
                <button
                  type="button"
                  disabled={messagesQuery.isFetchingNextPage}
                  onClick={() => messagesQuery.fetchNextPage()}
                  className="rounded-full border border-black/10 bg-white px-4 py-2 text-xs font-semibold text-foreground hover:bg-black/[.04] disabled:opacity-50 dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]"
                >
                  {messagesQuery.isFetchingNextPage ? "Loading…" : "Load older messages"}
                </button>
              </div>
            ) : null}

            <div
              style={{
                height: `${rowVirtualizer.getTotalSize()}px`,
                width: "100%",
                position: "relative",
              }}
            >
            {rowVirtualizer.getVirtualItems().map((virtualRow) => {
              const row = rows[virtualRow.index];
              if (!row) return null;
              return (
                <div
                  key={row.kind === "divider" ? `d:${row.key}` : `m:${row.id}`}
                  data-index={virtualRow.index}
                  ref={rowVirtualizer.measureElement}
                  style={{
                    position: "absolute",
                    top: 0,
                    left: 0,
                    width: "100%",
                    transform: `translateY(${virtualRow.start}px)`,
                    paddingBottom: "12px",
                  }}
                >
                  {row.kind === "divider" ? (
                    <div className="flex items-center gap-3 py-2">
                      <div className="h-px flex-1 bg-black/10 dark:bg-white/10" />
                      <div className="rounded-full border border-black/10 bg-white px-3 py-1 text-xs font-semibold text-zinc-600 dark:border-white/10 dark:bg-black dark:text-zinc-400">
                        {row.label}
                      </div>
                      <div className="h-px flex-1 bg-black/10 dark:bg-white/10" />
                    </div>
                  ) : (
                    <MessageItem
                      workspaceId={workspaceId}
                      channelId={channelId}
                      message={items[row.messageIndex]!}
                    />
                  )}
                </div>
              );
            })}
            {items.length === 0 ? (
              <div className="text-sm text-zinc-600 dark:text-zinc-400">
                No messages yet.
              </div>
            ) : null}
            </div>
          </div>
        ) : null}
      </div>
    </div>
  );
}
