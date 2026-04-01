"use client";

import { useMemo, useState } from "react";

type TabKey = "collaboration" | "knowledge" | "integrations" | "ai";

type Tab = {
  key: TabKey;
  label: string;
  title: string;
  description: string;
  stats: readonly { label: string; value: string }[];
};

const TABS: readonly Tab[] = [
  {
    key: "collaboration",
    label: "Collaboration",
    title: "Turn conversations into outcomes.",
    description:
      "Keep decisions, files, and updates together so your team can move fast and stay aligned.",
    stats: [
      { value: "↓ 32%", label: "fewer status meetings" },
      { value: "↑ 24%", label: "faster handoffs" },
    ],
  },
  {
    key: "knowledge",
    label: "Knowledge",
    title: "Find the answer in seconds.",
    description:
      "Search across messages and files so new hires ramp faster and teams avoid repeating work.",
    stats: [
      { value: "15", label: "top results returned" },
      { value: "Hybrid", label: "keyword + semantic" },
    ],
  },
  {
    key: "integrations",
    label: "Integrations",
    title: "Bring tools into the flow of work.",
    description:
      "Connect your stack so alerts, tasks, and updates land in the right channel at the right time.",
    stats: [
      { value: "Webhooks", label: "automation-ready" },
      { value: "Events", label: "real-time updates" },
    ],
  },
  {
    key: "ai",
    label: "AI",
    title: "Let AI handle the busywork.",
    description:
      "Summaries, recaps, and writing help so you can focus on the work that matters.",
    stats: [
      { value: "Recaps", label: "daily channel highlights" },
      { value: "Summaries", label: "threads + huddles" },
    ],
  },
] as const;

export default function ValuePropsSection() {
  const [active, setActive] = useState<TabKey>("collaboration");

  const activeTab = useMemo(() => {
    return TABS.find((t) => t.key === active) ?? TABS[0];
  }, [active]);

  return (
    <section className="flex flex-col gap-6">
      <div>
        <h2 className="text-2xl font-semibold tracking-tight text-foreground">
          Built for teams that ship.
        </h2>
        <p className="mt-2 max-w-2xl text-sm text-zinc-600 dark:text-zinc-400">
          Explore how channels, search, and AI help your team stay aligned.
        </p>
      </div>

      <div className="flex flex-col gap-4">
        <div className="flex flex-wrap gap-2">
          {TABS.map((tab) => (
            <button
              key={tab.key}
              type="button"
              onClick={() => setActive(tab.key)}
              className={[
                "rounded-full px-4 py-2 text-sm font-medium transition-colors",
                tab.key === active
                  ? "bg-foreground text-background"
                  : "border border-black/10 bg-white text-zinc-700 hover:bg-black/[.04] dark:border-white/10 dark:bg-black dark:text-zinc-300 dark:hover:bg-white/[.06]",
              ].join(" ")}
            >
              {tab.label}
            </button>
          ))}
        </div>

        <div className="grid gap-6 rounded-2xl border border-black/10 bg-white p-6 dark:border-white/10 dark:bg-black lg:grid-cols-2 lg:items-center">
          <div className="flex flex-col gap-3">
            <h3 className="text-xl font-semibold tracking-tight text-foreground">
              {activeTab.title}
            </h3>
            <p className="text-sm leading-6 text-zinc-600 dark:text-zinc-400">
              {activeTab.description}
            </p>
            <div className="mt-2 grid gap-3 sm:grid-cols-2">
              {activeTab.stats.map((s) => (
                <div
                  key={s.label}
                  className="rounded-xl bg-[color:var(--surface-2)] p-4"
                >
                  <div className="text-lg font-semibold text-foreground">
                    {s.value}
                  </div>
                  <div className="mt-1 text-xs text-zinc-600 dark:text-zinc-400">
                    {s.label}
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div className="rounded-2xl bg-[color:var(--surface-2)] p-4">
            <div className="aspect-[16/10] w-full rounded-xl bg-black/10 dark:bg-white/10" />
            <p className="mt-3 text-xs text-zinc-600 dark:text-zinc-400">
              Visual placeholder for {activeTab.label}.
            </p>
          </div>
        </div>
      </div>
    </section>
  );
}

