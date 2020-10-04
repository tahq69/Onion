import React, {FC} from 'react';
import {Button} from 'antd';
import logo from './../../images/logo.svg';
import rest from './../../utils/rest';
import './App.less';

const App: FC = () => (
    <div className="App">
        <header className="App-header">
            <img src={logo} className="App-logo" alt="logo"/>
            <p>
                Edit <code>src/App.tsx</code> and save to reload.
            </p>
            <Button
                className="App-link"
                href="https://reactjs.org"
                target="_blank"
                rel="noopener noreferrer"
            >
                Learn React
            </Button>
        </header>
    </div>
);

rest.get('/info').then(r => console.log(r.data));

export default App;
