import React, { Component } from 'react';
import { Switch, Route } from 'react-router-dom';
import ValuePage from './pages/ValuePage';
import LoginContainer from './containers/LoginContainer';


class App extends Component {
  render() {
    return (
       <Switch>
         <Route path="/login" component={LoginContainer} />
         <Route path="/" component={ValuePage} />
       </Switch>
    );
  }
}

export default App;
