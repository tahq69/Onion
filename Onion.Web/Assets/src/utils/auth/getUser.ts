import {setDefaultHeaders} from '../rest';
import refreshToken from './refreshToken';
import {AuthenticatedUser} from './types';
import * as dataStore from './dataStore';
import {authHeader} from './authHeader';
import {handleHttpUnauthorized} from './unauthorized';

export const getUser = (): Promise<null | AuthenticatedUser> => {
    const user = dataStore.getUser();
    if (!user) {
        return Promise.resolve(null);
    }

    let currDate = new Date();
    let diff = currDate.getTime() - user.lastRefresh;
    
    console.log(diff, user.expiresIn * 1000)

    if (diff >= user.expiresIn * 1000) {
        // Access token expired. We need to refresh token.
        return refreshToken()
            .then(token => {
                dataStore.add(token);
                setDefaultHeaders(authHeader());
                handleHttpUnauthorized();

                return user;
            }, error => {
                dataStore.remove();

                return null;
            });
    }

    setDefaultHeaders(authHeader());
    handleHttpUnauthorized();

    return Promise.resolve(user);
}

export default getUser;