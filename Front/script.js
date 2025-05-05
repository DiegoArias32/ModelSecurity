// Configuración global
const API_BASE_URL = 'http://localhost:5163/api';
let currentRole = null;
let menuData = null;
let modalInstance = null;
let confirmModalInstance = null;
let logDetailModalInstance = null;

// Elementos del DOM
const sidebarMenu = document.getElementById('sidebarMenu');
const pageContent = document.getElementById('pageContent');
const roleSelect = document.getElementById('roleSelect');
const messageModal = document.getElementById('messageModal');
const confirmModal = document.getElementById('confirmModal');
const logDetailModal = document.getElementById('logDetailModal');

// Inicialización
document.addEventListener('DOMContentLoaded', async function() {
    try {
        // Inicializar los modales
        modalInstance = new bootstrap.Modal(messageModal);
        confirmModalInstance = new bootstrap.Modal(confirmModal);
        logDetailModalInstance = new bootstrap.Modal(logDetailModal);
        
        // Cargar roles disponibles
        await loadRoles();
        
        // Manejar cambios en el selector de rol
        roleSelect.addEventListener('change', async function() {
            const rolId = parseInt(this.value);
            currentRole = rolId;
            await loadMenuForRole(rolId);
        });
        
        // Cargar el menú inicial con el primer rol seleccionado
        if (roleSelect.value) {
            currentRole = parseInt(roleSelect.value);
            await loadMenuForRole(currentRole);
        }
        
        // Mostrar página inicial
        renderHomePage();
    } catch (error) {
        console.error('Error en la inicialización:', error);
        showError('Error de inicialización', error);
    }
});

// Función para cargar roles desde la API
async function loadRoles() {
    try {
        // Mostrar indicador de carga en el selector
        roleSelect.innerHTML = '<option value="">Cargando roles...</option>';
        roleSelect.disabled = true;
        
        // Llamar a la API para obtener roles
        const response = await fetch(`${API_BASE_URL}/Rol`);
        
        if (!response.ok) {
            throw new Error(`Error al cargar roles: ${response.status} ${response.statusText}`);
        }
        
        const roles = await response.json();
        
        // Limpiar y llenar el selector de roles
        roleSelect.innerHTML = '';
        
        if (roles && roles.length > 0) {
            roles.forEach(rol => {
                const option = document.createElement('option');
                option.value = rol.id;
                option.textContent = rol.name;
                roleSelect.appendChild(option);
            });
            
            roleSelect.disabled = false;
        } else {
            roleSelect.innerHTML = '<option value="">No hay roles disponibles</option>';
            showMessage('No hay roles', 'No se encontraron roles en el sistema. Por favor, cree al menos un rol.');
        }
    } catch (error) {
        console.error('Error al cargar roles:', error);
        roleSelect.innerHTML = '<option value="">Error al cargar roles</option>';
        showError('Error al cargar roles', error);
    }
}

// Función para cargar el menú según el rol
async function loadMenuForRole(rolId) {
    try {
        // Mostrar indicador de carga
        sidebarMenu.innerHTML = `
            <li class="nav-item">
                <div class="text-center p-3">
                    <div class="spinner-border text-light" role="status"></div>
                    <div class="mt-2">Cargando menú...</div>
                </div>
            </li>
        `;
        
        // Llamada real a la API de menú
        const response = await fetch(`${API_BASE_URL}/Menu/byrol/${rolId}`);
        
        if (!response.ok) {
            throw new Error(`Error ${response.status}: ${response.statusText}`);
        }
        
        // Guardar los datos del menú para uso posterior
        menuData = await response.json();
        
        // Limpiar el menú actual
        sidebarMenu.innerHTML = '';
        
        // Generar y añadir los elementos del menú
        if (menuData && menuData.length > 0) {
            menuData.forEach(item => {
                const menuItem = createMenuItem(item);
                sidebarMenu.appendChild(menuItem);
            });
            
            // Añadir elemento de menú para logs de actividad (solo para administradores)
            if (rolId === 1) { // Asumir que el rol con ID 1 es el administrador
                const logsMenuItem = document.createElement('li');
                logsMenuItem.className = 'nav-item mt-3';
                logsMenuItem.innerHTML = `
                    <a class="nav-link" href="#" data-url="admin/logs">
                        <i class="fas fa-history me-2"></i>Logs de Actividad
                    </a>
                `;
                
                logsMenuItem.querySelector('.nav-link').addEventListener('click', function(e) {
                    e.preventDefault();
                    
                    // Desactivar elementos activos anteriores
                    document.querySelectorAll('.nav-link.active').forEach(el => {
                        el.classList.remove('active');
                    });
                    
                    // Marcar este elemento como activo
                    this.classList.add('active');
                    
                    // Cargar la página de logs
                    loadLogsPage();
                });
                
                sidebarMenu.appendChild(logsMenuItem);
            }
        } else {
            sidebarMenu.innerHTML = `
                <li class="nav-item">
                    <div class="alert alert-warning text-dark">
                        No hay elementos de menú disponibles para este rol.
                    </div>
                </li>
            `;
        }
        
        // Cargar la página inicial
        renderHomePage();
    } catch (error) {
        console.error('Error al cargar el menú:', error);
        sidebarMenu.innerHTML = `
            <li class="nav-item">
                <div class="alert alert-danger">
                    Error al cargar el menú: ${error.message}
                </div>
            </li>
        `;
        showError('Error al cargar el menú', error);
    }
}

// Función para crear un elemento de menú
function createMenuItem(item) {
    const li = document.createElement('li');
    li.className = 'nav-item';
    
    if (item.children && item.children.length > 0) {
        // Es un menú con submenús
        li.innerHTML = `
            <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                <i class="fas fa-${item.icon || 'circle'} me-2"></i>${item.name}
            </a>
            <ul class="dropdown-menu">
                ${item.children.map(child => `
                    <li>
                        <a class="dropdown-item" href="#" data-url="${child.url}">
                            <i class="fas fa-${child.icon || 'circle'} me-2"></i>${child.name}
                        </a>
                    </li>
                `).join('')}
            </ul>
        `;
        
        // Agregar eventos a los elementos del submenú
        setTimeout(() => {
            const dropdownItems = li.querySelectorAll('.dropdown-item');
            dropdownItems.forEach(link => {
                link.addEventListener('click', function(e) {
                    e.preventDefault();
                    const url = this.getAttribute('data-url');
                    
                    // Desactivar elementos activos anteriores
                    document.querySelectorAll('.dropdown-item.active').forEach(el => {
                        el.classList.remove('active');
                    });
                    
                    // Marcar este elemento como activo
                    this.classList.add('active');
                    
                    // Cargar la página
                    loadPage(url);
                });
            });
        }, 0);
        
    } else {
        // Es un elemento de menú simple
        li.innerHTML = `
            <a class="nav-link" href="#" data-url="${item.url}">
                <i class="fas fa-${item.icon || 'circle'} me-2"></i>${item.name}
            </a>
        `;
        
        // Agregar evento al elemento de menú
        setTimeout(() => {
            const link = li.querySelector('.nav-link');
            link.addEventListener('click', function(e) {
                e.preventDefault();
                const url = this.getAttribute('data-url');
                
                // Desactivar elementos activos anteriores
                document.querySelectorAll('.nav-link.active').forEach(el => {
                    el.classList.remove('active');
                });
                
                // Marcar este elemento como activo
                this.classList.add('active');
                
                // Cargar la página
                loadPage(url);
            });
        }, 0);
    }
    
    return li;
}

// Función para cargar y renderizar una página
async function loadPage(url) {
    try {
        // Mostrar indicador de carga
        pageContent.innerHTML = `
            <div class="text-center mt-5">
                <div class="spinner-border" role="status"></div>
                <div class="mt-2">Cargando contenido...</div>
            </div>
        `;
        
        // Procesar URL para eliminar barras iniciales si existen
        url = url.startsWith('/') ? url.substring(1) : url;
        
        // Si es la página de inicio
        if (url === '#' || url === '' || url === 'home') {
            renderHomePage();
            return;
        }
        
        // Determinar qué módulo cargar
        if (url.startsWith('admin/users') || url === 'admin/users') {
            await loadUsersPage();
        } else if (url.startsWith('admin/roles') || url === 'admin/roles') {
            await loadRolesPage();
        } else if (url.startsWith('admin/modules') || url === 'admin/modules') {
            await loadModulesPage();
        } else if (url.startsWith('admin/forms') || url === 'admin/forms') {
            await loadFormsPage();
        } else if (url.startsWith('admin/logs') || url === 'admin/logs') {
            await loadLogsPage();
        } else if (url.startsWith('profile') || url === 'profile') {
            await loadProfilePage();
        } else {
            // Página no reconocida
            pageContent.innerHTML = `
                <div class="text-center mt-5">
                    <h2>Página no encontrada</h2>
                    <p>La página "${url}" no está implementada o no existe.</p>
                </div>
            `;
        }
    } catch (error) {
        console.error(`Error al cargar la página ${url}:`, error);
        pageContent.innerHTML = `
            <div class="api-error">
                <h4>Error al cargar la página</h4>
                <p>No se pudo cargar la página "${url}".</p>
                <hr>
                <p>${error.message}</p>
                <div class="error-details">${error.stack}</div>
            </div>
        `;
    }
}

// Función para mostrar la página de inicio
function renderHomePage() {
    const roleName = roleSelect.options[roleSelect.selectedIndex]?.text || 'Seleccionado';
    
    pageContent.innerHTML = `
        <div class="text-center mt-5">
            <h2 class="page-header">Bienvenido al Sistema de Gestión</h2>
            <p class="lead">Seleccione una opción del menú para comenzar</p>
            <div class="alert alert-info">
                Rol actual: <strong>${roleName}</strong>
            </div>
            
            <div class="mt-4">
                <h4>Información del sistema</h4>
                <div class="row mt-3">
                    <div class="col-md-4">
                        <div class="card bg-primary text-white">
                            <div class="card-body text-center">
                                <i class="fas fa-users fa-3x mb-3"></i>
                                <h5 class="card-title">Gestión de Usuarios</h5>
                                <p class="card-text">Administre los usuarios del sistema.</p>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="card bg-success text-white">
                            <div class="card-body text-center">
                                <i class="fas fa-user-tag fa-3x mb-3"></i>
                                <h5 class="card-title">Gestión de Roles</h5>
                                <p class="card-text">Configure los permisos y roles.</p>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="card bg-info text-white">
                            <div class="card-body text-center">
                                <i class="fas fa-file-alt fa-3x mb-3"></i>
                                <h5 class="card-title">Formularios</h5>
                                <p class="card-text">Administre los formularios del sistema.</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
}

// Función para cargar la página de usuarios
async function loadUsersPage() {
    try {
        // Llamada a la API para obtener usuarios
        const response = await fetch(`${API_BASE_URL}/User`);
        
        if (!response.ok) {
            throw new Error(`Error al obtener usuarios: ${response.status} ${response.statusText}`);
        }
        
        const users = await response.json();
        
        // Crear filas de la tabla con los datos reales
        let userRows = '';
        
        if (users && users.length > 0) {
            userRows = users.map(user => `
                <tr>
                    <td>${user.id}</td>
                    <td>${user.name}</td>
                    <td>${user.email}</td>
                    <td id="userRole-${user.id}"><span class="badge bg-secondary">Cargando...</span></td>
                    <td>
                        <button class="btn btn-sm btn-primary" onclick="editUser(${user.id})">
                            <i class="fas fa-edit"></i> Editar
                        </button>
                        <button class="btn btn-sm btn-danger" onclick="deleteUser(${user.id})">
                            <i class="fas fa-trash"></i> Eliminar
                        </button>
                    </td>
                </tr>
            `).join('');
        } else {
            userRows = '<tr><td colspan="5" class="text-center">No hay usuarios registrados</td></tr>';
        }
        
        // Renderizar la página completa
        pageContent.innerHTML = `
            <h2 class="page-header">Gestión de Usuarios</h2>
            <p>Aquí puede administrar los usuarios del sistema.</p>
            
            <div class="mb-3">
                <button class="btn btn-success" id="btnAddUser">
                    <i class="fas fa-plus me-1"></i> Nuevo Usuario
                </button>
            </div>
            
            <div class="table-responsive">
                <table class="table table-striped table-hover">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Nombre</th>
                            <th>Email</th>
                            <th>Rol</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${userRows}
                    </tbody>
                </table>
            </div>
        `;
        
        // Cargar roles de usuarios
        if (users && users.length > 0) {
            users.forEach(user => {
                loadUserRole(user.id);
            });
        }
        
        // Agregar evento para el botón "Nuevo Usuario"
        document.getElementById('btnAddUser')?.addEventListener('click', showCreateUserModal);
    } catch (error) {
        console.error('Error al cargar usuarios:', error);
        showError('Error al cargar usuarios', error);
    }
}

// Función para cargar el rol de un usuario
async function loadUserRole(userId) {
    try {
        // En un sistema real, aquí obtendrías el rol del usuario mediante una llamada a la API
        const rolUserResponse = await fetch(`${API_BASE_URL}/RolUser`);
        
        if (!rolUserResponse.ok) {
            throw new Error(`Error al obtener roles de usuarios: ${rolUserResponse.status} ${rolUserResponse.statusText}`);
        }
        
        const rolUsers = await rolUserResponse.json();
        const rolesResponse = await fetch(`${API_BASE_URL}/Rol`);
        
        if (!rolesResponse.ok) {
            throw new Error(`Error al obtener roles: ${rolesResponse.status} ${rolesResponse.statusText}`);
        }
        
        const roles = await rolesResponse.json();
        
        const userRoles = rolUsers.filter(ru => ru.userId === userId);
        
        const userRoleCell = document.getElementById(`userRole-${userId}`);
        if (userRoleCell) {
            if (userRoles && userRoles.length > 0) {
                const roleNames = userRoles.map(ur => {
                    const role = roles.find(r => r.id === ur.rolId);
                    return role ? role.name : 'Desconocido';
                });
                
                userRoleCell.innerHTML = roleNames.map(name => 
                    `<span class="badge bg-info">${name}</span>`
                ).join(' ');
            } else {
                userRoleCell.innerHTML = '<span class="badge bg-warning">Sin rol</span>';
            }
        }
    } catch (error) {
        console.error(`Error al cargar rol del usuario ${userId}:`, error);
        const userRoleCell = document.getElementById(`userRole-${userId}`);
        if (userRoleCell) {
            userRoleCell.innerHTML = '<span class="badge bg-danger">Error</span>';
        }
    }
}

// Función para mostrar el modal de creación de usuario
async function showCreateUserModal() {
    try {
        // Obtener los roles disponibles
        const response = await fetch(`${API_BASE_URL}/Rol`);
        if (!response.ok) {
            throw new Error(`Error al cargar roles: ${response.status} ${response.statusText}`);
        }
        const roles = await response.json();

        // Generar opciones para el selector de roles
        const roleOptions = roles.map(role => `<option value="${role.id}">${role.name}</option>`).join('');

        // Crear y mostrar el modal
        const modalHTML = `
            <div class="modal fade" id="createUserModal" tabindex="-1" aria-labelledby="createUserModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="createUserModalLabel">Crear Nuevo Usuario</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <form id="createUserForm">
                                <div class="mb-3">
                                    <label for="userName" class="form-label">Nombre</label>
                                    <input type="text" class="form-control" id="userName" required>
                                </div>
                                <div class="mb-3">
                                    <label for="userEmail" class="form-label">Correo Electrónico</label>
                                    <input type="email" class="form-control" id="userEmail" required>
                                </div>
                                <div class="mb-3">
                                    <label for="userPassword" class="form-label">Contraseña</label>
                                    <input type="password" class="form-control" id="userPassword" required>
                                </div>
                                <div class="mb-3">
                                    <label for="userRole" class="form-label">Rol</label>
                                    <select class="form-select" id="userRole" required>
                                        <option value="">Seleccione un rol</option>
                                        ${roleOptions}
                                    </select>
                                </div>
                            </form>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <button type="button" class="btn btn-primary" id="saveUserBtn">Guardar</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Añadir el modal al DOM
        const modalContainer = document.createElement('div');
        modalContainer.innerHTML = modalHTML;
        document.body.appendChild(modalContainer);

        // Inicializar y mostrar el modal
        const createUserModal = new bootstrap.Modal(document.getElementById('createUserModal'));
        createUserModal.show();

        // Manejar el evento de guardar
        document.getElementById('saveUserBtn').addEventListener('click', async () => {
            const userName = document.getElementById('userName').value.trim();
            const userEmail = document.getElementById('userEmail').value.trim();
            const userPassword = document.getElementById('userPassword').value.trim();
            const userRoleId = parseInt(document.getElementById('userRole').value);

            if (!userName || !userEmail || !userPassword || isNaN(userRoleId)) {
                showMessage('Error', 'Por favor complete todos los campos correctamente.');
                return;
            }

            try {
                // Crear el usuario
                const userResponse = await fetch(`${API_BASE_URL}/User`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        name: userName,
                        email: userEmail,
                        password: userPassword
                    })
                });

                if (!userResponse.ok) {
                    throw new Error(`Error al crear usuario: ${userResponse.status} ${userResponse.statusText}`);
                }

                const newUser = await userResponse.json();

                // Asignar el rol al usuario
                const rolUserResponse = await fetch(`${API_BASE_URL}/RolUser`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        userId: newUser.id,
                        rolId: userRoleId
                    })
                });

                if (!rolUserResponse.ok) {
                    throw new Error(`Error al asignar rol: ${rolUserResponse.status} ${rolUserResponse.statusText}`);
                }

                // Registrar la actividad
                await logActivity('Create', 'User', newUser.id, `Creado usuario ${userName}`);

                // Cerrar el modal y actualizar la vista
                createUserModal.hide();
                modalContainer.remove();
                showMessage('Éxito', 'Usuario creado correctamente.');
                await loadUsersPage(); // Recargar la página para mostrar el nuevo usuario
            } catch (error) {
                console.error('Error al crear usuario:', error);
                showError('Error al crear usuario', error);
            }
        });

        // Manejar el cierre del modal para eliminar el elemento del DOM
        document.getElementById('createUserModal').addEventListener('hidden.bs.modal', () => {
            modalContainer.remove();
        });
    } catch (error) {
        console.error('Error al preparar el modal de creación de usuario:', error);
        showError('Error', error);
    }
}

// Función para editar un usuario
window.editUser = async function(userId) {
    try {
        // Obtener los datos del usuario
        const userResponse = await fetch(`${API_BASE_URL}/User/${userId}`);
        if (!userResponse.ok) {
            throw new Error(`Error al obtener usuario: ${userResponse.status} ${userResponse.statusText}`);
        }
        const user = await userResponse.json();

        // Obtener los roles
        const rolesResponse = await fetch(`${API_BASE_URL}/Rol`);
        if (!rolesResponse.ok) {
            throw new Error(`Error al cargar roles: ${rolesResponse.status} ${rolesResponse.statusText}`);
        }
        const roles = await rolesResponse.json();

        // Obtener la asignación de rol del usuario
        const rolUserResponse = await fetch(`${API_BASE_URL}/RolUser`);
        if (!rolUserResponse.ok) {
            throw new Error(`Error al obtener roles de usuarios: ${rolUserResponse.status} ${rolUserResponse.statusText}`);
        }
        const rolUsers = await rolUserResponse.json();
        const userRolUser = rolUsers.find(ru => ru.userId === userId);

        // Generar opciones de roles
        const roleOptions = roles.map(role => `
            <option value="${role.id}" ${userRolUser && userRolUser.rolId === role.id ? 'selected' : ''}>
                ${role.name}
            </option>
        `).join('');

        // Crear modal de edición
        const modalHTML = `
            <div class="modal fade" id="editUserModal" tabindex="-1" aria-labelledby="editUserModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="editUserModalLabel">Editar Usuario</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <form id="editUserForm">
                                <div class="mb-3">
                                    <label for="editUserName" class="form-label">Nombre</label>
                                    <input type="text" class="form-control" id="editUserName" value="${user.name}" required>
                                </div>
                                <div class="mb-3">
                                    <label for="editUserEmail" class="form-label">Correo Electrónico</label>
                                    <input type="email" class="form-control" id="editUserEmail" value="${user.email}" required>
                                </div>
                                <div class="mb-3">
                                    <label for="editUserPassword" class="form-label">Contraseña (dejar en blanco para mantener la actual)</label>
                                    <input type="password" class="form-control" id="editUserPassword" placeholder="****">
                                </div>
                                <div class="mb-3">
                                    <label for="editUserRole" class="form-label">Rol</label>
                                    <select class="form-select" id="editUserRole" required>
                                        <option value="">Seleccione un rol</option>
                                        ${roleOptions}
                                    </select>
                                </div>
                            </form>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <button type="button" class="btn btn-primary" id="updateUserBtn">Guardar Cambios</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Añadir el modal al DOM
        const modalContainer = document.createElement('div');
        modalContainer.innerHTML = modalHTML;
        document.body.appendChild(modalContainer);

        // Inicializar y mostrar el modal
        const editUserModal = new bootstrap.Modal(document.getElementById('editUserModal'));
        editUserModal.show();

        // Manejar el evento de actualizar
        document.getElementById('updateUserBtn').addEventListener('click', async () => {
            const updatedName = document.getElementById('editUserName').value.trim();
            const updatedEmail = document.getElementById('editUserEmail').value.trim();
            const updatedPassword = document.getElementById('editUserPassword').value.trim();
            const updatedRoleId = parseInt(document.getElementById('editUserRole').value);

            if (!updatedName || !updatedEmail || isNaN(updatedRoleId)) {
                showMessage('Error', 'Por favor complete los campos obligatorios.');
                return;
            }

            try {
                // Actualizar el usuario
                const updatedUser = {
                    id: user.id,
                    name: updatedName,
                    email: updatedEmail,
                    password: updatedPassword || user.password // Mantener la contraseña si no se proporciona una nueva
                };

                const userUpdateResponse = await fetch(`${API_BASE_URL}/User`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(updatedUser)
                });

                if (!userUpdateResponse.ok) {
                    throw new Error(`Error al actualizar usuario: ${userUpdateResponse.status} ${userUpdateResponse.statusText}`);
                }

                // Actualizar o crear la asignación de rol
                if (userRolUser) {
                    // Actualizar rol existente
                    if (userRolUser.rolId !== updatedRoleId) {
                        const rolUserUpdateResponse = await fetch(`${API_BASE_URL}/RolUser`, {
                            method: 'PUT',
                            headers: {
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify({
                                id: userRolUser.id,
                                userId: userId,
                                rolId: updatedRoleId
                            })
                        });

                        if (!rolUserUpdateResponse.ok) {
                            throw new Error(`Error al actualizar rol: ${rolUserUpdateResponse.status} ${rolUserUpdateResponse.statusText}`);
                        }
                    }
                } else {
                    // Crear nueva asignación de rol
                    const rolUserCreateResponse = await fetch(`${API_BASE_URL}/RolUser`, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({
                            userId: userId,
                            rolId: updatedRoleId
                        })
                    });

                    if (!rolUserCreateResponse.ok) {
                        throw new Error(`Error al asignar rol: ${rolUserCreateResponse.status} ${rolUserCreateResponse.statusText}`);
                    }
                }

                // Registrar la actividad
                await logActivity('Update', 'User', userId, `Actualizado usuario ${updatedName}`);

                // Cerrar el modal y actualizar la vista
                editUserModal.hide();
                modalContainer.remove();
                showMessage('Éxito', 'Usuario actualizado correctamente.');
                await loadUsersPage(); // Recargar la página para mostrar los cambios
            } catch (error) {
                console.error('Error al actualizar usuario:', error);
                showError('Error al actualizar usuario', error);
            }
        });

        // Manejar el cierre del modal para eliminar el elemento del DOM
        document.getElementById('editUserModal').addEventListener('hidden.bs.modal', () => {
            modalContainer.remove();
        });
    } catch (error) {
        console.error('Error al preparar el modal de edición de usuario:', error);
        showError('Error', error);
    }
};

// Función para eliminar un usuario
window.deleteUser = async function(userId) {
    try {
        // Obtener información del usuario para el log
        const userResponse = await fetch(`${API_BASE_URL}/User/${userId}`);
        if (!userResponse.ok) {
            throw new Error(`Error al obtener usuario: ${userResponse.status} ${userResponse.statusText}`);
        }
        const user = await userResponse.json();
        
        // Configurar el modal de confirmación
        document.getElementById('confirmModalLabel').textContent = 'Confirmar Eliminación';
        document.getElementById('confirmModalBody').innerHTML = `
            <p>¿Está seguro de que desea eliminar el usuario <strong>${user.name}</strong>?</p>
            <p>Esta acción no se puede deshacer.</p>
        `;
        
        // Configurar el botón de confirmación
        const confirmButton = document.getElementById('confirmButton');
        confirmButton.textContent = 'Eliminar';
        confirmButton.className = 'btn btn-danger';
        
        // Mostrar el modal
        confirmModalInstance.show();
        
        // Manejar el evento de confirmación
        const handleConfirm = async () => {
            try {
                // Primero, eliminar las asignaciones de rol del usuario
                const rolUserResponse = await fetch(`${API_BASE_URL}/RolUser`);
                if (!rolUserResponse.ok) {
                    throw new Error(`Error al obtener roles de usuarios: ${rolUserResponse.status} ${rolUserResponse.statusText}`);
                }
                
                const rolUsers = await rolUserResponse.json();
                const userRolUsers = rolUsers.filter(ru => ru.userId === userId);
                
                // Eliminar cada asignación de rol
                for (const rolUser of userRolUsers) {
                    const deleteRolUserResponse = await fetch(`${API_BASE_URL}/RolUser/${rolUser.id}`, {
                        method: 'DELETE'
                    });
                    
                    if (!deleteRolUserResponse.ok) {
                        throw new Error(`Error al eliminar asignación de rol: ${deleteRolUserResponse.status} ${deleteRolUserResponse.statusText}`);
                    }
                }
                
                // Luego, eliminar el usuario
                const deleteUserResponse = await fetch(`${API_BASE_URL}/User/${userId}`, {
                    method: 'DELETE'
                });
                
                if (!deleteUserResponse.ok) {
                    throw new Error(`Error al eliminar usuario: ${deleteUserResponse.status} ${deleteUserResponse.statusText}`);
                }
                
                // Registrar la actividad
                await logActivity('Delete', 'User', userId, `Eliminado usuario ${user.name}`);
                
                // Cerrar el modal y mostrar mensaje de éxito
                confirmModalInstance.hide();
                showMessage('Éxito', 'Usuario eliminado correctamente.');
                
                // Recargar la página de usuarios
                await loadUsersPage();
            } catch (error) {
                console.error('Error al eliminar usuario:', error);
                confirmModalInstance.hide();
                showError('Error al eliminar usuario', error);
            }
            
            // Remover el evento para evitar múltiples ejecuciones
            confirmButton.removeEventListener('click', handleConfirm);
        };
        
        // Añadir el evento al botón
        confirmButton.addEventListener('click', handleConfirm);
    } catch (error) {
        console.error('Error al preparar eliminación de usuario:', error);
        showError('Error', error);
    }
};

// Función para cargar la página de roles
async function loadRolesPage() {
    try {
        // Llamada a la API para obtener roles
        const response = await fetch(`${API_BASE_URL}/Rol`);
        
        if (!response.ok) {
            throw new Error(`Error al obtener roles: ${response.status} ${response.statusText}`);
        }
        
        const roles = await response.json();
        
        // Crear filas de la tabla con los datos reales
        let roleRows = '';
        
        if (roles && roles.length > 0) {
            roleRows = roles.map(role => `
                <tr>
                    <td>${role.id}</td>
                    <td>${role.name}</td>
                    <td>${role.description || 'Sin descripción'}</td>
                    <td>
                        <button class="btn btn-sm btn-primary" onclick="editRole(${role.id})">
                            <i class="fas fa-edit"></i> Editar
                        </button>
                        <button class="btn btn-sm btn-danger" onclick="deleteRole(${role.id})">
                            <i class="fas fa-trash"></i> Eliminar
                        </button>
                    </td>
                </tr>
            `).join('');
        } else {
            roleRows = '<tr><td colspan="4" class="text-center">No hay roles registrados</td></tr>';
        }
        
        // Renderizar la página completa
        pageContent.innerHTML = `
            <h2 class="page-header">Gestión de Roles</h2>
            <p>Aquí puede administrar los roles del sistema.</p>
            
            <div class="mb-3">
                <button class="btn btn-success" id="btnAddRole">
                    <i class="fas fa-plus me-1"></i> Nuevo Rol
                </button>
            </div>
            
            <div class="table-responsive">
                <table class="table table-striped table-hover">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Nombre</th>
                            <th>Descripción</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${roleRows}
                    </tbody>
                </table>
            </div>
        `;
        
        // Agregar eventos a botones
        document.getElementById('btnAddRole')?.addEventListener('click', showCreateRoleModal);
    } catch (error) {
        console.error('Error al cargar roles:', error);
        showError('Error al cargar roles', error);
    }
}

// Función para mostrar el modal de creación de rol
function showCreateRoleModal() {
    try {
        // Crear modal de creación
        const modalHTML = `
            <div class="modal fade" id="createRoleModal" tabindex="-1" aria-labelledby="createRoleModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="createRoleModalLabel">Crear Nuevo Rol</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <form id="createRoleForm">
                                <div class="mb-3">
                                    <label for="roleName" class="form-label">Nombre</label>
                                    <input type="text" class="form-control" id="roleName" required>
                                </div>
                                <div class="mb-3">
                                    <label for="roleDescription" class="form-label">Descripción</label>
                                    <textarea class="form-control" id="roleDescription" rows="3"></textarea>
                                </div>
                            </form>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <button type="button" class="btn btn-primary" id="saveRoleBtn">Guardar</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Añadir el modal al DOM
        const modalContainer = document.createElement('div');
        modalContainer.innerHTML = modalHTML;
        document.body.appendChild(modalContainer);

        // Inicializar y mostrar el modal
        const createRoleModal = new bootstrap.Modal(document.getElementById('createRoleModal'));
        createRoleModal.show();

        // Manejar el evento de guardar
        document.getElementById('saveRoleBtn').addEventListener('click', async () => {
            const roleName = document.getElementById('roleName').value.trim();
            const roleDescription = document.getElementById('roleDescription').value.trim();

            if (!roleName) {
                showMessage('Error', 'El nombre del rol es obligatorio.');
                return;
            }

            try {
                // Crear el rol
                const roleResponse = await fetch(`${API_BASE_URL}/Rol`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        name: roleName,
                        description: roleDescription
                    })
                });

                if (!roleResponse.ok) {
                    throw new Error(`Error al crear rol: ${roleResponse.status} ${roleResponse.statusText}`);
                }

                const newRole = await roleResponse.json();

                // Registrar la actividad
                await logActivity('Create', 'Role', newRole.id, `Creado rol ${roleName}`);

                // Cerrar el modal y actualizar la vista
                createRoleModal.hide();
                modalContainer.remove();
                showMessage('Éxito', 'Rol creado correctamente.');
                await loadRolesPage(); // Recargar la página para mostrar el nuevo rol
            } catch (error) {
                console.error('Error al crear rol:', error);
                showError('Error al crear rol', error);
            }
        });

        // Manejar el cierre del modal para eliminar el elemento del DOM
        document.getElementById('createRoleModal').addEventListener('hidden.bs.modal', () => {
            modalContainer.remove();
        });
    } catch (error) {
        console.error('Error al preparar el modal de creación de rol:', error);
        showError('Error', error);
    }
}

// Función para editar un rol
window.editRole = async function(roleId) {
    try {
        // Obtener los datos del rol
        const roleResponse = await fetch(`${API_BASE_URL}/Rol/${roleId}`);
        if (!roleResponse.ok) {
            throw new Error(`Error al obtener rol: ${roleResponse.status} ${roleResponse.statusText}`);
        }
        const role = await roleResponse.json();

        // Crear modal de edición
        const modalHTML = `
            <div class="modal fade" id="editRoleModal" tabindex="-1" aria-labelledby="editRoleModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="editRoleModalLabel">Editar Rol</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <form id="editRoleForm">
                                <div class="mb-3">
                                    <label for="editRoleName" class="form-label">Nombre</label>
                                    <input type="text" class="form-control" id="editRoleName" value="${role.name}" required>
                                </div>
                                <div class="mb-3">
                                    <label for="editRoleDescription" class="form-label">Descripción</label>
                                    <textarea class="form-control" id="editRoleDescription" rows="3">${role.description || ''}</textarea>
                                </div>
                            </form>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <button type="button" class="btn btn-primary" id="updateRoleBtn">Guardar Cambios</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Añadir el modal al DOM
        const modalContainer = document.createElement('div');
        modalContainer.innerHTML = modalHTML;
        document.body.appendChild(modalContainer);

        // Inicializar y mostrar el modal
        const editRoleModal = new bootstrap.Modal(document.getElementById('editRoleModal'));
        editRoleModal.show();

        // Manejar el evento de actualizar
        document.getElementById('updateRoleBtn').addEventListener('click', async () => {
            const updatedName = document.getElementById('editRoleName').value.trim();
            const updatedDescription = document.getElementById('editRoleDescription').value.trim();

            if (!updatedName) {
                showMessage('Error', 'El nombre del rol es obligatorio.');
                return;
            }

            try {
                // Actualizar el rol
                const updatedRole = {
                    id: role.id,
                    name: updatedName,
                    description: updatedDescription
                };

                const roleUpdateResponse = await fetch(`${API_BASE_URL}/Rol`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(updatedRole)
                });

                if (!roleUpdateResponse.ok) {
                    throw new Error(`Error al actualizar rol: ${roleUpdateResponse.status} ${roleUpdateResponse.statusText}`);
                }

                // Registrar la actividad
                await logActivity('Update', 'Role', roleId, `Actualizado rol ${updatedName}`);

                // Cerrar el modal y actualizar la vista
                editRoleModal.hide();
                modalContainer.remove();
                showMessage('Éxito', 'Rol actualizado correctamente.');
                await loadRolesPage(); // Recargar la página para mostrar los cambios
            } catch (error) {
                console.error('Error al actualizar rol:', error);
                showError('Error al actualizar rol', error);
            }
        });

        // Manejar el cierre del modal para eliminar el elemento del DOM
        document.getElementById('editRoleModal').addEventListener('hidden.bs.modal', () => {
            modalContainer.remove();
        });
    } catch (error) {
        console.error('Error al preparar el modal de edición de rol:', error);
        showError('Error', error);
    }
};

// Función para eliminar un rol
window.deleteRole = async function(roleId) {
    try {
        // Obtener información del rol para el log
        const roleResponse = await fetch(`${API_BASE_URL}/Rol/${roleId}`);
        if (!roleResponse.ok) {
            throw new Error(`Error al obtener rol: ${roleResponse.status} ${roleResponse.statusText}`);
        }
        const role = await roleResponse.json();
        
        // Configurar el modal de confirmación
        document.getElementById('confirmModalLabel').textContent = 'Confirmar Eliminación';
        document.getElementById('confirmModalBody').innerHTML = `
            <p>¿Está seguro de que desea eliminar el rol <strong>${role.name}</strong>?</p>
            <p>Esta acción eliminará también todas las asignaciones de este rol a usuarios.</p>
            <p>Esta acción no se puede deshacer.</p>
        `;
        
        // Configurar el botón de confirmación
        const confirmButton = document.getElementById('confirmButton');
        confirmButton.textContent = 'Eliminar';
        confirmButton.className = 'btn btn-danger';
        
        // Mostrar el modal
        confirmModalInstance.show();
        
        // Manejar el evento de confirmación
        const handleConfirm = async () => {
            try {
                // Primero, eliminar las asignaciones de este rol a usuarios
                const rolUserResponse = await fetch(`${API_BASE_URL}/RolUser`);
                if (!rolUserResponse.ok) {
                    throw new Error(`Error al obtener asignaciones de rol: ${rolUserResponse.status} ${rolUserResponse.statusText}`);
                }
                
                const rolUsers = await rolUserResponse.json();
                const roleRolUsers = rolUsers.filter(ru => ru.rolId === roleId);
                
                // Eliminar cada asignación
                for (const rolUser of roleRolUsers) {
                    const deleteRolUserResponse = await fetch(`${API_BASE_URL}/RolUser/${rolUser.id}`, {
                        method: 'DELETE'
                    });
                    
                    if (!deleteRolUserResponse.ok) {
                        throw new Error(`Error al eliminar asignación de rol: ${deleteRolUserResponse.status} ${deleteRolUserResponse.statusText}`);
                    }
                }
                
                // Luego, eliminar el rol
                const deleteRoleResponse = await fetch(`${API_BASE_URL}/Rol/${roleId}`, {
                    method: 'DELETE'
                });
                
                if (!deleteRoleResponse.ok) {
                    throw new Error(`Error al eliminar rol: ${deleteRoleResponse.status} ${deleteRoleResponse.statusText}`);
                }
                
                // Registrar la actividad
                await logActivity('Delete', 'Role', roleId, `Eliminado rol ${role.name}`);
                
                // Cerrar el modal y mostrar mensaje de éxito
                confirmModalInstance.hide();
                showMessage('Éxito', 'Rol eliminado correctamente.');
                
                // Recargar la página de roles
                await loadRolesPage();
            } catch (error) {
                console.error('Error al eliminar rol:', error);
                confirmModalInstance.hide();
                showError('Error al eliminar rol', error);
            }
            
            // Remover el evento para evitar múltiples ejecuciones
            confirmButton.removeEventListener('click', handleConfirm);
        };
        
        // Añadir el evento al botón
        confirmButton.addEventListener('click', handleConfirm);
    } catch (error) {
        console.error('Error al preparar eliminación de rol:', error);
        showError('Error', error);
    }
};

// Función para cargar la página de módulos
async function loadModulesPage() {
    try {
        // Llamada a la API para obtener módulos
        const response = await fetch(`${API_BASE_URL}/Module`);
        
        if (!response.ok) {
            throw new Error(`Error al obtener módulos: ${response.status} ${response.statusText}`);
        }
        
        const modules = await response.json();
        
        // Crear filas de la tabla con los datos reales
        let moduleRows = '';
        
        if (modules && modules.length > 0) {
            moduleRows = modules.map(module => `
                <tr>
                    <td>${module.id}</td>
                    <td>${module.code}</td>
                    <td><span class="badge ${module.active ? 'bg-success' : 'bg-danger'}">${module.active ? 'Activo' : 'Inactivo'}</span></td>
                    <td>
                        <button class="btn btn-sm btn-primary" onclick="editModule(${module.id})">
                            <i class="fas fa-edit"></i> Editar
                        </button>
                        <button class="btn btn-sm btn-danger" onclick="deleteModule(${module.id})">
                            <i class="fas fa-trash"></i> Eliminar
                        </button>
                    </td>
                </tr>
            `).join('');
        } else {
            moduleRows = '<tr><td colspan="4" class="text-center">No hay módulos registrados</td></tr>';
        }
        
        // Renderizar la página completa
        pageContent.innerHTML = `
            <h2 class="page-header">Gestión de Módulos</h2>
            <p>Aquí puede administrar los módulos del sistema.</p>
            
            <div class="mb-3">
                <button class="btn btn-success" id="btnAddModule">
                    <i class="fas fa-plus me-1"></i> Nuevo Módulo
                </button>
            </div>
            
            <div class="table-responsive">
                <table class="table table-striped table-hover">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Código</th>
                            <th>Estado</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${moduleRows}
                    </tbody>
                </table>
            </div>
        `;
        
        // Agregar eventos a botones
        document.getElementById('btnAddModule')?.addEventListener('click', showCreateModuleModal);
    } catch (error) {
        console.error('Error al cargar módulos:', error);
        showError('Error al cargar módulos', error);
    }
}

// Función para mostrar el modal de creación de módulo
function showCreateModuleModal() {
    try {
        // Crear modal de creación
        const modalHTML = `
            <div class="modal fade" id="createModuleModal" tabindex="-1" aria-labelledby="createModuleModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="createModuleModalLabel">Crear Nuevo Módulo</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <form id="createModuleForm">
                                <div class="mb-3">
                                    <label for="moduleCode" class="form-label">Código</label>
                                    <input type="text" class="form-control" id="moduleCode" required>
                                </div>
                                <div class="mb-3">
                                    <div class="form-check form-switch">
                                        <input class="form-check-input" type="checkbox" id="moduleActive" checked>
                                        <label class="form-check-label" for="moduleActive">Activo</label>
                                    </div>
                                </div>
                            </form>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <button type="button" class="btn btn-primary" id="saveModuleBtn">Guardar</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Añadir el modal al DOM
        const modalContainer = document.createElement('div');
        modalContainer.innerHTML = modalHTML;
        document.body.appendChild(modalContainer);

        // Inicializar y mostrar el modal
        const createModuleModal = new bootstrap.Modal(document.getElementById('createModuleModal'));
        createModuleModal.show();

        // Manejar el evento de guardar
        document.getElementById('saveModuleBtn').addEventListener('click', async () => {
            const moduleCode = document.getElementById('moduleCode').value.trim();
            const moduleActive = document.getElementById('moduleActive').checked;

            if (!moduleCode) {
                showMessage('Error', 'El código del módulo es obligatorio.');
                return;
            }

            try {
                // Crear el módulo
                const moduleResponse = await fetch(`${API_BASE_URL}/Module`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        code: moduleCode,
                        active: moduleActive
                    })
                });

                if (!moduleResponse.ok) {
                    throw new Error(`Error al crear módulo: ${moduleResponse.status} ${moduleResponse.statusText}`);
                }

                const newModule = await moduleResponse.json();

                // Registrar la actividad
                await logActivity('Create', 'Module', newModule.id, `Creado módulo ${moduleCode}`);

                // Cerrar el modal y actualizar la vista
                createModuleModal.hide();
                modalContainer.remove();
                showMessage('Éxito', 'Módulo creado correctamente.');
                await loadModulesPage(); // Recargar la página para mostrar el nuevo módulo
            } catch (error) {
                console.error('Error al crear módulo:', error);
                showError('Error al crear módulo', error);
            }
        });

        // Manejar el cierre del modal para eliminar el elemento del DOM
        document.getElementById('createModuleModal').addEventListener('hidden.bs.modal', () => {
            modalContainer.remove();
        });
    } catch (error) {
        console.error('Error al preparar el modal de creación de módulo:', error);
        showError('Error', error);
    }
}

// Función para editar un módulo
window.editModule = async function(moduleId) {
    try {
        // Obtener los datos del módulo
        const moduleResponse = await fetch(`${API_BASE_URL}/Module/${moduleId}`);
        if (!moduleResponse.ok) {
            throw new Error(`Error al obtener módulo: ${moduleResponse.status} ${moduleResponse.statusText}`);
        }
        const module = await moduleResponse.json();

        // Crear modal de edición
        const modalHTML = `
            <div class="modal fade" id="editModuleModal" tabindex="-1" aria-labelledby="editModuleModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="editModuleModalLabel">Editar Módulo</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <form id="editModuleForm">
                                <div class="mb-3">
                                    <label for="editModuleCode" class="form-label">Código</label>
                                    <input type="text" class="form-control" id="editModuleCode" value="${module.code}" required>
                                </div>
                                <div class="mb-3">
                                    <div class="form-check form-switch">
                                        <input class="form-check-input" type="checkbox" id="editModuleActive" ${module.active ? 'checked' : ''}>
                                        <label class="form-check-label" for="editModuleActive">Activo</label>
                                    </div>
                                </div>
                            </form>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <button type="button" class="btn btn-primary" id="updateModuleBtn">Guardar Cambios</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Añadir el modal al DOM
        const modalContainer = document.createElement('div');
        modalContainer.innerHTML = modalHTML;
        document.body.appendChild(modalContainer);

        // Inicializar y mostrar el modal
        const editModuleModal = new bootstrap.Modal(document.getElementById('editModuleModal'));
        editModuleModal.show();

        // Manejar el evento de actualizar
        document.getElementById('updateModuleBtn').addEventListener('click', async () => {
            const updatedCode = document.getElementById('editModuleCode').value.trim();
            const updatedActive = document.getElementById('editModuleActive').checked;

            if (!updatedCode) {
                showMessage('Error', 'El código del módulo es obligatorio.');
                return;
            }

            try {
                // Actualizar el módulo
                const updatedModule = {
                    id: module.id,
                    code: updatedCode,
                    active: updatedActive
                };

                const moduleUpdateResponse = await fetch(`${API_BASE_URL}/Module/${module.id}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(updatedModule)
                });

                if (!moduleUpdateResponse.ok) {
                    throw new Error(`Error al actualizar módulo: ${moduleUpdateResponse.status} ${moduleUpdateResponse.statusText}`);
                }

                // Registrar la actividad
                await logActivity('Update', 'Module', moduleId, `Actualizado módulo ${updatedCode}`);

                // Cerrar el modal y actualizar la vista
                editModuleModal.hide();
                modalContainer.remove();
                showMessage('Éxito', 'Módulo actualizado correctamente.');
                await loadModulesPage(); // Recargar la página para mostrar los cambios
            } catch (error) {
                console.error('Error al actualizar módulo:', error);
                showError('Error al actualizar módulo', error);
            }
        });

        // Manejar el cierre del modal para eliminar el elemento del DOM
        document.getElementById('editModuleModal').addEventListener('hidden.bs.modal', () => {
            modalContainer.remove();
        });
    } catch (error) {
        console.error('Error al preparar el modal de edición de módulo:', error);
        showError('Error', error);
    }
};

// Función para eliminar un módulo
window.deleteModule = async function(moduleId) {
    try {
        // Obtener información del módulo para el log
        const moduleResponse = await fetch(`${API_BASE_URL}/Module/${moduleId}`);
        if (!moduleResponse.ok) {
            throw new Error(`Error al obtener módulo: ${moduleResponse.status} ${moduleResponse.statusText}`);
        }
        const module = await moduleResponse.json();
        
        // Configurar el modal de confirmación
        document.getElementById('confirmModalLabel').textContent = 'Confirmar Eliminación';
        document.getElementById('confirmModalBody').innerHTML = `
            <p>¿Está seguro de que desea eliminar el módulo <strong>${module.code}</strong>?</p>
            <p>Esta acción no se puede deshacer.</p>
        `;
        
        // Configurar el botón de confirmación
        const confirmButton = document.getElementById('confirmButton');
        confirmButton.textContent = 'Eliminar';
        confirmButton.className = 'btn btn-danger';
        
        // Mostrar el modal
        confirmModalInstance.show();
        
        // Manejar el evento de confirmación
        const handleConfirm = async () => {
            try {
                // Eliminar el módulo
                const deleteModuleResponse = await fetch(`${API_BASE_URL}/Module/${moduleId}`, {
                    method: 'DELETE'
                });
                
                if (!deleteModuleResponse.ok) {
                    throw new Error(`Error al eliminar módulo: ${deleteModuleResponse.status} ${deleteModuleResponse.statusText}`);
                }
                
                // Registrar la actividad
                await logActivity('Delete', 'Module', moduleId, `Eliminado módulo ${module.code}`);
                
                // Cerrar el modal y mostrar mensaje de éxito
                confirmModalInstance.hide();
                showMessage('Éxito', 'Módulo eliminado correctamente.');
                
                // Recargar la página de módulos
                await loadModulesPage();
            } catch (error) {
                console.error('Error al eliminar módulo:', error);
                confirmModalInstance.hide();
                showError('Error al eliminar módulo', error);
            }
            
            // Remover el evento para evitar múltiples ejecuciones
            confirmButton.removeEventListener('click', handleConfirm);
        };
        
        // Añadir el evento al botón
        confirmButton.addEventListener('click', handleConfirm);
    } catch (error) {
        console.error('Error al preparar eliminación de módulo:', error);
        showError('Error', error);
    }
};

// Función para cargar la página de formularios
async function loadFormsPage() {
    try {
        // Llamada a la API para obtener formularios
        const response = await fetch(`${API_BASE_URL}/Form`);
        
        if (!response.ok) {
            throw new Error(`Error al obtener formularios: ${response.status} ${response.statusText}`);
        }
        
        const forms = await response.json();
        
        // Crear filas de la tabla con los datos reales
        let formRows = '';
        
        if (forms && forms.length > 0) {
            formRows = forms.map(form => `
                <tr>
                    <td>${form.id}</td>
                    <td>${form.name}</td>
                    <td>${form.code}</td>
                    <td><span class="badge ${form.active ? 'bg-success' : 'bg-danger'}">${form.active ? 'Activo' : 'Inactivo'}</span></td>
                    <td>
                        <button class="btn btn-sm btn-primary" onclick="editForm(${form.id})">
                            <i class="fas fa-edit"></i> Editar
                        </button>
                        <button class="btn btn-sm btn-danger" onclick="deleteForm(${form.id})">
                            <i class="fas fa-trash"></i> Eliminar
                        </button>
                    </td>
                </tr>
            `).join('');
        } else {
            formRows = '<tr><td colspan="5" class="text-center">No hay formularios registrados</td></tr>';
        }
        
        // Renderizar la página completa
        pageContent.innerHTML = `
            <h2 class="page-header">Gestión de Formularios</h2>
            <p>Aquí puede administrar los formularios del sistema.</p>
            
            <div class="mb-3">
                <button class="btn btn-success" id="btnAddForm">
                    <i class="fas fa-plus me-1"></i> Nuevo Formulario
                </button>
            </div>
            
            <div class="table-responsive">
                <table class="table table-striped table-hover">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Nombre</th>
                            <th>Código</th>
                            <th>Estado</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${formRows}
                    </tbody>
                </table>
            </div>
        `;
        
        // Agregar eventos a botones
        document.getElementById('btnAddForm')?.addEventListener('click', showCreateFormModal);
    } catch (error) {
        console.error('Error al cargar formularios:', error);
        showError('Error al cargar formularios', error);
    }
}

// Función para mostrar el modal de creación de formulario
function showCreateFormModal() {
    try {
        // Crear modal de creación
        const modalHTML = `
            <div class="modal fade" id="createFormModal" tabindex="-1" aria-labelledby="createFormModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="createFormModalLabel">Crear Nuevo Formulario</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <form id="createFormForm">
                                <div class="mb-3">
                                    <label for="formName" class="form-label">Nombre</label>
                                    <input type="text" class="form-control" id="formName" required>
                                </div>
                                <div class="mb-3">
                                    <label for="formCode" class="form-label">Código</label>
                                    <input type="text" class="form-control" id="formCode" required>
                                </div>
                                <div class="mb-3">
                                    <div class="form-check form-switch">
                                        <input class="form-check-input" type="checkbox" id="formActive" checked>
                                        <label class="form-check-label" for="formActive">Activo</label>
                                    </div>
                                </div>
                            </form>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <button type="button" class="btn btn-primary" id="saveFormBtn">Guardar</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Añadir el modal al DOM
        const modalContainer = document.createElement('div');
        modalContainer.innerHTML = modalHTML;
        document.body.appendChild(modalContainer);

        // Inicializar y mostrar el modal
        const createFormModal = new bootstrap.Modal(document.getElementById('createFormModal'));
        createFormModal.show();

        // Manejar el evento de guardar
        document.getElementById('saveFormBtn').addEventListener('click', async () => {
            const formName = document.getElementById('formName').value.trim();
            const formCode = document.getElementById('formCode').value.trim();
            const formActive = document.getElementById('formActive').checked;

            if (!formName || !formCode) {
                showMessage('Error', 'El nombre y código del formulario son obligatorios.');
                return;
            }

            try {
                // Crear el formulario
                const formResponse = await fetch(`${API_BASE_URL}/Form`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        name: formName,
                        code: formCode,
                        active: formActive
                    })
                });

                if (!formResponse.ok) {
                    throw new Error(`Error al crear formulario: ${formResponse.status} ${formResponse.statusText}`);
                }

                const newForm = await formResponse.json();

                // Registrar la actividad
                await logActivity('Create', 'Form', newForm.id, `Creado formulario ${formName}`);

                // Cerrar el modal y actualizar la vista
                createFormModal.hide();
                modalContainer.remove();
                showMessage('Éxito', 'Formulario creado correctamente.');
                await loadFormsPage(); // Recargar la página para mostrar el nuevo formulario
            } catch (error) {
                console.error('Error al crear formulario:', error);
                showError('Error al crear formulario', error);
            }
        });

        // Manejar el cierre del modal para eliminar el elemento del DOM
        document.getElementById('createFormModal').addEventListener('hidden.bs.modal', () => {
            modalContainer.remove();
        });
    } catch (error) {
        console.error('Error al preparar el modal de creación de formulario:', error);
        showError('Error', error);
    }
}

// Función para editar un formulario
window.editForm = async function(formId) {
    try {
        // Obtener los datos del formulario
        const formResponse = await fetch(`${API_BASE_URL}/Form/${formId}`);
        if (!formResponse.ok) {
            throw new Error(`Error al obtener formulario: ${formResponse.status} ${formResponse.statusText}`);
        }
        const form = await formResponse.json();

        // Crear modal de edición
        const modalHTML = `
            <div class="modal fade" id="editFormModal" tabindex="-1" aria-labelledby="editFormModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="editFormModalLabel">Editar Formulario</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <form id="editFormForm">
                                <div class="mb-3">
                                    <label for="editFormName" class="form-label">Nombre</label>
                                    <input type="text" class="form-control" id="editFormName" value="${form.name}" required>
                                </div>
                                <div class="mb-3">
                                    <label for="editFormCode" class="form-label">Código</label>
                                    <input type="text" class="form-control" id="editFormCode" value="${form.code}" required>
                                </div>
                                <div class="mb-3">
                                    <div class="form-check form-switch">
                                        <input class="form-check-input" type="checkbox" id="editFormActive" ${form.active ? 'checked' : ''}>
                                        <label class="form-check-label" for="editFormActive">Activo</label>
                                    </div>
                                </div>
                            </form>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <button type="button" class="btn btn-primary" id="updateFormBtn">Guardar Cambios</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Añadir el modal al DOM
        const modalContainer = document.createElement('div');
        modalContainer.innerHTML = modalHTML;
        document.body.appendChild(modalContainer);

        // Inicializar y mostrar el modal
        const editFormModal = new bootstrap.Modal(document.getElementById('editFormModal'));
        editFormModal.show();

        // Manejar el evento de actualizar
        document.getElementById('updateFormBtn').addEventListener('click', async () => {
            const updatedName = document.getElementById('editFormName').value.trim();
            const updatedCode = document.getElementById('editFormCode').value.trim();
            const updatedActive = document.getElementById('editFormActive').checked;

            if (!updatedName || !updatedCode) {
                showMessage('Error', 'El nombre y código del formulario son obligatorios.');
                return;
            }

            try {
                // Actualizar el formulario
                const updatedForm = {
                    id: form.id,
                    name: updatedName,
                    code: updatedCode,
                    active: updatedActive
                };

                const formUpdateResponse = await fetch(`${API_BASE_URL}/Form`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(updatedForm)
                });

                if (!formUpdateResponse.ok) {
                    throw new Error(`Error al actualizar formulario: ${formUpdateResponse.status} ${formUpdateResponse.statusText}`);
                }

                // Registrar la actividad
                await logActivity('Update', 'Form', formId, `Actualizado formulario ${updatedName}`);

                // Cerrar el modal y actualizar la vista
                editFormModal.hide();
                modalContainer.remove();
                showMessage('Éxito', 'Formulario actualizado correctamente.');
                await loadFormsPage(); // Recargar la página para mostrar los cambios
            } catch (error) {
                console.error('Error al actualizar formulario:', error);
                showError('Error al actualizar formulario', error);
            }
        });

        // Manejar el cierre del modal para eliminar el elemento del DOM
        document.getElementById('editFormModal').addEventListener('hidden.bs.modal', () => {
            modalContainer.remove();
        });
    } catch (error) {
        console.error('Error al preparar el modal de edición de formulario:', error);
        showError('Error', error);
    }
};

// Función para eliminar un formulario
window.deleteForm = async function(formId) {
    try {
        // Obtener información del formulario para el log
        const formResponse = await fetch(`${API_BASE_URL}/Form/${formId}`);
        if (!formResponse.ok) {
            throw new Error(`Error al obtener formulario: ${formResponse.status} ${formResponse.statusText}`);
        }
        const form = await formResponse.json();
        
        // Configurar el modal de confirmación
        document.getElementById('confirmModalLabel').textContent = 'Confirmar Eliminación';
        document.getElementById('confirmModalBody').innerHTML = `
            <p>¿Está seguro de que desea eliminar el formulario <strong>${form.name}</strong>?</p>
            <p>Esta acción no se puede deshacer.</p>
        `;
        
        // Configurar el botón de confirmación
        const confirmButton = document.getElementById('confirmButton');
        confirmButton.textContent = 'Eliminar';
        confirmButton.className = 'btn btn-danger';
        
        // Mostrar el modal
        confirmModalInstance.show();
        
        // Manejar el evento de confirmación
        const handleConfirm = async () => {
            try {
                // Eliminar el formulario
                const deleteFormResponse = await fetch(`${API_BASE_URL}/Form/${formId}`, {
                    method: 'DELETE'
                });
                
                if (!deleteFormResponse.ok) {
                    throw new Error(`Error al eliminar formulario: ${deleteFormResponse.status} ${deleteFormResponse.statusText}`);
                }
                
                // Registrar la actividad
                await logActivity('Delete', 'Form', formId, `Eliminado formulario ${form.name}`);
                
                // Cerrar el modal y mostrar mensaje de éxito
                confirmModalInstance.hide();
                showMessage('Éxito', 'Formulario eliminado correctamente.');
                
                // Recargar la página de formularios
                await loadFormsPage();
            } catch (error) {
                console.error('Error al eliminar formulario:', error);
                confirmModalInstance.hide();
                showError('Error al eliminar formulario', error);
            }
            
            // Remover el evento para evitar múltiples ejecuciones
            confirmButton.removeEventListener('click', handleConfirm);
        };
        
        // Añadir el evento al botón
        confirmButton.addEventListener('click', handleConfirm);
    } catch (error) {
        console.error('Error al preparar eliminación de formulario:', error);
        showError('Error', error);
    }
};

// Función para cargar la página de logs de actividad
async function loadLogsPage() {
    try {
        // Llamada a la API para obtener logs
        const response = await fetch(`${API_BASE_URL}/ActivityLog?limit=50`);
        
        if (!response.ok) {
            throw new Error(`Error al obtener logs: ${response.status} ${response.statusText}`);
        }
        
        const logs = await response.json();
        
        // Crear el contenido de la página
        let logsContent = '';
        
        if (logs && logs.length > 0) {
            logsContent = logs.map(log => `
                <div class="activity-log-entry ${log.action.toLowerCase()}">
                    <div class="row">
                        <div class="col-md-3">
                            <span class="timestamp">${new Date(log.timestamp).toLocaleString()}</span>
                        </div>
                        <div class="col-md-9">
                            <span class="user">${log.userName}</span>
                            <span class="action ${log.action.toLowerCase()}">${log.action}</span>
                            <span class="entity">${log.entityType} #${log.entityId}</span>
                            <p class="mt-1">${log.details}</p>
                            <button class="btn btn-sm btn-link view-log-details" data-log-id="${log.id}">Ver detalles</button>
                        </div>
                    </div>
                </div>
            `).join('');
        } else {
            logsContent = `
                <div class="alert alert-info">
                    No hay registros de actividad disponibles.
                </div>
            `;
        }
        
        // Renderizar la página completa
        pageContent.innerHTML = `
            <h2 class="page-header">Logs de Actividad</h2>
            <p>Registro de actividades y cambios en el sistema.</p>
            
            <div class="activity-log-filters">
                <div class="row">
                    <div class="col-md-3 mb-3">
                        <label for="filterEntityType" class="form-label">Tipo de Entidad</label>
                        <select id="filterEntityType" class="form-select">
                            <option value="">Todos</option>
                            <option value="User">Usuario</option>
                            <option value="Role">Rol</option>
                            <option value="Module">Módulo</option>
                            <option value="Form">Formulario</option>
                        </select>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label for="filterAction" class="form-label">Acción</label>
                        <select id="filterAction" class="form-select">
                            <option value="">Todas</option>
                            <option value="Create">Creación</option>
                            <option value="Update">Actualización</option>
                            <option value="Delete">Eliminación</option>
                        </select>
                    </div>
                    <div class="col-md-4 mb-3">
                        <label for="filterDateRange" class="form-label">Rango de Fechas</label>
                        <div class="input-group">
                            <input type="date" id="filterStartDate" class="form-control">
                            <span class="input-group-text">a</span>
                            <input type="date" id="filterEndDate" class="form-control">
                        </div>
                    </div>
                    <div class="col-md-2 mb-3 d-flex align-items-end">
                        <button id="btnApplyFilters" class="btn btn-primary w-100">Filtrar</button>
                    </div>
                </div>
            </div>
            
            <div class="activity-log">
                ${logsContent}
            </div>
            
            <div class="load-more-logs" id="loadMoreLogs">
                Cargar más registros
            </div>
        `;
        
        // Agregar eventos a los botones de filtro
        document.getElementById('btnApplyFilters')?.addEventListener('click', filterLogs);
        
        // Agregar eventos a los botones de ver detalles
        document.querySelectorAll('.view-log-details').forEach(button => {
            button.addEventListener('click', function() {
                const logId = this.getAttribute('data-log-id');
                viewLogDetails(logId);
            });
        });
        
        // Agregar evento al botón de cargar más
        document.getElementById('loadMoreLogs')?.addEventListener('click', loadMoreLogs);
    } catch (error) {
        console.error('Error al cargar logs:', error);
        showError('Error al cargar logs', error);
    }
}

// Función para filtrar logs de actividad
async function filterLogs() {
    try {
        const entityType = document.getElementById('filterEntityType').value;
        const action = document.getElementById('filterAction').value;
        const startDate = document.getElementById('filterStartDate').value;
        const endDate = document.getElementById('filterEndDate').value;
        
        // Construir la URL con los filtros
        let url = `${API_BASE_URL}/ActivityLog?limit=50`;
        
        if (entityType) {
            url += `&entityType=${entityType}`;
        }
        
        if (action) {
            url += `&action=${action}`;
        }
        
        if (startDate && endDate) {
            url += `&start=${startDate}T00:00:00&end=${endDate}T23:59:59`;
        }
        
        // Realizar la llamada a la API
        const response = await fetch(url);
        
        if (!response.ok) {
            throw new Error(`Error al obtener logs filtrados: ${response.status} ${response.statusText}`);
        }
        
        const logs = await response.json();
        
        // Actualizar la sección de logs con los resultados filtrados
        let logsContent = '';
        
        if (logs && logs.length > 0) {
            logsContent = logs.map(log => `
                <div class="activity-log-entry ${log.action.toLowerCase()}">
                    <div class="row">
                        <div class="col-md-3">
                            <span class="timestamp">${new Date(log.timestamp).toLocaleString()}</span>
                        </div>
                        <div class="col-md-9">
                            <span class="user">${log.userName}</span>
                            <span class="action ${log.action.toLowerCase()}">${log.action}</span>
                            <span class="entity">${log.entityType} #${log.entityId}</span>
                            <p class="mt-1">${log.details}</p>
                            <button class="btn btn-sm btn-link view-log-details" data-log-id="${log.id}">Ver detalles</button>
                        </div>
                    </div>
                </div>
            `).join('');
        } else {
            logsContent = `
                <div class="alert alert-info">
                    No se encontraron registros con los filtros seleccionados.
                </div>
            `;
        }
        
        // Actualizar el contenido
        document.querySelector('.activity-log').innerHTML = logsContent;
        
        // Restaurar los eventos de los botones de ver detalles
        document.querySelectorAll('.view-log-details').forEach(button => {
            button.addEventListener('click', function() {
                const logId = this.getAttribute('data-log-id');
                viewLogDetails(logId);
            });
        });
    } catch (error) {
        console.error('Error al filtrar logs:', error);
        showError('Error al filtrar logs', error);
    }
}

// Función para cargar más logs
async function loadMoreLogs() {
    try {
        // Obtener el número actual de logs mostrados
        const currentLogs = document.querySelectorAll('.activity-log-entry').length;
        
        // Construir la URL para obtener más logs
        let url = `${API_BASE_URL}/ActivityLog?limit=20&offset=${currentLogs}`;
        
        // Aplicar los filtros si están establecidos
        const entityType = document.getElementById('filterEntityType').value;
        const action = document.getElementById('filterAction').value;
        const startDate = document.getElementById('filterStartDate').value;
        const endDate = document.getElementById('filterEndDate').value;
        
        if (entityType) {
            url += `&entityType=${entityType}`;
        }
        
        if (action) {
            url += `&action=${action}`;
        }
        
        if (startDate && endDate) {
            url += `&start=${startDate}T00:00:00&end=${endDate}T23:59:59`;
        }
        
        // Realizar la llamada a la API
        const response = await fetch(url);
        
        if (!response.ok) {
            throw new Error(`Error al obtener más logs: ${response.status} ${response.statusText}`);
        }
        
        const logs = await response.json();
        
        // Si no hay más logs, mostrar mensaje y desactivar el botón
        if (!logs || logs.length === 0) {
            document.getElementById('loadMoreLogs').innerHTML = 'No hay más registros para cargar';
            document.getElementById('loadMoreLogs').disabled = true;
            document.getElementById('loadMoreLogs').style.cursor = 'default';
            return;
        }
        
        // Crear el HTML para los nuevos logs
        const newLogsHTML = logs.map(log => `
            <div class="activity-log-entry ${log.action.toLowerCase()}">
                <div class="row">
                    <div class="col-md-3">
                        <span class="timestamp">${new Date(log.timestamp).toLocaleString()}</span>
                    </div>
                    <div class="col-md-9">
                        <span class="user">${log.userName}</span>
                        <span class="action ${log.action.toLowerCase()}">${log.action}</span>
                        <span class="entity">${log.entityType} #${log.entityId}</span>
                        <p class="mt-1">${log.details}</p>
                        <button class="btn btn-sm btn-link view-log-details" data-log-id="${log.id}">Ver detalles</button>
                    </div>
                </div>
            </div>
        `).join('');
        
        // Añadir los nuevos logs al final de la lista
        document.querySelector('.activity-log').innerHTML += newLogsHTML;
        
        // Restaurar los eventos de los botones de ver detalles
        document.querySelectorAll('.view-log-details').forEach(button => {
            button.addEventListener('click', function() {
                const logId = this.getAttribute('data-log-id');
                viewLogDetails(logId);
            });
        });
    } catch (error) {
        console.error('Error al cargar más logs:', error);
        showError('Error al cargar más logs', error);
    }
}

// Función para ver detalles de un log
async function viewLogDetails(logId) {
    try {
        // Obtener los detalles del log
        const response = await fetch(`${API_BASE_URL}/ActivityLog/${logId}`);
        
        if (!response.ok) {
            throw new Error(`Error al obtener detalles del log: ${response.status} ${response.statusText}`);
        }
        
        const log = await response.json();
        
        // Mostrar detalles en el modal
        document.getElementById('logDetailModalLabel').textContent = `Detalles del Registro #${log.id}`;
        
        // Formatear los detalles JSON si existen
        let formattedDetails = log.details;
        try {
            const detailsObj = JSON.parse(log.details);
            formattedDetails = JSON.stringify(detailsObj, null, 2);
        } catch (e) {
            // No es JSON, mantener como texto
        }
        
        document.getElementById('logDetailModalBody').innerHTML = `
            <div class="card mb-3">
                <div class="card-header">Información básica</div>
                <div class="card-body">
                    <p><strong>Fecha y hora:</strong> ${new Date(log.timestamp).toLocaleString()}</p>
                    <p><strong>Usuario:</strong> ${log.userName} (ID: ${log.userId})</p>
                    <p><strong>Acción:</strong> <span class="badge ${log.action.toLowerCase() === 'create' ? 'bg-success' : log.action.toLowerCase() === 'update' ? 'bg-info' : 'bg-danger'}">${log.action}</span></p>
                    <p><strong>Entidad afectada:</strong> ${log.entityType} #${log.entityId}</p>
                </div>
            </div>
            
            <div class="card">
                <div class="card-header">Detalles de la acción</div>
                <div class="card-body">
                    <pre class="mb-0" style="max-height: 300px; overflow-y: auto;">${formattedDetails}</pre>
                </div>
            </div>
        `;
        
        // Mostrar el modal
        logDetailModalInstance.show();
    } catch (error) {
        console.error('Error al obtener detalles del log:', error);
        showError('Error al obtener detalles', error);
    }
}

// Función para cargar la página de perfil
async function loadProfilePage() {
    try {
        // En una implementación real, obtendríamos los datos del usuario actual
        const userName = "Usuario Actual"; // Esto se reemplazaría con información real del usuario logueado
        const userEmail = "usuario@ejemplo.com";
        const userRole = roleSelect.options[roleSelect.selectedIndex]?.text || 'No asignado';

        // Renderizar la página de perfil
        pageContent.innerHTML = `
            <h2 class="page-header">Mi Perfil</h2>
            <div class="row">
                <div class="col-md-4">
                    <div class="card">
                        <div class="card-body text-center">
                            <i class="fas fa-user-circle fa-5x mb-3"></i>
                            <h5 class="card-title">${userName}</h5>
                            <p class="card-text">${userEmail}</p>
                            <p><span class="badge bg-info">${userRole}</span></p>
                            <button class="btn btn-primary" id="btnEditProfile">Editar Perfil</button>
                        </div>
                    </div>
                    
                    <div class="card mt-3">
                        <div class="card-header">
                            Mis Actividades Recientes
                        </div>
                        <div class="card-body p-0">
                            <ul class="list-group list-group-flush" id="userActivities">
                                <li class="list-group-item text-center">
                                    <div class="spinner-border spinner-border-sm" role="status"></div>
                                    Cargando actividades...
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div class="col-md-8">
                    <div class="card">
                        <div class="card-header">Información Personal</div>
                        <div class="card-body">
                            <form id="profileForm">
                                <div class="mb-3">
                                    <label for="name" class="form-label">Nombre</label>
                                    <input type="text" class="form-control" id="name" value="${userName}" readonly>
                                </div>
                                <div class="mb-3">
                                    <label for="email" class="form-label">Email</label>
                                    <input type="email" class="form-control" id="email" value="${userEmail}" readonly>
                                </div>
                                <div class="mb-3">
                                    <label for="role" class="form-label">Rol</label>
                                    <input type="text" class="form-control" id="role" value="${userRole}" readonly>
                                </div>
                                <div class="mb-3">
                                    <label for="password" class="form-label">Contraseña</label>
                                    <input type="password" class="form-control" id="password" value="********" readonly>
                                </div>
                                <button type="button" class="btn btn-success" id="btnSaveProfile" disabled>Guardar Cambios</button>
                            </form>
                        </div>
                    </div>
                    
                    <div class="card mt-3">
                        <div class="card-header">Preferencias del Sistema</div>
                        <div class="card-body">
                            <div class="form-check form-switch mb-3">
                                <input class="form-check-input" type="checkbox" id="darkModeSwitch">
                                <label class="form-check-label" for="darkModeSwitch">Modo Oscuro</label>
                            </div>
                            <div class="form-check form-switch mb-3">
                                <input class="form-check-input" type="checkbox" id="notificationsSwitch" checked>
                                <label class="form-check-label" for="notificationsSwitch">Recibir Notificaciones</label>
                            </div>
                            <div class="mb-3">
                                <label for="language" class="form-label">Idioma</label>
                                <select class="form-select" id="language">
                                    <option value="es" selected>Español</option>
                                    <option value="en">English</option>
                                </select>
                            </div>
                            <button type="button" class="btn btn-primary" id="btnSavePreferences">Guardar Preferencias</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        // Cargar actividades recientes del usuario
        await loadUserActivities();
        
        // Agregar evento para el botón de editar perfil
        document.getElementById('btnEditProfile')?.addEventListener('click', function() {
            // Habilitar los campos del formulario
            document.querySelectorAll('#profileForm input').forEach(input => {
                if (input.id !== 'role') { // El rol no se puede editar directamente
                    input.readOnly = false;
                }
            });
            
            // Habilitar el botón de guardar
            document.getElementById('btnSaveProfile').disabled = false;
        });
        
        // Agregar evento para el botón de guardar cambios del perfil
        document.getElementById('btnSaveProfile')?.addEventListener('click', async function() {
            try {
                const updatedName = document.getElementById('name').value.trim();
                const updatedEmail = document.getElementById('email').value.trim();
                const updatedPassword = document.getElementById('password').value;
                
                // Validar campos
                if (!updatedName || !updatedEmail) {
                    showMessage('Error', 'Por favor complete los campos obligatorios.');
                    return;
                }
                
                // Simular actualización
                showMessage('Perfil Actualizado', 'Los cambios se han guardado correctamente.');
                
                // Volver a establecer los campos como solo lectura
                document.querySelectorAll('#profileForm input').forEach(input => {
                    input.readOnly = true;
                });
                
                // Deshabilitar el botón de guardar
                this.disabled = true;
                
                // Registrar la actividad
                await logActivity('Update', 'User', '1', `Actualizado perfil de usuario ${updatedName}`);
                
                // Recargar actividades
                await loadUserActivities();
            } catch (error) {
                console.error('Error al actualizar perfil:', error);
                showError('Error', 'No se pudo actualizar el perfil.');
            }
        });
        
        // Agregar evento para el botón de guardar preferencias
        document.getElementById('btnSavePreferences')?.addEventListener('click', function() {
            const darkMode = document.getElementById('darkModeSwitch').checked;
            const notifications = document.getElementById('notificationsSwitch').checked;
            const language = document.getElementById('language').value;
            
            // Simular guardado de preferencias
            showMessage('Preferencias Guardadas', 'Las preferencias se han guardado correctamente.');
            
            // En una implementación real, aquí guardaríamos las preferencias
            // Y aplicaríamos los cambios de tema, etc.
        });
    } catch (error) {
        console.error('Error al cargar el perfil:', error);
        showError('Error al cargar el perfil', error);
    }
}

// Función para cargar actividades recientes del usuario
async function loadUserActivities() {
    try {
        // En una implementación real, obtendríamos las actividades del usuario actual
        // Utilizando el endpoint de ActivityLog filtrado por userId
        
        // Simular carga de actividades
        setTimeout(() => {
            const userActivitiesList = document.getElementById('userActivities');
            
            if (userActivitiesList) {
                userActivitiesList.innerHTML = `
                    <li class="list-group-item">
                        <small class="text-muted">${new Date().toLocaleString()}</small>
                        <p class="mb-0">Has iniciado sesión en el sistema.</p>
                    </li>
                    <li class="list-group-item">
                        <small class="text-muted">${new Date(Date.now() - 3600000).toLocaleString()}</small>
                        <p class="mb-0">Has actualizado tu perfil.</p>
                    </li>
                    <li class="list-group-item">
                        <small class="text-muted">${new Date(Date.now() - 86400000).toLocaleString()}</small>
                        <p class="mb-0">Has creado un nuevo formulario.</p>
                    </li>
                `;
            }
        }, 1000);
    } catch (error) {
        console.error('Error al cargar actividades del usuario:', error);
        const userActivitiesList = document.getElementById('userActivities');
        if (userActivitiesList) {
            userActivitiesList.innerHTML = `
                <li class="list-group-item text-danger">
                    Error al cargar actividades.
                </li>
            `;
        }
    }
}

// Función para registrar una actividad en el sistema
async function logActivity(action, entityType, entityId, details) {
    try {
        // Obtener información del usuario actual
        const userId = currentRole?.toString() || "1";
        const userName = roleSelect.options[roleSelect.selectedIndex]?.text || "Usuario";
        
        // Crear el objeto de log
        const logData = {
            userId: userId,
            userName: userName,
            action: action,
            entityType: entityType,
            entityId: entityId,
            details: details,
            timestamp: new Date().toISOString()
        };
        
        // Enviar a la API
        const response = await fetch(`${API_BASE_URL}/ActivityLog`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(logData)
        });
        
        if (!response.ok) {
            throw new Error(`Error al registrar actividad: ${response.status} ${response.statusText}`);
        }
        
        console.log(`Actividad registrada: ${action} ${entityType} #${entityId}`);
    } catch (error) {
        console.error('Error al registrar actividad:', error);
        // No mostrar error al usuario para no interrumpir el flujo principal
    }
}

// Función para mostrar mensajes en el modal
function showMessage(title, message) {
    const modalTitle = document.getElementById('messageModalLabel');
    const modalBody = document.getElementById('messageModalBody');

    modalTitle.textContent = title;
    modalBody.innerHTML = message;

    modalInstance.show();
}

// Función para mostrar errores de manera amigable
function showError(title, error) {
    const modalTitle = document.getElementById('messageModalLabel');
    const modalBody = document.getElementById('messageModalBody');

    modalTitle.textContent = title;
    modalBody.innerHTML = `
        <div class="api-error">
            <p>${error.message || 'Se produjo un error inesperado.'}</p>
            ${error.stack ? `
                <hr>
                <div class="error-details">${error.stack}</div>
            ` : ''}
        </div>
    `;

    modalInstance.show();
}