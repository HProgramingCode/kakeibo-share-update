"use client";

import { useRouter } from "next/navigation";
import { Icon } from "./icon";

/**
 * 詳細画面用の戻る付きヘッダー（戻る / タイトル / 右アクション）
 * 右側に何も無くてもタイトルが中央に来るようプレースホルダを置く。
 */
export function AppHeader({
  title,
  right,
}: {
  title: string;
  right?: React.ReactNode;
}) {
  const router = useRouter();
  return (
    <header className="flex shrink-0 items-center justify-between px-4 pb-2 pt-3">
      <button
        type="button"
        onClick={() => router.back()}
        aria-label="戻る"
        className="flex h-9 w-9 items-center justify-center rounded-xl border border-border bg-surface text-sub"
      >
        <Icon name="chevron-left" size={20} />
      </button>
      <h1 className="text-base font-extrabold">{title}</h1>
      <div className="flex h-9 w-9 items-center justify-center">{right}</div>
    </header>
  );
}
