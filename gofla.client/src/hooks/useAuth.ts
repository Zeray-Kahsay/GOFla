import { useAppSelector } from "../app/store/store";

export function useAuth(){
    const {user, isAuthenticated, token} = useAppSelector(state => state.auth);

    return {
        user,
        isAuthenticated,
        token,
        isGuest: !isAuthenticated,
        userId: user?.id,
        userFullName: user ? `${user.firstName} ${user.lastName}` : null,
    }
}