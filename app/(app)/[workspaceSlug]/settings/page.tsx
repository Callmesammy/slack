export default async function WorkspaceSettingsPage({
  params,
}: Readonly<{
  params: Promise<{ workspaceSlug: string }>;
}>) {
  const { workspaceSlug } = await params;

  return (
    <main className="flex flex-1 flex-col px-6 py-10">
      <h1 className="text-xl font-semibold tracking-tight text-foreground">
        Settings
      </h1>
      <p className="mt-2 text-sm text-zinc-600 dark:text-zinc-400">
        Workspace settings placeholder for {workspaceSlug}.
      </p>
    </main>
  );
}

