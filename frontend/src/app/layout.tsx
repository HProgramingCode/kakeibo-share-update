import type { Metadata, Viewport } from "next";
import { Noto_Sans_JP } from "next/font/google";
import "./globals.css";

const notoSansJP = Noto_Sans_JP({
  variable: "--font-noto-sans-jp",
  subsets: ["latin"],
  weight: ["400", "500", "700", "800", "900"],
  display: "swap",
});

export const metadata: Metadata = {
  title: "かぞく精算帳",
  description: "家族の立て替えを記録して、最小回数で気持ちよく精算するアプリ。",
  applicationName: "かぞく精算帳",
  appleWebApp: {
    capable: true,
    statusBarStyle: "default",
    title: "かぞく精算帳",
  },
};

export const viewport: Viewport = {
  themeColor: "#1a9b5e",
  width: "device-width",
  initialScale: 1,
  maximumScale: 1,
  viewportFit: "cover",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="ja" className={notoSansJP.variable}>
      <body>
        <div className="app-shell">{children}</div>
      </body>
    </html>
  );
}
