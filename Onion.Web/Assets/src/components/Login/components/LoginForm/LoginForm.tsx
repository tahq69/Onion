import React, {FC, useState, useEffect} from 'react';
import {Form, Input, Button, Checkbox, Space} from 'antd';
import {UserOutlined, LockOutlined} from '@ant-design/icons';

import {rest, validateMessage, validateStatus} from './../../../../utils';

import {LoginRequest, LoginResponse, Errors, Response} from "./types";

const LoginForm: FC = () => {
    const [errors, setErrors] = useState<Errors | undefined | null>(null);
    const status = validateStatus(errors);
    const message = validateMessage(errors);

    const onFinish = (values: LoginRequest) => {
        console.log('Received values of form: ', values);

        rest.post<LoginResponse>('/account/authenticate', values)
            .then(r => r.data)
            .then(data => {
                console.log('request response:', data);
            })
            .catch(error => {
                const body: Response<undefined> = error.response.data;
                setErrors(body.errors);

                console.log('request error:', error, body);
            });
    };

    const emailRules = [{required: true, message: 'Please input your Email!'}];
    const passwordRules = [{required: true, message: 'Please input your Password!'}];

    return (
        <Form
            id="components-login-form"
            name="normal_login"
            className="login-form"
            initialValues={{remember: true}}
            onFinish={onFinish}
        >
            <Form.Item
                name="email"
                rules={emailRules}
                validateStatus={status("Email")}
                help={message("Email")}
            >
                <Input
                    type="email"
                    prefix={<UserOutlined className="site-form-item-icon"/>}
                    placeholder="Email"/>
            </Form.Item>
            <Form.Item name="password" rules={passwordRules}>
                <Input
                    prefix={<LockOutlined className="site-form-item-icon"/>}
                    type="password"
                    placeholder="Password"
                />
            </Form.Item>
            <Form.Item>
                <Form.Item name="remember" valuePropName="checked" noStyle>
                    <Checkbox>Remember me</Checkbox>
                </Form.Item>

                <a className="login-form-forgot" href="">
                    Forgot password
                </a>
            </Form.Item>
            <Form.Item>
                <Space size={20}>
                    <Button type="primary" htmlType="submit" className="login-form-button">
                        Log in
                    </Button>
                    <span>Or <a href="">register now!</a></span>
                </Space>
            </Form.Item>
        </Form>
    );
};

export default LoginForm;