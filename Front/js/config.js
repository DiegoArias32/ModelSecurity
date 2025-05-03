/**
 * Configuración global para la API
 */
const API_CONFIG = {
    // URL base de la API (ajustada al puerto correcto)
    BASE_URL: 'http://localhost:5163/api',
    
    // Endpoints
    ENDPOINTS: {
        // Endpoints de usuarios
        USER: '/User',
        
        // Endpoints de login
        LOGIN: '/Login',
        WORKER_LOGIN: '/WorkerLogin',
        
        // Endpoints de roles
        ROL: '/Rol',
        
        // Endpoints de permisos
        PERMISSION: '/Permission',
        
        // Endpoints de rol-usuario
        ROL_USER: '/RolUser',
        
        // Endpoints de rol-formulario-permiso
        ROL_FORM_PERMISSION: '/RolFormPermission',
        
        // Endpoints de módulos
        MODULE: '/Module',
        
        // Endpoints de formularios
        FORM: '/Form',
        
        // Endpoints de módulo-formulario
        FORM_MODULE: '/FormModule',
        
        // Endpoints de trabajadores
        WORKER: '/Worker',
        
        // Endpoints de clientes
        CLIENT: '/Client',
        
        // Endpoints de PQR
        PQR: '/Pqr'
    },
    
    // IDs de roles (se actualizarán dinámicamente con los valores reales de la BD)
    ROLES: {
        USER: 1,  // Valor predeterminado, se actualizará dinámicamente
        ADMIN: 2  // Valor predeterminado, se actualizará dinámicamente
    }
};

/**
 * Función para guardar datos en localStorage
 */
function saveToStorage(key, data) {
    localStorage.setItem(key, JSON.stringify(data));
}

/**
 * Función para obtener datos de localStorage
 */
function getFromStorage(key) {
    const data = localStorage.getItem(key);
    return data ? JSON.parse(data) : null;
}

/**
 * Función para eliminar datos de localStorage
 */
function removeFromStorage(key) {
    localStorage.removeItem(key);
}

/**
 * Función para comprobar si el usuario está autenticado
 */
function isAuthenticated() {
    return !!getFromStorage('user');
}

/**
 * Función para comprobar si el usuario es administrador
 */
function isAdmin() {
    const user = getFromStorage('user');
    return user && user.rolId === API_CONFIG.ROLES.ADMIN;
}

/**
 * Función para redirigir a la página de login si no está autenticado
 */
function requireAuth() {
    if (!isAuthenticated()) {
        window.location.href = 'login.html';
    }
}

/**
 * Función para realizar una petición a la API
 */
async function apiRequest(endpoint, method = 'GET', data = null) {
    try {
        const url = API_CONFIG.BASE_URL + endpoint;
        
        // Log para depuración
        console.log(`Realizando solicitud ${method} a ${url}`);
        if (data) {
            console.log('Datos enviados:', data);
        }
        
        const options = {
            method,
            headers: {
                'Content-Type': 'application/json'
            }
        };
        
        // Agregar token de autenticación si existe
        const user = getFromStorage('user');
        if (user && user.token) {
            options.headers['Authorization'] = `Bearer ${user.token}`;
        }
        
        // Agregar datos si existen
        if (data) {
            options.body = JSON.stringify(data);
        }
        
        const response = await fetch(url, options);
        
        // Log para depuración
        console.log(`Respuesta recibida: ${response.status} ${response.statusText}`);
        
        // Si la respuesta no es exitosa, lanzar un error
        if (!response.ok) {
            let errorMessage = `Error ${response.status}: ${response.statusText}`;
            
            try {
                // Intentar obtener mensaje de error detallado
                const errorData = await response.json();
                console.log('Datos de error:', errorData);
                
                if (errorData.message) {
                    errorMessage = errorData.message;
                } else if (typeof errorData === 'string') {
                    errorMessage = errorData;
                } else if (errorData.errors) {
                    // Extraer errores de validación
                    const validationErrors = Object.values(errorData.errors).flat();
                    errorMessage = validationErrors.join('. ');
                }
            } catch (parseError) {
                // Si no se puede parsear como JSON, intentar obtener el texto
                try {
                    const errorText = await response.text();
                    if (errorText) {
                        errorMessage = errorText;
                    }
                    console.log('Texto de error:', errorText);
                } catch (textError) {
                    console.log('Error al leer el texto de error:', textError);
                }
            }
            
            throw new Error(errorMessage);
        }
        
        // Si la respuesta es 204 (No Content), retornar true
        if (response.status === 204) {
            return true;
        }
        
        // Si la respuesta es vacía, retornar null
        const text = await response.text();
        if (!text) {
            return null;
        }
        
        // Parsear la respuesta como JSON
        const result = JSON.parse(text);
        console.log('Datos recibidos:', result);
        return result;
    } catch (error) {
        console.error('Error en la petición a la API:', error);
        throw error;
    }
}

/**
 * Función para realizar una petición PATCH a la API con JsonPatch
 */
async function apiPatchRequest(endpoint, patchDocument) {
    try {
        const url = API_CONFIG.BASE_URL + endpoint;
        
        const options = {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json-patch+json'
            },
            body: JSON.stringify(patchDocument)
        };
        
        // Agregar token de autenticación si existe
        const user = getFromStorage('user');
        if (user && user.token) {
            options.headers['Authorization'] = `Bearer ${user.token}`;
        }
        
        const response = await fetch(url, options);
        
        // Si la respuesta no es exitosa, lanzar un error
        if (!response.ok) {
            let errorMessage = `Error ${response.status}: ${response.statusText}`;
            
            try {
                // Intentar obtener mensaje de error detallado
                const errorData = await response.json();
                
                if (errorData.message) {
                    errorMessage = errorData.message;
                } else if (typeof errorData === 'string') {
                    errorMessage = errorData;
                } else if (errorData.errors) {
                    // Extraer errores de validación
                    const validationErrors = Object.values(errorData.errors).flat();
                    errorMessage = validationErrors.join('. ');
                }
            } catch (parseError) {
                // Si no se puede parsear como JSON, intentar obtener el texto
                try {
                    const errorText = await response.text();
                    if (errorText) {
                        errorMessage = errorText;
                    }
                } catch (textError) {
                    console.log('Error al leer el texto de error:', textError);
                }
            }
            
            throw new Error(errorMessage);
        }
        
        // Si la respuesta es 204 (No Content), retornar true
        if (response.status === 204) {
            return true;
        }
        
        // Si la respuesta es vacía, retornar null
        const text = await response.text();
        if (!text) {
            return null;
        }
        
        // Parsear la respuesta como JSON
        return JSON.parse(text);
    } catch (error) {
        console.error('Error en la petición PATCH a la API:', error);
        throw error;
    }
}

// Cargar configuración de roles guardada si existe
(function loadRoleConfig() {
    const savedRoleConfig = getFromStorage('role_config');
    if (savedRoleConfig) {
        if (savedRoleConfig.USER_ROLE_ID) {
            API_CONFIG.ROLES.USER = savedRoleConfig.USER_ROLE_ID;
        }
        if (savedRoleConfig.ADMIN_ROLE_ID) {
            API_CONFIG.ROLES.ADMIN = savedRoleConfig.ADMIN_ROLE_ID;
        }
        console.log('Configuración de roles cargada:', API_CONFIG.ROLES);
    }
})();

// Mostrar mensajes en la interfaz
function showMessage(elementId, message, type) {
    const messageElement = document.getElementById(elementId);
    if (!messageElement) return;
    
    messageElement.textContent = message;
    messageElement.className = `message ${type}`;
    
    // Desaparecer el mensaje después de 5 segundos
    setTimeout(() => {
        messageElement.textContent = '';
        messageElement.className = 'message';
    }, 5000);
}

// Función para convertir el primer carácter a mayúscula
function capitalizeFirstLetter(string) {
    if (!string) return '';
    return string.charAt(0).toUpperCase() + string.slice(1);
}

// Función para formatear fechas
function formatDate(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString();
}

