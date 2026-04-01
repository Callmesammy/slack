import Link from "next/link";

const GROUPS = [
  {
    title: "Product",
    links: [
      { label: "Features", href: "/features/channels" },
      { label: "Pricing", href: "/pricing" },
      { label: "Enterprise", href: "/enterprise" },
    ],
  },
  {
    title: "Solutions",
    links: [
      { label: "Engineering", href: "/solutions/engineering" },
      { label: "Sales", href: "/solutions/sales" },
    ],
  },
  {
    title: "Resources",
    links: [
      { label: "Resources", href: "/resources" },
      { label: "AI", href: "/features/ai" },
    ],
  },
  {
    title: "Get started",
    links: [
      { label: "Sign in", href: "/sign-in" },
      { label: "Sign up", href: "/sign-up" },
    ],
  },
] as const;

export default function Footer() {
  return (
    <footer className="border-t border-black/10 bg-white dark:border-white/10 dark:bg-black">
      <div className="mx-auto w-full max-w-6xl px-6 py-12">
        <div className="grid gap-10 sm:grid-cols-2 lg:grid-cols-5">
          <div className="flex flex-col gap-3">
            <div className="text-sm font-semibold tracking-tight text-foreground">
              Slack Clone
            </div>
            <p className="max-w-sm text-sm text-zinc-600 dark:text-zinc-400">
              Messaging, search, files, and AI—built to help your team move
              faster.
            </p>
          </div>

          {GROUPS.map((g) => (
            <div key={g.title} className="flex flex-col gap-3">
              <div className="text-sm font-semibold text-foreground">
                {g.title}
              </div>
              <ul className="flex flex-col gap-2 text-sm text-zinc-600 dark:text-zinc-400">
                {g.links.map((l) => (
                  <li key={l.href}>
                    <Link
                      href={l.href}
                      className="hover:text-foreground hover:underline"
                    >
                      {l.label}
                    </Link>
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </div>

        <div className="mt-10 flex flex-col gap-3 border-t border-black/10 pt-6 text-xs text-zinc-600 dark:border-white/10 dark:text-zinc-400 sm:flex-row sm:items-center sm:justify-between">
          <div>© {new Date().getFullYear()} Slack Clone</div>
          <div className="flex flex-wrap gap-x-4 gap-y-2">
            <span>Privacy</span>
            <span>Terms</span>
            <span>Status</span>
          </div>
        </div>
      </div>
    </footer>
  );
}
