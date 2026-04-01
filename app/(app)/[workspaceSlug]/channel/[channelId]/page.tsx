import ChannelMessageComposer from "@/components/app/ChannelMessageComposer";
import ChannelMessageList from "@/components/app/ChannelMessageList";
import ChannelRealtimeBinder from "@/components/app/ChannelRealtimeBinder";
import ChannelMemberList from "@/components/app/ChannelMemberList";
import ChannelHeader from "@/components/app/ChannelHeader";

export default async function ChannelPage({
  params,
}: Readonly<{
  params: Promise<{ workspaceSlug: string; channelId: string }>;
}>) {
  const { workspaceSlug, channelId } = await params;

  return (
    <div className="flex min-h-full flex-1 flex-col">
      <ChannelHeader workspaceSlug={workspaceSlug} channelId={channelId} />

      <div className="flex min-h-0 flex-1">
        <div className="flex min-w-0 flex-1 flex-col">
          <ChannelMessageList workspaceSlug={workspaceSlug} channelId={channelId} />
          <ChannelRealtimeBinder channelId={channelId} />
          <ChannelMessageComposer workspaceSlug={workspaceSlug} channelId={channelId} />
        </div>
        <aside className="hidden w-72 shrink-0 border-l border-black/10 bg-[color:var(--surface)] dark:border-white/10 dark:bg-black lg:block">
          <ChannelMemberList workspaceSlug={workspaceSlug} channelId={channelId} />
        </aside>
      </div>
    </div>
  );
}
