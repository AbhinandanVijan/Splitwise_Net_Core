export const SplitMethod = {
  Equal: 0,
  Percent: 1,
  Exact: 2
} as const;
export type SplitMethod = typeof SplitMethod[keyof typeof SplitMethod];

export type AuthResponse = {
  token: string; userId: string; name: string; email: string;
}

export type CreateGroupRequest = { name: string; description?: string }
export type GroupResponse = { id: string; name: string }

export type AddExpenseRequest = {
  groupId: string;
  payerId: string;
  amount: number;
  description: string;
  splitMethod: SplitMethod;
  participantIds: string[];
  percents?: number[];
  exactAmounts?: number[];
}

export type ExpenseResponse = {
  expenseId: string; amount: number; description: string; splitMethod: SplitMethod; createdAt: string;
}

export type BalanceLine = { counterpartyId: string; youOwe: number; oweYou: number }
