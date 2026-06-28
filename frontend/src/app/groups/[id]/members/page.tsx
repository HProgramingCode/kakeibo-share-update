import { notFound } from "next/navigation";
import { CURRENT_USER_ID, getGroup, getGroupMembers } from "@/lib/mock-data";
import { Icon } from "@/components/icon";
import { AppHeader } from "@/components/app-header";
import { MemberAvatar } from "@/components/member-avatar";
import { Badge } from "@/components/badges";
import { Button } from "@/components/button";

export default async function MembersPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const group = getGroup(id);
  if (!group) notFound();

  const members = getGroupMembers(id);

  return (
    <div className="flex min-h-dvh flex-col">
      <AppHeader title="メンバー" />

      <div className="shrink-0 px-4 pb-2.5 text-xs font-bold text-faint">
        {group.name} · {members.length}人
      </div>

      <div className="flex flex-1 flex-col gap-2.5 px-4">
        {members.map((m) => {
          const isMe = m.id === CURRENT_USER_ID;
          return (
            <div
              key={m.id}
              className="flex items-center gap-3 rounded-[14px] border border-border bg-surface px-[15px] py-[13px]"
            >
              <MemberAvatar member={m} size={42} solid={isMe} />
              <div className="min-w-0 flex-1">
                <div className="text-[14.5px] font-bold">
                  {m.name}
                  {isMe && <span className="text-[11px] font-semibold text-faint">（あなた）</span>}
                </div>
                <div className="truncate text-[11px] font-semibold text-faint">{m.email}</div>
              </div>
              {m.owner ? (
                <Badge tone="primary">オーナー</Badge>
              ) : m.pending ? (
                <Badge tone="amber">招待中</Badge>
              ) : (
                <Icon name="chevron-right" size={18} className="text-[#C4C5BE]" />
              )}
            </div>
          );
        })}
      </div>

      <div className="shrink-0 px-4 pb-6 pt-3">
        <Button href={`/groups/${id}/invite`} className="text-[15px]">
          <Icon name="user-plus" size={19} />
          メンバーを招待
        </Button>
      </div>
    </div>
  );
}
