"use client";

import { useEffect, useRef, useState } from "react";

type Stat = {
  label: string;
  value: number;
  suffix?: string;
  note?: string;
};

const STATS: readonly Stat[] = [
  { value: 700, suffix: "M+", label: "messages processed", note: "at scale" },
  { value: 2, suffix: "×", label: "faster decisions", note: "with clear threads" },
  { value: 99.9, suffix: "%", label: "uptime target", note: "reliable by design" },
] as const;

function easeOutCubic(t: number) {
  return 1 - Math.pow(1 - t, 3);
}

function formatValue(value: number) {
  if (Number.isInteger(value)) return value.toString();
  return value.toFixed(1);
}

export default function StatsSection() {
  const [progress, setProgress] = useState(0);
  const startedAtRef = useRef<number | null>(null);

  useEffect(() => {
    const durationMs = 900;
    let raf = 0;

    const tick = () => {
      if (startedAtRef.current === null) startedAtRef.current = Date.now();
      const elapsed = Date.now() - startedAtRef.current;
      const next = Math.min(elapsed / durationMs, 1);
      setProgress(easeOutCubic(next));
      if (next < 1) raf = window.requestAnimationFrame(tick);
    };

    raf = window.requestAnimationFrame(tick);
    return () => window.cancelAnimationFrame(raf);
  }, []);

  return (
    <section className="rounded-3xl border border-black/10 bg-white p-8 dark:border-white/10 dark:bg-black">
      <div className="grid gap-6 lg:grid-cols-2 lg:items-center">
        <div>
          <h2 className="text-2xl font-semibold tracking-tight text-foreground">
            Built for speed, designed for focus.
          </h2>
          <p className="mt-2 max-w-xl text-sm text-zinc-600 dark:text-zinc-400">
            Get the signal without the noise—then find it later with search and
            AI.
          </p>
        </div>

        <div className="grid gap-4 sm:grid-cols-3">
          {STATS.map((s) => {
            const animated = s.value * progress;
            return (
              <div
                key={s.label}
                className="rounded-2xl bg-[color:var(--surface-2)] p-5"
              >
                <div className="text-3xl font-semibold tracking-tight text-foreground">
                  {formatValue(animated)}
                  {s.suffix ?? ""}
                </div>
                <div className="mt-2 text-sm font-medium text-foreground">
                  {s.label}
                </div>
                {s.note ? (
                  <div className="mt-1 text-xs text-zinc-600 dark:text-zinc-400">
                    {s.note}
                  </div>
                ) : null}
              </div>
            );
          })}
        </div>
      </div>
    </section>
  );
}
