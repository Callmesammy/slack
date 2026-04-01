import WorkspaceHomeRedirect from "@/components/app/WorkspaceHomeRedirect";

export default async function WorkspaceHomePage({
  params,
}: Readonly<{
  params: Promise<{ workspaceSlug: string }>;
}>) {
  const { workspaceSlug } = await params;

  return <WorkspaceHomeRedirect workspaceSlug={workspaceSlug} />;
}
