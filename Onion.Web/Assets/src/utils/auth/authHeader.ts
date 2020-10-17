import {Dictionary} from '../../types';
import {getToken} from './dataStore';

export const authHeader = (): Dictionary<string> => {
    const token = getToken();
    if (token) {
        return {Authorization: `Bearer ${token}`}
    }

    return {};
}

export default authHeader;