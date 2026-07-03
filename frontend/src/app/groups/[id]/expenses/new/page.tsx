import Link from "next/link";
import { notFound } from "next/navigation";
import { getGroup, getGroupMembers, getCurrentUser } from "@/lib/mock-data";
import { Icon } from "@/components/icon";
import { MemberAvatar } from "@/components/member-avatar";
import { Button } from "@/components/button";

export default async function ExpenseCreatePage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const group = getGroup(id);
  if (!group) notFound();

  const members = getGroupMembers(id);
  const me = getCurrentUser();
  const perPerson = Math.floor(2980 / members.length);

  return (
    <div className="flex min-h-dvh flex-col">
      <header className="flex shrink-0 items-center justify-between px-[18px] pb-2.5 pt-3">
        <Link href={`/groups/${id}`} className="text-sm font-bold text-muted">
          キャンセル
        </Link>
        <h1 className="text-base font-extrabold">支出を追加</h1>
        <button
          type="button"
          title="準備中"
          aria-label="レシートを撮影"
          className="flex h-9 w-9 items-center justify-center rounded-xl bg-primary-tint text-primary"
        >
          <Icon name="camera" size={19} />
        </button>
      </header>

      {/* 金額 */}
      <div className="shrink-0 border-b border-border bg-surface px-5 py-5 text-center">
        <div className="mb-1 text-xs font-bold text-faint">金額</div>
        <div className="tnum text-[46px] font-extrabold tracking-tight">
          ¥2,980<span className="font-normal text-primary">|</span>
        </div>
      </div>

      <div className="flex flex-1 flex-col gap-2.5 p-4">
        {/* カテゴリ */}
        <Field label="カテゴリ">
          <span className="flex items-center gap-2 text-sm font-bold">
            <span className="flex h-6 w-6 items-center justify-center rounded-[7px] bg-primary-tint text-primary">
              <Icon name="shopping-cart" size={14} />
            </span>
            食費
            <Icon name="chevron-right" size={17} className="text-[#C4C5BE]" />
          </span>
        </Field>

        {/* 支払った人 */}
        <Field label="支払った人">
          <span className="flex items-center gap-2 text-sm font-bold">
            <MemberAvatar member={me} size={24} />
            {me.name}
            <Icon name="chevron-right" size={17} className="text-[#C4C5BE]" />
          </span>
        </Field>

        {/* 分け方（強調） */}
        <div className="rounded-[14px] border-[1.5px] border-primary-line bg-surface px-[15px] py-[13px]">
          <div className="flex items-center justify-between">
            <span className="text-[13.5px] font-bold text-muted">分け方</span>
            <span className="flex items-center gap-1.5 text-sm font-bold">
              均等割り · {members.length}人
              <Icon name="chevron-right" size={17} className="text-[#C4C5BE]" />
            </span>
          </div>
          <div className="mt-2.5 flex items-center justify-between border-t border-dashed border-[#DCEAE0] pt-2.5">
            <span className="text-xs font-semibold text-faint">1人あたり</span>
            <span className="tnum text-base font-extrabold text-primary-deep">
              ¥{perPerson.toLocaleString("ja-JP")}
            </span>
          </div>
        </div>

        {/* 日付 + メモ */}
        <div className="flex gap-2.5">
          <div className="flex flex-1 items-center justify-between rounded-[14px] border border-border bg-surface px-[15px] py-[13px]">
            <span className="text-[13.5px] font-bold text-muted">日付</span>
            <span className="tnum text-[13.5px] font-bold">5/13</span>
          </div>
          <div className="flex flex-[1.4] items-center gap-2 rounded-[14px] border border-border bg-surface px-[15px] py-[13px] text-faint">
            <Icon name="sticky-note" size={17} />
            <span className="text-[13.5px] font-semibold">メモを追加</span>
          </div>
        </div>
      </div>

      <div className="shrink-0 border-t border-border bg-bg px-4 pb-6 pt-3">
        <Button href={`/groups/${id}/expenses`}>保存する</Button>
      </div>
    </div>
  );
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div className="flex items-center justify-between rounded-[14px] border border-border bg-surface px-[15px] py-[13px]">
      <span className="text-[13.5px] font-bold text-muted">{label}</span>
      {children}
    </div>
  );
}
