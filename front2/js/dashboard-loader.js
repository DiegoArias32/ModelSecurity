/**
 * Este archivo se encarga de cargar e inicializar todos los componentes 
 * del dashboard, asegurando la integración de los diferentes módulos.
 */
document.addEventListener('DOMContentLoaded', function() {
    // Verificar que estamos en la página del dashboard
    if (!document.querySelector('.dashboard-container')) return;
    
    // Inicializar roles y permisos básicos si no existen
    initializeRolesAndPermissions();
    
    // Cargar componentes adicionales dinámicamente según sea necesario
    loadScripts();
    
    /**
     * Verifica y crea los roles y permisos básicos si no existen
     */
    async function initializeRolesAndPermissions() {
        try {
            // Obtener roles existentes
            const roles = await apiRequest(API_CONFIG.ENDPOINTS.ROL);
            
            // Verificar si se necesita crear roles básicos
            let userRoleExists = false;
            let adminRoleExists = false;
            
            // IDs actuales configurados
            let userRoleId = API_CONFIG.ROLES.USER;
            let adminRoleId = API_CONFIG.ROLES.ADMIN;
            
            // Verificar roles existentes
            if (roles && roles.length > 0) {
                userRoleExists = roles.some(r => r.name === 'Usuario');
                adminRoleExists = roles.some(r => r.name === 'Administrador');
                
                // Actualizar IDs si los roles existen pero con diferentes IDs
                for (const role of roles) {
                    if (role.name === 'Usuario') {
                        userRoleId = role.id;
                    }
                    if (role.name === 'Administrador') {
                        adminRoleId = role.id;
                    }
                }
            }
            
            // Crear rol de Usuario si no existe
            if (!userRoleExists) {
                console.log('Creando rol de Usuario...');
                const userRoleData = {
                    Name: 'Usuario',
                    Description: 'Usuario normal del sistema con acceso limitado'
                };
                
                try {
                    const createdUserRole = await apiRequest(API_CONFIG.ENDPOINTS.ROL, 'POST', userRoleData);
                    if (createdUserRole) {
                        userRoleId = createdUserRole.id;
                        console.log('Rol de Usuario creado con ID:', userRoleId);
                    }
                } catch (error) {
                    console.error('Error al crear rol de Usuario:', error);
                }
            }
            
            // Crear rol de Administrador si no existe
            if (!adminRoleExists) {
                console.log('Creando rol de Administrador...');
                const adminRoleData = {
                    Name: 'Administrador',
                    Description: 'Administrador del sistema con acceso completo'
                };
                
                try {
                    const createdAdminRole = await apiRequest(API_CONFIG.ENDPOINTS.ROL, 'POST', adminRoleData);
                    if (createdAdminRole) {
                        adminRoleId = createdAdminRole.id;
                        console.log('Rol de Administrador creado con ID:', adminRoleId);
                    }
                } catch (error) {
                    console.error('Error al crear rol de Administrador:', error);
                }
            }
            
            // Actualizar configuración con los IDs correctos
            API_CONFIG.ROLES.USER = userRoleId;
            API_CONFIG.ROLES.ADMIN = adminRoleId;
            
            console.log('Configuración de roles actualizada:', API_CONFIG.ROLES);
            
            // Guardar configuración de roles para uso en toda la aplicación
            saveToStorage('role_config', {
                USER_ROLE_ID: userRoleId,
                ADMIN_ROLE_ID: adminRoleId
            });
            
            // Crear permisos básicos si se necesitan
            await initializeBasicPermissions();
            
        } catch (error) {
            console.error('Error al inicializar roles:', error);
        }
    }
    
    /**
     * Inicializa permisos básicos para el sistema
     */
    async function initializeBasicPermissions() {
        try {
            // Obtener permisos existentes
            const permissions = await apiRequest(API_CONFIG.ENDPOINTS.PERMISSION);
            
            // Verificar si hay necesidad de crear permisos básicos
            if (!permissions || permissions.length === 0) {
                console.log('Creando permisos básicos...');
                
                // Permiso solo lectura
                const readOnlyPermission = {
                    CanRead: true,
                    CanCreate: false,
                    CanUpdate: false,
                    CanDelete: false
                };
                
                // Permiso completo
                const fullPermission = {
                    CanRead: true,
                    CanCreate: true,
                    CanUpdate: true,
                    CanDelete: true
                };
                
                // Crear permiso de solo lectura
                try {
                    const createdReadOnlyPermission = await apiRequest(API_CONFIG.ENDPOINTS.PERMISSION, 'POST', readOnlyPermission);
                    console.log('Permiso de solo lectura creado con ID:', createdReadOnlyPermission.id);
                } catch (error) {
                    console.error('Error al crear permiso de solo lectura:', error);
                }
                
                // Crear permiso completo
                try {
                    const createdFullPermission = await apiRequest(API_CONFIG.ENDPOINTS.PERMISSION, 'POST', fullPermission);
                    console.log('Permiso completo creado con ID:', createdFullPermission.id);
                } catch (error) {
                    console.error('Error al crear permiso completo:', error);
                }
            }
        } catch (error) {
            console.error('Error al inicializar permisos:', error);
        }
    }
    
    /**
     * Carga scripts adicionales para el dashboard
     */
    function loadScripts() {
        // Cargar script principal del dashboard si aún no se ha cargado
        if (!window.dashboardLoaded) {
            loadScript('../js/dashboard.js', function() {
                window.dashboardLoaded = true;
            });
        }
        
        // Cargar scripts adicionales solo si estamos logueados como administrador
        if (isAdmin()) {
            // Cargar script de módulos y formularios
            loadScript('../js/forms-modules.js');
            
            // Cargar script de permisos
            loadScript('../js/permissions.js');
        }
    }
    
    /**
     * Función auxiliar para cargar un script dinámicamente
     * @param {string} src - Ruta al archivo JavaScript
     * @param {Function} callback - Función a llamar cuando el script se cargue
     */
    function loadScript(src, callback) {
        const script = document.createElement('script');
        script.src = src;
        script.async = true;
        
        if (callback) {
            script.onload = callback;
        }
        
        document.head.appendChild(script);
    }
    
    // Crear interceptor para manejar el borrado lógico y permanente
    setupApiDeleteInterceptor();
    
    /**
     * Configura un interceptor para asegurar que las operaciones de borrado
     * usan el endpoint correcto según sea borrado lógico o permanente
     */
    function setupApiDeleteInterceptor() {
        // Guardar referencia a la función original
        const originalApiRequest = window.apiRequest;
        
        // Redefinir apiRequest para interceptar las operaciones DELETE
        window.apiRequest = function(endpoint, method = 'GET', data = null) {
            // Solo interceptar DELETE
            if (method === 'DELETE' && endpoint.includes('/permanent/')) {
                console.log('Interceptando DELETE permanente:', endpoint);
                
                // La operación ya está configurada correctamente para borrado permanente
                return originalApiRequest(endpoint, method, data);
            }
            
            // Mantener comportamiento original para todas las demás solicitudes
            return originalApiRequest(endpoint, method, data);
        };
    }
});