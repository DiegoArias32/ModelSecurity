
const API_URL = 'http://localhost:5163/api';

const getToken = () => localStorage.getItem('token');
const setToken = (token) => localStorage.setItem('token', token);
const removeToken = () => localStorage.removeItem('token');


const isAuthenticated = () => {
  const token = getToken();
  return !!token;
};


const getCurrentUser = () => {
  const token = getToken();
  if (!token) return null;
  
  try {

    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => {
      return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));
    
    return JSON.parse(jsonPayload);
  } catch (error) {
    console.error('Error al decodificar el token', error);
    removeToken();
    return null;
  }
};


const isAdmin = () => {
  const user = getCurrentUser();
  return user && user.role === 'Admin';
};


const getHeaders = () => {
  const headers = {
    'Content-Type': 'application/json'
  };
  
  const token = getToken();
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }
  
  return headers;
};


const fetchApi = async (endpoint, options = {}) => {
  const url = `${API_URL}/${endpoint}`;
  
  const defaultOptions = {
    headers: getHeaders()
  };
  
  const fetchOptions = {
    ...defaultOptions,
    ...options
  };
  
  try {
    const response = await fetch(url, fetchOptions);
    

    if (response.status === 401) {
      removeToken();
      window.location.href = '../html/login.html';
      throw new Error('Sesión expirada o inválida');
    }

    if (!response.ok) {

      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || `Error ${response.status}: ${response.statusText}`);
    }
    

    if (response.status === 204) {
      return { success: true };
    }

    return await response.json();
  } catch (error) {
    console.error(`Error en la petición a ${endpoint}:`, error);
    throw error;
  }
};


const api = {

  login: async (credentials) => {
    const response = await fetchApi('login', {
      method: 'POST',
      body: JSON.stringify(credentials)
    });
    
    if (response.token) {
      setToken(response.token);
    }
    
    return response;
  },
  
  logout: () => {
    removeToken();
  },
  

  getAll: async (entity) => {
    return await fetchApi(`${entity}`);
  },
  
  getById: async (entity, id) => {
    return await fetchApi(`${entity}/${id}`);
  },
  
  create: async (entity, data) => {
    return await fetchApi(`${entity}`, {
      method: 'POST',
      body: JSON.stringify(data)
    });
  },
  
  update: async (entity, data) => {
    return await fetchApi(`${entity}`, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  },
  
  patch: async (entity, id, operations) => {
    return await fetchApi(`${entity}/${id}`, {
      method: 'PATCH',
      body: JSON.stringify(operations)
    });
  },
  
  delete: async (entity, id) => {
    return await fetchApi(`${entity}/${id}`, {
      method: 'DELETE'
    });
  },
  

  permanentDelete: async (entity, id) => {
    if (!isAdmin()) {
      throw new Error('Solo los administradores pueden realizar eliminación permanente');
    }
    
    return await fetchApi(`${entity}/permanent/${id}`, {
      method: 'DELETE'
    });
  }
};

export default api;
export { isAuthenticated, isAdmin, getCurrentUser };