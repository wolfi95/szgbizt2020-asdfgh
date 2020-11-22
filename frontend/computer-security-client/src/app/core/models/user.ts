import { Role } from './role';

export interface IUserModel {
    id: string;
    email?: string;
    firstName?: string;
    lastName?: string;
    role?: Role;
    token?: string;
}