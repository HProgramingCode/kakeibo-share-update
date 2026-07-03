import Link from "next/link";
import type { Expense } from "@/lib/types";
import { getMember } from "@/lib/mock-data";
import { formatYen } from "@/lib/format";
import { ExpenseIcon } from "./expense-icon";
import { cn } from "@/lib/utils";

/**
 * 支出のリスト行。
 * - bordered: 独立した枠付きカード（支出履歴）
 * - bordered=false: 枠なし（グループ詳細の「最近の支出」カード内の分割行）
 */
export function ExpenseRow({
  expense,
  subtitle,
  bordered = true,
}: {
  expense: Expense;
  subtitle?: string;
  bordered?: boolean;
}) {
  const payer = getMember(expense.paidByMemberId);
  const sub =
    subtitle ??
    (expense.splitType === "Equal"
      ? `${payer.name} · 均等割り ${expense.shares.length}人`
      : `${payer.name} · 分担`);

  return (
    <Link
      href={`/groups/${expense.groupId}/expenses/${expense.id}`}
      className={cn(
        "flex items-center gap-3 px-3.5 py-3",
        bordered && "rounded-[14px] border border-border bg-surface",
      )}
    >
      <ExpenseIcon expense={expense} size={36} />
      <div className="min-w-0 flex-1">
        <div className="truncate text-sm font-bold">{expense.title}</div>
        <div className="truncate text-[11px] font-semibold text-faint">{sub}</div>
      </div>
      <div className="tnum text-[15px] font-extrabold">{formatYen(expense.amount)}</div>
    </Link>
  );
}
