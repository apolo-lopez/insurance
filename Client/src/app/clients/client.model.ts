export interface IdentificationNumber {
    value: string;
}

export interface Client {
    id: string;
    identificationNumber: IdentificationNumber;
    name: string;
    email: string;
    phoneNumber: string;
    address?: string;
    createdAt: string;
    updatedAt?: string;
}
