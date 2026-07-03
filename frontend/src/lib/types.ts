// ドメイン型（docs/requirements・database-design に準拠）
// 金額はすべて整数（円, JPY）。

/** カテゴリ（固定enum: business-rules.md） */
export type Category = "食費" | "日用品" | "光熱費" | "交通" | "娯楽" | "その他";

/** 割り勘方式 */
export type SplitType = "Equal" | "Ratio";

/** リスト/詳細でのアイコン配色 */
export type ExpenseTone = "green" | "blue" | "coral" | "gold" | "violet" | "neutral";

/** アバターの配色（イニシャル表示用パステル） */
export interface AvatarColor {
  bg: string;
  fg: string;
}

export interface Member {
  id: string;
  name: string;
  initial: string;
  email: string;
  avatar: AvatarColor;
  /** 招待リンク送信済みで未参加 */
  pending?: boolean;
  /** グループ作成者 */
  owner?: boolean;
}

/** 1人分の負担（対象メンバーのみ行を持つ） */
export interface ExpenseShare {
  memberId: string;
  /** Ratio のとき使用（合計100） */
  ratio?: number;
  /** 確定した負担額（整数/円） */
  shareAmount: number;
}

export interface Expense {
  id: string;
  groupId: string;
  title: string;
  amount: number;
  category: Category;
  paidByMemberId: string;
  /** YYYY-MM-DD */
  date: string;
  /** 表示用の時刻ラベル（任意） */
  time?: string;
  note?: string;
  splitType: SplitType;
  shares: ExpenseShare[];
  /** 表示アイコン上書き（未指定はカテゴリ既定） */
  icon?: string;
  tone?: ExpenseTone;
  /** null/未設定 = 未精算 */
  settlementId?: string | null;
  /** OCR（レシート）から作成された支出 */
  fromReceipt?: boolean;
}

/** 精算の送金1本 */
export interface Transfer {
  fromMemberId: string;
  toMemberId: string;
  amount: number;
}

export interface Settlement {
  id: string;
  groupId: string;
  /** 集計対象期間ラベル（例: 2024/05/01 – 05/31） */
  periodLabel: string;
  /** 確定日ラベル（例: 6/1 精算完了） */
  settledLabel: string;
  memberCount: number;
  /** ログインユーザーから見た収支 */
  net: number;
  transfers: Transfer[];
}

export interface Group {
  id: string;
  name: string;
  /** lucide アイコン名 */
  icon: string;
  iconTone: "green" | "amber" | "violet";
  memberIds: string[];
  inviteCode: string;
  /** 一覧カードのサブテキスト（例: スーパー · 2時間前） */
  subtitle: string;
  /** ログインユーザーから見た収支（精算済みグループは null） */
  net: number | null;
}
