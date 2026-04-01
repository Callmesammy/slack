"use client";

import { useMutation } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { useState } from "react";

import { api } from "@/lib/api";

type CreateWorkspaceResponse = {
  workspaceId: string;
  slug: string;
};

export default function CreateWorkspaceForm() {
  const router = useRouter();

  const [workspaceName, setWorkspaceName] = useState("");
  const [description, setDescription] = useState("");

  const mutation = useMutation({
    mutationFn: async () => {
      const name = workspaceName.trim();
      if (!name) throw new Error("Workspace name is required.");

      return api.post<CreateWorkspaceResponse>("/workspaces", {
        name,
        description: description.trim() ? description.trim() : null,
      });
    },
    onSuccess: (data) => {
      router.push(`/app/${data.slug}`);
    },
  });

  return (
    <form
      className="grid gap-3"
      onSubmit={(e) => {
        e.preventDefault();
        mutation.mutate();
      }}
    >
      <label className="grid gap-2">
        <span className="text-xs font-medium text-foreground">Workspace name</span>
        <input
          value={workspaceName}
          onChange={(e) => setWorkspaceName(e.target.value)}
          type="text"
          placeholder="Acme Inc."
          className="h-11 rounded-2xl border border-black/10 bg-white px-4 text-sm text-foreground outline-none placeholder:text-zinc-400 focus:border-black/20 dark:border-white/10 dark:bg-black dark:focus:border-white/20"
        />
      </label>

      <label className="grid gap-2">
        <span className="text-xs font-medium text-foreground">
          Description (optional)
        </span>
        <input
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          type="text"
          placeholder="What’s this workspace for?"
          className="h-11 rounded-2xl border border-black/10 bg-white px-4 text-sm text-foreground outline-none placeholder:text-zinc-400 focus:border-black/20 dark:border-white/10 dark:bg-black dark:focus:border-white/20"
        />
      </label>

      <button
        type="submit"
        disabled={mutation.isPending}
        className="mt-1 inline-flex h-11 items-center justify-center rounded-full bg-[color:var(--brand)] px-4 text-sm font-semibold text-white hover:opacity-90 disabled:opacity-50"
      >
        {mutation.isPending ? "Creating…" : "Continue"}
      </button>

      {mutation.isError ? (
        <p className="text-xs text-zinc-600 dark:text-zinc-400">
          {mutation.error instanceof Error ? mutation.error.message : "Request failed"}
        </p>
      ) : null}
    </form>
  );
}

