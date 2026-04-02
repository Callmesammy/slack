import NextAuth from "next-auth";
import GoogleProvider from "next-auth/providers/google";

const googleClientId = process.env.GOOGLE_CLIENT_ID;
const googleClientSecret = process.env.GOOGLE_CLIENT_SECRET;
const backendBaseUrl = process.env.NEXT_PUBLIC_API_URL;

const missingConfigMessage = (() => {
  if (!backendBaseUrl) return "Missing NEXT_PUBLIC_API_URL env var.";
  if (!googleClientId || !googleClientSecret) {
    return "Missing GOOGLE_CLIENT_ID/GOOGLE_CLIENT_SECRET env vars for NextAuth Google provider.";
  }
  return null;
})();

async function exchangeGoogleIdTokenForApiJwt(idToken: string) {
  if (!backendBaseUrl) throw new Error("Missing NEXT_PUBLIC_API_URL env var.");
  const baseUrl = backendBaseUrl.endsWith("/")
    ? backendBaseUrl.slice(0, -1)
    : backendBaseUrl;

  const res = await fetch(`${baseUrl}/auth/google`, {
    method: "POST",
    headers: { "content-type": "application/json" },
    body: JSON.stringify({ idToken }),
  });

  if (!res.ok) {
    const text = await res.text().catch(() => "");
    throw new Error(
      `Backend auth exchange failed (${res.status}): ${text || res.statusText}`,
    );
  }

  const data = (await res.json().catch(() => null)) as
    | { jwt?: string; token?: string }
    | null;

  const jwt = data?.jwt ?? data?.token;
  if (!jwt) throw new Error("Backend auth exchange response missing jwt/token.");

  return jwt;
}

const handler = missingConfigMessage
  ? async () =>
      new Response(missingConfigMessage, {
        status: 500,
        headers: { "content-type": "text/plain; charset=utf-8" },
      })
  : NextAuth({
      providers: [
        GoogleProvider({
          clientId: googleClientId!,
          clientSecret: googleClientSecret!,
          httpOptions: {
            timeout: 15_000,
          },
        }),
      ],
      callbacks: {
        async jwt({ token, account }) {
          if (account?.provider === "google" && typeof account.id_token === "string") {
            token.apiJwt = await exchangeGoogleIdTokenForApiJwt(account.id_token);
          }
          return token;
        },
        async session({ session, token }) {
          if (token.apiJwt && typeof token.apiJwt === "string") {
            session.apiJwt = token.apiJwt;
          }
          return session;
        },
      },
    });

export { handler as GET, handler as POST };
