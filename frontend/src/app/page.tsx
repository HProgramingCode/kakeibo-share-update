import { groups, getCurrentUser } from "@/lib/mock-data";
import { Icon } from "@/components/icon";
import { MemberAvatar } from "@/components/member-avatar";
import { GroupCard } from "@/components/group-card";
import { BottomNav } from "@/components/bottom-nav";

export default function HomePage() {
  const me = getCurrentUser();

  return (
    <div className="flex min-h-dvh flex-col">
      <header className="flex shrink-0 items-center justify-between px-5 pb-3.5 pt-4">
        <h1 className="text-[25px] font-extrabold tracking-tight">グループ</h1>
        <div className="flex items-center gap-2">
          <button
            type="button"
            aria-label="グループを検索"
            className="flex h-[38px] w-[38px] items-center justify-center rounded-xl border border-border bg-surface text-sub"
          >
            <Icon name="search" size={19} />
          </button>
          <MemberAvatar member={me} size={38} solid />
        </div>
      </header>

      <div className="flex flex-1 flex-col gap-3 px-4 pb-5">
        {groups.map((g) => (
          <GroupCard key={g.id} group={g} />
        ))}

        <button
          type="button"
          className="flex h-[50px] items-center justify-center gap-2 rounded-2xl border-[1.5px] border-dashed border-[#CFCEC8] text-[14.5px] font-bold text-muted"
        >
          <Icon name="plus" size={19} />
          新しいグループ
        </button>
      </div>

      <BottomNav />
    </div>
  );
}
