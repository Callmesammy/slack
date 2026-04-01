import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";

function getApiOrigin(): string {
  const raw = process.env.NEXT_PUBLIC_API_URL ?? "";
  if (!raw) return "";

  try {
    const url = new URL(raw);
    if (url.pathname.endsWith("/api/v1") || url.pathname.endsWith("/api/v1/")) {
      url.pathname = "";
    }
    return url.origin;
  } catch {
    return raw.replace(/\/api\/v1\/?$/, "");
  }
}

function getHubUrl(): string {
  const origin = getApiOrigin();
  return `${origin}/api/v1/hubs/chat`;
}

let current: { token: string; connection: HubConnection } | null = null;
let starting: Promise<void> | null = null;

export function getChatHubConnection(token: string): HubConnection {
  if (!token) {
    throw new Error("Missing API JWT for SignalR.");
  }

  if (current && current.token === token) {
    return current.connection;
  }

  current?.connection.stop().catch(() => {});

  const connection = new HubConnectionBuilder()
    .withUrl(getHubUrl(), {
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build();

  current = { token, connection };
  return connection;
}

export async function ensureChatHubStarted(connection: HubConnection) {
  if (connection.state === HubConnectionState.Connected) return;
  if (starting) return starting;

  starting = connection
    .start()
    .finally(() => {
      starting = null;
    });

  return starting;
}

