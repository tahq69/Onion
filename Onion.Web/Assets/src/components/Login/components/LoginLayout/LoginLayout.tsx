import React, {FC} from 'react';
import {Row, Col, Card} from 'antd';

import './LoginLayout.less';

const LoginLayout: FC = (props) => (
    <Row justify={"center"} align={"middle"} id={"components-login-layout"}>
        <Col>
            <Card>
                {props.children}
            </Card>
        </Col>
    </Row>
);

export default LoginLayout;