/**
 * Este archivo contiene la funcionalidad específica para gestionar 
 * los permisos de los roles y su visualización en la interfaz.
 */
document.addEventListener('DOMContentLoaded', function() {
    // Verificar si el usuario está autenticado
    if (!isAuthenticated()) return;
    
    // Configurar eventos para el modal de edición de permisos
    setupEditRolePermissionsModal();
    
    // Configurar visualización mejorada de permisos en el dashboard
    enhancePermissionsDisplay();
    
    /**
     * Configura el modal para editar permisos de roles
     */
    function setupEditRolePermissionsModal() {
        // Cerrar modal al hacer clic en X
        const closeModalBtn = document.querySelector('#editRolePermissionsModal .close-modal');
        if (closeModalBtn) {
            closeModalBtn.addEventListener('click', () => {
                document.getElementById('editRolePermissionsModal').style.display = 'none';
            });
        }
        
        // Configurar eventos para el formulario de edición de permisos
        const editRolePermissionsForm = document.getElementById('editRolePermissionsForm');
        if (editRolePermissionsForm) {
            editRolePermissionsForm.addEventListener('submit', async (e) => {
                e.preventDefault();
                
                try {
                    // Los permisos individuales ya se guardan al cambiar los checkboxes
                    // Este evento solo cierra el modal y muestra un mensaje de confirmación
                    
                    // Cerrar el modal
                    document.getElementById('editRolePermissionsModal').style.display = 'none';
                    
                    // Mostrar mensaje de éxito
                    showMessage('roleMessage', 'Permisos del rol actualizados correctamente.', 'success');
                    
                    // Actualizar información de permisos en el dashboard si es el rol del usuario actual
                    const user = getFromStorage('user');
                    if (user && user.rolId === parseInt(document.getElementById('editRoleId').value)) {
                        loadRolePermissions();
                    }
                } catch (error) {
                    console.error('Error al guardar permisos del rol:', error);
                    showMessage('roleMessage', error.message || 'Error al guardar los permisos del rol', 'error');
                }
            });
        }
    }
    
    /**
     * Mejora la visualización de permisos en el dashboard
     */
    function enhancePermissionsDisplay() {
        // Si no estamos en la página de dashboard, salir
        if (!document.getElementById('rolePermissions')) return;
        
        // Obtener información del usuario
        const user = getFromStorage('user');
        if (!user || !user.rolId) return;
        
        // Actualizar estilos para la visualización de permisos
        addEnhancedPermissionsStyles();
        
        // Agregar botón para refrescar permisos
        const roleInfoCard = document.querySelector('.role-info');
        if (roleInfoCard) {
            const refreshBtn = document.createElement('button');
            refreshBtn.className = 'button primary refresh-permissions-btn';
            refreshBtn.textContent = 'Actualizar Permisos';
            refreshBtn.addEventListener('click', function() {
                if (typeof loadRolePermissions === 'function') {
                    loadRolePermissions();
                } else {
                    location.reload(); // Alternativa: recargar la página
                }
            });
            
            roleInfoCard.appendChild(refreshBtn);
        }
    }
})

/** 
 * Agrega estilos CSS para mejorar la visualización de permisos 
 */
function addEnhancedPermissionsStyles() {
    // Crear elemento de estilo
    const styleEl = document.createElement('style');
    styleEl.textContent = `
        .permissions-list {
            margin-top: 15px;
        }
        
        .permission-item {
            display: flex;
            flex-wrap: wrap;
            align-items: center;
            padding: 12px;
            margin-bottom: 10px;
            background-color: #f5f7fa;
            border-radius: 6px;
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
            transition: all 0.3s ease;
        }
        
        .permission-item:hover {
            background-color: #eef1f5;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.15);
        }
        
        .permission-item strong {
            flex: 1 0 100%;
            margin-bottom: 8px;
            font-size: 16px;
            color: #3f51b5;
        }
        
        .permission-item span {
            margin-right: 15px;
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 14px;
        }
        
        .permission-item .granted {
            background-color: rgba(76, 175, 80, 0.15);
            color: #2e7d32;
            font-weight: 600;
        }
        
        .permission-item .denied {
            background-color: rgba(244, 67, 54, 0.1);
            color: #d32f2f;
        }
        
        .role-description {
            margin-top: 20px;
            padding: 15px;
            background-color: #e8eaf6;
            border-radius: 6px;
        }
        
        .role-description h4 {
            margin-bottom: 10px;
            color: #303f9f;
        }
        
        .refresh-permissions-btn {
            margin-top: 15px;
            display: block;
            width: 100%;
        }
        
        /* Estilos para el modal de edición de permisos */
        #editRolePermissionsModal .modal-content {
            width: 80%;
            max-width: 900px;
        }
        
        #roleFormPermissionsTable {
            margin-top: 15px;
        }
        
        #roleFormPermissionsTable th, 
        #roleFormPermissionsTable td {
            text-align: center;
        }
        
        #roleFormPermissionsTable td:first-child {
            text-align: left;
        }
        
        #roleFormPermissionsTable input[type="checkbox"] {
            width: 18px;
            height: 18px;
        }
    `;
    
    // Agregar al DOM
    document.head.appendChild(styleEl);
}

/**
 * Verifica si el usuario tiene un permiso específico
 * @param {string} formCode - Código del formulario
 * @param {string} permission - Tipo de permiso (read, create, update, delete)
 * @returns {boolean} - True si tiene el permiso, false en caso contrario
 */
function userHasPermission(formCode, permission) {
    // Si no hay permisos cargados, asumir que no tiene permiso
    if (!window.userPermissions) return false;
    
    // Si es administrador, asumir que tiene todos los permisos
    const user = getFromStorage('user');
    if (user && isAdmin()) return true;
    
    // Buscar el formulario por código
    const formPermission = window.userPermissions.find(p => p.formCode === formCode);
    if (!formPermission) return false;
    
    // Verificar el tipo de permiso
    switch(permission.toLowerCase()) {
        case 'read':
            return formPermission.canRead;
        case 'create':
            return formPermission.canCreate;
        case 'update':
            return formPermission.canUpdate;
        case 'delete':
            return formPermission.canDelete;
        default:
            return false;
    }
}

/**
 * Muestra u oculta elementos según los permisos del usuario
 * @param {string} selector - Selector CSS para los elementos
 * @param {string} formCode - Código del formulario
 * @param {string} permission - Tipo de permiso requerido
 */
function toggleElementsByPermission(selector, formCode, permission) {
    const hasPermission = userHasPermission(formCode, permission);
    const elements = document.querySelectorAll(selector);
    
    elements.forEach(el => {
        el.style.display = hasPermission ? '' : 'none';
    });
    
    return hasPermission;
}

/**
 * Agrega información visual sobre permisos faltantes
 * @param {Element} container - Elemento contenedor donde agregar la información
 * @param {string} formCode - Código del formulario
 * @param {string} permission - Tipo de permiso faltante
 * @param {string} message - Mensaje a mostrar
 */
function addMissingPermissionInfo(container, formCode, permission, message) {
    if (userHasPermission(formCode, permission)) return;
    
    const infoElement = document.createElement('div');
    infoElement.className = 'missing-permission-info';
    infoElement.innerHTML = `
        <p>${message || `No tienes permiso de ${permission} para este formulario.`}</p>
    `;
    
    container.appendChild(infoElement);
}

/**
 * Carga permisos disponibles para mostrar en un select
 * @param {string} selectId - ID del elemento select
 * @param {number} selectedId - ID del permiso seleccionado
 */
async function loadPermissionsForSelect(selectId, selectedId = null) {
    try {
        const permissions = await apiRequest(API_CONFIG.ENDPOINTS.PERMISSION);
        
        if (!permissions || permissions.length === 0) {
            console.warn('No se encontraron permisos');
            return;
        }
        
        const selectElement = document.getElementById(selectId);
        if (!selectElement) return;
        
        // Limpiar opciones existentes
        selectElement.innerHTML = '<option value="">Seleccione un permiso</option>';
        
        // Agregar opciones
        permissions.forEach(permission => {
            const permDesc = [];
            if (permission.canRead) permDesc.push('Lectura');
            if (permission.canCreate) permDesc.push('Creación');
            if (permission.canUpdate) permDesc.push('Actualización');
            if (permission.canDelete) permDesc.push('Eliminación');
            
            const description = permDesc.length > 0 ? permDesc.join(', ') : 'Sin permisos';
            
            const option = document.createElement('option');
            option.value = permission.id;
            option.textContent = `ID: ${permission.id} - ${description}`;
            
            if (selectedId && permission.id === selectedId) {
                option.selected = true;
            }
            
            selectElement.appendChild(option);
        });
    } catch (error) {
        console.error('Error al cargar permisos para select:', error);
    }
}

// Exportar funciones para uso global
window.userHasPermission = userHasPermission;
window.toggleElementsByPermission = toggleElementsByPermission;
window.addMissingPermissionInfo = addMissingPermissionInfo;
window.loadPermissionsForSelect = loadPermissionsForSelect;