import { notFound } from "next/navigation";
import { getGroup } from "@/lib/mock-data";
import { Icon } from "@/components/icon";
import { AppHeader } from "@/components/app-header";
import { Button } from "@/components/button";

export default async function InvitePage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const group = getGroup(id);
  if (!group) notFound();

  const inviteUrl = `family-settle.app/invite/${group.inviteCode}`;

  return (
    <div className="flex min-h-dvh flex-col">
      <AppHeader title="メンバーを招待" />

      <div className="flex flex-1 flex-col items-center px-[22px] pt-3.5">
        <div className="mt-1.5 flex h-16 w-16 items-center justify-center rounded-[20px] bg-primary-tint text-primary">
          <Icon name="user-plus" size={31} />
        </div>
        <h2 className="mt-4 text-center text-[17px] font-extrabold">招待リンクを共有しましょう</h2>
        <p className="mt-2 text-center text-[12.5px] leading-relaxed text-muted">
          リンクを送るだけで「{group.name}」に
          <br />
          かんたんに参加できます
        </p>

        {/* QR プレースホルダ */}
        <div className="mt-[22px] flex h-40 w-40 items-center justify-center rounded-[18px] border border-border bg-surface">
          <Icon name="qr-code" size={108} className="text-ink" />
        </div>

        {/* リンク + コピー */}
        <div className="mt-[22px] flex w-full items-center gap-2 rounded-[13px] border border-[#E4E3DE] bg-surface py-1.5 pl-3.5 pr-1.5">
          <span className="flex-1 truncate font-mono text-xs font-semibold text-sub">
            {inviteUrl}
          </span>
          <button
            type="button"
            aria-label="リンクをコピー"
            className="flex h-[38px] w-[38px] items-center justify-center rounded-[10px] bg-primary-tint text-primary"
          >
            <Icon name="copy" size={18} />
          </button>
        </div>
        <div className="mt-2 text-[11px] font-semibold text-faint">リンクの有効期限：7日間</div>
      </div>

      <div className="shrink-0 px-4 pb-6 pt-2">
        <Button className="text-[15px]">
          <Icon name="share-2" size={18} />
          リンクを共有
        </Button>
      </div>
    </div>
  );
}
