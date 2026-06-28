import Link from "next/link";
import type { Group } from "@/lib/types";
import { getGroupMembers } from "@/lib/mock-data";
import { Icon } from "./icon";
import { AvatarStack } from "./avatar-stack";
import { NetBadge } from "./badges";

const ICON_TONE: Record<Group["iconTone"], { bg: string; fg: string }> = {
  green: { bg: "#E7F4EC", fg: "#1A9B5E" },
  amber: { bg: "#FCF3E2", fg: "#C28A2A" },
  violet: { bg: "#EDEAF3", fg: "#6A5A9A" },
};

export function GroupCard({ group }: { group: Group }) {
  const members = getGroupMembers(group.id);
  const tone = ICON_TONE[group.iconTone];
  const settled = group.net === null;

  return (
    <Link
      href={`/groups/${group.id}`}
      className="block rounded-[18px] border border-border bg-surface p-4 shadow-[0_1px_2px_rgba(20,30,25,.04)]"
    >
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <span
            className="flex h-[42px] w-[42px] items-center justify-center rounded-[13px]"
            style={{ background: tone.bg, color: tone.fg }}
          >
            <Icon name={group.icon} size={21} />
          </span>
          <div>
            <div className="text-base font-bold">{group.name}</div>
            <div className="mt-px text-[11.5px] font-semibold text-faint">{group.subtitle}</div>
          </div>
        </div>
        {settled ? (
          <NetBadge value={null} />
        ) : (
          <Icon name="chevron-right" size={20} className="text-[#C4C5BE]" />
        )}
      </div>

      {!settled && (
        <div className="mt-[13px] flex items-center justify-between border-t border-line pt-[13px]">
          <AvatarStack members={members} max={3} size={28} />
          <NetBadge value={group.net} />
        </div>
      )}
    </Link>
  );
}
