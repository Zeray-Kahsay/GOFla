import { Outlet } from "react-router-dom"
import {Header} from "./Header"

function App() {
  return (
    <>
  <Header />

  <div>
    <Outlet />
  </div>
    </>
  )
}

export default App
