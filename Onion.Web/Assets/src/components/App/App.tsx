import React, {FC, useState} from 'react';
import {useRecoilState} from 'recoil';

import {authenticatedUserState} from '../../state/userState';
import {getUser} from '../../utils/auth';

import './App.less';
import AuthenticatedApp from './AuthenticatedApp';
import UnauthenticatedApp from './UnauthenticatedApp';
import LoadingPage from "../LoadingPage/LoadingPage";

const App: FC = () => {
    const [authenticatedUser, setAuthenticatedUser] = useRecoilState(authenticatedUserState);
    const [loading, setLoading] = useState(true);
    const [once, setOnce] = useState(true);

    if (once && !authenticatedUser.authenticated) {
        setOnce(false);

        getUser()
            .then(user => {
                console.log()
                setAuthenticatedUser(curr => {
                    const state = {...curr, ...user, authenticated: !!user};
                    console.log('user state: ', state);
                    return state;
                });

                setLoading(false);
            })
            .catch(error => setLoading(false));
    }

    if (loading)
        return <LoadingPage/>;

    return (
        <div className="App">
            {authenticatedUser.authenticated ? <AuthenticatedApp/> : <UnauthenticatedApp/>}
        </div>
    );
}

export default App;
