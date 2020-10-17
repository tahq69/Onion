import {AuthenticatedUser} from './types';

export const add = (token: string, user?: AuthenticatedUser) => {
    localStorage.setItem('auth-token', token);
    if (user)
        localStorage.setItem('auth-user', JSON.stringify(user));
}

export const remove = () => {
    localStorage.removeItem('auth-token');
    localStorage.removeItem('auth-user');
}

export const getToken = (): string | null =>
    localStorage.getItem('auth-token');

export const getUser = (): AuthenticatedUser | null => {
    const data = localStorage.getItem('auth-user');
    if (data)
        return JSON.parse(data);

    return null;
}

export default {add, remove, getToken, getUser};
