"use client";

import { signIn } from "next-auth/react";

export default function GoogleSignInButton() {
  return (
    <button
      type="button"
      className="inline-flex h-11 items-center justify-center gap-2 rounded-full border border-black/10 bg-white px-4 text-sm font-semibold text-foreground hover:bg-black/[.04] dark:border-white/10 dark:bg-black dark:hover:bg-white/[.06]"
      onClick={() => signIn("google")}
    >
      <span
        aria-hidden="true"
        className="inline-flex h-5 w-5 items-center justify-center rounded bg-black/10 text-xs dark:bg-white/10"
      >
        G
      </span>
      Continue with Google
    </button>
  );
}

