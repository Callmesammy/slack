import Link from "next/link";

const MENU = [
  {
    label: "Features",
    items: [
      { label: "Channels", href: "/features/channels" },
      { label: "AI", href: "/features/ai" },
      { label: "Huddles", href: "/features/huddles" },
      { label: "Canvas", href: "/features/canvas" },
      { label: "Workflow automation", href: "/features/workflow-automation" },
    ],
  },
  {
    label: "Solutions",
    items: [
      { label: "Engineering", href: "/solutions/engineering" },
      { label: "Sales", href: "/solutions/sales" },
    ],
  },
  {
    label: "Enterprise",
    items: [{ label: "Overview", href: "/enterprise" }],
  },
  {
    label: "Resources",
    items: [{ label: "Blog", href: "/resources" }],
  },
] as const;

export default function MegaMenu() {
  return (
    <div className="flex items-center gap-6 text-sm text-zinc-700 dark:text-zinc-300">
      {MENU.map((section) => (
        <details key={section.label} className="relative">
          <summary className="cursor-pointer list-none select-none hover:text-foreground">
            {section.label}
          </summary>
          <div className="absolute left-0 top-full pt-3">
            <div className="w-72 rounded-xl border border-black/10 bg-white p-3 shadow-lg dark:border-white/10 dark:bg-black">
              <div className="grid gap-1">
                {section.items.map((item) => (
                  <Link
                    key={item.href}
                    href={item.href}
                    className="rounded-lg px-3 py-2 text-sm text-zinc-700 hover:bg-black/[.04] hover:text-foreground dark:text-zinc-300 dark:hover:bg-white/[.06]"
                  >
                    {item.label}
                  </Link>
                ))}
              </div>
            </div>
          </div>
        </details>
      ))}

      <Link href="/pricing" className="hover:text-foreground">
        Pricing
      </Link>
    </div>
  );
}

