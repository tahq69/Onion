import React, {FC, useState} from 'react';
import {Button, Checkbox, Form, Input, Space, Typography} from 'antd';
import {LockOutlined, UserOutlined} from '@ant-design/icons';

import {rest, useErrors, useResponseMessage} from './../../../../utils';

import {LoginData, LoginRequest, LoginResponse, Response} from "./types";

const LoginForm: FC = () => {
    const [status, error, errorHandler, clear] = useErrors();
    const [message, checkSucceeded] = useResponseMessage<LoginResponse, LoginData>();

    const onFinish = (values: LoginRequest) => {
        rest.post<LoginResponse>('/account/authenticate', values)
            .then(checkSucceeded)
            .then(data => {
                console.log('request response:', data);
            })
            .catch(errorHandler);
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
            {message ? <Typography.Text type="danger">{message}</Typography.Text> : undefined}

            <Form.Item
                name="email"
                rules={emailRules}
                validateStatus={status("email")}
                help={error("email")}
            >
                <Input
                    type="email"
                    onChange={() => clear("email")}
                    prefix={<UserOutlined className="site-form-item-icon"/>}
                    placeholder="Email"/>
            </Form.Item>
            <Form.Item name="password" rules={passwordRules}>
                <Input
                    onChange={() => clear("email")}
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