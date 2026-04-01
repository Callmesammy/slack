import Link from "next/link";
import CreateWorkspaceForm from "@/components/auth/CreateWorkspaceForm";

export default function SignUpPage() {
  return (
    <main className="flex flex-1 items-center justify-center px-6 py-16">
      <div className="w-full max-w-md">
        <div className="rounded-3xl border border-black/10 bg-white p-6 shadow-sm dark:border-white/10 dark:bg-black">
          <div className="flex flex-col gap-2">
            <h1 className="text-2xl font-semibold tracking-tight text-foreground">
              Create your workspace
            </h1>
            <p className="text-sm text-zinc-600 dark:text-zinc-400">
              Start with a name. You can invite teammates after.
            </p>
          </div>

          <div className="mt-6">
            <CreateWorkspaceForm />
          </div>

          <p className="mt-6 text-xs text-zinc-600 dark:text-zinc-400">
            Already have an account?{" "}
            <Link
              href="/sign-in"
              className="font-medium text-foreground underline underline-offset-4"
            >
              Sign in
            </Link>
            .
          </p>
        </div>
      </div>
    </main>
  );
}
