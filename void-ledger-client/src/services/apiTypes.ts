// Shared frontend DTO types matching the backend API response shapes.

// Portfolio API responses.
export type PortfolioPositionDto = {
  name: string;
  quantity: number;
  currentPrice: number;
  positionValue: number;
};
export type PortfolioValuationDto = {
  cashBalance: number;
  totalPortfolioValue: number;
  totalAccountValue: number;
  positions: PortfolioPositionDto[];
};


// Prices API responses.
export type PriceDto = {
  name: string;
  price: number;
  previousPrice?: number | null;
  direction: string;
  changeAmount?: number | null;
  updatedAtUtc: string;
};


// Shared response for API actions that return only a display message.
export type MessageResponseDto = {
  message: string;
};


// Reports API responses.
export type ReportTotalsDto = {
  actionCount: number;
  totalDeposited: number;
  totalSpentOnBuys: number;
  totalEarnedFromSells: number;
  netCashflow: number;
};

export type ActionDto = {
  type: string;
  name?: string | null;
  quantity?: number | null;
  amount?: number | null;
  unitPrice?: number | null;
  total?: number | null;
  price?: number | null;
  at: string;
};

export type ActionListResponseDto = {
  items: ActionDto[];
};