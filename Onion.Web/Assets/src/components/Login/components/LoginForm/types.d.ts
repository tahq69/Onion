import {Response} from '../../../../types';

export {Errors, Response} from '../../../../types';

export interface LoginRequest {
    email: string
    password: string
    remember: boolean
}
