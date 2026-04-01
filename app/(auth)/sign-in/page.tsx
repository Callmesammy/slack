import GoogleSignInButton from "@/components/auth/GoogleSignInButton";

export default function SignInPage() {
  return (
    <main className="flex flex-1 items-center justify-center px-6 py-16">
      <div className="w-full max-w-md">
        <div className="rounded-3xl border border-black/10 bg-white p-6 shadow-sm dark:border-white/10 dark:bg-black">
          <div className="flex flex-col gap-2">
            <h1 className="text-2xl font-semibold tracking-tight text-foreground">
              Sign in
            </h1>
            <p className="text-sm text-zinc-600 dark:text-zinc-400">
              Use Google to continue, or sign in with email.
            </p>
          </div>

          <div className="mt-6 grid gap-3">
            <GoogleSignInButton />

            <div className="flex items-center gap-3 py-1">
              <div className="h-px flex-1 bg-black/10 dark:bg-white/10" />
              <div className="text-xs text-zinc-600 dark:text-zinc-400">or</div>
              <div className="h-px flex-1 bg-black/10 dark:bg-white/10" />
            </div>

            <label className="grid gap-2">
              <span className="text-xs font-medium text-foreground">Email</span>
              <input
                type="email"
                inputMode="email"
                placeholder="you@company.com"
                className="h-11 rounded-2xl border border-black/10 bg-white px-4 text-sm text-foreground outline-none ring-0 placeholder:text-zinc-400 focus:border-black/20 dark:border-white/10 dark:bg-black dark:focus:border-white/20"
              />
            </label>

            <button
              type="button"
              className="inline-flex h-11 items-center justify-center rounded-full bg-foreground px-4 text-sm font-semibold text-background hover:opacity-90"
            >
              Continue
            </button>
          </div>

          <p className="mt-6 text-xs text-zinc-600 dark:text-zinc-400">
            By continuing, you agree to our Terms and acknowledge our Privacy
            Policy.
          </p>
        </div>
      </div>
    </main>
  );
}
