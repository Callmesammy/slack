import Link from "next/link";
import WorkspaceSwitcher from "@/components/app/WorkspaceSwitcher";
import ChannelList from "@/components/app/ChannelList";
import WorkspacePresenceBinder from "@/components/app/WorkspacePresenceBinder";

export default async function WorkspaceLayout({
  children,
  params,
}: Readonly<{
  children: React.ReactNode;
  params: Promise<{ workspaceSlug: string }>;
}>) {
  const { workspaceSlug } = await params;

  return (
    <div className="flex min-h-full flex-1">
      <WorkspacePresenceBinder workspaceSlug={workspaceSlug} />
      <aside className="hidden w-72 shrink-0 border-r border-black/10 bg-[color:var(--surface)] p-4 dark:border-white/10 dark:bg-black md:block">
        <div className="text-sm font-semibold tracking-tight text-foreground">
          Workspaces
        </div>
        <div className="mt-3">
          <WorkspaceSwitcher activeSlug={workspaceSlug} />
        </div>
        <div className="mt-4 grid gap-2 text-sm text-zinc-700 dark:text-zinc-300">
          <Link href={`/app/${workspaceSlug}`} className="hover:text-foreground">
            Home
          </Link>
          <Link
            href={`/app/${workspaceSlug}/settings`}
            className="hover:text-foreground"
          >
            Settings
          </Link>
        </div>

        <ChannelList workspaceSlug={workspaceSlug} />
      </aside>
      <div className="flex min-w-0 flex-1 flex-col">{children}</div>
    </div>
  );
}
