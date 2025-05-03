/**
 * Este archivo implementa la gestión de permisos y asignación de roles
 */
document.addEventListener('DOMContentLoaded', function() {
    // Verificar si el usuario está autenticado
    requireAuth();
    
    // Solo continuar si es administrador
    if (!isAdmin()) return;
    
    // Verificar que estamos en la página correcta
    if (!document.getElementById('permissionsSection')) {
        // Crear sección si no existe
        createPermissionsSection();
    }
    
    // Agregar elemento al menú si no existe
    addPermissionsMenuItem();
    
    // Cargar datos iniciales
    loadPermissionsData();
    loadRolFormPermissionsData();
    
    /**
     * Crea la sección de permisos en el DOM
     */
    function createPermissionsSection() {
        const permissionsSection = document.createElement('div');
        permissionsSection.id = 'permissionsSection';
        permissionsSection.className = 'section-content hidden';
        
        permissionsSection.innerHTML = `
            <div class="table-container">
                <div class="table-header">
                    <h3>Lista de Permisos</h3>
                    <button id="addPermissionBtn" class="button primary">Agregar Permiso</button>
                </div>
                
                <table class="data-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Lectura</th>
                            <th>Creación</th>
                            <th>Actualización</th>
                            <th>Eliminación</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody id="permissionsTableBody">
                        <!-- Se cargará dinámicamente -->
                    </tbody>
                </table>
            </div>
            
            <div class="table-container">
                <div class="table-header">
                    <h3>Asignación de Permisos a Roles y Formularios</h3>
                    <button id="addRolFormPermissionBtn" class="button primary">Asignar Permiso</button>
                </div>
                
                <table class="data-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Rol</th>
                            <th>Formulario</th>
                            <th>Permisos</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody id="rolFormPermissionsTableBody">
                        <!-- Se cargará dinámicamente -->
                    </tbody>
                </table>
            </div>
        `;
        
        document.querySelector('.content').appendChild(permissionsSection);
        
        // Agregar eventos a los botones
        document.getElementById('addPermissionBtn').addEventListener('click', showAddPermissionModal);
        document.getElementById('addRolFormPermissionBtn').addEventListener('click', showAddRolFormPermissionModal);
    }
    
    /**
     * Agrega el elemento de menú para acceder a la sección de permisos
     */
    function addPermissionsMenuItem() {
        const menu = document.querySelector('.menu');
        if (!menu) return;
        
        if (!document.querySelector('a[data-section="permissions"]')) {
            const permissionsItem = document.createElement('li');
            permissionsItem.classList.add('admin-only');
            permissionsItem.innerHTML = '<a href="#" data-section="permissions">Permisos</a>';
            menu.appendChild(permissionsItem);
            
            // Actualizar los eventos para cambiar entre secciones
            document.querySelectorAll('.menu a[data-section]').forEach(link => {
                link.addEventListener('click', function(e) {
                    e.preventDefault();
                    
                    // Obtener la sección a mostrar
                    const sectionToShow = this.getAttribute('data-section');
                    
                    // Actualizar clases activas en menú
                    document.querySelectorAll('.menu a').forEach(l => l.classList.remove('active'));
                    this.classList.add('active');
                    
                    // Actualizar título de la sección
                    document.getElementById('sectionTitle').textContent = this.textContent;
                    
                    // Ocultar todas las secciones
                    document.querySelectorAll('.section-content').forEach(section => section.classList.add('hidden'));
                    
                    // Mostrar la sección seleccionada
                    document.getElementById(sectionToShow + 'Section').classList.remove('hidden');
                });
            });
        }
    }
    
    /**
     * Carga la lista de permisos
     */
    async function loadPermissionsData() {
        try {
            const permissions = await apiRequest(API_CONFIG.ENDPOINTS.PERMISSION);
            
            const permissionsTableBody = document.getElementById('permissionsTableBody');
            
            if (!permissions || permissions.length === 0) {
                permissionsTableBody.innerHTML = '<tr><td colspan="6">No hay permisos registrados.</td></tr>';
                return;
            }
            
            permissionsTableBody.innerHTML = '';
            
            permissions.forEach(permission => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${permission.id}</td>
                    <td>${permission.canRead ? 'Sí' : 'No'}</td>
                    <td>${permission.canCreate ? 'Sí' : 'No'}</td>
                    <td>${permission.canUpdate ? 'Sí' : 'No'}</td>
                    <td>${permission.canDelete ? 'Sí' : 'No'}</td>
                    <td>
                        <div class="action-buttons">
                            <button class="button primary btn-edit-permission" data-id="${permission.id}">Editar</button>
                            <button class="button danger btn-delete-permission" data-id="${permission.id}">Eliminar</button>
                            <button class="button danger btn-permanent-delete-permission" data-id="${permission.id}">Eliminar Permanente</button>
                        </div>
                    </td>
                `;
                permissionsTableBody.appendChild(row);
            });
            
            // Agregar eventos a los botones
            document.querySelectorAll('.btn-edit-permission').forEach(btn => {
                btn.addEventListener('click', handleEditPermission);
            });
            
            document.querySelectorAll('.btn-delete-permission').forEach(btn => {
                btn.addEventListener('click', handleDeletePermission);
            });
            
            document.querySelectorAll('.btn-permanent-delete-permission').forEach(btn => {
                btn.addEventListener('click', handlePermanentDeletePermission);
            });
            
        } catch (error) {
            console.error('Error al cargar permisos:', error);
            document.getElementById('permissionsTableBody').innerHTML = '<tr><td colspan="6">Error al cargar permisos.</td></tr>';
        }
    }
    
    /**
     * Carga la lista de asignaciones de permisos a roles y formularios
     */
    async function loadRolFormPermissionsData() {
        try {
            const rolFormPermissions = await apiRequest(API_CONFIG.ENDPOINTS.ROL_FORM_PERMISSION);
            
            const rolFormPermissionsTableBody = document.getElementById('rolFormPermissionsTableBody');
            
            if (!rolFormPermissions || rolFormPermissions.length === 0) {
                rolFormPermissionsTableBody.innerHTML = '<tr><td colspan="5">No hay asignaciones de permisos registradas.</td></tr>';
                return;
            }
            
            // Obtener roles, formularios y permisos para mostrar nombres en lugar de IDs
            const [roles, forms, permissions] = await Promise.all([
                apiRequest(API_CONFIG.ENDPOINTS.ROL),
                apiRequest(API_CONFIG.ENDPOINTS.FORM),
                apiRequest(API_CONFIG.ENDPOINTS.PERMISSION)
            ]);
            
            // Crear mapas para búsqueda rápida
            const roleMap = new Map(roles.map(role => [role.id, role]));
            const formMap = new Map(forms.map(form => [form.id, form]));
            const permissionMap = new Map(permissions.map(permission => [permission.id, permission]));
            
            rolFormPermissionsTableBody.innerHTML = '';
            
            for (const rolFormPermission of rolFormPermissions) {
                const role = roleMap.get(rolFormPermission.rolId);
                const form = formMap.get(rolFormPermission.formId);
                const permission = permissionMap.get(rolFormPermission.permissionId);
                
                const row = document.createElement('tr');
                
                // Preparar una lista de permisos concedidos
                let permissionsText = '';
                if (permission) {
                    const permissionsList = [];
                    if (permission.canRead) permissionsList.push('Lectura');
                    if (permission.canCreate) permissionsList.push('Creación');
                    if (permission.canUpdate) permissionsList.push('Actualización');
                    if (permission.canDelete) permissionsList.push('Eliminación');
                    
                    permissionsText = permissionsList.join(', ') || 'Ninguno';
                } else {
                    permissionsText = 'Permiso no encontrado';
                }
                
                row.innerHTML = `
                    <td>${rolFormPermission.id}</td>
                    <td>${role ? role.name : 'Rol no encontrado'}</td>
                    <td>${form ? form.name : 'Formulario no encontrado'}</td>
                    <td>${permissionsText}</td>
                    <td>
                        <div class="action-buttons">
                            <button class="button danger btn-delete-rfp" data-id="${rolFormPermission.id}">Eliminar</button>
                            <button class="button danger btn-permanent-delete-rfp" data-id="${rolFormPermission.id}">Eliminar Permanente</button>
                        </div>
                    </td>
                `;
                rolFormPermissionsTableBody.appendChild(row);
            }
            
            // Agregar eventos a los botones
            document.querySelectorAll('.btn-delete-rfp').forEach(btn => {
                btn.addEventListener('click', handleDeleteRolFormPermission);
            });
            
            document.querySelectorAll('.btn-permanent-delete-rfp').forEach(btn => {
                btn.addEventListener('click', handlePermanentDeleteRolFormPermission);
            });
            
        } catch (error) {
            console.error('Error al cargar asignaciones de permisos:', error);
            document.getElementById('rolFormPermissionsTableBody').innerHTML = '<tr><td colspan="5">Error al cargar asignaciones de permisos.</td></tr>';
        }
    }
    
    /**
     * Muestra un modal para agregar un nuevo permiso
     */
    function showAddPermissionModal() {
        // Crear modal
        const modal = document.createElement('div');
        modal.className = 'modal';
        modal.id = 'addPermissionModal';
        
        modal.innerHTML = `
            <div class="modal-content">
                <span class="close-modal">&times;</span>
                <h3>Agregar Nuevo Permiso</h3>
                
                <form id="addPermissionForm">
                    <div class="form-group">
                        <label>Permisos:</label>
                        <div class="permissions-checkboxes">
                            <div class="checkbox-group">
                                <input type="checkbox" id="permissionCanRead" name="canRead" checked>
                                <label for="permissionCanRead">Lectura</label>
                            </div>
                            <div class="checkbox-group">
                                <input type="checkbox" id="permissionCanCreate" name="canCreate">
                                <label for="permissionCanCreate">Creación</label>
                            </div>
                            <div class="checkbox-group">
                                <input type="checkbox" id="permissionCanUpdate" name="canUpdate">
                                <label for="permissionCanUpdate">Actualización</label>
                            </div>
                            <div class="checkbox-group">
                                <input type="checkbox" id="permissionCanDelete" name="canDelete">
                                <label for="permissionCanDelete">Eliminación</label>
                            </div>
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <button type="submit" class="button primary">Guardar Permiso</button>
                    </div>
                </form>
            </div>
        `;
        
        document.body.appendChild(modal);
        
        // Mostrar modal
        modal.style.display = 'block';
        
        // Manejar cierre del modal
        modal.querySelector('.close-modal').addEventListener('click', () => {
            modal.remove();
        });
        
        // Manejar envío del formulario
        modal.querySelector('#addPermissionForm').addEventListener('submit', async (e) => {
            e.preventDefault();
            
            // Obtener valores de los checkboxes
            const canRead = document.getElementById('permissionCanRead').checked;
            const canCreate = document.getElementById('permissionCanCreate').checked;
            const canUpdate = document.getElementById('permissionCanUpdate').checked;
            const canDelete = document.getElementById('permissionCanDelete').checked;
            
            try {
                const permissionData = {
                    CanRead: canRead,
                    CanCreate: canCreate,
                    CanUpdate: canUpdate,
                    CanDelete: canDelete
                };
                
                const response = await apiRequest(API_CONFIG.ENDPOINTS.PERMISSION, 'POST', permissionData);
                
                if (response) {
                    // Cerrar modal y recargar datos
                    modal.remove();
                    loadPermissionsData();
                    alert('Permiso creado correctamente.');
                } else {
                    alert('Error al crear el permiso.');
                }
            } catch (error) {
                console.error('Error al crear permiso:', error);
                alert(`Error al crear permiso: ${error.message || 'Error desconocido'}`);
            }
        });
    }
    
    /**
     * Muestra un modal para agregar una nueva asignación de permiso a rol y formulario
     */
    async function showAddRolFormPermissionModal() {
        try {
            // Obtener roles, formularios y permisos disponibles
            const [roles, forms, permissions] = await Promise.all([
                apiRequest(API_CONFIG.ENDPOINTS.ROL),
                apiRequest(API_CONFIG.ENDPOINTS.FORM),
                apiRequest(API_CONFIG.ENDPOINTS.PERMISSION)
            ]);
            
            if (!roles || roles.length === 0) {
                alert('No hay roles disponibles para asignar permisos.');
                return;
            }
            
            if (!forms || forms.length === 0) {
                alert('No hay formularios disponibles para asignar permisos.');
                return;
            }
            
            if (!permissions || permissions.length === 0) {
                alert('No hay permisos disponibles para asignar.');
                return;
            }
            
            // Crear modal
            const modal = document.createElement('div');
            modal.className = 'modal';
            modal.id = 'addRolFormPermissionModal';
            
            // Generar opciones para los selects
            let roleOptions = roles.map(role => `<option value="${role.id}">${role.name}</option>`).join('');
            let formOptions = forms.map(form => `<option value="${form.id}">${form.name}</option>`).join('');
            
            // Preparar una descripción de cada permiso para mostrarlo en el select
            let permissionOptions = permissions.map(permission => {
                const permDesc = [];
                if (permission.canRead) permDesc.push('Lectura');
                if (permission.canCreate) permDesc.push('Creación');
                if (permission.canUpdate) permDesc.push('Actualización');
                if (permission.canDelete) permDesc.push('Eliminación');
                
                const description = permDesc.length > 0 ? permDesc.join(', ') : 'Sin permisos';
                
                return `<option value="${permission.id}">ID: ${permission.id} - ${description}</option>`;
            }).join('');
            
            modal.innerHTML = `
                <div class="modal-content">
                    <span class="close-modal">&times;</span>
                    <h3>Asignar Permisos a Rol y Formulario</h3>
                    
                    <form id="addRolFormPermissionForm">
                        <div class="form-group">
                            <label for="rolFormPermissionRolId">Rol:</label>
                            <select id="rolFormPermissionRolId" name="rolId" required>
                                <option value="">Seleccione un rol</option>
                                ${roleOptions}
                            </select>
                        </div>
                        
                        <div class="form-group">
                            <label for="rolFormPermissionFormId">Formulario:</label>
                            <select id="rolFormPermissionFormId" name="formId" required>
                                <option value="">Seleccione un formulario</option>
                                ${formOptions}
                            </select>
                        </div>
                        
                        <div class="form-group">
                            <label for="rolFormPermissionPermissionId">Permiso:</label>
                            <select id="rolFormPermissionPermissionId" name="permissionId" required>
                                <option value="">Seleccione un permiso</option>
                                ${permissionOptions}
                            </select>
                        </div>
                        
                        <div class="form-group">
                            <button type="submit" class="button primary">Guardar Asignación</button>
                        </div>
                    </form>
                </div>
            `;
            
            document.body.appendChild(modal);
            
            // Mostrar modal
            modal.style.display = 'block';
            
            // Manejar cierre del modal
            modal.querySelector('.close-modal').addEventListener('click', () => {
                modal.remove();
            });
            
            // Manejar envío del formulario
            modal.querySelector('#addRolFormPermissionForm').addEventListener('submit', async (e) => {
                e.preventDefault();
                
                const rolId = parseInt(document.getElementById('rolFormPermissionRolId').value);
                const formId = parseInt(document.getElementById('rolFormPermissionFormId').value);
                const permissionId = parseInt(document.getElementById('rolFormPermissionPermissionId').value);
                
                try {
                    const rfpData = {
                        RolId: rolId,
                        FormId: formId,
                        PermissionId: permissionId
                    };
                    
                    const response = await apiRequest(API_CONFIG.ENDPOINTS.ROL_FORM_PERMISSION, 'POST', rfpData);
                    
                    if (response) {
                        // Cerrar modal y recargar datos
                        modal.remove();
                        loadRolFormPermissionsData();
                        alert('Asignación creada correctamente.');
                    } else {
                        alert('Error al crear la asignación.');
                    }
                } catch (error) {
                    console.error('Error al crear asignación:', error);
                    alert(`Error al crear asignación: ${error.message || 'Error desconocido'}`);
                }
            });
        } catch (error) {
            console.error('Error al preparar el modal de asignación:', error);
            alert('Error al cargar los datos necesarios para la asignación.');
        }
    }
    
    /**
     * Maneja la edición de un permiso
     */
    async function handleEditPermission(event) {
        const permissionId = event.target.getAttribute('data-id');
        
        try {
            // Obtener detalles del permiso
            const permission = await apiRequest(`${API_CONFIG.ENDPOINTS.PERMISSION}/${permissionId}`);
            
            if (!permission) {
                alert('No se pudo obtener la información del permiso.');
                return;
            }
            
            // Crear modal
            const modal = document.createElement('div');
            modal.className = 'modal';
            modal.id = 'editPermissionModal';
            
            modal.innerHTML = `
                <div class="modal-content">
                    <span class="close-modal">&times;</span>
                    <h3>Editar Permiso</h3>
                    
                    <form id="editPermissionForm">
                        <input type="hidden" id="editPermissionId" value="${permission.id}">
                        
                        <div class="form-group">
                            <label>Permisos:</label>
                            <div class="permissions-checkboxes">
                                <div class="checkbox-group">
                                    <input type="checkbox" id="editPermissionCanRead" name="canRead" ${permission.canRead ? 'checked' : ''}>
                                    <label for="editPermissionCanRead">Lectura</label>
                                </div>
                                <div class="checkbox-group">
                                    <input type="checkbox" id="editPermissionCanCreate" name="canCreate" ${permission.canCreate ? 'checked' : ''}>
                                    <label for="editPermissionCanCreate">Creación</label>
                                </div>
                                <div class="checkbox-group">
                                    <input type="checkbox" id="editPermissionCanUpdate" name="canUpdate" ${permission.canUpdate ? 'checked' : ''}>
                                    <label for="editPermissionCanUpdate">Actualización</label>
                                </div>
                                <div class="checkbox-group">
                                    <input type="checkbox" id="editPermissionCanDelete" name="canDelete" ${permission.canDelete ? 'checked' : ''}>
                                    <label for="editPermissionCanDelete">Eliminación</label>
                                </div>
                            </div>
                        </div>
                        
                        <div class="form-group">
                            <button type="submit" class="button primary">Guardar Cambios</button>
                        </div>
                    </form>
                </div>
            `;
            
            document.body.appendChild(modal);
            
            // Mostrar modal
            modal.style.display = 'block';
            
            // Manejar cierre del modal
            modal.querySelector('.close-modal').addEventListener('click', () => {
                modal.remove();
            });
            
            // Manejar envío del formulario
            modal.querySelector('#editPermissionForm').addEventListener('submit', async (e) => {
                e.preventDefault();
                
                const id = parseInt(document.getElementById('editPermissionId').value);
                const canRead = document.getElementById('editPermissionCanRead').checked;
                const canCreate = document.getElementById('editPermissionCanCreate').checked;
                const canUpdate = document.getElementById('editPermissionCanUpdate').checked;
                const canDelete = document.getElementById('editPermissionCanDelete').checked;
                
                try {
                    const permissionData = {
                        Id: id,
                        CanRead: canRead,
                        CanCreate: canCreate,
                        CanUpdate: canUpdate,
                        CanDelete: canDelete
                    };
                    
                    const response = await apiRequest(`${API_CONFIG.ENDPOINTS.PERMISSION}/${id}`, 'PUT', permissionData);
                    
                    if (response !== null) {
                        // Cerrar modal y recargar datos
                        modal.remove();
                        loadPermissionsData();
                        loadRolFormPermissionsData(); // Recargar también las asignaciones
                        alert('Permiso actualizado correctamente.');
                    } else {
                        alert('Error al actualizar el permiso.');
                    }
                } catch (error) {
                    console.error('Error al actualizar permiso:', error);
                    alert(`Error al actualizar permiso: ${error.message || 'Error desconocido'}`);
                }
            });
        } catch (error) {
            console.error('Error al editar permiso:', error);
            alert('Error al obtener los datos del permiso.');
        }
    }
    
    /**
     * Maneja la eliminación de un permiso
     */
    async function handleDeletePermission(event) {
        const permissionId = event.target.getAttribute('data-id');
        
        if (confirm('¿Está seguro de que desea eliminar este permiso? Esta acción se puede revertir, pero afectará a todas las asignaciones que lo utilicen.')) {
            try {
                // Intentar eliminar el permiso
                const deleted = await apiRequest(`${API_CONFIG.ENDPOINTS.PERMISSION}/${permissionId}`, 'DELETE');
                
                if (deleted) {
                    // Recargar los datos
                    loadPermissionsData();
                    loadRolFormPermissionsData();
                    alert('Permiso eliminado correctamente.');
                } else {
                    alert('No se pudo eliminar el permiso.');
                }
            } catch (error) {
                console.error('Error al eliminar permiso:', error);
                alert(`Error al eliminar permiso: ${error.message || 'Error desconocido'}`);
            }
        }
    }
    
    /**
     * Maneja la eliminación permanente de un permiso
     */
    async function handlePermanentDeletePermission(event) {
        const permissionId = event.target.getAttribute('data-id');
        
        if (confirm('¿Está seguro de que desea eliminar permanentemente este permiso? Esta acción NO se puede deshacer y afectará a todas las asignaciones que lo utilicen.')) {
            try {
                // Intentar eliminar permanentemente el permiso
                const deleted = await apiRequest(`${API_CONFIG.ENDPOINTS.PERMISSION}/permanent/${permissionId}`, 'DELETE');
                
                if (deleted) {
                    // Recargar los datos
                    loadPermissionsData();
                    loadRolFormPermissionsData();
                    alert('Permiso eliminado permanentemente.');
                } else {
                    alert('No se pudo eliminar permanentemente el permiso.');
                }
            } catch (error) {
                console.error('Error al eliminar permanentemente permiso:', error);
                alert(`Error al eliminar permanentemente permiso: ${error.message || 'Error desconocido'}`);
            }
        }
    }
    
    /**
     * Maneja la eliminación de una asignación de permiso a rol y formulario
     */
    async function handleDeleteRolFormPermission(event) {
        const rfpId = event.target.getAttribute('data-id');
        
        if (confirm('¿Está seguro de que desea eliminar esta asignación de permiso? Esta acción se puede revertir.')) {
            try {
                // Intentar eliminar la asignación
                const deleted = await apiRequest(`${API_CONFIG.ENDPOINTS.ROL_FORM_PERMISSION}/${rfpId}`, 'DELETE');
                
                if (deleted) {
                    // Recargar los datos
                    loadRolFormPermissionsData();
                    alert('Asignación de permiso eliminada correctamente.');
                } else {
                    alert('No se pudo eliminar la asignación de permiso.');
                }
            } catch (error) {
                console.error('Error al eliminar asignación de permiso:', error);
                alert(`Error al eliminar asignación de permiso: ${error.message || 'Error desconocido'}`);
            }
        }
    }
    
    /**
     * Maneja la eliminación permanente de una asignación de permiso a rol y formulario
     */
    async function handlePermanentDeleteRolFormPermission(event) {
        const rfpId = event.target.getAttribute('data-id');
        
        if (confirm('¿Está seguro de que desea eliminar permanentemente esta asignación de permiso? Esta acción NO se puede deshacer.')) {
            try {
                // Intentar eliminar permanentemente la asignación
                const deleted = await apiRequest(`${API_CONFIG.ENDPOINTS.ROL_FORM_PERMISSION}/permanent/${rfpId}`, 'DELETE');
                
                if (deleted) {
                    // Recargar los datos
                    loadRolFormPermissionsData();
                    alert('Asignación de permiso eliminada permanentemente.');
                } else {
                    alert('No se pudo eliminar permanentemente la asignación de permiso.');
                }
            } catch (error) {
                console.error('Error al eliminar permanentemente asignación de permiso:', error);
                alert(`Error al eliminar permanentemente asignación de permiso: ${error.message || 'Error desconocido'}`);
            }
        }
    }
});

/**
 * Muestra un modal para agregar un nuevo permiso
 */
function showAddPermissionModal() {
    console.log("Ejecutando showAddPermissionModal");
    
    // Crear modal
    const modal = document.createElement('div');
    modal.className = 'modal';
    modal.id = 'addPermissionModal';
    
    modal.innerHTML = `
        <div class="modal-content">
            <span class="close-modal">&times;</span>
            <h3>Agregar Nuevo Permiso</h3>
            
            <form id="addPermissionForm">
                <div class="form-group">
                    <label>Permisos:</label>
                    <div class="permissions-checkboxes">
                        <div class="checkbox-group">
                            <input type="checkbox" id="permissionCanRead" name="canRead" checked>
                            <label for="permissionCanRead">Lectura</label>
                        </div>
                        <div class="checkbox-group">
                            <input type="checkbox" id="permissionCanCreate" name="canCreate">
                            <label for="permissionCanCreate">Creación</label>
                        </div>
                        <div class="checkbox-group">
                            <input type="checkbox" id="permissionCanUpdate" name="canUpdate">
                            <label for="permissionCanUpdate">Actualización</label>
                        </div>
                        <div class="checkbox-group">
                            <input type="checkbox" id="permissionCanDelete" name="canDelete">
                            <label for="permissionCanDelete">Eliminación</label>
                        </div>
                    </div>
                </div>
                
                <div class="form-group">
                    <button type="submit" class="button primary">Guardar Permiso</button>
                </div>
            </form>
        </div>
    `;
    
    document.body.appendChild(modal);
    
    // Mostrar modal
    modal.style.display = 'block';
    
    // Manejar cierre del modal
    modal.querySelector('.close-modal').addEventListener('click', () => {
        modal.remove();
    });
    
    // Manejar envío del formulario
    modal.querySelector('#addPermissionForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        
        // Obtener valores de los checkboxes
        const canRead = document.getElementById('permissionCanRead').checked;
        const canCreate = document.getElementById('permissionCanCreate').checked;
        const canUpdate = document.getElementById('permissionCanUpdate').checked;
        const canDelete = document.getElementById('permissionCanDelete').checked;
        
        try {
            const permissionData = {
                CanRead: canRead,
                CanCreate: canCreate,
                CanUpdate: canUpdate,
                CanDelete: canDelete
            };
            
            console.log('Enviando datos de permiso:', permissionData);
            const response = await apiRequest(API_CONFIG.ENDPOINTS.PERMISSION, 'POST', permissionData);
            
            if (response) {
                // Cerrar modal y recargar datos
                modal.remove();
                if (typeof loadPermissionsData === 'function') {
                    loadPermissionsData();
                } else {
                    console.warn('Función loadPermissionsData no encontrada');
                    location.reload(); // Recargar página como alternativa
                }
                alert('Permiso creado correctamente.');
            } else {
                alert('Error al crear el permiso.');
            }
        } catch (error) {
            console.error('Error al crear permiso:', error);
            alert(`Error al crear permiso: ${error.message || 'Error desconocido'}`);
        }
    });
}

/**
 * Muestra un modal para agregar una nueva asignación de permiso a rol y formulario
 */
async function showAddRolFormPermissionModal() {
    console.log("Ejecutando showAddRolFormPermissionModal");
    
    try {
        // Obtener roles, formularios y permisos disponibles
        const [roles, forms, permissions] = await Promise.all([
            apiRequest(API_CONFIG.ENDPOINTS.ROL),
            apiRequest(API_CONFIG.ENDPOINTS.FORM),
            apiRequest(API_CONFIG.ENDPOINTS.PERMISSION)
        ]);
        
        if (!roles || roles.length === 0) {
            alert('No hay roles disponibles para asignar permisos.');
            return;
        }
        
        if (!forms || forms.length === 0) {
            alert('No hay formularios disponibles para asignar permisos.');
            return;
        }
        
        if (!permissions || permissions.length === 0) {
            alert('No hay permisos disponibles para asignar.');
            return;
        }
        
        // Crear modal
        const modal = document.createElement('div');
        modal.className = 'modal';
        modal.id = 'addRolFormPermissionModal';
        
        // Generar opciones para los selects
        let roleOptions = roles.map(role => `<option value="${role.id}">${role.name}</option>`).join('');
        let formOptions = forms.map(form => `<option value="${form.id}">${form.name}</option>`).join('');
        
        // Preparar una descripción de cada permiso para mostrarlo en el select
        let permissionOptions = permissions.map(permission => {
            const permDesc = [];
            if (permission.canRead) permDesc.push('Lectura');
            if (permission.canCreate) permDesc.push('Creación');
            if (permission.canUpdate) permDesc.push('Actualización');
            if (permission.canDelete) permDesc.push('Eliminación');
            
            const description = permDesc.length > 0 ? permDesc.join(', ') : 'Sin permisos';
            
            return `<option value="${permission.id}">ID: ${permission.id} - ${description}</option>`;
        }).join('');
        
        modal.innerHTML = `
            <div class="modal-content">
                <span class="close-modal">&times;</span>
                <h3>Asignar Permisos a Rol y Formulario</h3>
                
                <form id="addRolFormPermissionForm">
                    <div class="form-group">
                        <label for="rolFormPermissionRolId">Rol:</label>
                        <select id="rolFormPermissionRolId" name="rolId" required>
                            <option value="">Seleccione un rol</option>
                            ${roleOptions}
                        </select>
                    </div>
                    
                    <div class="form-group">
                        <label for="rolFormPermissionFormId">Formulario:</label>
                        <select id="rolFormPermissionFormId" name="formId" required>
                            <option value="">Seleccione un formulario</option>
                            ${formOptions}
                        </select>
                    </div>
                    
                    <div class="form-group">
                        <label for="rolFormPermissionPermissionId">Permiso:</label>
                        <select id="rolFormPermissionPermissionId" name="permissionId" required>
                            <option value="">Seleccione un permiso</option>
                            ${permissionOptions}
                        </select>
                    </div>
                    
                    <div class="form-group">
                        <button type="submit" class="button primary">Guardar Asignación</button>
                    </div>
                </form>
            </div>
        `;
        
        document.body.appendChild(modal);
        
        // Mostrar modal
        modal.style.display = 'block';
        
        // Manejar cierre del modal
        modal.querySelector('.close-modal').addEventListener('click', () => {
            modal.remove();
        });
        
        // Manejar envío del formulario
        modal.querySelector('#addRolFormPermissionForm').addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const rolId = parseInt(document.getElementById('rolFormPermissionRolId').value);
            const formId = parseInt(document.getElementById('rolFormPermissionFormId').value);
            const permissionId = parseInt(document.getElementById('rolFormPermissionPermissionId').value);
            
            try {
                const rfpData = {
                    RolId: rolId,
                    FormId: formId,
                    PermissionId: permissionId
                };
                
                console.log('Enviando datos de asignación:', rfpData);
                const response = await apiRequest(API_CONFIG.ENDPOINTS.ROL_FORM_PERMISSION, 'POST', rfpData);
                
                if (response) {
                    // Cerrar modal y recargar datos
                    modal.remove();
                    if (typeof loadRolFormPermissionsData === 'function') {
                        loadRolFormPermissionsData();
                    } else {
                        console.warn('Función loadRolFormPermissionsData no encontrada');
                        location.reload(); // Recargar página como alternativa
                    }
                    alert('Asignación creada correctamente.');
                } else {
                    alert('Error al crear la asignación.');
                }
            } catch (error) {
                console.error('Error al crear asignación:', error);
                alert(`Error al crear asignación: ${error.message || 'Error desconocido'}`);
            }
        });
    } catch (error) {
        console.error('Error al preparar el modal de asignación:', error);
        alert('Error al cargar los datos necesarios para la asignación.');
    }
}

// Agregar listeners para los botones al cargar el documento
document.addEventListener('DOMContentLoaded', function() {
    console.log('Document loaded, configuring permission buttons');
    
    // Intentar asignar los eventos inmediatamente
    const addPermissionBtn = document.getElementById('addPermissionBtn');
    if (addPermissionBtn) {
        console.log('Add Permission button found, adding event listener');
        addPermissionBtn.addEventListener('click', showAddPermissionModal);
    }
    
    const addRolFormPermissionBtn = document.getElementById('addRolFormPermissionBtn');
    if (addRolFormPermissionBtn) {
        console.log('Add RolFormPermission button found, adding event listener');
        addRolFormPermissionBtn.addEventListener('click', showAddRolFormPermissionModal);
    }
    
    // Si los botones no están disponibles ahora, intentarlo después
    setTimeout(function() {
        const delayedPermissionBtn = document.getElementById('addPermissionBtn');
        if (delayedPermissionBtn && !delayedPermissionBtn.onclick) {
            console.log('Adding delayed event listener to Permission button');
            delayedPermissionBtn.addEventListener('click', showAddPermissionModal);
        }
        
        const delayedRolFormPermissionBtn = document.getElementById('addRolFormPermissionBtn');
        if (delayedRolFormPermissionBtn && !delayedRolFormPermissionBtn.onclick) {
            console.log('Adding delayed event listener to RolFormPermission button');
            delayedRolFormPermissionBtn.addEventListener('click', showAddRolFormPermissionModal);
        }
    }, 1000);
    
    // Para la sección roles, asegurarse de que el botón de agregar rol tenga su evento
    const addRoleBtn = document.getElementById('addRoleBtn');
    if (addRoleBtn) {
        console.log('Add Role button found, adding event listener');
        addRoleBtn.addEventListener('click', function() {
            if (typeof showAddRoleModal === 'function') {
                showAddRoleModal();
            } else {
                console.error('Function showAddRoleModal not found');
                alert('Error: Función para agregar rol no encontrada');
            }
        });
    }
    
    // Para la sección usuarios, asegurarse de que el botón de agregar usuario tenga su evento
    const addUserBtn = document.getElementById('addUserBtn');
    if (addUserBtn) {
        console.log('Add User button found, adding event listener');
        addUserBtn.addEventListener('click', function() {
            if (typeof showAddUserModal === 'function') {
                showAddUserModal();
            } else {
                console.error('Function showAddUserModal not found');
                alert('Error: Función para agregar usuario no encontrada');
            }
        });
    }
});

// Exponer las funciones globalmente
window.showAddPermissionModal = showAddPermissionModal;
window.showAddRolFormPermissionModal = showAddRolFormPermissionModal;