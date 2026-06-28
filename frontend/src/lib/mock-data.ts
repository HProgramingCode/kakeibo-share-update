import type { Expense, Group, Member, Settlement } from "./types";

// ---- モックデータ ----------------------------------------------------------
// バックエンド未実装のため、UI 確認用のダミーデータ。
// 将来は src/lib/api.ts 等のデータ取得関数に差し替える前提で、
// 画面は下記の getter 関数経由でのみ参照する。

const AVATAR = {
  green: { bg: "#DCE9DA", fg: "#3C5E3A" },
  pink: { bg: "#F3DCE0", fg: "#8A4A56" },
  blue: { bg: "#DCE4F0", fg: "#3F5680" },
  gold: { bg: "#F3E6CE", fg: "#8A6A38" },
} as const;

export const members: Member[] = [
  {
    id: "m_taro",
    name: "たろう",
    initial: "た",
    email: "taro@example.com",
    avatar: AVATAR.green,
    owner: true,
  },
  {
    id: "m_hanako",
    name: "はなこ",
    initial: "は",
    email: "hanako@example.com",
    avatar: AVATAR.pink,
  },
  {
    id: "m_yuta",
    name: "ゆうた",
    initial: "ゆ",
    email: "yuta@example.com",
    avatar: AVATAR.blue,
  },
  {
    id: "m_misaki",
    name: "みさき",
    initial: "み",
    email: "招待リンクを送信済み",
    avatar: AVATAR.gold,
    pending: true,
  },
];

/** ログイン中のユーザー */
export const CURRENT_USER_ID = "m_taro";

export const groups: Group[] = [
  {
    id: "g_home",
    name: "家族の家計",
    icon: "house",
    iconTone: "green",
    memberIds: ["m_taro", "m_hanako", "m_yuta", "m_misaki"],
    inviteCode: "abc123",
    subtitle: "4人 · スーパー · 2時間前",
    net: 2850,
  },
  {
    id: "g_okinawa",
    name: "沖縄旅行 2024",
    icon: "palmtree",
    iconTone: "amber",
    memberIds: ["m_taro", "m_yuta", "m_misaki"],
    inviteCode: "oki456",
    subtitle: "3人 · ホテル · 昨日",
    net: -1200,
  },
  {
    id: "g_room",
    name: "ルームシェア",
    icon: "users",
    iconTone: "violet",
    memberIds: ["m_taro", "m_hanako"],
    inviteCode: "room789",
    subtitle: "2人 · すべて精算済み",
    net: null,
  },
];

/** 下タブの「支出 / 精算」など、グループ文脈が要る導線のデフォルト先 */
export const DEFAULT_GROUP_ID = "g_home";

const eq = (memberIds: string[], amount: number) => {
  const each = Math.floor(amount / memberIds.length);
  return memberIds.map((memberId, i) => ({
    memberId,
    // 端数は支払者（先頭=立替者）が吸収する想定。ここでは表示用に均等値。
    shareAmount: i === 0 ? amount - each * (memberIds.length - 1) : each,
  }));
};

export const expenses: Expense[] = [
  {
    id: "e_super",
    groupId: "g_home",
    title: "スーパー",
    amount: 3240,
    category: "食費",
    paidByMemberId: "m_taro",
    date: "2024-05-13",
    time: "12:30",
    note: "特売の食材など",
    splitType: "Equal",
    shares: eq(["m_taro", "m_hanako", "m_yuta", "m_misaki"], 3240),
    settlementId: null,
    fromReceipt: true,
  },
  {
    id: "e_drug",
    groupId: "g_home",
    title: "ドラッグストア",
    amount: 1980,
    category: "日用品",
    paidByMemberId: "m_hanako",
    date: "2024-05-12",
    splitType: "Equal",
    shares: eq(["m_taro", "m_hanako", "m_yuta", "m_misaki"], 1980),
    settlementId: null,
    icon: "pill",
    tone: "blue",
  },
  {
    id: "e_dinner",
    groupId: "g_home",
    title: "外食 ・ 焼肉",
    amount: 12000,
    category: "食費",
    paidByMemberId: "m_taro",
    date: "2024-05-10",
    time: "19:40",
    splitType: "Ratio",
    shares: [
      { memberId: "m_taro", ratio: 42, shareAmount: 5000 },
      { memberId: "m_hanako", ratio: 33, shareAmount: 4000 },
      { memberId: "m_yuta", ratio: 17, shareAmount: 2000 },
      { memberId: "m_misaki", ratio: 8, shareAmount: 1000 },
    ],
    settlementId: null,
    icon: "utensils",
    tone: "coral",
  },
  {
    id: "e_daily",
    groupId: "g_home",
    title: "日用品",
    amount: 1320,
    category: "日用品",
    paidByMemberId: "m_yuta",
    date: "2024-05-10",
    splitType: "Equal",
    shares: eq(["m_taro", "m_hanako", "m_yuta", "m_misaki"], 1320),
    settlementId: null,
  },
];

export const settlements: Settlement[] = [
  {
    id: "s_2024_05",
    groupId: "g_home",
    periodLabel: "2024/05/01 – 05/31",
    settledLabel: "6/1 精算完了 · 4人",
    memberCount: 4,
    net: 2850,
    transfers: [
      { fromMemberId: "m_hanako", toMemberId: "m_taro", amount: 1500 },
      { fromMemberId: "m_yuta", toMemberId: "m_taro", amount: 1350 },
    ],
  },
  {
    id: "s_2024_04",
    groupId: "g_home",
    periodLabel: "2024/04/01 – 04/30",
    settledLabel: "5/1 精算完了 · 4人",
    memberCount: 4,
    net: 1200,
    transfers: [],
  },
  {
    id: "s_2024_03",
    groupId: "g_home",
    periodLabel: "2024/03/01 – 03/31",
    settledLabel: "4/1 精算完了 · 4人",
    memberCount: 4,
    net: -530,
    transfers: [],
  },
  {
    id: "s_2024_02",
    groupId: "g_home",
    periodLabel: "2024/02/01 – 02/29",
    settledLabel: "3/1 精算完了 · 4人",
    memberCount: 4,
    net: 3410,
    transfers: [],
  },
];

// ---- getter（データ取得の単一窓口） ---------------------------------------

export function getCurrentUser(): Member {
  return getMember(CURRENT_USER_ID);
}

export function getMember(id: string): Member {
  const m = members.find((x) => x.id === id);
  if (!m) throw new Error(`member not found: ${id}`);
  return m;
}

export function getGroup(id: string): Group | undefined {
  return groups.find((g) => g.id === id);
}

export function getGroupMembers(groupId: string): Member[] {
  const g = getGroup(groupId);
  if (!g) return [];
  return g.memberIds.map(getMember);
}

export function getGroupExpenses(groupId: string): Expense[] {
  return expenses.filter((e) => e.groupId === groupId);
}

export function getExpense(id: string): Expense | undefined {
  return expenses.find((e) => e.id === id);
}

export function getGroupSettlements(groupId: string): Settlement[] {
  return settlements.filter((s) => s.groupId === groupId);
}

/** 未精算支出から、ログインユーザーの収支・受け取り予定送金を組み立てる（モック） */
export function getOpenSettlement(groupId: string): Settlement {
  const open = settlements.find((s) => s.groupId === groupId);
  return (
    open ?? {
      id: "open",
      groupId,
      periodLabel: "未精算",
      settledLabel: "",
      memberCount: getGroupMembers(groupId).length,
      net: 0,
      transfers: [],
    }
  );
}
