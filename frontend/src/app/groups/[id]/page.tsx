import Link from "next/link";
import { notFound } from "next/navigation";
import { getGroup, getGroupExpenses } from "@/lib/mock-data";
import { Icon } from "@/components/icon";
import { AppHeader } from "@/components/app-header";
import { BalanceCard } from "@/components/balance-card";
import { ExpenseRow } from "@/components/expense-row";
import { BottomNav } from "@/components/bottom-nav";

const QUICK_ACTIONS = [
  { icon: "plus", label: "支出追加", href: (id: string) => `/groups/${id}/expenses/new` },
  { icon: "camera", label: "レシート", href: null }, // OCRはMVP対象外（ダミー）
  { icon: "arrow-left-right", label: "精算", href: (id: string) => `/groups/${id}/settlement` },
  { icon: "user-plus", label: "メンバー", href: (id: string) => `/groups/${id}/members` },
] as const;

export default async function GroupDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const group = getGroup(id);
  if (!group || group.net === null) notFound();

  const net = group.net;
  const expenses = getGroupExpenses(id).slice(0, 3);

  return (
    <div className="flex min-h-dvh flex-col">
      <AppHeader
        title={group.name}
        right={
          <Link
            href={`/groups/${id}/members`}
            aria-label="メンバー"
            className="flex h-9 w-9 items-center justify-center rounded-xl border border-border bg-surface text-sub"
          >
            <Icon name="users" size={19} />
          </Link>
        }
      />

      <div className="flex flex-1 flex-col gap-3 px-4 pb-5">
        <BalanceCard
          label="あなたの収支 · 今月"
          amount={net}
          caption={net >= 0 ? "受け取り予定" : "支払い予定"}
          breakdown={{ receive: net > 0 ? net : 0, pay: net < 0 ? -net : 0 }}
        />

        {/* クイック操作 */}
        <div className="grid grid-cols-4 gap-2">
          {QUICK_ACTIONS.map((a) => {
            const inner = (
              <>
                <span className="text-primary">
                  <Icon name={a.icon} size={21} />
                </span>
                <span className="text-[10.5px] font-bold text-sub">{a.label}</span>
              </>
            );
            const cls =
              "flex flex-col items-center gap-1.5 rounded-[14px] border border-border bg-surface px-1 py-[11px]";
            return a.href ? (
              <Link key={a.label} href={a.href(id)} className={cls}>
                {inner}
              </Link>
            ) : (
              <button key={a.label} type="button" title="準備中" className={cls}>
                {inner}
              </button>
            );
          })}
        </div>

        {/* 最近の支出 */}
        <div className="mt-0.5 flex items-center justify-between">
          <span className="text-sm font-extrabold">最近の支出</span>
          <Link href={`/groups/${id}/expenses`} className="text-[12.5px] font-bold text-primary">
            すべて見る
          </Link>
        </div>

        <div className="overflow-hidden rounded-[16px] border border-border bg-surface">
          {expenses.map((e, i) => (
            <div key={e.id} className={i < expenses.length - 1 ? "border-b border-line" : ""}>
              <ExpenseRow expense={e} bordered={false} />
            </div>
          ))}
        </div>
      </div>

      <BottomNav groupId={id} />
    </div>
  );
}
