import React, {FC} from 'react';
import {BrowserRouter as Router, Switch, Route} from 'react-router-dom';
import {RecoilRoot} from 'recoil';

import rest from './../../utils/rest';
import About from './../About/About';
import Login from './../Login/Login';

import './App.less';

const App: FC = () => (
    <div className="App">
        <RecoilRoot>
            <Router>
                <Switch>
                    <Route path="/about"><About/></Route>
                    <Route path="/"><Login/></Route>
                </Switch>
            </Router>
        </RecoilRoot>
    </div>
);

rest.get('/info').then(r => console.log(r.data));

export default App;
