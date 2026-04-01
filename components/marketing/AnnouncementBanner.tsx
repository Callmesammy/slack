"use client";

import { useState } from "react";

const COOKIE_NAME = "sr_banner_dismissed";
const COOKIE_MAX_AGE_DAYS = 30;

function getCookie(name: string) {
  if (typeof document === "undefined") return null;
  const match = document.cookie.match(
    new RegExp(`(?:^|; )${name.replace(/[.*+?^${}()|[\\]\\\\]/g, "\\\\$&")}=([^;]*)`),
  );
  return match ? decodeURIComponent(match[1]) : null;
}

function setCookie(name: string, value: string, maxAgeDays: number) {
  const maxAgeSeconds = maxAgeDays * 24 * 60 * 60;
  document.cookie = `${encodeURIComponent(name)}=${encodeURIComponent(value)}; Max-Age=${maxAgeSeconds}; Path=/; SameSite=Lax`;
}

export default function AnnouncementBanner() {
  const [isVisible, setIsVisible] = useState(() => {
    if (typeof document === "undefined") return false;
    return getCookie(COOKIE_NAME) !== "1";
  });

  if (!isVisible) return null;

  return (
    <div className="border-b border-black/10 bg-[color:var(--surface-2)] dark:border-white/10">
      <div className="mx-auto flex max-w-6xl items-center justify-between gap-4 px-6 py-2 text-sm">
        <p className="text-zinc-700 dark:text-zinc-300">
          We’re building a Slack-style app step-by-step — pricing and marketing
          are in progress.
        </p>
        <button
          type="button"
          className="inline-flex h-8 items-center justify-center rounded-full border border-black/10 bg-white px-3 text-xs font-semibold text-foreground hover:bg-black/[.04] dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]"
          onClick={() => {
            setCookie(COOKIE_NAME, "1", COOKIE_MAX_AGE_DAYS);
            setIsVisible(false);
          }}
        >
          Dismiss
        </button>
      </div>
    </div>
  );
}
