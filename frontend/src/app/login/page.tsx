import Link from "next/link";
import { Icon } from "@/components/icon";
import { Button } from "@/components/button";

export default function LoginPage() {
  return (
    <div className="flex min-h-dvh flex-col bg-bg px-7">
      {/* ロゴ + サービス名 */}
      <div className="flex flex-1 flex-col items-center justify-center gap-[18px]">
        <div
          className="flex h-[74px] w-[74px] items-center justify-center rounded-[23px] text-white shadow-[0_14px_30px_-10px_rgba(19,138,83,.55)]"
          style={{ background: "linear-gradient(160deg,#26B071,#138A53)" }}
        >
          <Icon name="home" size={36} />
        </div>
        <div className="text-center">
          <div className="text-[23px] font-extrabold tracking-tight">かぞく精算帳</div>
          <div className="mt-[7px] text-[13.5px] font-semibold text-muted">
            家族の支出を、かんたん精算
          </div>
        </div>
      </div>

      {/* 認証アクション */}
      <div className="flex flex-col gap-[11px] pb-9">
        <Button href="/">ログイン</Button>
        <Button href="/" variant="secondary">
          新規登録
        </Button>

        <div className="my-1.5 flex items-center gap-3">
          <span className="h-px flex-1 bg-[#E4E3DE]" />
          <span className="text-xs font-semibold text-hint">または</span>
          <span className="h-px flex-1 bg-[#E4E3DE]" />
        </div>

        <Button href="/" variant="secondary" className="h-[50px] text-[14.5px]">
          <span className="flex h-5 w-5 items-center justify-center rounded-full border border-[#E4E3DE] bg-white text-xs font-extrabold text-[#4285F4]">
            G
          </span>
          Googleで続ける
        </Button>

        <div className="mt-1 text-center">
          <Link href="/" className="text-[13.5px] font-bold text-primary">
            ゲストで試してみる
          </Link>
        </div>

        <p className="mt-2 text-center text-[10.5px] leading-relaxed text-hint">
          続行すると利用規約・プライバシーポリシーに
          <br />
          同意したものとみなされます
        </p>
      </div>
    </div>
  );
}
