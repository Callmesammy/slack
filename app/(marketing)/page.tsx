import Link from "next/link";

import SlackbotSpotlight from "@/components/marketing/SlackbotSpotlight";
import StatsSection from "@/components/marketing/StatsSection";
import ValuePropsSection from "@/components/marketing/ValuePropsSection";

export default function Home() {
  return (
    <main className="mx-auto flex w-full max-w-6xl flex-1 flex-col px-6 py-16">
      <div className="grid gap-10 lg:grid-cols-2 lg:items-center">
        <div className="flex flex-col gap-6">
          <p className="inline-flex w-fit items-center rounded-full border border-black/10 bg-white px-3 py-1 text-xs font-medium text-zinc-700 dark:border-white/10 dark:bg-black dark:text-zinc-300">
            New: AI summaries, recaps, and search
          </p>
          <h1 className="text-4xl font-semibold tracking-tight text-foreground sm:text-5xl">
            Move work forward in channels, not meetings.
          </h1>
          <p className="max-w-xl text-base leading-7 text-zinc-600 dark:text-zinc-400">
            A Slack-style collaboration hub for your team: messaging, huddles,
            file sharing, search, and AI that keeps everyone aligned.
          </p>
          <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
            <Link
              href="/sign-up"
              className="inline-flex h-11 items-center justify-center rounded-full bg-[color:var(--brand)] px-6 text-sm font-semibold text-white hover:opacity-90"
            >
              Get started
            </Link>
            <Link
              href="/pricing"
              className="inline-flex h-11 items-center justify-center rounded-full border border-black/10 bg-white px-6 text-sm font-semibold text-foreground hover:bg-black/[.04] dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]"
            >
              See pricing
            </Link>
          </div>
          <p className="text-xs text-zinc-500 dark:text-zinc-400">
            No credit card required. Upgrade anytime.
          </p>
        </div>

        <div className="rounded-2xl border border-black/10 bg-[color:var(--surface-2)] p-6 dark:border-white/10">
          <div className="aspect-[16/10] w-full rounded-xl bg-black/10 dark:bg-white/10" />
          <p className="mt-4 text-sm text-zinc-600 dark:text-zinc-400">
            Product preview placeholder.
          </p>
        </div>
      </div>

      <div className="mt-16">
        <ValuePropsSection />
      </div>

      <div className="mt-16">
        <SlackbotSpotlight />
      </div>

      <div className="mt-16">
        <StatsSection />
      </div>
    </main>
  );
}
