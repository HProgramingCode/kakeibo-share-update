import Link from "next/link";
import { getCurrentUser } from "@/lib/mock-data";
import { Icon } from "@/components/icon";
import { MemberAvatar } from "@/components/member-avatar";
import { BottomNav } from "@/components/bottom-nav";

export default function SettingsPage() {
  const me = getCurrentUser();

  return (
    <div className="flex min-h-dvh flex-col">
      <header className="shrink-0 px-5 pb-3 pt-4">
        <h1 className="text-[25px] font-extrabold tracking-tight">設定</h1>
      </header>

      <div className="flex flex-1 flex-col gap-3.5 px-4">
        {/* プロフィール */}
        <div className="flex items-center gap-3 rounded-[16px] border border-border bg-surface p-[15px]">
          <MemberAvatar member={me} size={48} solid />
          <div className="flex-1">
            <div className="text-[15.5px] font-bold">{me.name}</div>
            <div className="text-[11.5px] font-semibold text-faint">{me.email}</div>
          </div>
          <Icon name="chevron-right" size={18} className="text-[#C4C5BE]" />
        </div>

        {/* 設定グループ1 */}
        <div className="overflow-hidden rounded-[16px] border border-border bg-surface">
          <SettingRow icon="bell" label="通知設定" />
          <SettingRow icon="palette" label="表示・テーマ" />
          <SettingRow icon="coins" label="通貨・言語" value="JPY · 日本語" last />
        </div>

        {/* 設定グループ2 */}
        <div className="overflow-hidden rounded-[16px] border border-border bg-surface">
          <SettingRow icon="download" label="アプリをインストール" />
          <SettingRow icon="circle-help" label="ヘルプ・お問い合わせ" />
          <Link
            href="/login"
            className="flex items-center gap-3 px-[15px] py-[13px]"
          >
            <span className="flex h-8 w-8 items-center justify-center rounded-[9px] bg-coral-tint text-coral-ink">
              <Icon name="log-out" size={17} />
            </span>
            <span className="flex-1 text-sm font-bold text-coral-ink">ログアウト</span>
          </Link>
        </div>

        <div className="text-center text-[11px] font-semibold text-hint">かぞく精算帳 v1.0.0</div>
      </div>

      <BottomNav />
    </div>
  );
}

function SettingRow({
  icon,
  label,
  value,
  last,
}: {
  icon: string;
  label: string;
  value?: string;
  last?: boolean;
}) {
  return (
    <button
      type="button"
      className={`flex w-full items-center gap-3 px-[15px] py-[13px] text-left ${
        last ? "" : "border-b border-line"
      }`}
    >
      <span className="flex h-8 w-8 items-center justify-center rounded-[9px] bg-line text-sub">
        <Icon name={icon} size={17} />
      </span>
      <span className="flex-1 text-sm font-bold">{label}</span>
      {value && <span className="mr-1.5 text-xs font-semibold text-faint">{value}</span>}
      <Icon name="chevron-right" size={18} className="text-[#C4C5BE]" />
    </button>
  );
}
