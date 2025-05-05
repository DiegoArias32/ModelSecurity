// Configuración global
const API_BASE_URL = 'http://localhost:5163/api';
let currentRole = null;
let menuData = null;
let modalInstance = null;

// Elementos del DOM
const sidebarMenu = document.getElementById('sidebarMenu');
const pageContent = document.getElementById('pageContent');
const roleSelect = document.getElementById('roleSelect');
const messageModal = document.getElementById('messageModal');

// Inicialización
document.addEventListener('DOMContentLoaded', async function() {
    try {
        // Inicializar el modal
        modalInstance = new bootstrap.Modal(messageModal);
        
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
                    <td><span class="badge bg-info">${getUserRolName(user.id)}</span></td>
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
        
// Modificar el evento del botón "Nuevo Usuario"
document.getElementById('btnAddUser')?.addEventListener('click', async function() {
    try {
        // Llamar a la API para obtener los roles disponibles
        const response = await fetch(`${API_BASE_URL}/Rol`);
        if (!response.ok) {
            throw new Error(`Error al cargar roles: ${response.status} ${response.statusText}`);
        }
        const roles = await response.json();

        // Generar opciones para el selector de roles
        const roleOptions = roles.map(role => `<option value="${role.id}">${role.name}</option>`).join('');

        // Mostrar el modal con el formulario de creación de usuario
        pageContent.innerHTML += `
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

        // Mostrar el modal
        const createUserModal = new bootstrap.Modal(document.getElementById('createUserModal'));
        createUserModal.show();

        // Agregar evento al botón "Guardar"
        document.getElementById('saveUserBtn').addEventListener('click', async function() {
            const userName = document.getElementById('userName').value.trim();
            const userEmail = document.getElementById('userEmail').value.trim();
            const userPassword = document.getElementById('userPassword').value.trim();
            const userRole = parseInt(document.getElementById('userRole').value);

            if (!userName || !userEmail || !userPassword || isNaN(userRole)) {
                showMessage('Error', 'Por favor complete todos los campos.');
                return;
            }

            try {
                // Enviar datos a la API
                const response = await fetch(`${API_BASE_URL}/User`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        name: userName,
                        email: userEmail,
                        password: userPassword,
                        roleId: userRole,
                    }),
                });

                if (!response.ok) {
                    throw new Error(`Error al crear usuario: ${response.status} ${response.statusText}`);
                }

                // Cerrar el modal y mostrar mensaje de éxito
                createUserModal.hide();
                showMessage('Éxito', 'El usuario ha sido creado correctamente.');

                // Recargar la lista de usuarios
                await loadUsersPage();
            } catch (error) {
                console.error('Error al crear usuario:', error);
                showError('Error al crear usuario', error);
            }
        });
    } catch (error) {
        console.error('Error al cargar roles:', error);
        showError('Error al cargar roles', error);
    }
});
        
window.editUser = async function(userId) {
    try {
        // Obtener los datos del usuario desde la API
        const response = await fetch(`${API_BASE_URL}/User/${userId}`);
        if (!response.ok) {
            throw new Error(`Error al obtener usuario: ${response.status} ${response.statusText}`);
        }
        const user = await response.json();

        // Obtener los roles disponibles
        const rolesResponse = await fetch(`${API_BASE_URL}/Rol`);
        if (!rolesResponse.ok) {
            throw new Error(`Error al cargar roles: ${rolesResponse.status} ${rolesResponse.statusText}`);
        }
        const roles = await rolesResponse.json();

        // Generar opciones de roles
        const roleOptions = roles.map(
            role => `<option value="${role.id}" ${user.roleId === role.id ? 'selected' : ''}>${role.name}</option>`
        ).join('');

        // Mostrar modal para editar usuario
        pageContent.innerHTML += `
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
                                    <label for="editUserRole" class="form-label">Rol</label>
                                    <select class="form-select" id="editUserRole" required>
                                        ${roleOptions}
                                    </select>
                                </div>
                            </form>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <button type="button" class="btn btn-primary" id="saveEditUserBtn">Guardar Cambios</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        const editUserModal = new bootstrap.Modal(document.getElementById('editUserModal'));
        editUserModal.show();

        // Guardar cambios al hacer clic en "Guardar Cambios"
        document.getElementById('saveEditUserBtn').addEventListener('click', async function() {
            const updatedUserName = document.getElementById('editUserName').value.trim();
            const updatedUserEmail = document.getElementById('editUserEmail').value.trim();
            const updatedUserRole = parseInt(document.getElementById('editUserRole').value);

            if (!updatedUserName || !updatedUserEmail || isNaN(updatedUserRole)) {
                showMessage('Error', 'Por favor complete todos los campos.');
                return;
            }

            try {
                // Enviar datos actualizados a la API
                const updateResponse = await fetch(`${API_BASE_URL}/User/${userId}`, {
                    method: 'PUT', // O PATCH dependiendo de tu API
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        name: updatedUserName,
                        email: updatedUserEmail,
                        roleId: updatedUserRole,
                    }),
                });

                if (!updateResponse.ok) {
                    throw new Error(`Error al actualizar usuario: ${updateResponse.status} ${updateResponse.statusText}`);
                }

                // Cerrar el modal y mostrar mensaje de éxito
                editUserModal.hide();
                showMessage('Éxito', 'El usuario ha sido actualizado correctamente.');

                // Recargar la lista de usuarios
                await loadUsersPage();
            } catch (error) {
                console.error('Error al actualizar usuario:', error);
                showError('Error al actualizar usuario', error);
            }
        });
    } catch (error) {
        console.error('Error al cargar datos del usuario:', error);
        showError('Error al cargar datos del usuario', error);
    }
};
        
window.deleteUser = async function(userId) {
    // Mostrar confirmación antes de eliminar
    const confirmDelete = confirm(`¿Está seguro de que desea eliminar el usuario con ID: ${userId}?`);
    if (!confirmDelete) {
        return;
    }

    try {
        // Solicitud DELETE a la API
        const response = await fetch(`${API_BASE_URL}/User/${userId}`, {
            method: 'DELETE',
        });

        if (!response.ok) {
            throw new Error(`Error al eliminar usuario: ${response.status} ${response.statusText}`);
        }

        // Mostrar mensaje de éxito y recargar la lista de usuarios
        showMessage('Éxito', 'El usuario ha sido eliminado correctamente.');
        await loadUsersPage();
    } catch (error) {
        console.error('Error al eliminar usuario:', error);
        showError('Error al eliminar usuario', error);
    }
};
    } catch (error) {
        console.error('Error al cargar usuarios:', error);
        showError('Error al cargar usuarios', error);
    }
}

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
        document.getElementById('btnAddRole')?.addEventListener('click', function() {
            showMessage('Crear Rol', 'Funcionalidad para crear un nuevo rol.');
            // Aquí implementarías la lógica para crear un rol
        });
        
        // Añadir funciones globales para los botones de acción
        window.editRole = function(id) {
            showMessage('Editar Rol', `Editando rol con ID: ${id}`);
            // Aquí implementarías la lógica para editar un rol
        };
        
        window.deleteRole = function(id) {
            showMessage('Eliminar Rol', `¿Está seguro de eliminar el rol con ID: ${id}?`);
            // Aquí implementarías la lógica para eliminar un rol
        };
    } catch (error) {
        console.error('Error al cargar roles:', error);
        showError('Error al cargar roles', error);
    }
}

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
        document.getElementById('btnAddModule')?.addEventListener('click', function() {
            showMessage('Crear Módulo', 'Funcionalidad para crear un nuevo módulo.');
            // Aquí implementarías la lógica para crear un módulo
        });
        
        // Añadir funciones globales para los botones de acción
        window.editModule = function(id) {
            showMessage('Editar Módulo', `Editando módulo con ID: ${id}`);
            // Aquí implementarías la lógica para editar un módulo
        };
        
        window.deleteModule = function(id) {
            showMessage('Eliminar Módulo', `¿Está seguro de eliminar el módulo con ID: ${id}?`);
            // Aquí implementarías la lógica para eliminar un módulo
        };
    } catch (error) {
        console.error('Error al cargar módulos:', error);
        showError('Error al cargar módulos', error);
    }
}

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
        document.getElementById('btnAddForm')?.addEventListener('click', function() {
            showMessage('Crear Formulario', 'Funcionalidad para crear un nuevo formulario.');
            // Aquí implementarías la lógica para crear un formulario
        });
        
// Añadir funciones globales para los botones de acción
window.editForm = function(id) {
    showMessage('Editar Formulario', `Editando formulario con ID: ${id}`);
    // Aquí implementarías la lógica para editar un formulario
};

window.deleteForm = function(id) {
    showMessage('Eliminar Formulario', `¿Está seguro de eliminar el formulario con ID: ${id}?`);
    // Aquí implementarías la lógica para eliminar un formulario
};
} catch (error) {
console.error('Error al cargar formularios:', error);
showError('Error al cargar formularios', error);
}
}

// Función para cargar la página de perfil
async function loadProfilePage() {
try {
// En una implementación real, obtendríamos los datos del usuario actual
// En este ejemplo, simulamos un usuario de ejemplo
pageContent.innerHTML = `
    <h2 class="page-header">Mi Perfil</h2>
    <div class="row">
        <div class="col-md-4">
            <div class="card">
                <div class="card-body text-center">
                    <i class="fas fa-user-circle fa-5x mb-3"></i>
                    <h5 class="card-title">Usuario del Sistema</h5>
                    <p class="card-text">usuario@ejemplo.com</p>
                    <button class="btn btn-primary" id="btnEditProfile">Editar Perfil</button>
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
                            <input type="text" class="form-control" id="name" value="Usuario del Sistema" readonly>
                        </div>
                        <div class="mb-3">
                            <label for="email" class="form-label">Email</label>
                            <input type="email" class="form-control" id="email" value="usuario@ejemplo.com" readonly>
                        </div>
                        <div class="mb-3">
                            <label for="password" class="form-label">Contraseña</label>
                            <input type="password" class="form-control" id="password" value="********" readonly>
                        </div>
                        <button type="button" class="btn btn-success" id="btnSaveProfile" disabled>Guardar Cambios</button>
                    </form>
                </div>
            </div>
        </div>
    </div>
`;

// Agregar evento para el botón de editar perfil
document.getElementById('btnEditProfile')?.addEventListener('click', function() {
    // Habilitar los campos del formulario
    document.querySelectorAll('#profileForm input').forEach(input => {
        input.readOnly = false;
    });
    
    // Habilitar el botón de guardar
    document.getElementById('btnSaveProfile').disabled = false;
});

// Agregar evento para el botón de guardar cambios
document.getElementById('btnSaveProfile')?.addEventListener('click', function() {
    showMessage('Guardar Perfil', 'Los cambios se han guardado correctamente.');
    
    // Volver a establecer los campos como solo lectura
    document.querySelectorAll('#profileForm input').forEach(input => {
        input.readOnly = true;
    });
    
    // Deshabilitar el botón de guardar
    this.disabled = true;
});
} catch (error) {
console.error('Error al cargar el perfil:', error);
showError('Error al cargar el perfil', error);
}
}

// Función auxiliar para obtener el nombre del rol de un usuario
// En una implementación real, este dato vendría de la API
function getUserRolName(userId) {
// Ejemplo simplificado
if (userId === 1) {
return 'Administrador';
} else {
return 'Usuario';
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
    <p>${error.message}</p>
    <hr>
    <div class="error-details">${error.stack || 'No hay detalles adicionales'}</div>
</div>
`;

modalInstance.show();
}