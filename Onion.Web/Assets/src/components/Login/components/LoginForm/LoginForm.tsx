import React, {FC} from 'react';
import {Form, Input, Button, Checkbox, Space} from 'antd';
import {UserOutlined, LockOutlined} from '@ant-design/icons';

const LoginForm: FC = () => {
    const onFinish = (values: any) => {
        console.log('Received values of form: ', values);
    };

    const usernameRules = [{required: true, message: 'Please input your Username!'}];
    const passwordRules = [{required: true, message: 'Please input your Password!'}];

    return (
        <Form
            id="components-login-form"
            name="normal_login"
            className="login-form"
            initialValues={{remember: true}}
            onFinish={onFinish}
        >
            <Form.Item name="username" rules={usernameRules}>
                <Input
                    prefix={<UserOutlined className="site-form-item-icon"/>}
                    placeholder="Username"/>
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