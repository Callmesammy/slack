export default function AppLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <div className="flex min-h-full flex-1">
      <aside className="hidden w-64 shrink-0 border-r border-black/10 bg-white dark:border-white/10 dark:bg-black md:block" />
      <main className="flex min-w-0 flex-1 flex-col">{children}</main>
    </div>
  );
}

