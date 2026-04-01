import Link from "next/link";

export default function SlackbotSpotlight() {
  return (
    <section className="overflow-hidden rounded-3xl border border-black/10 bg-[color:var(--brand)] text-white dark:border-white/10">
      <div className="grid gap-8 px-6 py-10 md:px-10 lg:grid-cols-2 lg:items-center lg:gap-12">
        <div className="flex flex-col gap-4">
          <p className="inline-flex w-fit items-center rounded-full bg-white/10 px-3 py-1 text-xs font-medium">
            Slackbot
          </p>
          <h2 className="text-3xl font-semibold tracking-tight">
            Ask. Summarize. Catch up.
          </h2>
          <p className="max-w-xl text-sm leading-6 text-white/80">
            Slackbot keeps you in the loop with thread summaries, channel
            recaps, and writing help—right where your team already works.
          </p>
          <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
            <Link
              href="/features/ai"
              className="inline-flex h-11 items-center justify-center rounded-full bg-white px-6 text-sm font-semibold text-[color:var(--brand)] hover:opacity-95"
            >
              Explore AI features
            </Link>
            <Link
              href="/pricing"
              className="inline-flex h-11 items-center justify-center rounded-full border border-white/20 bg-transparent px-6 text-sm font-semibold text-white hover:bg-white/10"
            >
              See pricing
            </Link>
          </div>
        </div>

        <div className="rounded-2xl bg-white/10 p-4">
          <div className="flex flex-col gap-3 rounded-xl bg-black/15 p-4">
            <div className="text-xs font-semibold text-white/80">Slackbot</div>
            <div className="rounded-lg bg-white/10 p-3 text-sm leading-6">
              <p className="font-medium">What did I miss in #engineering?</p>
              <p className="mt-2 text-white/80">
                Here’s a quick recap: the team agreed to ship the auth flow
                first, then finish pricing and marketing sections next.
              </p>
            </div>
            <div className="flex gap-2">
              <div className="h-8 w-20 rounded-full bg-white/10" />
              <div className="h-8 w-28 rounded-full bg-white/10" />
              <div className="h-8 w-24 rounded-full bg-white/10" />
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}

