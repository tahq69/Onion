import {Response} from '../../../../types';

export {Errors, Response} from '../../../../types';

export interface LoginRequest {
    email: string
    password: string
    remember: boolean
}

export interface LoginData {
    id: string
    userName: string
    email: string
    roles: Array<string>,
    isVerified: boolean,
    token: string
}

export interface LoginResponse extends Response<LoginData> {
}