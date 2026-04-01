"use client";

import Link from "next/link";
import { useEffect, useState } from "react";

import MegaMenu from "@/components/marketing/MegaMenu";

export default function Navbar() {
  const [isScrolled, setIsScrolled] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  useEffect(() => {
    const onScroll = () => setIsScrolled(window.scrollY > 8);
    onScroll();
    window.addEventListener("scroll", onScroll, { passive: true });
    return () => window.removeEventListener("scroll", onScroll);
  }, []);

  useEffect(() => {
    if (!isMobileMenuOpen) return;
    const onKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") setIsMobileMenuOpen(false);
    };
    window.addEventListener("keydown", onKeyDown);
    return () => window.removeEventListener("keydown", onKeyDown);
  }, [isMobileMenuOpen]);

  return (
    <header
      className={[
        "sticky top-0 z-50 backdrop-blur transition-colors",
        isScrolled
          ? "border-b border-black/10 bg-white/85 dark:border-white/10 dark:bg-black/70"
          : "border-b border-transparent bg-white/60 dark:bg-black/40",
      ].join(" ")}
    >
      <div className="mx-auto flex h-14 max-w-6xl items-center justify-between px-6">
        <Link
          href="/"
          className={[
            "text-sm font-semibold tracking-tight transition-colors",
            isScrolled ? "text-foreground" : "text-foreground",
          ].join(" ")}
        >
          <span className="sr-only">Slack Clone</span>
          <span
            aria-hidden="true"
            className={[
              "inline-flex items-center gap-2",
              isScrolled ? "text-foreground" : "text-foreground",
            ].join(" ")}
          >
            <span
              className={[
                "inline-flex h-6 w-6 items-center justify-center rounded-md text-xs font-bold text-white",
                isScrolled
                  ? "bg-[color:var(--brand)]"
                  : "bg-[color:var(--brand-2)]",
              ].join(" ")}
            >
              S
            </span>
            <span>Slack Clone</span>
          </span>
        </Link>

        <nav className="hidden md:flex">
          <MegaMenu />
        </nav>

        <div className="hidden items-center gap-4 text-sm text-zinc-700 dark:text-zinc-300 md:flex">
          <Link href="/sign-in" className="hover:text-foreground">
            Sign in
          </Link>
          <Link
            href="/pricing"
            className="rounded-full bg-foreground px-4 py-2 text-background hover:opacity-90"
          >
            See pricing
          </Link>
        </div>

        <button
          type="button"
          className="inline-flex h-10 w-10 items-center justify-center rounded-full border border-black/10 bg-white text-sm text-foreground hover:bg-black/[.04] dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06] md:hidden"
          aria-label="Open menu"
          aria-expanded={isMobileMenuOpen}
          onClick={() => setIsMobileMenuOpen(true)}
        >
          <span aria-hidden="true">≡</span>
        </button>
      </div>

      {isMobileMenuOpen ? (
        <div className="fixed inset-0 z-50 md:hidden">
          <div
            className="absolute inset-0 bg-black/40"
            onClick={() => setIsMobileMenuOpen(false)}
            aria-hidden="true"
          />
          <div className="absolute inset-x-0 top-0 max-h-[100dvh] overflow-auto rounded-b-3xl border-b border-black/10 bg-white p-6 shadow-xl dark:border-white/10 dark:bg-black">
            <div className="flex items-center justify-between">
              <div className="text-sm font-semibold tracking-tight text-foreground">
                Menu
              </div>
              <button
                type="button"
                className="inline-flex h-10 w-10 items-center justify-center rounded-full border border-black/10 bg-white text-sm text-foreground hover:bg-black/[.04] dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]"
                aria-label="Close menu"
                onClick={() => setIsMobileMenuOpen(false)}
              >
                <span aria-hidden="true">×</span>
              </button>
            </div>

            <div className="mt-6 grid gap-2 text-sm">
              <MobileLink href="/features/channels" onNavigate={setIsMobileMenuOpen}>
                Features
              </MobileLink>
              <MobileLink href="/solutions/engineering" onNavigate={setIsMobileMenuOpen}>
                Solutions
              </MobileLink>
              <MobileLink href="/enterprise" onNavigate={setIsMobileMenuOpen}>
                Enterprise
              </MobileLink>
              <MobileLink href="/resources" onNavigate={setIsMobileMenuOpen}>
                Resources
              </MobileLink>
              <MobileLink href="/pricing" onNavigate={setIsMobileMenuOpen}>
                Pricing
              </MobileLink>
            </div>

            <div className="mt-6 grid gap-3">
              <Link
                href="/sign-in"
                onClick={() => setIsMobileMenuOpen(false)}
                className="inline-flex h-11 items-center justify-center rounded-full border border-black/10 bg-white px-6 text-sm font-semibold text-foreground hover:bg-black/[.04] dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]"
              >
                Sign in
              </Link>
              <Link
                href="/sign-up"
                onClick={() => setIsMobileMenuOpen(false)}
                className="inline-flex h-11 items-center justify-center rounded-full bg-[color:var(--brand)] px-6 text-sm font-semibold text-white hover:opacity-90"
              >
                Get started
              </Link>
            </div>
          </div>
        </div>
      ) : null}
    </header>
  );
}

function MobileLink({
  href,
  onNavigate,
  children,
}: Readonly<{
  href: string;
  onNavigate: (open: boolean) => void;
  children: React.ReactNode;
}>) {
  return (
    <Link
      href={href}
      onClick={() => onNavigate(false)}
      className="rounded-xl border border-black/10 bg-white px-4 py-3 font-medium text-foreground hover:bg-black/[.04] dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]"
    >
      {children}
    </Link>
  );
}
