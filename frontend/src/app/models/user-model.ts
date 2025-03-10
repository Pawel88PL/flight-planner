export interface User {}

export interface TwoFactorRequest {}

export interface Role {
    id: number;
    name: string;
}

export interface UserListModel {
    data: User[];
    totalRecords: number;
}