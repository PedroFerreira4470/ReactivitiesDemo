import React from 'react';
import './App.css';
import { Header,List } from 'semantic-ui-react'
const App: React.FC = () => {
  return (
    <div>
    <Header as='h2' icon='users' content='Reactivities' />
    <List>
      <List.Item>Apples</List.Item>
      <List.Item>Pears</List.Item>
      <List.Item>Oranges</List.Item>
    </List>

    </div>
  );
}

export default App;
