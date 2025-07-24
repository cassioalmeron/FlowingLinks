const USER_KEY = 'user';

interface User {
    name: string;
    isAdmin: boolean;
    token: string;
}

export const session = {
    getToken: () => {
        const user = session.getUser();
        return user?.token ?? null;
    },
    getUser: () => {
        const user = localStorage.getItem(USER_KEY);
        if (user) {
            return JSON.parse(user) as User;
        }
        return null;
    },
    isAuthenticated: () => {
        return localStorage.getItem(USER_KEY) !== null;
    },
    logout: () => {
        localStorage.removeItem(USER_KEY);
    },
    login: (user: User) => {
        localStorage.setItem(USER_KEY, JSON.stringify(user));
    }
}