import { Outlet } from "react-router-dom"
import {Header} from "./Header"
import { Footer } from "./Footer"
import { CartSidebar } from "../../features/cart/CartSidebar"

function App() {
  return (
    <>
  <Header />

  <div>
    <Outlet />
  </div>
  
  <Footer />
  <CartSidebar />
    </>
  )
}

export default App
