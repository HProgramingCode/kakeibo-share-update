import { formatSignedYen, formatYen } from "@/lib/format";

/**
 * グラデーションの収支カード（このアプリのシグネチャー）。
 * design.html のグループ詳細 / 精算結果で使用。
 */
export function BalanceCard({
  label,
  amount,
  caption,
  breakdown,
}: {
  label: string;
  amount: number;
  caption: string;
  breakdown?: { receive: number; pay: number };
}) {
  return (
    <div
      className="rounded-[20px] p-5 text-white shadow-[0_14px_28px_-12px_rgba(19,122,76,.55)]"
      style={{ background: "linear-gradient(155deg,#22A368,#147A4C)" }}
    >
      <div className="text-[12.5px] font-semibold opacity-85">{label}</div>
      <div className="tnum text-[38px] font-extrabold leading-none tracking-tight">
        {formatSignedYen(amount)}
      </div>
      <div className="mt-1.5 text-[12.5px] font-semibold opacity-85">{caption}</div>

      {breakdown && (
        <div className="mt-4 flex gap-2.5">
          <div className="flex-1 rounded-xl bg-white/15 px-3 py-2.5">
            <div className="text-[11px] font-semibold opacity-85">受け取る</div>
            <div className="tnum mt-0.5 text-base font-extrabold">{formatYen(breakdown.receive)}</div>
          </div>
          <div className="flex-1 rounded-xl bg-white/15 px-3 py-2.5">
            <div className="text-[11px] font-semibold opacity-85">支払う</div>
            <div className="tnum mt-0.5 text-base font-extrabold">{formatYen(breakdown.pay)}</div>
          </div>
        </div>
      )}
    </div>
  );
}
