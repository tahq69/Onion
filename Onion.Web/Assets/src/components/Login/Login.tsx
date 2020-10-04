import React, {FC} from 'react';

import LoginLayout from './components/LoginLayout/LoginLayout';
import LoginForm from "./components/LoginForm/LoginForm";

const Login: FC = () => (
    <LoginLayout>
        <LoginForm/>
    </LoginLayout>
);

export default Login;