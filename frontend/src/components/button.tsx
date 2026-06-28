import Link from "next/link";
import { cn } from "@/lib/utils";

type Variant = "primary" | "secondary" | "ghost";

const VARIANT: Record<Variant, string> = {
  primary:
    "bg-primary text-white shadow-[0_8px_18px_-8px_rgba(26,155,94,.55)] active:bg-primary-deep",
  secondary: "border border-[#DDDCD6] bg-surface text-ink",
  ghost: "bg-bg text-muted",
};

type CommonProps = {
  variant?: Variant;
  className?: string;
  children: React.ReactNode;
};

/** デザインの主要/セカンダリ/ゴーストボタン。href があれば Link としてレンダリング */
export function Button({
  variant = "primary",
  className,
  children,
  href,
  ...rest
}: CommonProps &
  (
    | ({ href: string } & React.ComponentProps<typeof Link>)
    | ({ href?: undefined } & React.ButtonHTMLAttributes<HTMLButtonElement>)
  )) {
  const cls = cn(
    "inline-flex h-[52px] w-full items-center justify-center gap-2 rounded-[14px] text-base font-bold transition-colors",
    VARIANT[variant],
    className,
  );

  if (href) {
    return (
      <Link
        href={href}
        className={cls}
        {...(rest as Omit<React.ComponentProps<typeof Link>, "href">)}
      >
        {children}
      </Link>
    );
  }
  return (
    <button className={cls} {...(rest as React.ButtonHTMLAttributes<HTMLButtonElement>)}>
      {children}
    </button>
  );
}
