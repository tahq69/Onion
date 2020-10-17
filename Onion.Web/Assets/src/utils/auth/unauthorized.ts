import {addResponseErrorHandler, setDefaultHeaders} from '../rest';
import {authHeader, refreshToken, dataStore} from './index';

export const handleHttpUnauthorized = () => {
    addResponseErrorHandler((rest, error) => {
        if (error.response.status !== 401) return;

        // 401 is Unauthorized error which means that this request failed.
        // What we need to do is send a refresh request, then resend the same
        // request that failed but with the new access token.
        // We can do this automatically using axios interceptors.

        return refreshToken()
            .then(token => {
                // Update currUser with new access_token
                // Set default headers to have new authorization token
                dataStore.add(token);
                setDefaultHeaders(authHeader());

                // Get the original request that failed due to 401 and
                // resend it with the new access token.
                return new Promise((resolve, reject) => {
                    rest.request(error.config)
                        .then(response => resolve(response))
                        .catch(innerError => reject([error, innerError]));
                });
            }, error => {
                // Just logout if anything goes wrong.
                dataStore.remove();

                return Promise.reject(error);
            });
    });
}