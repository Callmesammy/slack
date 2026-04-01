export type ApiError = {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
};

export class ApiClientError extends Error {
  status: number;
  error: ApiError | null;

  constructor(message: string, status: number, error: ApiError | null) {
    super(message);
    this.name = "ApiClientError";
    this.status = status;
    this.error = error;
  }
}

let authTokenProvider: (() => Promise<string | null> | string | null) | null =
  null;

export function setApiAuthTokenProvider(
  provider: (() => Promise<string | null> | string | null) | null,
) {
  authTokenProvider = provider;
}

function getBaseUrl() {
  const baseUrl = process.env.NEXT_PUBLIC_API_URL;
  if (!baseUrl) {
    throw new Error("Missing NEXT_PUBLIC_API_URL env var.");
  }
  return baseUrl.replace(/\/+$/, "");
}

async function parseJsonSafe(res: Response) {
  const text = await res.text();
  if (!text) return null;
  try {
    return JSON.parse(text) as unknown;
  } catch {
    return null;
  }
}

async function request<T>(
  path: string,
  init: RequestInit & { json?: unknown } = {},
): Promise<T> {
  const url = `${getBaseUrl()}${path.startsWith("/") ? path : `/${path}`}`;

  const headers = new Headers(init.headers);
  if (init.json !== undefined) headers.set("content-type", "application/json");

  if (!headers.has("authorization") && authTokenProvider) {
    const maybeToken = await authTokenProvider();
    if (maybeToken) headers.set("authorization", `Bearer ${maybeToken}`);
  }

  const res = await fetch(url, {
    ...init,
    headers,
    body: init.json !== undefined ? JSON.stringify(init.json) : init.body,
    credentials: "include",
  });

  if (!res.ok) {
    const data = (await parseJsonSafe(res)) as ApiError | null;
    throw new ApiClientError(
      data?.title ?? `Request failed (${res.status})`,
      res.status,
      data,
    );
  }

  const data = (await parseJsonSafe(res)) as T | null;
  if (data === null) {
    throw new ApiClientError("Empty response body", res.status, null);
  }
  return data;
}

export const api = {
  get<T>(path: string, init?: RequestInit) {
    return request<T>(path, { ...init, method: "GET" });
  },
  post<T>(path: string, json?: unknown, init?: RequestInit) {
    return request<T>(path, { ...init, method: "POST", json });
  },
  put<T>(path: string, json?: unknown, init?: RequestInit) {
    return request<T>(path, { ...init, method: "PUT", json });
  },
  delete<T>(path: string, init?: RequestInit) {
    return request<T>(path, { ...init, method: "DELETE" });
  },
};
