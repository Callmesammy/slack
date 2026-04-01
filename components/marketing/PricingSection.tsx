"use client";

import { useMemo, useState } from "react";

type BillingPeriod = "monthly" | "annual";

type Plan = {
  name: string;
  description: string;
  monthlyUsd: number;
  annualUsd: number;
  cta: string;
  highlights: readonly string[];
  badge?: string;
};

const PLANS: readonly Plan[] = [
  {
    name: "Free",
    description: "For small teams getting started.",
    monthlyUsd: 0,
    annualUsd: 0,
    cta: "Get started",
    highlights: ["Messaging", "Channels", "Basic search", "10 AI assists/day"],
  },
  {
    name: "Pro",
    description: "For growing teams that need more power.",
    monthlyUsd: 8,
    annualUsd: 7,
    cta: "Try Pro",
    badge: "Most popular",
    highlights: ["Unlimited history", "Group huddles", "AI summaries", "20 summaries/day"],
  },
  {
    name: "Business+",
    description: "For advanced controls and scale.",
    monthlyUsd: 15,
    annualUsd: 13,
    cta: "Contact sales",
    highlights: ["Admin controls", "SAML SSO", "Channel recaps", "Compliance exports"],
  },
  {
    name: "Enterprise",
    description: "For large orgs with security requirements.",
    monthlyUsd: 0,
    annualUsd: 0,
    cta: "Talk to sales",
    highlights: ["SCIM", "Custom retention", "Azure OpenAI option", "Enterprise support"],
  },
] as const;

function formatUsd(amount: number) {
  if (amount === 0) return "$0";
  return `$${amount}`;
}

export default function PricingSection() {
  const [period, setPeriod] = useState<BillingPeriod>("monthly");

  const subtitle = useMemo(() => {
    return period === "monthly"
      ? "Billed monthly. Cancel anytime."
      : "Billed annually. Save with annual pricing.";
  }, [period]);

  return (
    <section className="flex flex-col gap-8">
      <div>
        <h1 className="text-3xl font-semibold tracking-tight text-foreground">
          Pricing
        </h1>
        <p className="mt-2 max-w-2xl text-sm text-zinc-600 dark:text-zinc-400">
          Pick a plan that fits your team. Upgrade anytime.
        </p>
      </div>

      <div className="flex items-center justify-between gap-6">
        <p className="text-sm text-zinc-600 dark:text-zinc-400">{subtitle}</p>
        <BillingToggle period={period} onChange={setPeriod} />
      </div>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {PLANS.map((plan) => (
          <PlanCard key={plan.name} plan={plan} period={period} />
        ))}
      </div>

      <FeatureComparisonTable />
    </section>
  );
}

function BillingToggle({
  period,
  onChange,
}: {
  period: BillingPeriod;
  onChange: (next: BillingPeriod) => void;
}) {
  return (
    <div className="flex items-center rounded-full border border-black/10 bg-white p-1 text-sm dark:border-white/10 dark:bg-black">
      <button
        type="button"
        onClick={() => onChange("monthly")}
        className={[
          "rounded-full px-3 py-1.5 transition-colors",
          period === "monthly"
            ? "bg-foreground text-background"
            : "text-zinc-700 hover:bg-black/[.04] dark:text-zinc-300 dark:hover:bg-white/[.06]",
        ].join(" ")}
      >
        Monthly
      </button>
      <button
        type="button"
        onClick={() => onChange("annual")}
        className={[
          "rounded-full px-3 py-1.5 transition-colors",
          period === "annual"
            ? "bg-foreground text-background"
            : "text-zinc-700 hover:bg-black/[.04] dark:text-zinc-300 dark:hover:bg-white/[.06]",
        ].join(" ")}
      >
        Annual
      </button>
    </div>
  );
}

function PlanCard({ plan, period }: { plan: Plan; period: BillingPeriod }) {
  const price =
    plan.name === "Enterprise"
      ? "Custom"
      : formatUsd(period === "monthly" ? plan.monthlyUsd : plan.annualUsd);

  const priceSuffix =
    plan.name === "Enterprise"
      ? ""
      : plan.name === "Free"
        ? "/mo"
        : period === "monthly"
          ? "/user/mo"
          : "/user/mo (annual)";

  const isFeatured = plan.badge !== undefined;

  return (
    <div
      className={[
        "relative flex flex-col rounded-2xl border bg-white p-5 shadow-sm dark:bg-black",
        isFeatured
          ? "border-black/20 dark:border-white/20"
          : "border-black/10 dark:border-white/10",
      ].join(" ")}
    >
      {plan.badge ? (
        <div className="absolute -top-3 left-5 rounded-full bg-[color:var(--brand)] px-3 py-1 text-xs font-semibold text-white">
          {plan.badge}
        </div>
      ) : null}

      <div className="flex flex-col gap-1">
        <h2 className="text-lg font-semibold tracking-tight text-foreground">
          {plan.name}
        </h2>
        <p className="text-sm text-zinc-600 dark:text-zinc-400">
          {plan.description}
        </p>
      </div>

      <div className="mt-5">
        <div className="flex items-baseline gap-2">
          <div className="text-3xl font-semibold tracking-tight text-foreground">
            {price}
          </div>
          <div className="text-sm text-zinc-600 dark:text-zinc-400">
            {priceSuffix}
          </div>
        </div>
      </div>

      <button
        type="button"
        className={[
          "mt-5 inline-flex h-11 items-center justify-center rounded-full px-4 text-sm font-medium transition-colors",
          isFeatured
            ? "bg-foreground text-background hover:opacity-90"
            : "border border-black/10 bg-white text-foreground hover:bg-black/[.04] dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]",
        ].join(" ")}
      >
        {plan.cta}
      </button>

      <ul className="mt-5 flex flex-col gap-2 text-sm text-zinc-700 dark:text-zinc-300">
        {plan.highlights.map((text) => (
          <li key={text} className="flex gap-2">
            <span className="mt-0.5 h-4 w-4 shrink-0 rounded-full bg-black/10 dark:bg-white/10" />
            <span>{text}</span>
          </li>
        ))}
      </ul>
    </div>
  );
}

function FeatureComparisonTable() {
  return (
    <section className="mt-10">
      <div className="flex items-end justify-between gap-6">
        <div>
          <h2 className="text-2xl font-semibold tracking-tight text-foreground">
            Compare plans
          </h2>
          <p className="mt-2 max-w-2xl text-sm text-zinc-600 dark:text-zinc-400">
            Feature matrix scaffold. We’ll fill this out category-by-category.
          </p>
        </div>
      </div>

      <div className="mt-6 overflow-hidden rounded-2xl border border-black/10 bg-white dark:border-white/10 dark:bg-black">
        <ComparisonCategory title="Core messaging" defaultOpen>
          <ComparisonRow label="Channels" values={["✓", "✓", "✓", "✓"]} />
          <ComparisonRow label="Direct messages" values={["✓", "✓", "✓", "✓"]} />
          <ComparisonRow label="Message history" values={["Limited", "Unlimited", "Unlimited", "Unlimited"]} />
        </ComparisonCategory>

        <ComparisonCategory title="AI" defaultOpen={false}>
          <ComparisonRow label="Writing assistance" values={["Limited", "✓", "✓", "✓"]} />
          <ComparisonRow label="Thread summaries" values={["—", "✓", "✓", "✓"]} />
          <ComparisonRow label="Channel recaps" values={["—", "—", "✓", "✓"]} />
        </ComparisonCategory>

        <ComparisonCategory title="Security & admin" defaultOpen={false}>
          <ComparisonRow label="Workspace roles" values={["✓", "✓", "✓", "✓"]} />
          <ComparisonRow label="SAML SSO" values={["—", "—", "✓", "✓"]} />
          <ComparisonRow label="SCIM" values={["—", "—", "—", "✓"]} />
        </ComparisonCategory>
      </div>

      <PricingFaq />
    </section>
  );
}

function ComparisonCategory({
  title,
  defaultOpen,
  children,
}: Readonly<{
  title: string;
  defaultOpen: boolean;
  children: React.ReactNode;
}>) {
  return (
    <details open={defaultOpen} className="group border-t border-black/10 dark:border-white/10">
      <summary className="flex cursor-pointer list-none items-center justify-between gap-4 px-5 py-4">
        <div className="text-sm font-semibold text-foreground">{title}</div>
        <div className="text-xs text-zinc-600 dark:text-zinc-400">
          <span className="group-open:hidden">Expand</span>
          <span className="hidden group-open:inline">Collapse</span>
        </div>
      </summary>
      <div className="px-5 pb-4">{children}</div>
    </details>
  );
}

function ComparisonRow({
  label,
  values,
}: Readonly<{
  label: string;
  values: readonly [string, string, string, string];
}>) {
  return (
    <div className="grid grid-cols-5 gap-3 py-3 text-sm">
      <div className="col-span-2 text-zinc-700 dark:text-zinc-300">{label}</div>
      {values.map((v, i) => (
        <div
          key={`${label}-${i}`}
          className="text-center text-zinc-700 dark:text-zinc-300"
        >
          {v}
        </div>
      ))}
    </div>
  );
}

function PricingFaq() {
  const items = [
    {
      q: "Can I start for free?",
      a: "Yes. Start on Free, then upgrade anytime as your team grows.",
    },
    {
      q: "Do you offer annual billing?",
      a: "Yes. Annual billing is available and typically costs less per user per month.",
    },
    {
      q: "Which AI features are included?",
      a: "Writing assistance is available on all plans with limits. Summaries and recaps are available on higher tiers.",
    },
    {
      q: "Can admins disable AI?",
      a: "Yes. AI can be disabled per workspace by an admin (per the architecture rules).",
    },
  ] as const;

  return (
    <section className="mt-12">
      <h2 className="text-2xl font-semibold tracking-tight text-foreground">
        FAQ
      </h2>
      <div className="mt-6 grid gap-3">
        {items.map((item) => (
          <details
            key={item.q}
            className="rounded-2xl border border-black/10 bg-white px-5 py-4 dark:border-white/10 dark:bg-black"
          >
            <summary className="flex cursor-pointer list-none items-center justify-between gap-4 text-sm font-semibold text-foreground">
              {item.q}
              <span className="text-zinc-600 dark:text-zinc-400" aria-hidden="true">
                +
              </span>
            </summary>
            <p className="mt-3 text-sm leading-6 text-zinc-600 dark:text-zinc-400">
              {item.a}
            </p>
          </details>
        ))}
      </div>
    </section>
  );
}
