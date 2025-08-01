import axios from 'axios';
import { session } from './session';

/**
 * Token Expiration Management
 * 
 * This module provides comprehensive token expiration checking functionality:
 * 
 * 1. Automatic expiration checking in RequireAuth component
 * 2. API-level expiration checking before requests
 * 3. Periodic background checking (every 60 seconds by default)
 * 4. Utility functions for manual checking
 * 
 * Usage:
 * - The system automatically checks token expiration on route changes
 * - API calls automatically check expiration before making requests
 * - Use checkTokenExpiration() for manual checks
 * - Use useTokenExpiration() hook in React components
 * 
 * When a token expires:
 * - User is automatically logged out
 * - Session is cleared from localStorage
 * - User is redirected to /login
 */

const BASE_URL = import.meta.env.VITE_API_URL;

// Centralized function to generate headers with authorization
const getAuthHeaders = () => {
  // Check if token has expired before making request
  if (session.isTokenExpired()) {
    session.logout();
    throw new Error('Token has expired. Please login again.');
  }
  
  const token = session.getToken();
  return {
    Authorization: `Bearer ${token}`
  };
};

// Utility function to check token expiration
export const checkTokenExpiration = () => {
  if (session.isTokenExpired()) {
    session.logout();
    // Redirect to login page
    window.location.href = '/login';
    return false;
  }
  return true;
};

// React hook for token expiration checking (for use in components)
export const useTokenExpiration = () => {
  const checkExpiration = () => {
    if (session.isTokenExpired()) {
      session.logout();
      window.location.href = '/login';
      return false;
    }
    return true;
  };

  return { checkExpiration };
};

// Centralized error handler for API calls
const handleApiError = (error: unknown) => {
  if (error instanceof Error && error.message === 'Token has expired. Please login again.') {
    session.logout();
    window.location.href = '/login';
    return;
  }
  throw error;
};

// Types
export interface AuthResponse {
    name: string;
    isAdmin: boolean;
    token: string;
    expires: Date;
}

export interface User {
    id: number;
    name: string;
    username: string;
}

export interface Project {
    id: number;
    name: string;
}

export interface Label {
    id: number;
    name: string;
}

export interface Link {
    id: number;
    description: string;
    url: string;
    comments?: string;
    read: boolean;
    tags?: number[]; // Array of label IDs
    favorite?: boolean; // Whether the link is favorited
}

// API functions
export const api = {
  // Chat endpoints
  login: {
    // Get all chats
    auth: async (username: string, password: string): Promise<AuthResponse> => {
      const response = await axios.post<AuthResponse>(`${BASE_URL}/Auth`, {
        username,
        password
      });

      const data = response.data;
      
      const user = {
        name: data.name,
        isAdmin: data.isAdmin,
        token: data.token,
        expires: data.expires
      };

      session.login(user);

      return data;
    },
  },
  users: {
    list: async (): Promise<User[]> => {
      try {
        const response = await axios.get<User[]>(`${BASE_URL}/User`, {
          headers: getAuthHeaders()
        });
        return response.data;
      } catch (error) {
        handleApiError(error);
        throw error;
      }
    },
    delete: async (id: number): Promise<void> => {
      await axios.delete(`${BASE_URL}/User/${id}`, {
        headers: getAuthHeaders()
      });
    },
    create: async (user: Omit<User, 'id'>): Promise<User> => {
      const response = await axios.post<User>(`${BASE_URL}/User`, user, {
        headers: getAuthHeaders()
      });
      return response.data;
    },
    update: async (id: number, user: Omit<User, 'id'>): Promise<User> => {
      const response = await axios.put<User>(`${BASE_URL}/User/${id}`, user, {
        headers: getAuthHeaders()
      });
      return response.data;
    },
  },
  projects: {
    list: async (): Promise<Project[]> => {
      const response = await axios.get<Project[]>(`${BASE_URL}/Project`, {
        headers: getAuthHeaders()
      });
      return response.data;
    },
    delete: async (id: number): Promise<void> => {
      await axios.delete(`${BASE_URL}/Project/${id}`, {
        headers: getAuthHeaders()
      });
    },
    create: async (project: Omit<Project, 'id'>): Promise<Project> => {
      const response = await axios.post<Project>(`${BASE_URL}/Project`, project, {
        headers: getAuthHeaders()
      });
      return response.data;
    },
    update: async (id: number, project: Omit<Project, 'id'>): Promise<Project> => {
      const response = await axios.put<Project>(`${BASE_URL}/Project/${id}`, project, {
        headers: getAuthHeaders()
      });
      return response.data;
    },
  },
  labels: {
    list: async (): Promise<Label[]> => {
      const response = await axios.get<Label[]>(`${BASE_URL}/Label`, {
        headers: getAuthHeaders()
      });
      return response.data;
    },
    delete: async (id: number): Promise<void> => {
      await axios.delete(`${BASE_URL}/Label/${id}`, {
        headers: getAuthHeaders()
      });
    },
    create: async (label: Omit<Label, 'id'>): Promise<Label> => {
      const response = await axios.post<Label>(`${BASE_URL}/Label`, label, {
        headers: getAuthHeaders()
      });
      return response.data;
    },
    update: async (id: number, label: Omit<Label, 'id'>): Promise<Label> => {
      const response = await axios.put<Label>(`${BASE_URL}/Label/${id}`, label, {
        headers: getAuthHeaders()
      });
      return response.data;
    },
  },
  links: {
    list: async (): Promise<Link[]> => {
      const response = await axios.get<Link[]>(`${BASE_URL}/Link`, {
        headers: getAuthHeaders()
      });
      return response.data;
    },
    delete: async (id: number): Promise<void> => {
      await axios.delete(`${BASE_URL}/Link/${id}`, {
        headers: getAuthHeaders()
      });
    },
    create: async (link: Omit<Link, 'id'>): Promise<Link> => {
      const response = await axios.post<Link>(`${BASE_URL}/Link`, link, {
        headers: getAuthHeaders()
      });
      return response.data;
    },
    update: async (id: number, link: Omit<Link, 'id'>): Promise<Link> => {
      const response = await axios.put<Link>(`${BASE_URL}/Link/${id}`, link, {
        headers: getAuthHeaders()
      });
      return response.data;
    },
    toggleFavorite: async (id: number, favorite: boolean): Promise<void> => {
      await axios.patch(`${BASE_URL}/Link/${id}/favorite`, favorite, {
        headers: {
          ...getAuthHeaders(),
          'Content-Type': 'application/json'
        }
      });
    },
  },
  profile: {
    getProfile: async (): Promise<{ name: string; username: string }> => {
      const response = await axios.get(`${BASE_URL}/Profile`, {
        headers: getAuthHeaders()
      });
      return response.data;
    },
    updateProfile: async (data: { name: string; username: string }): Promise<void> => {
      await axios.put(`${BASE_URL}/Profile`, data, {
        headers: getAuthHeaders()
      });
    },
    changePassword: async (newPassword: string): Promise<void> => {
      await axios.put(`${BASE_URL}/Profile/Password`, { newPassword }, {
        headers: getAuthHeaders()
      });
    },
  },
};