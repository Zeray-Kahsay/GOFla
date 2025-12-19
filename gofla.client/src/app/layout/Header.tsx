import { useState } from "react";
import { useAuth } from "../../hooks/useAuth";
import { useCart } from "../../hooks/useCart";
import { useAppDispatch } from "../store/store";
import { logout } from "../store/slices/authSlice";
import { Link } from "react-router-dom";
import { openCart } from "../store/slices/cartSlice";
import { Heart, LogOut, Menu, ShoppingCart, User, X } from "lucide-react";
import { Button } from "./ui/Button";
import { LoginModal } from "../../features/auth/LoginModal";
import { RegisterModal } from "../../features/auth/RegisterModal";
import SearchBar from "./SearchBar";
import { useAuthModal } from "../../hooks/useAuthModal";

export function Header() {
  const dispatch = useAppDispatch();
  const { user, isAuthenticated } = useAuth();
  const { itemCount } = useCart();
  const [showMobileMenu, setShowMobileMenu] = useState(false);
  const { 
    showLogin, 
    showRegister, 
    openLogin, 
    closeAll, 
    switchToRegister, 
    switchToLogin 
  } = useAuthModal();

  const handleLogout = () => {
    dispatch(logout());
    setShowMobileMenu(false);
  };

  return (
    <>
      <header className="sticky top-0 z-40 bg-white border-b shadow-sm">
        <div className="container mx-auto px-4">
          <div className="flex items-center justify-between h-16">
            {/* Logo */}
            <Link to="/" className="flex items-center gap-2">
              <div className="w-10 h-10 bg-primary-600 rounded-lg flex items-center justify-center">
                <span className="text-white font-bold text-xl">FO</span>
              </div>
              <span className="text-xl font-bold text-gray-900 hidden sm:block">
                GO-FLA
              </span>
            </Link>

            {/* Search Bar - Desktop */}
            <div className="hidden md:block flex-1 max-w-2xl mx-8">
              <SearchBar />
            </div>

            {/* Actions */}
            <div className="flex items-center gap-4">
              {isAuthenticated ? (
                <>
                  {/* Cart Button */}
                  <button
                    onClick={() => dispatch(openCart())}
                    className="relative p-2 hover:bg-gray-100 rounded-full transition-colors"
                    aria-label="Shopping Cart"
                  >
                    <ShoppingCart size={24} />
                    {itemCount > 0 && (
                      <span className="absolute -top-1 -right-1 bg-primary-600 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                        {itemCount > 99 ? '99+' : itemCount}
                      </span>
                    )}
                  </button>

                  {/* Favorites Button - Hidden on Mobile */}
                  <Link
                    to="/favorites"
                    className="p-2 hover:bg-gray-100 rounded-full hidden sm:block transition-colors"
                    aria-label="Favorites"
                  >
                    <Heart size={24} />
                  </Link>

                  {/* User Dropdown */}
                  <div className="relative group">
                    <button 
                      className="flex items-center gap-2 p-2 hover:bg-gray-100 rounded-full transition-colors"
                      aria-label="User Menu"
                    >
                      {user?.profileImageUrl ? (
                        <img
                          src={user.profileImageUrl}
                          alt={user.firstName}
                          className="w-8 h-8 rounded-full object-cover"
                        />
                      ) : (
                        <div className="w-8 h-8 bg-primary-100 rounded-full flex items-center justify-center">
                          <span className="text-primary-600 font-semibold text-sm">
                            {user?.firstName.charAt(0)}{user?.lastName.charAt(0)}
                          </span>
                        </div>
                      )}
                      <span className="hidden sm:block text-sm font-medium max-w-24 truncate">
                        {user?.firstName}
                      </span>
                    </button>

                    {/* Dropdown Menu */}
                    <div className="absolute right-0 mt-2 w-48 bg-white rounded-lg shadow-lg border opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200">
                      <div className="px-4 py-3 border-b">
                        <p className="text-sm font-semibold text-gray-900">
                          {user?.firstName} {user?.lastName}
                        </p>
                        <p className="text-xs text-gray-500 truncate">{user?.email}</p>
                      </div>
                      
                      <Link
                        to="/profile"
                        className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 flex items-center gap-2"
                      >
                        <User size={16} />
                        My Profile
                      </Link>
                      <Link
                        to="/orders"
                        className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-50"
                      >
                        My Orders
                      </Link>
                      <Link
                        to="/favorites"
                        className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 sm:hidden"
                      >
                        Favorites
                      </Link>
                      <Link
                        to="/addresses"
                        className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-50"
                      >
                        Addresses
                      </Link>
                      <button
                        onClick={handleLogout}
                        className="w-full text-left px-4 py-2 text-sm text-red-600 hover:bg-gray-50 flex items-center gap-2 border-t rounded-b-lg"
                      >
                        <LogOut size={16} />
                        Logout
                      </button>
                    </div>
                  </div>
                </>
              ) : (
                <Button onClick={openLogin}>Sign In</Button>
              )}

              {/* Mobile Menu Toggle */}
              <button
                onClick={() => setShowMobileMenu(!showMobileMenu)}
                className="md:hidden p-2 hover:bg-gray-100 rounded-full"
                aria-label="Menu"
              >
                {showMobileMenu ? <X size={24} /> : <Menu size={24} />}
              </button>
            </div>
          </div>

          {/* Search Bar - Mobile */}
          <div className="md:hidden pb-4">
            <SearchBar />
          </div>

          {/* Mobile Menu */}
          {showMobileMenu && (
            <div className="md:hidden py-4 border-t">
              <nav className="space-y-2">
                <Link
                  to="/"
                  className="block px-4 py-2 text-gray-700 hover:bg-gray-50 rounded"
                  onClick={() => setShowMobileMenu(false)}
                >
                  Home
                </Link>
                {isAuthenticated && (
                  <>
                    <Link
                      to="/orders"
                      className="block px-4 py-2 text-gray-700 hover:bg-gray-50 rounded"
                      onClick={() => setShowMobileMenu(false)}
                    >
                      My Orders
                    </Link>
                    <Link
                      to="/favorites"
                      className="block px-4 py-2 text-gray-700 hover:bg-gray-50 rounded"
                      onClick={() => setShowMobileMenu(false)}
                    >
                      Favorites
                    </Link>
                    <Link
                      to="/profile"
                      className="block px-4 py-2 text-gray-700 hover:bg-gray-50 rounded"
                      onClick={() => setShowMobileMenu(false)}
                    >
                      Profile
                    </Link>
                  </>
                )}
              </nav>
            </div>
          )}
        </div>
      </header>

      {/* Auth Modals */}
      <LoginModal 
        isOpen={showLogin} 
        onClose={closeAll} 
        onSwitchToRegister={switchToRegister}
      />
      <RegisterModal 
        isOpen={showRegister} 
        onClose={closeAll} 
        onSwitchToLogin={switchToLogin}
      />
    </>
  );
}
