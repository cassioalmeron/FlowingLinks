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

// Centralized API request functions
const apiRequest = {
  get: async <T>(url: string): Promise<T> => {
    try {
      const response = await axios.get<T>(url, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  },

  post: async <T>(url: string, data?: unknown): Promise<T> => {
    try {
      const response = await axios.post<T>(url, data, {
        headers: {
          ...getAuthHeaders(),
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  },

  put: async <T>(url: string, data?: unknown): Promise<T> => {
    try {
      const response = await axios.put<T>(url, data, {
        headers: {
          ...getAuthHeaders(),
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  },

  patch: async <T>(url: string, data?: unknown): Promise<T> => {
    try {
      const response = await axios.patch<T>(url, data, {
        headers: {
          ...getAuthHeaders(),
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  },

  delete: async (url: string): Promise<void> => {
    try {
      await axios.delete(url, {
        headers: getAuthHeaders()
      });
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  }
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
      return apiRequest.get<User[]>(`${BASE_URL}/User`);
    },
    delete: async (id: number): Promise<void> => {
      return apiRequest.delete(`${BASE_URL}/User/${id}`);
    },
    create: async (user: Omit<User, 'id'>): Promise<User> => {
      return apiRequest.post<User>(`${BASE_URL}/User`, user);
    },
    update: async (id: number, user: Omit<User, 'id'>): Promise<User> => {
      return apiRequest.put<User>(`${BASE_URL}/User/${id}`, user);
    },
  },
  projects: {
    list: async (): Promise<Project[]> => {
      return apiRequest.get<Project[]>(`${BASE_URL}/Project`);
    },
    delete: async (id: number): Promise<void> => {
      return apiRequest.delete(`${BASE_URL}/Project/${id}`);
    },
    create: async (project: Omit<Project, 'id'>): Promise<Project> => {
      return apiRequest.post<Project>(`${BASE_URL}/Project`, project);
    },
    update: async (id: number, project: Omit<Project, 'id'>): Promise<Project> => {
      return apiRequest.put<Project>(`${BASE_URL}/Project/${id}`, project);
    },
  },
  labels: {
    list: async (): Promise<Label[]> => {
      return apiRequest.get<Label[]>(`${BASE_URL}/Label`);
    },
    delete: async (id: number): Promise<void> => {
      return apiRequest.delete(`${BASE_URL}/Label/${id}`);
    },
    create: async (label: Omit<Label, 'id'>): Promise<Label> => {
      return apiRequest.post<Label>(`${BASE_URL}/Label`, label);
    },
    update: async (id: number, label: Omit<Label, 'id'>): Promise<Label> => {
      return apiRequest.put<Label>(`${BASE_URL}/Label/${id}`, label);
    },
  },
  links: {
    list: async (): Promise<Link[]> => {
      return apiRequest.get<Link[]>(`${BASE_URL}/Link`);
    },
    delete: async (id: number): Promise<void> => {
      return apiRequest.delete(`${BASE_URL}/Link/${id}`);
    },
    create: async (link: Omit<Link, 'id'>): Promise<Link> => {
      return apiRequest.post<Link>(`${BASE_URL}/Link`, link);
    },
    update: async (id: number, link: Omit<Link, 'id'>): Promise<Link> => {
      return apiRequest.put<Link>(`${BASE_URL}/Link/${id}`, link);
    },
    toggleFavorite: async (id: number, favorite: boolean): Promise<void> => {
      return apiRequest.patch<void>(`${BASE_URL}/Link/${id}/favorite`, favorite);
    },
    search: async (filters: {
      description?: string;
      labelIds?: number[];
      favorite?: number;
    }): Promise<Link[]> => {
      return apiRequest.post<Link[]>(`${BASE_URL}/Link/search`, filters);
    },
  },
  profile: {
    getProfile: async (): Promise<{ name: string; username: string }> => {
      return apiRequest.get<{ name: string; username: string }>(`${BASE_URL}/Profile`);
    },
    updateProfile: async (data: { name: string; username: string }): Promise<void> => {
      return apiRequest.put<void>(`${BASE_URL}/Profile`, data);
    },
    changePassword: async (newPassword: string): Promise<void> => {
      return apiRequest.put<void>(`${BASE_URL}/Profile/Password`, { newPassword });
    },
  },
};