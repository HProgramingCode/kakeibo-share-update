import Link from "next/link";
import { notFound } from "next/navigation";
import { CURRENT_USER_ID, getGroup, getMember, getOpenSettlement } from "@/lib/mock-data";
import { formatYen } from "@/lib/format";
import { Icon } from "@/components/icon";
import { AppHeader } from "@/components/app-header";
import { BalanceCard } from "@/components/balance-card";
import { MemberAvatar } from "@/components/member-avatar";
import { Button } from "@/components/button";

export default async function SettlementPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const group = getGroup(id);
  if (!group) notFound();

  const open = getOpenSettlement(id);
  const count = open.transfers.length;

  return (
    <div className="flex min-h-dvh flex-col">
      <AppHeader title="精算" />

      {/* 未精算 / 精算済み トグル */}
      <div className="shrink-0 px-4 pb-3">
        <div className="flex rounded-xl bg-[#F1F0EC] p-[3px]">
          <span className="flex-1 rounded-[9px] bg-surface py-2.5 text-center text-[13px] font-bold shadow-[0_1px_2px_rgba(0,0,0,.06)]">
            未精算
          </span>
          <Link
            href={`/groups/${id}/settlement/history`}
            className="flex-1 rounded-[9px] py-2.5 text-center text-[13px] font-bold text-faint"
          >
            精算済み
          </Link>
        </div>
      </div>

      <div className="flex flex-1 flex-col gap-3 px-4">
        <BalanceCard
          label="あなたの収支"
          amount={open.net}
          caption={`${count}人から受け取り予定`}
        />

        <div className="flex items-center gap-1.5 rounded-[11px] border border-primary-line bg-primary-tint px-3 py-2.5">
          <Icon name="git-merge" size={16} className="text-primary-deep" />
          <span className="text-xs font-bold text-primary-deep">
            最小回数で精算 · 送金は{count}回でOK
          </span>
        </div>

        <div className="px-0.5 text-[13px] font-extrabold">精算の内訳</div>

        {open.transfers.map((t, i) => {
          const from = getMember(t.fromMemberId);
          const toIsMe = t.toMemberId === CURRENT_USER_ID;
          const to = getMember(t.toMemberId);
          return (
            <div
              key={i}
              className="flex items-center gap-2.5 rounded-[16px] border border-border bg-surface p-3.5"
            >
              <MemberAvatar member={from} size={32} />
              <span className="text-[13px] font-bold">{from.name}</span>
              <Icon name="arrow-right" size={16} className="text-[#C4C5BE]" />
              {toIsMe ? (
                <span
                  className="flex h-8 w-8 items-center justify-center rounded-full text-xs font-bold text-white"
                  style={{ background: "var(--color-primary)" }}
                >
                  あ
                </span>
              ) : (
                <MemberAvatar member={to} size={32} />
              )}
              <span className="text-[13px] font-bold">{toIsMe ? "あなた" : to.name}</span>
              <span className="tnum ml-auto text-[15px] font-extrabold text-primary-deep">
                {formatYen(t.amount)}
              </span>
            </div>
          );
        })}
      </div>

      <div className="shrink-0 px-4 pb-6 pt-2.5">
        <div className="mb-2.5 flex items-center justify-center gap-1.5 text-faint">
          <Icon name="info" size={13} />
          <span className="text-[11px] font-semibold">確定できるのはグループ作成者のみです</span>
        </div>
        <Button>精算を確定する</Button>
      </div>
    </div>
  );
}
