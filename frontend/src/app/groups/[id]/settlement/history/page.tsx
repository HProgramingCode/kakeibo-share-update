import { notFound } from "next/navigation";
import { getGroup, getGroupSettlements } from "@/lib/mock-data";
import { AppHeader } from "@/components/app-header";
import { Badge, NetText } from "@/components/badges";
import { BottomNav } from "@/components/bottom-nav";

export default async function SettlementHistoryPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const group = getGroup(id);
  if (!group) notFound();

  const history = getGroupSettlements(id);

  return (
    <div className="flex min-h-dvh flex-col">
      <AppHeader title="精算履歴" />

      <div className="flex flex-1 flex-col gap-2.5 px-4 pt-1">
        {history.map((s) => (
          <div key={s.id} className="rounded-[16px] border border-border bg-surface p-4">
            <div className="mb-2 flex items-center justify-between">
              <div>
                <div className="tnum text-sm font-bold">{s.periodLabel}</div>
                <div className="mt-0.5 text-[11px] font-semibold text-faint">{s.settledLabel}</div>
              </div>
              <Badge>完了</Badge>
            </div>
            <div className="flex items-center justify-between border-t border-line pt-2.5">
              <span className="text-xs font-bold text-muted">あなたの収支</span>
              <NetText value={s.net} className="text-base" />
            </div>
          </div>
        ))}
      </div>

      <BottomNav groupId={id} />
    </div>
  );
}
