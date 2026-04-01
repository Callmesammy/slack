"use client";

import Link from "next/link";

import { useUserWorkspaces } from "@/lib/hooks/useUserWorkspaces";

export default function WorkspaceSwitcher({
  activeSlug,
}: Readonly<{
  activeSlug: string;
}>) {
  const query = useUserWorkspaces();

  if (query.status === "pending") {
    return <div className="text-xs text-zinc-600 dark:text-zinc-400">Loading…</div>;
  }

  if (query.status === "error") {
    return (
      <div className="text-xs text-zinc-600 dark:text-zinc-400">
        Failed to load workspaces.
      </div>
    );
  }

  return (
    <div className="grid gap-2">
      {query.data.map((w) => (
        <Link
          key={w.id}
          href={`/app/${w.slug}`}
          className={[
            "rounded-xl border px-3 py-2 text-sm transition-colors",
            w.slug === activeSlug
              ? "border-black/20 bg-black/[.04] text-foreground dark:border-white/20 dark:bg-white/[.06]"
              : "border-black/10 bg-white text-zinc-700 hover:bg-black/[.04] dark:border-white/10 dark:bg-black dark:text-zinc-300 dark:hover:bg-white/[.06]",
          ].join(" ")}
        >
          <div className="font-medium">{w.name}</div>
          <div className="mt-0.5 text-xs opacity-75">
            {w.role} · {w.plan}
          </div>
        </Link>
      ))}
      {query.data.length === 0 ? (
        <div className="text-xs text-zinc-600 dark:text-zinc-400">
          No workspaces yet.
        </div>
      ) : null}
    </div>
  );
}

