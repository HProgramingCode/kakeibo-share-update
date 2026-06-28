import { formatSignedYen, netTone } from "@/lib/format";
import { cn } from "@/lib/utils";

/** 符号つき金額の意味色テキスト（受取=緑 / 支払=コーラル） */
export function NetText({
  value,
  className,
}: {
  value: number;
  className?: string;
}) {
  const tone = netTone(value);
  return (
    <span
      className={cn(
        "tnum font-extrabold",
        tone === "positive" && "text-primary-deep",
        tone === "negative" && "text-coral-ink",
        tone === "neutral" && "text-faint",
        className,
      )}
    >
      {formatSignedYen(value)}
    </span>
  );
}

/** 収支のピル（グループカード等） */
export function NetBadge({ value }: { value: number | null }) {
  const tone = netTone(value);
  const label = value === null ? "±0" : formatSignedYen(value);
  return (
    <span
      className={cn(
        "tnum rounded-[10px] px-3 py-1.5 text-sm font-extrabold",
        tone === "positive" && "bg-primary-tint text-primary-deep",
        tone === "negative" && "bg-coral-tint text-coral-ink",
        tone === "neutral" && "bg-bg text-faint",
      )}
    >
      {label}
    </span>
  );
}

type BadgeTone = "neutral" | "primary" | "amber";

/** 汎用ステータスバッジ（未精算 / 精算済み / 招待中 / オーナー / 完了） */
export function Badge({
  children,
  tone = "neutral",
  className,
}: {
  children: React.ReactNode;
  tone?: BadgeTone;
  className?: string;
}) {
  return (
    <span
      className={cn(
        "rounded-lg px-2.5 py-1 text-xs font-bold",
        tone === "neutral" && "bg-bg text-faint",
        tone === "primary" && "bg-primary-tint text-primary-deep",
        tone === "amber" && "bg-amber-tint text-amber-ink",
        className,
      )}
    >
      {children}
    </span>
  );
}
