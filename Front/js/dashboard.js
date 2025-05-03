document.addEventListener('DOMContentLoaded', function() {
    // Verificar si el usuario está autenticado
    requireAuth();
    
    // Cargar datos del usuario
    const user = getFromStorage('user');
    
    // Elementos del DOM
    const userDisplayName = document.getElementById('userDisplayName');
    const userRole = document.getElementById('userRole');
    const currentRole = document.getElementById('currentRole');
    const sectionTitle = document.getElementById('sectionTitle');
    const logoutBtn = document.getElementById('logoutBtn');
    const menuLinks = document.querySelectorAll('.menu a[data-section]');
    const sections = document.querySelectorAll('.section-content');
    
    // Elementos que solo son visibles para administradores
    const adminOnlyElements = document.querySelectorAll('.admin-only');
    
    // Mostrar/ocultar elementos según el rol del usuario
    if (isAdmin()) {
        adminOnlyElements.forEach(el => el.style.display = 'block');
    } else {
        adminOnlyElements.forEach(el => el.style.display = 'none');
    }
    
    // Mostrar información del usuario
    userDisplayName.textContent = user.name || user.username;
    userRole.textContent = user.rolName || 'Usuario';
    currentRole.textContent = user.rolName || 'Usuario';
    
    // Cargar permisos del rol
    loadRolePermissions();
    
    // Sección de perfil
    loadProfileData();
    
    // Si es administrador, cargar datos para las secciones de admin
    if (isAdmin()) {
        loadUsersData();
        loadRolesData();
    }
    
    // Agregar evento para cambiar entre secciones
    menuLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Obtener la sección a mostrar
            const sectionToShow = this.getAttribute('data-section');
            
            // Actualizar clases activas en menú
            menuLinks.forEach(l => l.classList.remove('active'));
            this.classList.add('active');
            
            // Actualizar título de la sección
            sectionTitle.textContent = this.textContent;
            
            // Ocultar todas las secciones
            sections.forEach(section => section.classList.add('hidden'));
            
            // Mostrar la sección seleccionada
            document.getElementById(sectionToShow + 'Section').classList.remove('hidden');
        });
    });
    
    // Agregar evento para cerrar sesión
    logoutBtn.addEventListener('click', function(e) {
        e.preventDefault();
        
        // Eliminar datos de sesión
        removeFromStorage('user');
        
        // Redirigir a la página de login
        window.location.href = 'login.html';
    });
    
    // Agregar evento para el formulario de perfil
    const profileForm = document.getElementById('profileForm');
    if (profileForm) {
        profileForm.addEventListener('submit', handleProfileSubmit);
    }
    
    /**
     * Carga la información de permisos del rol del usuario
     */
    async function loadRolePermissions() {
        try {
            if (!user.rolId) {
                console.error('No se encontró rolId del usuario');
                return;
            }
            
            // Obtener los permisos de rol-formulario para el rol del usuario
            const rolFormPermissions = await apiRequest(`${API_CONFIG.ENDPOINTS.ROL_FORM_PERMISSION}/rol/${user.rolId}`);
            
            if (!rolFormPermissions || rolFormPermissions.length === 0) {
                const rolePermissionsElement = document.getElementById('rolePermissions');
                rolePermissionsElement.innerHTML = '<p>No tienes permisos específicos asignados.</p>';
                return;
            }
            
            // Obtener los detalles de cada formulario y permiso
            const permissionsDetails = await Promise.all(rolFormPermissions.map(async (rfp) => {
                // Obtener detalles del formulario
                const form = await apiRequest(`${API_CONFIG.ENDPOINTS.FORM}/${rfp.formId}`);
                
                // Obtener detalles del permiso
                const permission = await apiRequest(`${API_CONFIG.ENDPOINTS.PERMISSION}/${rfp.permissionId}`);
                
                return {
                    formId: rfp.formId,
                    permissionId: rfp.permissionId,
                    formName: form ? form.name : 'Formulario desconocido',
                    formCode: form ? form.code : '',
                    canRead: permission ? permission.canRead : false,
                    canCreate: permission ? permission.canCreate : false,
                    canUpdate: permission ? permission.canUpdate : false,
                    canDelete: permission ? permission.canDelete : false
                };
            }));
            
            // Mostrar permisos en la interfaz
            const rolePermissionsElement = document.getElementById('rolePermissions');
            let permissionsHTML = '<h4>Tus Permisos:</h4><ul class="permissions-list">';
            
            permissionsDetails.forEach(permission => {
                permissionsHTML += `
                    <li class="permission-item">
                        <strong>${permission.formName}</strong> (${permission.formCode}):
                        <span class="${permission.canRead ? 'granted' : 'denied'}">Lectura</span> |
                        <span class="${permission.canCreate ? 'granted' : 'denied'}">Creación</span> |
                        <span class="${permission.canUpdate ? 'granted' : 'denied'}">Actualización</span> |
                        <span class="${permission.canDelete ? 'granted' : 'denied'}">Eliminación</span>
                    </li>
                `;
            });
            
            permissionsHTML += '</ul>';
            
            // Agregar información del rol
            permissionsHTML += `
                <div class="role-description">
                    <h4>Detalles del Rol:</h4>
                    <p><strong>ID del Rol:</strong> ${user.rolId}</p>
                    <p><strong>Nombre:</strong> ${user.rolName}</p>
                </div>
            `;
            
            rolePermissionsElement.innerHTML = permissionsHTML;
            
            // Guardar información de permisos para uso global
            window.userPermissions = permissionsDetails;
            
            // Disparar evento para indicar que los permisos se han cargado
document.dispatchEvent(new Event('permissionsLoaded'));

        } catch (error) {
            console.error('Error al cargar permisos:', error);
            document.getElementById('rolePermissions').innerHTML = '<p class="error">Error al cargar permisos.</p>';
        }
    }
    
    /**
     * Carga los datos del perfil del usuario
     */
    function loadProfileData() {
        // Completar el formulario con los datos del usuario
        document.getElementById('profileName').value = user.name || '';
        document.getElementById('profileEmail').value = user.email || '';
        document.getElementById('profileUsername').value = user.username || '';
    }
    
    /**
     * Maneja el envío del formulario de perfil
     */
    async function handleProfileSubmit(event) {
        event.preventDefault();
        
        // Obtener los valores del formulario
        const name = document.getElementById('profileName').value;
        const email = document.getElementById('profileEmail').value;
        const currentPassword = document.getElementById('currentPassword').value;
        const newPassword = document.getElementById('newPassword').value;
        const confirmNewPassword = document.getElementById('confirmNewPassword').value;
        
        // Validar campos obligatorios
        if (!name || !email) {
            showMessage('profileMessage', 'Nombre y correo electrónico son obligatorios.', 'error');
            return;
        }
        
        // Verificar si se está cambiando la contraseña
        const isChangingPassword = currentPassword && newPassword && confirmNewPassword;
        
        // Si se está cambiando la contraseña, validar
        if (isChangingPassword) {
            if (newPassword !== confirmNewPassword) {
                showMessage('profileMessage', 'Las nuevas contraseñas no coinciden.', 'error');
                return;
            }
        }
        
        try {
            // Crear objeto con los datos actualizados
            const updatedUserData = {
                Id: user.id,
                Name: name,
                Email: email,
                Password: user.password // Mantener la contraseña actual por defecto
            };
            
            if (isChangingPassword) {
                // Verificar la contraseña actual
                if (currentPassword !== user.password) {
                    showMessage('profileMessage', 'La contraseña actual es incorrecta.', 'error');
                    return;
                }
                
                // Actualizar contraseña
                updatedUserData.Password = newPassword;
            }
            
            // Enviar actualización al backend
            const response = await apiRequest(`${API_CONFIG.ENDPOINTS.USER}`, 'PUT', updatedUserData);
            
            if (response) {
                // Actualizar datos en localStorage
                const updatedUser = {
                    ...user,
                    name: name,
                    email: email,
                    password: isChangingPassword ? newPassword : user.password
                };
                saveToStorage('user', updatedUser);
                
                // Actualizar nombre mostrado en la interfaz
                userDisplayName.textContent = name;
                
                // Mostrar mensaje de éxito
                showMessage('profileMessage', 'Perfil actualizado correctamente.', 'success');
                
                // Limpiar campos de contraseña
                document.getElementById('currentPassword').value = '';
                document.getElementById('newPassword').value = '';
                document.getElementById('confirmNewPassword').value = '';
            } else {
                showMessage('profileMessage', 'No se pudo actualizar el perfil.', 'error');
            }
        } catch (error) {
            showMessage('profileMessage', error.message || 'Error al actualizar el perfil.', 'error');
            console.error('Error al actualizar perfil:', error);
        }
    }
    
    /**
     * Carga la lista de usuarios (solo para administradores)
     */
    async function loadUsersData() {
        if (!isAdmin()) return;
        
        try {
            // Obtener usuarios desde la API
            const users = await apiRequest(API_CONFIG.ENDPOINTS.USER);
            
            if (!users || users.length === 0) {
                document.getElementById('usersTableBody').innerHTML = '<tr><td colspan="5">No hay usuarios registrados.</td></tr>';
                return;
            }
            
            // Para cada usuario, obtener su rol
            const usersWithRoles = await Promise.all(users.map(async (user) => {
                try {
                    // Obtener roles del usuario
                    const rolUsers = await apiRequest(`${API_CONFIG.ENDPOINTS.ROL_USER}`);
                    const userRol = rolUsers.find(ru => ru.userId === user.id);
                    
                    if (userRol) {
                        // Obtener detalles del rol
                        const rol = await apiRequest(`${API_CONFIG.ENDPOINTS.ROL}/${userRol.rolId}`);
                        
                        return {
                            ...user,
                            rolId: userRol.rolId,
                            rolName: rol ? rol.name : 'Sin rol'
                        };
                    }
                    
                    return {
                        ...user,
                        rolName: 'Sin rol'
                    };
                } catch (error) {
                    console.error(`Error al obtener rol para usuario ${user.id}:`, error);
                    return {
                        ...user,
                        rolName: 'Error al cargar rol'
                    };
                }
            }));
            
            // Mostrar usuarios en la tabla
            const usersTableBody = document.getElementById('usersTableBody');
            usersTableBody.innerHTML = '';
            
            usersWithRoles.forEach(user => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${user.id}</td>
                    <td>${user.name || 'N/A'}</td>
                    <td>${user.email || 'N/A'}</td>
                    <td>${user.rolName || 'Sin rol'}</td>
                    <td>
                        <div class="action-buttons">
                            <button class="button primary btn-edit-user" data-id="${user.id}">Editar</button>
                            <button class="button danger btn-delete-user" data-id="${user.id}">Eliminar</button>
                        </div>
                    </td>
                `;
                usersTableBody.appendChild(row);
            });
            
            // Agregar eventos a los botones de acción
            document.querySelectorAll('.btn-edit-user').forEach(btn => {
                btn.addEventListener('click', handleEditUser);
            });
            
            document.querySelectorAll('.btn-delete-user').forEach(btn => {
                btn.addEventListener('click', handleDeleteUser);
            });
            
        } catch (error) {
            console.error('Error al cargar usuarios:', error);
            document.getElementById('usersTableBody').innerHTML = '<tr><td colspan="5">Error al cargar usuarios.</td></tr>';
        }
    }
    
    /**
     * Carga la lista de roles (solo para administradores)
     */
    async function loadRolesData() {
        if (!isAdmin()) return;
        
        try {
            // Obtener roles desde la API
            const roles = await apiRequest(API_CONFIG.ENDPOINTS.ROL);
            
            if (!roles || roles.length === 0) {
                document.getElementById('rolesTableBody').innerHTML = '<tr><td colspan="4">No hay roles registrados.</td></tr>';
                return;
            }
            
            // Guardar roles para uso global
            window.allRoles = roles;
            
            // Mostrar roles en la tabla
            const rolesTableBody = document.getElementById('rolesTableBody');
            rolesTableBody.innerHTML = '';
            
            roles.forEach(role => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${role.id}</td>
                    <td>${role.name || 'N/A'}</td>
                    <td>${role.description || 'Sin descripción'}</td>
                    <td>
                        <div class="action-buttons">
                            <button class="button primary btn-edit-role" data-id="${role.id}">Editar</button>
                            <button class="button secondary btn-edit-role-permissions" data-id="${role.id}">Permisos</button>
                            <button class="button danger btn-delete-role" data-id="${role.id}">Eliminar</button>
                        </div>
                    </td>
                `;
                rolesTableBody.appendChild(row);
            });
            
            // Agregar eventos a los botones de acción
            document.querySelectorAll('.btn-edit-role').forEach(btn => {
                btn.addEventListener('click', handleEditRole);
            });
            
            document.querySelectorAll('.btn-edit-role-permissions').forEach(btn => {
                btn.addEventListener('click', handleEditRolePermissions);
            });
            
            document.querySelectorAll('.btn-delete-role').forEach(btn => {
                btn.addEventListener('click', handleDeleteRole);
            });
            
        } catch (error) {
            console.error('Error al cargar roles:', error);
            document.getElementById('rolesTableBody').innerHTML = '<tr><td colspan="4">Error al cargar roles.</td></tr>';
        }
    }
    
    /**
     * Maneja el evento de editar un usuario
     */
    async function handleEditUser(event) {
        const userId = event.target.getAttribute('data-id');
        
        try {
            // Obtener detalles del usuario
            const user = await apiRequest(`${API_CONFIG.ENDPOINTS.USER}/${userId}`);
            
            if (!user) {
                alert('No se pudo obtener la información del usuario.');
                return;
            }
            
            // Obtener todos los roles para el selector
            const roles = window.allRoles || await apiRequest(API_CONFIG.ENDPOINTS.ROL);
            
            // Obtener el rol actual del usuario
            const rolUsers = await apiRequest(API_CONFIG.ENDPOINTS.ROL_USER);
            const userRol = rolUsers.find(ru => ru.userId === user.id);
            const currentRolId = userRol ? userRol.rolId : null;
            
            // Crear modal
            const modal = document.createElement('div');
            modal.className = 'modal';
            modal.id = 'editUserModal';
            
// Generar opciones para el selector de roles
let roleOptions = '';
roles.forEach(role => {
    const selected = role.id === currentRolId ? 'selected' : '';
    roleOptions += `<option value="${role.id}" ${selected}>${role.name}</option>`;
});

modal.innerHTML = `
    <div class="modal-content">
        <span class="close-modal">&times;</span>
        <h3>Editar Usuario</h3>
        
        <form id="editUserForm">
            <input type="hidden" id="editUserId" value="${user.id}">
            
            <div class="form-group">
                <label for="editUserName">Nombre:</label>
                <input type="text" id="editUserName" name="name" value="${user.name || ''}" required>
            </div>
            
            <div class="form-group">
                <label for="editUserEmail">Correo Electrónico:</label>
                <input type="email" id="editUserEmail" name="email" value="${user.email || ''}" required>
            </div>
            
            <div class="form-group">
                <label for="editUserRole">Rol:</label>
                <select id="editUserRole" name="rolId">
                    <option value="">Sin rol</option>
                    ${roleOptions}
                </select>
            </div>
            
            <div class="form-group">
                <label for="editUserPassword">Nueva Contraseña (dejar en blanco para mantener la actual):</label>
                <input type="password" id="editUserPassword" name="password">
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
modal.querySelector('#editUserForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const userId = document.getElementById('editUserId').value;
    const name = document.getElementById('editUserName').value;
    const email = document.getElementById('editUserEmail').value;
    const rolId = document.getElementById('editUserRole').value;
    const password = document.getElementById('editUserPassword').value;
    
    try {
        // Actualizar usuario
        const userData = {
            Id: parseInt(userId),
            Name: name,
            Email: email
        };
        
        // Si se proporcionó una nueva contraseña, incluirla
        if (password) {
            userData.Password = password;
        }
        
        const updateResponse = await apiRequest(API_CONFIG.ENDPOINTS.USER, 'PUT', userData);
        
        if (!updateResponse) {
            throw new Error('No se pudo actualizar el usuario');
        }
        
        // Actualizar asignación de rol si es necesario
        if (rolId) {
            // Verificar si ya existe una asignación
            if (currentRolId) {
                // Obtener el ID de la asignación
                const rolUserId = userRol.id;
                
                // Si el rol cambió, actualizar
                if (parseInt(rolId) !== currentRolId) {
                    const rolUserData = {
                        Id: rolUserId,
                        UserId: parseInt(userId),
                        RolId: parseInt(rolId)
                    };
                    
                    const updateRolResponse = await apiRequest(API_CONFIG.ENDPOINTS.ROL_USER, 'PUT', rolUserData);
                    
                    if (!updateRolResponse) {
                        throw new Error('No se pudo actualizar el rol del usuario');
                    }
                }
            } else {
                // Crear nueva asignación
                const rolUserData = {
                    UserId: parseInt(userId),
                    RolId: parseInt(rolId)
                };
                
                const createRolResponse = await apiRequest(API_CONFIG.ENDPOINTS.ROL_USER, 'POST', rolUserData);
                
                if (!createRolResponse) {
                    throw new Error('No se pudo asignar el rol al usuario');
                }
            }
        } else if (currentRolId) {
            // Si se eliminó el rol, eliminar la asignación
            await apiRequest(`${API_CONFIG.ENDPOINTS.ROL_USER}/${userRol.id}`, 'DELETE');
        }
        
        // Cerrar modal y actualizar lista
        modal.remove();
        loadUsersData();
        showMessage('userMessage', 'Usuario actualizado correctamente', 'success');
    } catch (error) {
        console.error('Error al actualizar usuario:', error);
        showMessage('userMessage', error.message || 'Error al actualizar usuario', 'error');
    }
});

        } catch (error) {
            console.error('Error al editar usuario:', error);
            showMessage('userMessage', 'Error al intentar editar el usuario.', 'error');
        }
    }
    
    /**
     * Maneja el evento de eliminar un usuario
     */
    async function handleDeleteUser(event) {
        const userId = event.target.getAttribute('data-id');
        
        if (confirm('¿Está seguro de que desea eliminar este usuario? Esta acción no se puede deshacer.')) {
            try {
                // Intentar eliminar el usuario
                const deleted = await apiRequest(`${API_CONFIG.ENDPOINTS.USER}/${userId}`, 'DELETE');
                
                if (deleted) {
                    // Recargar la lista de usuarios
                    loadUsersData();
                    showMessage('userMessage', 'Usuario eliminado correctamente.', 'success');
                } else {
                    showMessage('userMessage', 'No se pudo eliminar el usuario.', 'error');
                }
            } catch (error) {
                console.error('Error al eliminar usuario:', error);
                showMessage('userMessage', error.message || 'Error al eliminar usuario', 'error');
            }
        }
    }
    
    /**
     * Maneja el evento de editar un rol
     */
    async function handleEditRole(event) {
        const roleId = event.target.getAttribute('data-id');
        
        try {
            // Obtener detalles del rol
            const role = await apiRequest(`${API_CONFIG.ENDPOINTS.ROL}/${roleId}`);
            
            if (!role) {
                showMessage('roleMessage', 'No se pudo obtener la información del rol.', 'error');
                return;
            }
            
            // Crear modal
            const modal = document.createElement('div');
            modal.className = 'modal';
            modal.id = 'editRoleModal';
            
            modal.innerHTML = `
                <div class="modal-content">
                    <span class="close-modal">&times;</span>
                    <h3>Editar Rol</h3>
                    
                    <form id="editRoleForm">
                        <input type="hidden" id="editRoleId" value="${role.id}">
                        
                        <div class="form-group">
                            <label for="editRoleName">Nombre:</label>
                            <input type="text" id="editRoleName" name="name" value="${role.name || ''}" required>
                        </div>
                        
                        <div class="form-group">
                            <label for="editRoleDescription">Descripción:</label>
                            <textarea id="editRoleDescription" name="description" rows="3">${role.description || ''}</textarea>
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
            modal.querySelector('#editRoleForm').addEventListener('submit', async (e) => {
                e.preventDefault();
                
                const roleId = document.getElementById('editRoleId').value;
                const name = document.getElementById('editRoleName').value;
                const description = document.getElementById('editRoleDescription').value;
                
                try {
                    // Actualizar rol
                    const roleData = {
                        Id: parseInt(roleId),
                        Name: name,
                        Description: description
                    };
                    
                    const updateResponse = await apiRequest(API_CONFIG.ENDPOINTS.ROL, 'PUT', roleData);
                    
                    if (!updateResponse) {
                        throw new Error('No se pudo actualizar el rol');
                    }
                    
                    // Cerrar modal y actualizar lista
                    modal.remove();
                    loadRolesData();
                    showMessage('roleMessage', 'Rol actualizado correctamente', 'success');
                } catch (error) {
                    console.error('Error al actualizar rol:', error);
                    showMessage('roleMessage', error.message || 'Error al actualizar rol', 'error');
                }
            });
            
        } catch (error) {
            console.error('Error al editar rol:', error);
            showMessage('roleMessage', 'Error al intentar editar el rol.', 'error');
        }
    }
    
    /**
     * Maneja el evento de editar los permisos de un rol
     */
    async function handleEditRolePermissions(event) {
        const roleId = event.target.getAttribute('data-id');
        
        try {
            // Obtener detalles del rol
            const role = await apiRequest(`${API_CONFIG.ENDPOINTS.ROL}/${roleId}`);
            
            if (!role) {
                showMessage('roleMessage', 'No se pudo obtener la información del rol.', 'error');
                return;
            }
            
            // Guardar el rol que se está editando para uso global
            window.editingRole = role;
            
            // Actualizar título y descripción en el modal
            document.getElementById('editRoleId').value = role.id;
            document.getElementById('editRoleName').textContent = role.name;
            document.getElementById('editRoleDescription').textContent = role.description || 'Sin descripción';
            
            // Cargar los permisos actuales del rol
            loadRoleFormPermissions(role.id);
            
            // Mostrar el modal
            const modal = document.getElementById('editRolePermissionsModal');
            modal.style.display = 'block';
            
            // Configurar botón para cerrar el modal
            document.getElementById('closeEditRoleBtn').addEventListener('click', () => {
                modal.style.display = 'none';
            });
            
            // Configurar botón para agregar formulario al rol
            document.getElementById('addFormToRoleBtn').addEventListener('click', showAddFormToRoleModal);
            
            // El evento de envío del formulario se configurará cuando se carguen los permisos
        } catch (error) {
            console.error('Error al editar permisos del rol:', error);
            showMessage('roleMessage', 'Error al intentar editar los permisos del rol.', 'error');
        }
    }
    
    /**
     * Carga los permisos de formularios asignados a un rol específico
     */
    async function loadRoleFormPermissions(roleId) {
        try {
            // Obtener los permisos de rol-formulario para el rol especificado
            const rolFormPermissions = await apiRequest(`${API_CONFIG.ENDPOINTS.ROL_FORM_PERMISSION}/rol/${roleId}`);
            
            const tableBody = document.getElementById('roleFormPermissionsTableBody');
            tableBody.innerHTML = '';
            
            if (!rolFormPermissions || rolFormPermissions.length === 0) {
                tableBody.innerHTML = '<tr><td colspan="6">No hay permisos asignados a este rol.</td></tr>';
                return;
            }
            
            // Obtener los detalles de cada formulario y permiso
            const formPromises = rolFormPermissions.map(rfp => apiRequest(`${API_CONFIG.ENDPOINTS.FORM}/${rfp.formId}`));
            const permissionPromises = rolFormPermissions.map(rfp => apiRequest(`${API_CONFIG.ENDPOINTS.PERMISSION}/${rfp.permissionId}`));
            
            const forms = await Promise.all(formPromises);
            const permissions = await Promise.all(permissionPromises);
            
            // Construir la tabla con los permisos
            rolFormPermissions.forEach((rfp, index) => {
                const form = forms[index];
                const permission = permissions[index];
                
                if (!form || !permission) return;
                
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${form.name} (${form.code})</td>
                    <td>
                        <div class="checkbox-group">
                            <input type="checkbox" id="perm-read-${rfp.id}" 
                                data-id="${rfp.id}" data-type="read" 
                                ${permission.canRead ? 'checked' : ''}>
                        </div>
                    </td>
                    <td>
                        <div class="checkbox-group">
                            <input type="checkbox" id="perm-create-${rfp.id}" 
                                data-id="${rfp.id}" data-type="create" 
                                ${permission.canCreate ? 'checked' : ''}>
                        </div>
                    </td>
                    <td>
                        <div class="checkbox-group">
                            <input type="checkbox" id="perm-update-${rfp.id}" 
                                data-id="${rfp.id}" data-type="update" 
                                ${permission.canUpdate ? 'checked' : ''}>
                        </div>
                    </td>
                    <td>
                        <div class="checkbox-group">
                            <input type="checkbox" id="perm-delete-${rfp.id}" 
                                data-id="${rfp.id}" data-type="delete" 
                                ${permission.canDelete ? 'checked' : ''}>
                        </div>
                    </td>
                    <td>
                        <button class="button danger btn-remove-permission" data-id="${rfp.id}">Quitar</button>
                    </td>
                `;
                tableBody.appendChild(row);
            });
            
            // Agregar eventos para los checkboxes y botones
            document.querySelectorAll('#roleFormPermissionsTableBody input[type="checkbox"]').forEach(checkbox => {
                checkbox.addEventListener('change', handlePermissionChange);
            });
            
            document.querySelectorAll('.btn-remove-permission').forEach(btn => {
                btn.addEventListener('click', handleRemoveFormPermission);
            });
            
            // Configurar evento de envío del formulario
            document.getElementById('editRolePermissionsForm').addEventListener('submit', handleSaveRolePermissions);
            
            // Guardar datos para uso global
            window.currentRoleFormPermissions = rolFormPermissions.map((rfp, index) => ({
                ...rfp,
                form: forms[index],
                permission: permissions[index]
            }));
            
        } catch (error) {
            console.error('Error al cargar permisos del rol:', error);
            document.getElementById('roleFormPermissionsTableBody').innerHTML = 
                '<tr><td colspan="6">Error al cargar los permisos.</td></tr>';
        }
    }
    
    /**
     * Maneja el cambio en un checkbox de permiso
     */
    async function handlePermissionChange(event) {
        const checkbox = event.target;
        const permissionId = checkbox.getAttribute('data-id');
        const permissionType = checkbox.getAttribute('data-type');
        const isChecked = checkbox.checked;
        
        try {
            // Encontrar el permiso en el array global
            const rfpInfo = window.currentRoleFormPermissions.find(p => p.id.toString() === permissionId);
            
            if (!rfpInfo) {
                console.error('No se encontró la información del permiso');
                return;
            }
            
            // Crear el objeto de permiso actualizado
            const updatedPermission = { ...rfpInfo.permission };
            
            // Actualizar el tipo de permiso correspondiente
            switch(permissionType) {
                case 'read':
                    updatedPermission.canRead = isChecked;
                    break;
                case 'create':
                    updatedPermission.canCreate = isChecked;
                    break;
                case 'update':
                    updatedPermission.canUpdate = isChecked;
                    break;
                case 'delete':
                    updatedPermission.canDelete = isChecked;
                    break;
            }
            
            // Actualizar el permiso en la base de datos
            await apiRequest(`${API_CONFIG.ENDPOINTS.PERMISSION}/${rfpInfo.permissionId}`, 'PUT', updatedPermission);
            
            // Actualizar el permiso en el array global
            rfpInfo.permission = updatedPermission;
            
        } catch (error) {
            console.error('Error al actualizar permiso:', error);
            // Revertir el cambio en la UI
            checkbox.checked = !isChecked;
            alert('Error al actualizar el permiso. Inténtelo de nuevo.');
        }
    }
    
    /**
     * Muestra un modal para agregar un formulario al rol
     */
    async function showAddFormToRoleModal() {
        try {
            // Obtener todos los formularios
            const forms = await apiRequest(API_CONFIG.ENDPOINTS.FORM);
            
            // Verificar que forms existe y es un array
            if (!forms || !Array.isArray(forms) || forms.length === 0) {
                alert('No hay formularios disponibles para asignar. Primero debe crear algunos formularios.');
                return;
            }
            
            // Filtrar formularios que ya están asignados al rol
            const roleId = window.editingRole.id;
            
            // Verificar que window.currentRoleFormPermissions existe
            if (!window.currentRoleFormPermissions) {
                window.currentRoleFormPermissions = [];
            }
            
            const assignedFormIds = window.currentRoleFormPermissions.map(rfp => rfp.formId);
            const availableForms = forms.filter(form => !assignedFormIds.includes(form.id));
            
            if (availableForms.length === 0) {
                alert('Todos los formularios ya están asignados a este rol.');
                return;
            }
            
            // Crear opciones para el select
            const formOptions = availableForms.map(form => 
                `<option value="${form.id}">${form.name} (${form.code})</option>`
            ).join('');
            
            // Crear modal
            const modal = document.createElement('div');
            modal.className = 'modal';
            modal.id = 'addFormToRoleModal';
            
            modal.innerHTML = `
                <div class="modal-content">
                    <span class="close-modal">&times;</span>
                    <h3>Agregar Formulario al Rol</h3>
                    
                    <form id="addFormToRoleForm">
                        <div class="form-group">
                            <label for="formSelect">Formulario:</label>
                            <select id="formSelect" required>
                                <option value="">Seleccione un formulario</option>
                                ${formOptions}
                            </select>
                        </div>
                        
                        <div class="form-group">
                            <h4>Permisos:</h4>
                            <div class="permissions-checkboxes">
                                <div class="checkbox-group">
                                    <input type="checkbox" id="newPermCanRead" checked>
                                    <label for="newPermCanRead">Lectura</label>
                                </div>
                                <div class="checkbox-group">
                                    <input type="checkbox" id="newPermCanCreate">
                                    <label for="newPermCanCreate">Creación</label>
                                </div>
                                <div class="checkbox-group">
                                    <input type="checkbox" id="newPermCanUpdate">
                                    <label for="newPermCanUpdate">Actualización</label>
                                </div>
                                <div class="checkbox-group">
                                    <input type="checkbox" id="newPermCanDelete">
                                    <label for="newPermCanDelete">Eliminación</label>
                                </div>
                            </div>
                        </div>
                        
                        <div class="form-group">
                            <button type="submit" class="button primary">Agregar</button>
                            <button type="button" class="button" id="cancelAddFormBtn">Cancelar</button>
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
            
            // Manejar cierre desde botón cancelar
            modal.querySelector('#cancelAddFormBtn').addEventListener('click', () => {
                modal.remove();
            });
            
            // Manejar envío del formulario
            modal.querySelector('#addFormToRoleForm').addEventListener('submit', async (e) => {
                e.preventDefault();
                
                const formId = document.getElementById('formSelect').value;
                const canRead = document.getElementById('newPermCanRead').checked;
                const canCreate = document.getElementById('newPermCanCreate').checked;
                const canUpdate = document.getElementById('newPermCanUpdate').checked;
                const canDelete = document.getElementById('newPermCanDelete').checked;
                
                try {
                    // 1. Crear el permiso
                    const permissionData = {
                        CanRead: canRead,
                        CanCreate: canCreate,
                        CanUpdate: canUpdate,
                        CanDelete: canDelete
                    };
                    
                    const permission = await apiRequest(API_CONFIG.ENDPOINTS.PERMISSION, 'POST', permissionData);
                    
                    if (!permission) {
                        throw new Error('No se pudo crear el permiso');
                    }
                    
                    // 2. Asignar el permiso al rol y formulario
                    const rolFormPermissionData = {
                        RolId: parseInt(roleId),
                        FormId: parseInt(formId),
                        PermissionId: permission.id
                    };
                    
                    const rolFormPermission = await apiRequest(API_CONFIG.ENDPOINTS.ROL_FORM_PERMISSION, 'POST', rolFormPermissionData);
                    
                    if (!rolFormPermission) {
                        // Si falla, intentar eliminar el permiso creado
                        await apiRequest(`${API_CONFIG.ENDPOINTS.PERMISSION}/${permission.id}`, 'DELETE');
                        throw new Error('No se pudo asignar el permiso al rol y formulario');
                    }
                    
                    // Cerrar modal y recargar permisos
                    modal.remove();
                    loadRoleFormPermissions(roleId);
                    
                } catch (error) {
                    console.error('Error al agregar formulario al rol:', error);
                    alert(error.message || 'Error al agregar formulario al rol');
                }
            });
            
        } catch (error) {
            console.error('Error al mostrar modal de agregar formulario:', error);
            alert('Error al cargar formularios disponibles');
        }
    }
    
    function showAddRoleModal() {
        // Crear modal
        const modal = document.createElement('div');
        modal.className = 'modal';
        modal.id = 'addRoleModal';
        
        modal.innerHTML = `
            <div class="modal-content">
                <span class="close-modal">&times;</span>
                <h3>Agregar Nuevo Rol</h3>
                
                <form id="addRoleForm">
                    <div class="form-group">
                        <label for="roleName">Nombre:</label>
                        <input type="text" id="roleName" name="name" required>
                    </div>
                    
                    <div class="form-group">
                        <label for="roleDescription">Descripción:</label>
                        <textarea id="roleDescription" name="description" rows="3"></textarea>
                    </div>
                    
                    <div class="form-group">
                        <button type="submit" class="button primary">Guardar Rol</button>
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
        modal.querySelector('#addRoleForm').addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const name = document.getElementById('roleName').value;
            const description = document.getElementById('roleDescription').value;
            
            try {
                const roleData = {
                    Name: name,
                    Description: description
                };
                
                console.log('Enviando datos de rol:', roleData);
                const response = await apiRequest(API_CONFIG.ENDPOINTS.ROL, 'POST', roleData);
                
                if (response) {
                    // Cerrar modal y recargar datos
                    modal.remove();
                    loadRolesData();
                    showMessage('roleMessage', 'Rol creado correctamente.', 'success');
                } else {
                    alert('Error al crear el rol.');
                }
            } catch (error) {
                console.error('Error al crear rol:', error);
                alert(`Error al crear rol: ${error.message || 'Error desconocido'}`);
            }
        });
    }


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
                
                console.log('Enviando datos de permiso:', permissionData);
                const response = await apiRequest(API_CONFIG.ENDPOINTS.PERMISSION, 'POST', permissionData);
                
                if (response) {
                    // Cerrar modal y recargar datos
                    modal.remove();
                    // Si estamos en la sección de permisos, recargar la lista
                    if (document.getElementById('permissionsSection').style.display !== 'none') {
                        loadPermissionsData();
                    }
                    showMessage('permissionMessage', 'Permiso creado correctamente.', 'success');
                } else {
                    alert('Error al crear el permiso.');
                }
            } catch (error) {
                console.error('Error al crear permiso:', error);
                alert(`Error al crear permiso: ${error.message || 'Error desconocido'}`);
            }
        });
    }

    // Agregar estos event listeners después de cargar los datos
document.getElementById('addRoleBtn').addEventListener('click', showAddRoleModal);
document.getElementById('addUserBtn').addEventListener('click', showAddUserModal);

// Definir la función para agregar usuario si no existe
function showAddUserModal() {
    // Crear modal
    const modal = document.createElement('div');
    modal.className = 'modal';
    modal.id = 'addUserModal';
    
    modal.innerHTML = `
        <div class="modal-content">
            <span class="close-modal">&times;</span>
            <h3>Agregar Nuevo Usuario</h3>
            
            <form id="addUserForm">
                <div class="form-group">
                    <label for="userName">Nombre:</label>
                    <input type="text" id="userName" name="name" required>
                </div>
                
                <div class="form-group">
                    <label for="userEmail">Correo Electrónico:</label>
                    <input type="email" id="userEmail" name="email" required>
                </div>
                
                <div class="form-group">
                    <label for="userPassword">Contraseña:</label>
                    <input type="password" id="userPassword" name="password" required>
                </div>
                
                <div class="form-group">
                    <button type="submit" class="button primary">Guardar Usuario</button>
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
    modal.querySelector('#addUserForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const name = document.getElementById('userName').value;
        const email = document.getElementById('userEmail').value;
        const password = document.getElementById('userPassword').value;
        
        try {
            const userData = {
                Name: name,
                Email: email,
                Password: password
            };
            
            const response = await apiRequest(API_CONFIG.ENDPOINTS.USER, 'POST', userData);
            
            if (response) {
                // Cerrar modal y recargar datos
                modal.remove();
                loadUsersData();
                showMessage('userMessage', 'Usuario creado correctamente.', 'success');
            } else {
                alert('Error al crear el usuario.');
            }
        } catch (error) {
            console.error('Error al crear usuario:', error);
            alert(`Error al crear usuario: ${error.message || 'Error desconocido'}`);
        }
    });
}

    /**
     * Maneja el evento de eliminar una asignación de permiso
     */
    async function handleRemoveFormPermission(event) {
        const permissionId = event.target.getAttribute('data-id');
        
        if (!permissionId) {
            console.error('ID de permiso no encontrado');
            return;
        }
        
        if (!confirm('¿Está seguro de que desea eliminar este permiso del rol?')) {
            return;
        }
        
        try {
            // Obtener la información del RolFormPermission
            const rfpInfo = window.currentRoleFormPermissions.find(p => p.id.toString() === permissionId);
            
            if (!rfpInfo) {
                console.error('No se encontró la información del permiso');
                return;
            }
            
            // Eliminar la asignación
            const deleted = await apiRequest(`${API_CONFIG.ENDPOINTS.ROL_FORM_PERMISSION}/${permissionId}`, 'DELETE');
            
            if (!deleted) {
                throw new Error('No se pudo eliminar la asignación');
            }
            
            // También eliminar el permiso (opcional, dependiendo de los requisitos)
            await apiRequest(`${API_CONFIG.ENDPOINTS.PERMISSION}/${rfpInfo.permissionId}`, 'DELETE');
            
            // Recargar permisos
            loadRoleFormPermissions(window.editingRole.id);
            
        } catch (error) {
            console.error('Error al eliminar permiso:', error);
            alert(error.message || 'Error al eliminar el permiso');
        }
    }
    
    /**
     * Maneja el evento de guardar todos los cambios en los permisos del rol
     */
    async function handleSaveRolePermissions(event) {
        event.preventDefault();
        
        try {
            // Cerrar el modal
            document.getElementById('editRolePermissionsModal').style.display = 'none';
            
            // Mostrar mensaje de éxito
            showMessage('roleMessage', 'Permisos actualizados correctamente', 'success');
            
            // Recargar datos de roles por si hay cambios
            loadRolesData();
            
        } catch (error) {
            console.error('Error al guardar permisos del rol:', error);
            showMessage('roleMessage', error.message || 'Error al guardar los permisos del rol', 'error');
        }
    }
    
    /**
     * Maneja el evento de eliminar un rol
     */
    async function handleDeleteRole(event) {
        const roleId = event.target.getAttribute('data-id');
        
        if (confirm('¿Está seguro de que desea eliminar este rol? Esta acción no se puede deshacer.')) {
            try {
                // Intentar eliminar el rol
                const deleted = await apiRequest(`${API_CONFIG.ENDPOINTS.ROL}/${roleId}`, 'DELETE');
                
                if (deleted) {
                    // Recargar la lista de roles
                    loadRolesData();
                    showMessage('roleMessage', 'Rol eliminado correctamente.', 'success');
                } else {
                    showMessage('roleMessage', 'No se pudo eliminar el rol.', 'error');
                }
            } catch (error) {
                console.error('Error al eliminar rol:', error);
                showMessage('roleMessage', error.message || 'Error al eliminar rol', 'error');
            }
        }
    }
});

/**
 * Actualiza la visibilidad de los elementos del menú según los permisos del usuario
 */
async function updateMenuVisibility() {
    // Obtener el usuario actual
    const user = getFromStorage('user');
    if (!user || !user.rolId) return;
    
    try {
        // Obtener los permisos de formularios asignados al rol del usuario
        const rolFormPermissions = await apiRequest(`${API_CONFIG.ENDPOINTS.ROL_FORM_PERMISSION}/rol/${user.rolId}`);
        
        if (!rolFormPermissions || rolFormPermissions.length === 0) {
            console.log("El usuario no tiene permisos específicos asignados");
            return;
        }
        
        // Crear un mapa de formularios accesibles
        const accessibleForms = new Map();
        
        // Obtener detalles de los formularios y permisos
        for (const rfp of rolFormPermissions) {
            // Verificar si el usuario tiene al menos permiso de lectura
            const permission = await apiRequest(`${API_CONFIG.ENDPOINTS.PERMISSION}/${rfp.permissionId}`);
            if (permission && permission.canRead) {
                const form = await apiRequest(`${API_CONFIG.ENDPOINTS.FORM}/${rfp.formId}`);
                if (form) {
                    accessibleForms.set(form.code, true);
                }
            }
        }
        
        // Mapear elementos del menú a códigos de formulario
        const menuMapping = {
            'users': 'USUARIOS',
            'roles': 'ROLES',
            'modules': 'MODULOS',
            'forms': 'FORMULARIOS',
            'permissions': 'PERMISOS'
        };
        
        // Actualizar visibilidad de elementos del menú
        document.querySelectorAll('.menu a[data-section]').forEach(menuItem => {
            const section = menuItem.getAttribute('data-section');
            if (section === 'dashboard' || section === 'profile') {
                // Secciones básicas siempre visibles
                menuItem.parentElement.style.display = 'block';
            } else if (menuMapping[section]) {
                // Verificar si el usuario tiene acceso al formulario correspondiente
                const hasAccess = accessibleForms.has(menuMapping[section]) || isAdmin();
                menuItem.parentElement.style.display = hasAccess ? 'block' : 'none';
            }
        });
    } catch (error) {
        console.error('Error al actualizar visibilidad del menú:', error);
    }
}

// Llamar a la función cuando se carga la página
updateMenuVisibility();


// Agregar al final de dashboard.js
document.addEventListener('DOMContentLoaded', function() {
    console.log("Dashboard script loaded successfully");
    
    // Verificar que los botones existen
    if (document.getElementById('addRoleBtn')) {
        console.log("Add Role button found, adding event listener");
        document.getElementById('addRoleBtn').addEventListener('click', showAddRoleModal);
    } else {
        console.error("Add Role button not found");
    }
    
    if (document.getElementById('addUserBtn')) {
        console.log("Add User button found, adding event listener");
        document.getElementById('addUserBtn').addEventListener('click', showAddUserModal);
    } else {
        console.error("Add User button not found");
    }
});


/**
 * Muestra un modal para agregar un nuevo rol
 */
function showAddRoleModal() {
    console.log("Ejecutando showAddRoleModal");
    
    // Crear modal
    const modal = document.createElement('div');
    modal.className = 'modal';
    modal.id = 'addRoleModal';
    
    modal.innerHTML = `
        <div class="modal-content">
            <span class="close-modal">&times;</span>
            <h3>Agregar Nuevo Rol</h3>
            
            <form id="addRoleForm">
                <div class="form-group">
                    <label for="roleName">Nombre:</label>
                    <input type="text" id="roleName" name="name" required>
                </div>
                
                <div class="form-group">
                    <label for="roleDescription">Descripción:</label>
                    <textarea id="roleDescription" name="description" rows="3"></textarea>
                </div>
                
                <div class="form-group">
                    <button type="submit" class="button primary">Guardar Rol</button>
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
    modal.querySelector('#addRoleForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const name = document.getElementById('roleName').value;
        const description = document.getElementById('roleDescription').value;
        
        try {
            const roleData = {
                Name: name,
                Description: description
            };
            
            console.log('Enviando datos de rol:', roleData);
            const response = await apiRequest(API_CONFIG.ENDPOINTS.ROL, 'POST', roleData);
            
            if (response) {
                // Cerrar modal y recargar datos
                modal.remove();
                if (typeof loadRolesData === 'function') {
                    loadRolesData();
                } else {
                    console.warn('Función loadRolesData no encontrada');
                    location.reload(); // Recargar página como alternativa
                }
                alert('Rol creado correctamente.');
            } else {
                alert('Error al crear el rol.');
            }
        } catch (error) {
            console.error('Error al crear rol:', error);
            alert(`Error al crear rol: ${error.message || 'Error desconocido'}`);
        }
    });
}

/**
 * Muestra un modal para agregar un nuevo usuario
 */
function showAddUserModal() {
    console.log("Ejecutando showAddUserModal");
    
    // Crear modal
    const modal = document.createElement('div');
    modal.className = 'modal';
    modal.id = 'addUserModal';
    
    modal.innerHTML = `
        <div class="modal-content">
            <span class="close-modal">&times;</span>
            <h3>Agregar Nuevo Usuario</h3>
            
            <form id="addUserForm">
                <div class="form-group">
                    <label for="userName">Nombre:</label>
                    <input type="text" id="userName" name="name" required>
                </div>
                
                <div class="form-group">
                    <label for="userEmail">Correo Electrónico:</label>
                    <input type="email" id="userEmail" name="email" required>
                </div>
                
                <div class="form-group">
                    <label for="userPassword">Contraseña:</label>
                    <input type="password" id="userPassword" name="password" required>
                </div>
                
                <div class="form-group">
                    <button type="submit" class="button primary">Guardar Usuario</button>
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
    modal.querySelector('#addUserForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const name = document.getElementById('userName').value;
        const email = document.getElementById('userEmail').value;
        const password = document.getElementById('userPassword').value;
        
        try {
            const userData = {
                Name: name,
                Email: email,
                Password: password
            };
            
            console.log('Enviando datos de usuario:', userData);
            const response = await apiRequest(API_CONFIG.ENDPOINTS.USER, 'POST', userData);
            
            if (response) {
                // Cerrar modal y recargar datos
                modal.remove();
                if (typeof loadUsersData === 'function') {
                    loadUsersData();
                } else {
                    console.warn('Función loadUsersData no encontrada');
                    location.reload(); // Recargar página como alternativa
                }
                alert('Usuario creado correctamente.');
            } else {
                alert('Error al crear el usuario.');
            }
        } catch (error) {
            console.error('Error al crear usuario:', error);
            alert(`Error al crear usuario: ${error.message || 'Error desconocido'}`);
        }
    });
}

// Exponer las funciones globalmente
window.showAddRoleModal = showAddRoleModal;
window.showAddUserModal = showAddUserModal;


/**
 * Verifica si el usuario tiene acceso a una sección específica
 * @param {string} sectionCode - Código de la sección (que corresponde a un código de formulario)
 * @returns {boolean} - True si tiene acceso, false en caso contrario
 */
function hasAccessToSection(sectionCode) {
    // Administradores tienen acceso a todo
    if (isAdmin()) return true;
    
    // Para usuarios regulares, verificar permisos específicos
    if (!window.userPermissions) return false;
    
    // Buscar si tiene acceso al formulario con ese código
    return window.userPermissions.some(p => 
        p.formCode.toUpperCase() === sectionCode.toUpperCase() && 
        p.canRead === true
    );
}

/**
 * Actualiza la visibilidad del menú según los permisos
 */
function updateMenuByPermissions() {
    // Mapeo entre secciones del menú y códigos de formulario
    const sectionMapping = {
        'dashboard': 'INICIO',
        'profile': 'PERFIL',
        'users': 'USUARIOS',
        'roles': 'ROLES',
        'modules': 'MODULOS',
        'forms': 'FORMULARIOS',
        'permissions': 'PERMISOS'
    };
    
    // Actualizar visibilidad de elementos del menú
    document.querySelectorAll('.menu a[data-section]').forEach(menuItem => {
        const section = menuItem.getAttribute('data-section');
        const formCode = sectionMapping[section];
        
        // Si no hay mapeo, dejar visible
        if (!formCode) return;
        
        // Verificar permisos
        const hasAccess = hasAccessToSection(formCode);
        
        // Actualizar visibilidad
        menuItem.parentElement.style.display = hasAccess ? 'block' : 'none';
    });
}

// Llamar a esta función después de cargar los permisos
document.addEventListener('permissionsLoaded', updateMenuByPermissions);