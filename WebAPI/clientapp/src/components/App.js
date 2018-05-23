import React, { Component } from 'react';
import { Switch, Route } from 'react-router-dom';
import ValuePage from './pages/ValuePage';


class App extends Component {
  render() {
    return (
       <Switch>
         <Route path="/" component={ValuePage} />
       </Switch>
    );
  }
}

export default App;
