import React, {FC} from 'react';
import {BrowserRouter as Router, Route, Switch} from 'react-router-dom';
import About from "../About/About";
import Login from "../Login/Login";

const UnauthenticatedApp: FC = () => {
    return (
        <Router>
            <Switch>
                <Route path="/about"><About/></Route>
                <Route path="/"><Login/></Route>
            </Switch>
        </Router>
    );
}

export default UnauthenticatedApp;