// API service para manejar todas las solicitudes HTTP
const API_BASE_URL = 'http://172.30.7.157:5163/api';

const api = {
    // Función auxiliar para realizar solicitudes
    async fetchData(endpoint, options = {}) {
        // Obtener token de autenticación si existe
        const token = localStorage.getItem('token');
        
        // Configurar headers por defecto
        const headers = {
            'Content-Type': 'application/json',
            ...options.headers
        };
        
        // Añadir token de autorización si existe
        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }
        
        try {
            const response = await fetch(`${API_BASE_URL}${endpoint}`, {
                ...options,
                headers
            });
            
            // Si la respuesta no es exitosa, lanzar un error
            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || `Error ${response.status}: ${response.statusText}`);
            }
            
            // Si la respuesta es exitosa, devolver los datos
            return await response.json();
        } catch (error) {
            console.error('API Error:', error);
            throw error;
        }
    },
    
    // Autenticación
    auth: {
        async login(username, password) {
            return api.fetchData('/Auth/login', {
                method: 'POST',
                body: JSON.stringify({ username, password })
            });
        },
        
        async logout() {
            localStorage.removeItem('token');
            localStorage.removeItem('user');
        }
    },
    
    // Usuarios
    users: {
        async getAll() {
            return api.fetchData('/User');
        },
        
        async getById(id) {
            return api.fetchData(`/User/${id}`);
        },
        
        async create(userData) {
            return api.fetchData('/User', {
                method: 'POST',
                body: JSON.stringify(userData)
            });
        },
        
        async update(userData) {
            return api.fetchData('/User', {
                method: 'PUT',
                body: JSON.stringify(userData)
            });
        },
        
        async delete(id) {
            return api.fetchData(`/User/${id}`, {
                method: 'DELETE'
            });
        },
        
        async permanentDelete(id) {
            return api.fetchData(`/User/permanent/${id}`, {
                method: 'DELETE'
            });
        }
    },
    
    // Roles
    roles: {
        async getAll() {
            return api.fetchData('/Rol');
        },
        
        async getById(id) {
            return api.fetchData(`/Rol/${id}`);
        },
        
        async create(rolData) {
            return api.fetchData('/Rol', {
                method: 'POST',
                body: JSON.stringify(rolData)
            });
        },
        
        async update(rolData) {
            return api.fetchData('/Rol', {
                method: 'PUT',
                body: JSON.stringify(rolData)
            });
        },
        
        async delete(id) {
            return api.fetchData(`/Rol/${id}`, {
                method: 'DELETE'
            });
        },
        
        async permanentDelete(id) {
            return api.fetchData(`/Rol/permanent/${id}`, {
                method: 'DELETE'
            });
        }
    },
    
    // Módulos
    modules: {
        async getAll() {
            return api.fetchData('/Module');
        },
        
        async getById(id) {
            return api.fetchData(`/Module/${id}`);
        },
        
        async create(moduleData) {
            return api.fetchData('/Module', {
                method: 'POST',
                body: JSON.stringify(moduleData)
            });
        },
        
        async update(moduleData) {
            return api.fetchData(`/Module/${id}`, {
                method: 'PUT',
                body: JSON.stringify(moduleData)
            });
        },
        
        async delete(id) {
            return api.fetchData(`/Module/${id}`, {
                method: 'DELETE'
            });
        },
        
        async permanentDelete(id) {
            return api.fetchData(`/Module/permanent/${id}`, {
                method: 'DELETE'
            });
        }
    },
    
    // Formularios
    forms: {
        async getAll() {
            return api.fetchData('/Form');
        },
        
        async getById(id) {
            return api.fetchData(`/Form/${id}`);
        },
        
        async create(formData) {
            return api.fetchData('/Form', {
                method: 'POST',
                body: JSON.stringify(formData)
            });
        },
        
        async update(formData) {
            return api.fetchData('/Form', {
                method: 'PUT',
                body: JSON.stringify(formData)
            });
        },
        
        async delete(id) {
            return api.fetchData(`/Form/${id}`, {
                method: 'DELETE'
            });
        },
        
        async permanentDelete(id) {
            return api.fetchData(`/Form/permanent/${id}`, {
                method: 'DELETE'
            });
        }
    },
    
    // Permisos
    permissions: {
        async getAll() {
            return api.fetchData('/Permission');
        },
        
        async getById(id) {
            return api.fetchData(`/Permission/${id}`);
        },
        
        async create(permissionData) {
            return api.fetchData('/Permission', {
                method: 'POST',
                body: JSON.stringify(permissionData)
            });
        },
        
        async update(permissionData) {
            return api.fetchData(`/Permission/${id}`, {
                method: 'PUT',
                body: JSON.stringify(permissionData)
            });
        },
        
        async delete(id) {
            return api.fetchData(`/Permission/${id}`, {
                method: 'DELETE'
            });
        },
        
        async permanentDelete(id) {
            return api.fetchData(`/Permission/permanent/${id}`, {
                method: 'DELETE'
            });
        }
    },
    
    // Relación RolFormPermission
    rolFormPermissions: {
        async getAll() {
            return api.fetchData('/RolFormPermission');
        },
        
        async getById(id) {
            return api.fetchData(`/RolFormPermission/${id}`);
        },
        
        async getByRolId(rolId) {
            return api.fetchData(`/RolFormPermission/rol/${rolId}`);
        },
        
        async create(data) {
            return api.fetchData('/RolFormPermission', {
                method: 'POST',
                body: JSON.stringify(data)
            });
        },
        
        async update(data) {
            return api.fetchData('/RolFormPermission', {
                method: 'PUT',
                body: JSON.stringify(data)
            });
        },
        
        async delete(id) {
            return api.fetchData(`/RolFormPermission/${id}`, {
                method: 'DELETE'
            });
        },
        
        async permanentDelete(id) {
            return api.fetchData(`/RolFormPermission/permanent/${id}`, {
                method: 'DELETE'
            });
        }
    },
    
    // Relación FormModule
    formModules: {
        async getAll() {
            return api.fetchData('/FormModule');
        },
        
        async getById(id) {
            return api.fetchData(`/FormModule/${id}`);
        },
        
        async create(data) {
            return api.fetchData('/FormModule', {
                method: 'POST',
                body: JSON.stringify(data)
            });
        },
        
        async update(data) {
            return api.fetchData('/FormModule', {
                method: 'PUT',
                body: JSON.stringify(data)
            });
        },
        
        async delete(id) {
            return api.fetchData(`/FormModule/${id}`, {
                method: 'DELETE'
            });
        },
        
        async permanentDelete(id) {
            return api.fetchData(`/FormModule/permanent/${id}`, {
                method: 'DELETE'
            });
        }
    },
    
    // Menú
    menu: {
        async getByRolId(rolId) {
            return api.fetchData(`/Menu/byrol/${rolId}`);
        }
    }
};