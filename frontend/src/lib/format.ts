import type { Category } from "./types";

/** 12345 -> "¥12,345" */
export function formatYen(amount: number): string {
  return `¥${Math.abs(amount).toLocaleString("ja-JP")}`;
}

/** 符号つき金額: +2850 -> "+¥2,850" / -1200 -> "−¥1,200" / 0 -> "±0" */
export function formatSignedYen(amount: number): string {
  if (amount === 0) return "±0";
  const sign = amount > 0 ? "+" : "−";
  return `${sign}${formatYen(amount)}`;
}

/** 収支の符号に応じた意味色（受取=緑 / 支払=コーラル / ゼロ=ニュートラル） */
export function netTone(amount: number | null): "positive" | "negative" | "neutral" {
  if (amount === null || amount === 0) return "neutral";
  return amount > 0 ? "positive" : "negative";
}

/** カテゴリ -> lucide アイコン名（design.html 準拠） */
export const categoryIcon: Record<Category, string> = {
  食費: "shopping-cart",
  日用品: "package",
  光熱費: "zap",
  交通: "train-front",
  娯楽: "utensils",
  その他: "tag",
};

/** YYYY-MM-DD -> "5月13日（月）" */
export function formatDateLabel(iso: string): string {
  const d = new Date(`${iso}T00:00:00`);
  const week = ["日", "月", "火", "水", "木", "金", "土"][d.getDay()];
  return `${d.getMonth() + 1}月${d.getDate()}日（${week}）`;
}

/** YYYY-MM-DD -> "5/13" */
export function formatShortDate(iso: string): string {
  const d = new Date(`${iso}T00:00:00`);
  return `${d.getMonth() + 1}/${d.getDate()}`;
}
