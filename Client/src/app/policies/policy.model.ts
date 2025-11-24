export interface Policy {
  id: string;
  policyNumber: string;
  policyType: string;
  policyStatus: string;
  startDate: string;
  endDate?: string;
  insuredAmount: number;
  clientId: string;
  createdAt: string;
  updatedAt?: string;
}