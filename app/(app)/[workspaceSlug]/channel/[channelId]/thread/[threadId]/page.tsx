export default async function ThreadPage({
  params,
}: Readonly<{
  params: Promise<{
    workspaceSlug: string;
    channelId: string;
    threadId: string;
  }>;
}>) {
  const { channelId, threadId } = await params;

  return (
    <main className="flex flex-1 flex-col px-6 py-10">
      <h1 className="text-xl font-semibold tracking-tight text-foreground">
        Thread {threadId}
      </h1>
      <p className="mt-2 text-sm text-zinc-600 dark:text-zinc-400">
        Thread placeholder under channel {channelId}.
      </p>
    </main>
  );
}

