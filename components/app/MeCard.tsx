"use client";

import { useQuery } from "@tanstack/react-query";

import { api } from "@/lib/api";

type MeResponse = {
  id: string | null;
  email: string | null;
  name: string | null;
};

export default function MeCard() {
  const query = useQuery({
    queryKey: ["me"],
    queryFn: () => api.get<MeResponse>("/me"),
  });

  return (
    <section className="rounded-2xl border border-black/10 bg-white p-5 dark:border-white/10 dark:bg-black">
      <h2 className="text-sm font-semibold text-foreground">Auth check</h2>
      <p className="mt-1 text-xs text-zinc-600 dark:text-zinc-400">
        Fetches <code className="font-mono">GET /api/v1/me</code> using the
        current JWT.
      </p>

      <div className="mt-4 rounded-xl bg-[color:var(--surface-2)] p-4 text-sm text-zinc-700 dark:text-zinc-300">
        {query.status === "pending" ? <div>Loading…</div> : null}
        {query.status === "error" ? (
          <div>
            <div className="font-medium text-foreground">Request failed</div>
            <div className="mt-1 text-xs opacity-80">
              {query.error instanceof Error ? query.error.message : "Unknown error"}
            </div>
          </div>
        ) : null}
        {query.status === "success" ? (
          <div className="grid gap-1">
            <div>
              <span className="text-xs text-zinc-600 dark:text-zinc-400">id:</span>{" "}
              <span className="font-mono">{query.data.id ?? "null"}</span>
            </div>
            <div>
              <span className="text-xs text-zinc-600 dark:text-zinc-400">
                email:
              </span>{" "}
              <span className="font-mono">{query.data.email ?? "null"}</span>
            </div>
            <div>
              <span className="text-xs text-zinc-600 dark:text-zinc-400">
                name:
              </span>{" "}
              <span className="font-mono">{query.data.name ?? "null"}</span>
            </div>
          </div>
        ) : null}
      </div>
    </section>
  );
}

