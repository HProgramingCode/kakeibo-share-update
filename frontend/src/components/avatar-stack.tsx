import type { Member } from "@/lib/types";

/** 重なり合うアバター + 余剰人数の +N */
export function AvatarStack({
  members,
  max = 3,
  size = 28,
}: {
  members: Member[];
  max?: number;
  size?: number;
}) {
  const shown = members.slice(0, max);
  const rest = members.length - shown.length;
  const fontSize = Math.round(size * 0.4);

  return (
    <div className="flex items-center">
      {shown.map((m, i) => (
        <span
          key={m.id}
          className="inline-flex items-center justify-center rounded-full font-bold ring-2 ring-white"
          style={{
            background: m.avatar.bg,
            color: m.avatar.fg,
            width: size,
            height: size,
            fontSize,
            marginLeft: i === 0 ? 0 : -8,
          }}
          aria-hidden
        >
          {m.initial}
        </span>
      ))}
      {rest > 0 && (
        <span
          className="inline-flex items-center justify-center rounded-full bg-bg font-bold text-faint ring-2 ring-white"
          style={{ width: size, height: size, fontSize: fontSize - 1, marginLeft: -8 }}
        >
          +{rest}
        </span>
      )}
    </div>
  );
}
