import {rest, setDefaultHeaders} from '../index';
import {AuthenticatedUser, LoginResponse} from './types';
import {authHeader} from './authHeader';
import {handleHttpUnauthorized} from './unauthorized';
import * as dataStore from './dataStore';

export const login = (email: string, password: string, remember?: boolean) =>
    new Promise<AuthenticatedUser>((resolve, reject) => {
        rest.post<LoginResponse>('/account/authenticate', {email, password, remember})
            .then(response => {
                const user: AuthenticatedUser = {
                    ...response.data.data,
                    lastRefresh: new Date().getTime()
                };

                dataStore.add(user.token, user);
                setDefaultHeaders(authHeader());
                handleHttpUnauthorized();

                resolve(user);
            })
            .catch(err => reject(err));
    });
