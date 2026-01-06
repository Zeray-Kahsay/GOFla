import { Outlet } from "react-router-dom"
import {Header} from "./Header"
import { Footer } from "./Footer"
import { CartSidebar } from "../../features/cart/CartSidebar"

function App() {
  return (
    <>
  <Header />

  <div className="min-h-screen bg-linear-to-br from-amber-50 via-neutral-50 to-amber-100">
    <Outlet />
  </div>
  
  <Footer />
  <CartSidebar />
    </>
  )
}

export default App
