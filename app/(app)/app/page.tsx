import MeCard from "@/components/app/MeCard";
import ApiPlayground from "@/components/app/ApiPlayground";

export default function AppIndexPage() {
  return (
    <main className="flex flex-1 items-center justify-center px-6 py-16">
      <div className="w-full max-w-xl">
        <h1 className="text-2xl font-semibold tracking-tight text-foreground">
          App
        </h1>
        <p className="mt-2 text-sm text-zinc-600 dark:text-zinc-400">
          Authenticated app shell goes here.
        </p>

        <div className="mt-6">
          <MeCard />
        </div>

        <div className="mt-6">
          <ApiPlayground />
        </div>
      </div>
    </main>
  );
}
