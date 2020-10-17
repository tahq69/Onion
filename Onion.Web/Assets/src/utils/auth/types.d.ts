import {Response} from '../../types';

export interface LoginData {
    id: string
    userName: string
    email: string
    roles: Array<string>
    isVerified: boolean
    token: string
    expiresIn: number
}

export interface AuthenticatedUser extends LoginData {
    lastRefresh: number
}

export interface LoginResponse extends Response<LoginData> {
}