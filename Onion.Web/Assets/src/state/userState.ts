import {atom} from 'recoil';

interface AuthenticatedUser {
    authenticated: boolean,
    id: string | null,
    userName: string | null,
    email: string | null,
    roles: Array<string> | null,
    isVerified: boolean | null,
    token: string | null,
}

export const authenticatedUserState = atom<AuthenticatedUser>({
    key: 'authenticatedUserState',
    default: {
        authenticated: false,
        id: null,
        userName: null,
        email: null,
        roles: null,
        isVerified: null,
        token: null,
    },
});