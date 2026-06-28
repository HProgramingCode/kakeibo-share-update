import { notFound } from "next/navigation";
import { getExpense, getGroupMembers, getMember } from "@/lib/mock-data";
import { formatYen } from "@/lib/format";
import { Icon } from "@/components/icon";
import { AppHeader } from "@/components/app-header";
import { ExpenseIcon } from "@/components/expense-icon";
import { MemberAvatar } from "@/components/member-avatar";
import { AvatarStack } from "@/components/avatar-stack";
import { Button } from "@/components/button";

function dateTimeLabel(iso: string, time?: string): string {
  const d = new Date(`${iso}T00:00:00`);
  const week = ["日", "月", "火", "水", "木", "金", "土"][d.getDay()];
  const base = `${d.getFullYear()}/${d.getMonth() + 1}/${d.getDate()}（${week}）`;
  return time ? `${base} ${time}` : base;
}

export default async function ExpenseDetailPage({
  params,
}: {
  params: Promise<{ id: string; eid: string }>;
}) {
  const { id, eid } = await params;
  const expense = getExpense(eid);
  if (!expense) notFound();

  const payer = getMember(expense.paidByMemberId);
  const members = getGroupMembers(id);
  const isRatio = expense.splitType === "Ratio";
  const equalPer = Math.floor(expense.amount / expense.shares.length);

  return (
    <div className="flex min-h-dvh flex-col">
      <AppHeader
        title="支出詳細"
        right={
          <button
            type="button"
            aria-label="その他の操作"
            className="flex h-9 w-9 items-center justify-center rounded-xl border border-border bg-surface text-sub"
          >
            <Icon name="more-horizontal" size={19} />
          </button>
        }
      />

      <div className="flex flex-1 flex-col gap-2.5 px-4 pt-2">
        {/* ヒーロー */}
        <div className="rounded-[18px] border border-border bg-surface p-[22px] text-center">
          <div className="mx-auto mb-3 w-fit">
            <ExpenseIcon expense={expense} size={58} />
          </div>
          <div className="text-base font-bold">{expense.title}</div>
          <div className="tnum mt-1 text-[34px] font-extrabold tracking-tight">
            {formatYen(expense.amount)}
          </div>
          <div className="mt-1 text-xs font-semibold text-faint">
            {dateTimeLabel(expense.date, expense.time)}
          </div>
        </div>

        {/* 基本情報 */}
        <div className="overflow-hidden rounded-[16px] border border-border bg-surface">
          <InfoRow label="支払った人">
            <span className="flex items-center gap-1.5 text-[13.5px] font-bold">
              <MemberAvatar member={payer} size={22} />
              {payer.name}
            </span>
          </InfoRow>
          <InfoRow label="カテゴリ" last={!expense.note}>
            <span className="text-[13.5px] font-bold">{expense.category}</span>
          </InfoRow>
          {expense.note && (
            <InfoRow label="メモ" last>
              <span className="text-[13.5px] font-bold">{expense.note}</span>
            </InfoRow>
          )}
        </div>

        {/* 分担見出し */}
        <div className="flex items-center justify-between px-0.5">
          <span className="flex items-center gap-1.5 text-[13.5px] font-extrabold">
            {isRatio && <Icon name="sliders-horizontal" size={15} className="text-coral-ink" />}
            {isRatio ? "分担 · 金額を個別指定" : `分担 · 均等割り ${expense.shares.length}人`}
          </span>
          <span className="tnum text-xs font-bold text-faint">
            {isRatio ? `合計 ${formatYen(expense.amount)}` : `1人 ${formatYen(equalPer)}`}
          </span>
        </div>

        {isRatio ? (
          <>
            <div className="overflow-hidden rounded-[16px] border border-border bg-surface">
              {expense.shares.map((s, i) => {
                const m = getMember(s.memberId);
                return (
                  <div
                    key={s.memberId}
                    className={`flex items-center gap-3 px-3.5 py-[11px] ${
                      i < expense.shares.length - 1 ? "border-b border-line" : ""
                    }`}
                  >
                    <MemberAvatar member={m} size={30} />
                    <div className="min-w-0 flex-1">
                      <div className="text-[13px] font-bold">{m.name}</div>
                      <div className="mt-[5px] h-[5px] overflow-hidden rounded-full bg-[#EDECE7]">
                        <div
                          className="h-full rounded-full bg-primary"
                          style={{ width: `${s.ratio ?? 0}%` }}
                        />
                      </div>
                    </div>
                    <div className="text-right">
                      <div className="tnum text-sm font-extrabold">{formatYen(s.shareAmount)}</div>
                      <div className="text-[10px] font-bold text-faint">{s.ratio}%</div>
                    </div>
                  </div>
                );
              })}
            </div>
            <div className="flex items-center gap-1.5 rounded-[11px] border border-primary-line bg-primary-tint px-3 py-2.5">
              <Icon name="circle-check" size={15} className="text-primary-deep" />
              <span className="text-[11.5px] font-bold text-primary-deep">
                合計が金額とぴったり一致しています
              </span>
            </div>
          </>
        ) : (
          <div className="flex items-center justify-between gap-2 rounded-[16px] border border-border bg-surface px-3.5 py-[11px]">
            <AvatarStack members={members} max={4} size={30} />
            <span className="text-[12.5px] font-bold text-muted">
              全員が {formatYen(equalPer)} ずつ負担
            </span>
          </div>
        )}

        {/* レシート（OCR由来） */}
        {expense.fromReceipt && (
          <div className="flex items-stretch gap-3">
            <div
              className="flex h-24 w-[78px] shrink-0 items-center justify-center rounded-xl border border-[#E4E3DE]"
              style={{
                background:
                  "repeating-linear-gradient(135deg,#F4F3EF,#F4F3EF 6px,#ECEBE6 6px,#ECEBE6 12px)",
              }}
            >
              <span className="rotate-[-90deg] whitespace-nowrap font-mono text-[9px] text-faint">
                RECEIPT
              </span>
            </div>
            <div className="flex flex-1 flex-col justify-center">
              <div className="mb-1 text-[12.5px] font-bold">レシート画像</div>
              <div className="text-[11px] font-semibold leading-relaxed text-faint">
                タップで拡大表示。OCRで自動入力された支出です。
              </div>
            </div>
          </div>
        )}
      </div>

      {/* 操作 */}
      <div className="flex shrink-0 gap-2.5 px-4 pb-6 pt-3">
        <Button href={`/groups/${id}/expenses/new`} variant="secondary" className="h-12 text-[14.5px]">
          <Icon name="pencil" size={17} />
          編集
        </Button>
        <button
          type="button"
          aria-label="削除"
          className="flex h-12 w-12 shrink-0 items-center justify-center rounded-[13px] border border-coral-line bg-coral-tint text-coral-ink"
        >
          <Icon name="trash-2" size={18} />
        </button>
      </div>
    </div>
  );
}

function InfoRow({
  label,
  children,
  last,
}: {
  label: string;
  children: React.ReactNode;
  last?: boolean;
}) {
  return (
    <div
      className={`flex items-center justify-between px-[15px] py-3 ${
        last ? "" : "border-b border-line"
      }`}
    >
      <span className="text-[13px] font-bold text-muted">{label}</span>
      {children}
    </div>
  );
}
