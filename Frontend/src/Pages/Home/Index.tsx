import React from 'react'
import { session } from '../../session';

const Index = () => {
  debugger
  const user = session.getUser();
  
  return (
    <div>Welcome to {import.meta.env.VITE_APP_NAME} <strong>{user?.name}</strong></div>
  )
}

export default Index