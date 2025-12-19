import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import { Provider } from 'react-redux'
import {RouterProvider } from "react-router-dom"
import {ToastContainer } from 'react-toastify'
import { store } from './app/store/store.ts'
import { router } from './app/routes/routes.tsx'
import 'react-toastify/dist/ReactToastify.css';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Provider store={store}>
      <ToastContainer position="top-right" hideProgressBar theme="colored" />
      <RouterProvider router={router}/>
    </Provider>
  </StrictMode>,
)
