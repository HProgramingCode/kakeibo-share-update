import type { Member } from "@/lib/types";
import { cn } from "@/lib/utils";

interface Props {
  member: Member;
  size?: number;
  /** プライマリ緑の塗り（プロフィール・「あなた」表現用） */
  solid?: boolean;
  /** スタック時に白縁を付ける */
  ring?: boolean;
  className?: string;
}

/** イニシャル + パステル背景のメンバーアバター */
export function MemberAvatar({ member, size = 40, solid, ring, className }: Props) {
  const style = solid
    ? { background: "var(--color-primary)", color: "#fff" }
    : { background: member.avatar.bg, color: member.avatar.fg };

  return (
    <span
      className={cn(
        "inline-flex shrink-0 items-center justify-center rounded-full font-bold",
        ring && "ring-2 ring-white",
        className,
      )}
      style={{ ...style, width: size, height: size, fontSize: Math.round(size * 0.36) }}
      aria-hidden
    >
      {member.initial}
    </span>
  );
}
