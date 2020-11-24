import { Role } from './role';

export interface IUserModel {
    id: string;
    email?: string;
    firstName?: string;
    lastName?: string;
    role?: Role;
    token?: string;
}

export interface IUserRegisterModel {
    email: string;
    firstName: string;
    lastName: string;
    password: string;
    confirmPassword: string;
}