"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { DEFAULT_GROUP_ID } from "@/lib/mock-data";
import { Icon } from "./icon";
import { cn } from "@/lib/utils";

type Tab = "home" | "expenses" | "settlement" | "settings";

/** 下タブ4つ + 中央FAB（design.html 05-NAVIGATION） */
export function BottomNav({ groupId = DEFAULT_GROUP_ID }: { groupId?: string }) {
  const pathname = usePathname();

  const active: Tab | null = (() => {
    if (pathname === "/") return "home";
    if (pathname === "/settings") return "settings";
    if (pathname.includes("/settlement")) return "settlement";
    if (pathname.includes("/expenses")) return "expenses";
    if (pathname.startsWith("/groups/")) return "home";
    return null;
  })();

  return (
    <nav
      aria-label="メインナビゲーション"
      className="safe-bottom sticky bottom-0 z-20 grid h-[62px] grid-cols-5 items-center border-t border-border bg-surface"
    >
      <TabLink href="/" label="ホーム" icon="home" active={active === "home"} />
      <TabLink
        href={`/groups/${groupId}/expenses`}
        label="支出"
        icon="receipt-text"
        active={active === "expenses"}
      />
      <div className="flex justify-center">
        <Link
          href={`/groups/${groupId}/expenses/new`}
          aria-label="支出を追加"
          className="-mt-7 flex h-12 w-12 items-center justify-center rounded-full border-4 border-surface text-white shadow-[0_10px_18px_-6px_rgba(19,138,83,.55)]"
          style={{ background: "linear-gradient(160deg,#26B071,#138A53)" }}
        >
          <Icon name="plus" size={23} />
        </Link>
      </div>
      <TabLink
        href={`/groups/${groupId}/settlement`}
        label="精算"
        icon="arrow-left-right"
        active={active === "settlement"}
      />
      <TabLink href="/settings" label="設定" icon="settings" active={active === "settings"} />
    </nav>
  );
}

function TabLink({
  href,
  label,
  icon,
  active,
}: {
  href: string;
  label: string;
  icon: string;
  active: boolean;
}) {
  return (
    <Link
      href={href}
      aria-current={active ? "page" : undefined}
      className={cn(
        "flex flex-col items-center gap-[3px]",
        active ? "text-primary" : "text-[#A2A49C]",
      )}
    >
      <Icon name={icon} size={21} />
      <span className="text-[9.5px] font-bold">{label}</span>
    </Link>
  );
}
