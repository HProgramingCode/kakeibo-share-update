import type { Expense, ExpenseTone } from "@/lib/types";
import { categoryIcon } from "@/lib/format";
import { Icon } from "./icon";
import { cn } from "@/lib/utils";

const TONE: Record<ExpenseTone, { bg: string; fg: string }> = {
  green: { bg: "#E7F4EC", fg: "#1A9B5E" },
  blue: { bg: "#DCE4F0", fg: "#3F5680" },
  coral: { bg: "#FCEDEA", fg: "#C9483A" },
  gold: { bg: "#FCF3E2", fg: "#C28A2A" },
  violet: { bg: "#EDEAF3", fg: "#6A5A9A" },
  neutral: { bg: "#F4F3EF", fg: "#54564F" },
};

/** カテゴリ既定 -> tone（icon/tone 未指定時のフォールバック） */
function defaultVisual(e: Expense): { icon: string; tone: ExpenseTone } {
  switch (e.category) {
    case "食費":
      return { icon: "shopping-cart", tone: "green" };
    case "日用品":
      return { icon: "package", tone: "gold" };
    case "光熱費":
      return { icon: "zap", tone: "gold" };
    case "交通":
      return { icon: "train-front", tone: "blue" };
    case "娯楽":
      return { icon: "utensils", tone: "coral" };
    default:
      return { icon: "tag", tone: "neutral" };
  }
}

export function ExpenseIcon({
  expense,
  size = 36,
  className,
}: {
  expense: Expense;
  size?: number;
  className?: string;
}) {
  const base = defaultVisual(expense);
  const icon = expense.icon ?? base.icon;
  const tone = TONE[expense.tone ?? base.tone];
  return (
    <span
      className={cn("inline-flex shrink-0 items-center justify-center", className)}
      style={{
        background: tone.bg,
        color: tone.fg,
        width: size,
        height: size,
        borderRadius: Math.round(size * 0.3),
      }}
    >
      <Icon name={icon} size={Math.round(size * 0.5)} />
    </span>
  );
}

export { categoryIcon };
