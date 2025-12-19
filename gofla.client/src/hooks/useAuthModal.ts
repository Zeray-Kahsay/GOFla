import { useState } from "react";

interface UseAuthModalReturn {
  showLogin: boolean;
  showRegister: boolean;
  openLogin: () => void;
  openRegister: () => void;
  closeAll: () => void;
  switchToRegister: () => void;
  switchToLogin: () => void;
}


export function useAuthModal(): UseAuthModalReturn {
  const [showLogin, setShowLogin] = useState(false);
  const [showRegister, setShowRegister] = useState(false);

  const openLogin = () => {
    setShowRegister(false);
    setShowLogin(true);
  };

  const openRegister = () => {
    setShowLogin(false);
    setShowRegister(true);
  };

  const closeAll = () => {
    setShowLogin(false);
    setShowRegister(false);
  };

  const switchToRegister = () => {
    setShowLogin(false);
    setShowRegister(true);
  };

  const switchToLogin = () => {
    setShowRegister(false);
    setShowLogin(true);
  };

  return {
    showLogin,
    showRegister,
    openLogin,
    openRegister,
    closeAll,
    switchToRegister,
    switchToLogin,
  };
}