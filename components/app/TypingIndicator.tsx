"use client";

export default function TypingIndicator({
  isTyping,
}: Readonly<{
  isTyping: boolean;
}>) {
  if (!isTyping) return null;

  return (
    <div className="px-6 py-2 text-xs text-zinc-600 dark:text-zinc-400">
      Someone is typing…
    </div>
  );
}

