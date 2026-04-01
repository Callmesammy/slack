export default async function DirectMessagePage({
  params,
}: Readonly<{
  params: Promise<{ workspaceSlug: string; userId: string }>;
}>) {
  const { userId } = await params;

  return (
    <main className="flex flex-1 flex-col px-6 py-10">
      <h1 className="text-xl font-semibold tracking-tight text-foreground">
        DM with {userId}
      </h1>
      <p className="mt-2 text-sm text-zinc-600 dark:text-zinc-400">
        Direct message placeholder.
      </p>
    </main>
  );
}

