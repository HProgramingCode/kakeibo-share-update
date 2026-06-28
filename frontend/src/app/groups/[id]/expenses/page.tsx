import { notFound } from "next/navigation";
import { getGroup, getGroupExpenses } from "@/lib/mock-data";
import { formatDateLabel, formatYen } from "@/lib/format";
import { Icon } from "@/components/icon";
import { ExpenseRow } from "@/components/expense-row";
import { BottomNav } from "@/components/bottom-nav";

export default async function ExpenseHistoryPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const group = getGroup(id);
  if (!group) notFound();

  const expenses = getGroupExpenses(id);
  const total = expenses.reduce((sum, e) => sum + e.amount, 0);

  // 日付ごとにグループ化（登録順を維持）
  const byDate = new Map<string, typeof expenses>();
  for (const e of expenses) {
    const list = byDate.get(e.date) ?? [];
    list.push(e);
    byDate.set(e.date, list);
  }

  return (
    <div className="flex min-h-dvh flex-col">
      <header className="flex shrink-0 items-center justify-between px-5 pb-2.5 pt-4">
        <h1 className="text-[25px] font-extrabold tracking-tight">支出履歴</h1>
        <button
          type="button"
          aria-label="支出を検索"
          className="flex h-[38px] w-[38px] items-center justify-center rounded-xl border border-border bg-surface text-sub"
        >
          <Icon name="search" size={19} />
        </button>
      </header>

      {/* フィルタ + 合計 */}
      <div className="flex shrink-0 items-center gap-2 px-4 pb-3">
        <span className="flex items-center gap-1.5 rounded-[10px] border border-border bg-surface px-3 py-[7px] text-[13px] font-bold">
          今月
          <Icon name="chevron-down" size={15} className="text-faint" />
        </span>
        <span className="rounded-[10px] border border-border bg-surface px-3 py-[7px] text-[13px] font-bold text-muted">
          カテゴリ
        </span>
        <div className="ml-auto text-right">
          <div className="text-[10.5px] font-bold text-faint">今月の支出</div>
          <div className="tnum text-base font-extrabold">{formatYen(total)}</div>
        </div>
      </div>

      <div className="flex flex-1 flex-col gap-2 px-4 pb-5">
        {[...byDate.entries()].map(([date, list]) => (
          <div key={date} className="flex flex-col gap-2">
            <div className="px-0.5 pt-1 text-xs font-bold text-faint">{formatDateLabel(date)}</div>
            {list.map((e) => (
              <ExpenseRow key={e.id} expense={e} />
            ))}
          </div>
        ))}
      </div>

      <BottomNav groupId={id} />
    </div>
  );
}
