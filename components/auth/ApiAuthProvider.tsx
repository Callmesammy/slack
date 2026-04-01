"use client";

import { useEffect } from "react";
import { useSession } from "next-auth/react";

import { setApiAuthTokenProvider } from "@/lib/api";

export default function ApiAuthProvider({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const { data } = useSession();

  useEffect(() => {
    setApiAuthTokenProvider(() => data?.apiJwt ?? null);
    return () => setApiAuthTokenProvider(null);
  }, [data?.apiJwt]);

  return children;
}

