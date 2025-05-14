// Aplicación principal
document.addEventListener('DOMContentLoaded', () => {
    // Comprobar si hay un usuario autenticado
    const isAuthenticated = auth.isAuthenticated();
    const currentUser = auth.getCurrentUser();
    
    // Configurar eventos de login
    setupLoginEvents();
    
    // Si el usuario está autenticado, mostrar el dashboard
    if (isAuthenticated && currentUser) {
        showDashboard(currentUser);
    }
});

// Configurar eventos de la página de login
function setupLoginEvents() {
    const loginForm = document.getElementById('login-form');
    const adminDemoButton = document.getElementById('admin-demo');
    const userDemoButton = document.getElementById('user-demo');
    
    // Evento de envío del formulario de login
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;
        
        try {
            // Intentar autenticación en API
            // En caso de error o mientras se desarrolla el backend, usar autenticación demo
            try {
                const response = await api.auth.login(username, password);
                showDashboard(response);
            } catch (apiError) {
                console.warn('Error al intentar autenticar con API, usando modo demo:', apiError);
                const userData = auth.demoLogin(username, password);
                showDashboard(userData);
            }
        } catch (error) {
            showErrorMessage(error.message);
        }
    });
    
    // Evento para ingresar como Admin (demo)
    adminDemoButton.addEventListener('click', () => {
        try {
            const userData = auth.demoLoginAsRole('Admin');
            showDashboard(userData);
        } catch (error) {
            showErrorMessage(error.message);
        }
    });
    
    // Evento para ingresar como Usuario regular (demo)
    userDemoButton.addEventListener('click', () => {
        try {
            const userData = auth.demoLoginAsRole('User');
            showDashboard(userData);
        } catch (error) {
            showErrorMessage(error.message);
        }
    });
}

// Mostrar mensaje de error en la pantalla de login
function showErrorMessage(message) {
    const errorElement = document.getElementById('error-message');
    errorElement.textContent = message;
    errorElement.classList.add('active');
}

// Mostrar el dashboard según el rol del usuario
async function showDashboard(userData) {
    // Ocultar pantalla de login
    document.getElementById('login-container').style.display = 'none';
    
    // Crear el contenedor del dashboard si no existe
    let dashboardContainer = document.getElementById('dashboard-container');
    
    if (!dashboardContainer) {
        dashboardContainer = document.createElement('div');
        dashboardContainer.id = 'dashboard-container';
        dashboardContainer.className = 'dashboard active';
        document.getElementById('app').appendChild(dashboardContainer);
    }
    
    // Generar contenido del dashboard según el rol
    if (userData.roles.includes('Admin')) {
        dashboardContainer.innerHTML = generateAdminDashboard(userData);
        setupAdminEvents();
    } else {
        dashboardContainer.innerHTML = await generateUserDashboard(userData);
        setupUserEvents();
    }
    
    // Configurar evento de logout
    document.getElementById('logout-btn').addEventListener('click', () => {
        auth.logout();
    });
    
    // Configurar evento para cambiar de rol (demo)
    document.getElementById('switch-role-btn').addEventListener('click', () => {
        const isAdmin = userData.roles.includes('Admin');
        
        try {
            const newUserData = auth.demoLoginAsRole(isAdmin ? 'User' : 'Admin');
            showDashboard(newUserData);
        } catch (error) {
            console.error('Error al cambiar de rol:', error);
        }
    });
    
    // Manejar la navegación del menú lateral
    setupSidebarNavigation();
}

// Generar HTML para el dashboard de administrador
function generateAdminDashboard(userData) {
    return `
        <div class="header">
            <div class="container header-content">
                <div class="logo">Sistema de Gestión</div>
                <div class="user-info">
                    <span class="user-name">Hola, ${userData.name}</span>
                    <button id="switch-role-btn" class="btn">Cambiar a Usuario</button>
                    <button id="logout-btn" class="logout-btn">Cerrar Sesión</button>
                </div>
            </div>
        </div>
        
        <div class="container main-content">
            <div class="sidebar">
                <ul class="sidebar-menu">
                    <li><a href="#dashboard" class="active" data-module="dashboard">Dashboard</a></li>
                    <li><a href="#usuarios" data-module="usuarios">Usuarios</a></li>
                    <li><a href="#roles" data-module="roles">Roles</a></li>
                    <li><a href="#modulos" data-module="modulos">Módulos</a></li>
                    <li><a href="#formularios" data-module="formularios">Formularios</a></li>
                    <li><a href="#permisos" data-module="permisos">Permisos</a></li>
                    <li><a href="#rolformpermisos" data-module="rolformpermisos">Rol-Form-Permisos</a></li>
                    <li><a href="#formmodulos" data-module="formmodulos">Form-Módulos</a></li>
                    <li><a href="#perfil" data-module="perfil">Mi Perfil</a></li>
                </ul>
            </div>
            
            <div class="content-area">
                <div id="module-content">
                    <h2 class="module-title">Dashboard</h2>
                    <p>Bienvenido al panel de administración. Utiliza el menú lateral para navegar por los diferentes módulos.</p>
                    
                    <div class="dashboard-stats">
                        <div class="stat-card">
                            <h3>Usuarios</h3>
                            <p class="stat-number" id="users-count">Cargando...</p>
                        </div>
                        <div class="stat-card">
                            <h3>Roles</h3>
                            <p class="stat-number" id="roles-count">Cargando...</p>
                        </div>
                        <div class="stat-card">
                            <h3>Módulos</h3>
                            <p class="stat-number" id="modules-count">Cargando...</p>
                        </div>
                        <div class="stat-card">
                            <h3>Formularios</h3>
                            <p class="stat-number" id="forms-count">Cargando...</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
}

// Generar HTML para el dashboard de usuario regular
async function generateUserDashboard(userData) {
    // Determinar el ID del rol del usuario
    let rolId;
    if (userData.roles.includes('Admin')) {
        rolId = 1; // Ajustar según tu configuración
    } else {
        rolId = 2; // Ajustar según tu configuración
    }
    
    // Obtener elementos del menú desde la API
    let menuItems = '<li><a href="#perfil" class="active" data-module="perfil">Mi Perfil</a></li>';
    
    try {
        // Llamar a la API para obtener los elementos del menú
        const menuElements = await api.menu.getByRolId(rolId);
        console.log('Elementos del menú recibidos:', menuElements);
        
        if (menuElements && Array.isArray(menuElements) && menuElements.length > 0) {
            // Añadir cada elemento del menú
            menuElements.forEach(item => {
                if (item && item.name) {
                    // Usar moduleCode si existe, o extraerlo de la URL
                    const moduleCode = item.moduleCode || item.url.replace('/', '');
                    
                    // Evitar duplicar Mi Perfil que ya incluimos al principio
                    if (moduleCode !== "perfil") {
                        menuItems += `<li><a href="#${moduleCode}" data-module="${moduleCode}">
                                      <i class="${item.icon || ''}"></i> ${item.name}
                                      </a></li>`;
                    }
                }
            });
        }
    } catch (error) {
        console.error('Error al cargar el menú:', error);
        // En caso de error, dejar solo el perfil
    }
    
    return `
        <div class="header">
            <div class="container header-content">
                <div class="logo">Sistema de Gestión</div>
                <div class="user-info">
                    <span class="user-name">Hola, ${userData.name}</span>
                    <button id="switch-role-btn" class="btn">Cambiar a Admin</button>
                    <button id="logout-btn" class="logout-btn">Cerrar Sesión</button>
                </div>
            </div>
        </div>
        
        <div class="container main-content">
            <div class="sidebar">
                <ul class="sidebar-menu">
                    ${menuItems}
                </ul>
            </div>
            
            <div class="content-area">
                <div id="module-content">
                    <h2 class="module-title">Mi Perfil</h2>
                    <div class="profile-container">
                        <div class="profile-info">
                            <div class="form-row">
                                <label>Nombre:</label>
                                <p>${userData.name}</p>
                            </div>
                            <div class="form-row">
                                <label>Usuario:</label>
                                <p>${userData.username}</p>
                            </div>
                            <div class="form-row">
                                <label>Email:</label>
                                <p>${userData.email}</p>
                            </div>
                            <div class="form-row">
                                <label>Rol:</label>
                                <p>${userData.roles.join(', ')}</p>
                            </div>
                        </div>
                        
                        <div class="profile-actions">
                            <button class="btn btn-primary" id="edit-profile-btn">Editar Perfil</button>
                            <button class="btn btn-secondary" id="change-password-btn">Cambiar Contraseña</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
}

// Configurar eventos del dashboard de administrador
function setupAdminEvents() {
    // Cargar estadísticas para el dashboard
    loadDashboardStats();
}

// Cargar estadísticas para el dashboard
async function loadDashboardStats() {
    try {
        // Obtener conteos de cada entidad
        const usersPromise = api.users.getAll();
        const rolesPromise = api.roles.getAll();
        const modulesPromise = api.modules.getAll();
        const formsPromise = api.forms.getAll();
        
        // Esperar a que todas las promesas se resuelvan
        const [users, roles, modules, forms] = await Promise.allSettled([
            usersPromise, rolesPromise, modulesPromise, formsPromise
        ]);
        
        // Actualizar contadores en el dashboard
        document.getElementById('users-count').textContent = users.status === 'fulfilled' ? users.value.length : '0';
        document.getElementById('roles-count').textContent = roles.status === 'fulfilled' ? roles.value.length : '0';
        document.getElementById('modules-count').textContent = modules.status === 'fulfilled' ? modules.value.length : '0';
        document.getElementById('forms-count').textContent = forms.status === 'fulfilled' ? forms.value.length : '0';
    } catch (error) {
        console.error('Error al cargar estadísticas del dashboard:', error);
        // Si hay error, mostrar valores por defecto
        document.getElementById('users-count').textContent = '0';
        document.getElementById('roles-count').textContent = '0';
        document.getElementById('modules-count').textContent = '0';
        document.getElementById('forms-count').textContent = '0';
    }
}

// Configurar eventos del dashboard de usuario
function setupUserEvents() {
    // Configurar eventos para el perfil de usuario
    console.log('Configurando eventos para el dashboard de usuario');
    
    const editProfileBtn = document.getElementById('edit-profile-btn');
    const changePasswordBtn = document.getElementById('change-password-btn');
    
    if (editProfileBtn) {
        editProfileBtn.addEventListener('click', () => {
            // Obtener información del usuario actual
            const currentUser = auth.getCurrentUser();
            showUserProfileForm(currentUser);
        });
    }
    
    if (changePasswordBtn) {
        changePasswordBtn.addEventListener('click', () => {
            showChangePasswordForm();
        });
    }
}

function setupSidebarNavigation() {
    const menuLinks = document.querySelectorAll('.sidebar-menu a');
    
    menuLinks.forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            
            // Obtener el moduleId del atributo data-module
            const moduleId = link.getAttribute('data-module');
            console.log("Módulo seleccionado:", moduleId);
            
            // Remover clase active de todos los enlaces
            menuLinks.forEach(l => l.classList.remove('active'));
            
            // Añadir clase active al enlace clickeado
            link.classList.add('active');
            
            // Cargar el contenido del módulo
            loadModuleContent(moduleId);
        });
    });
}

// Modificar la función loadModuleContent en app.js
function loadModuleContent(moduleId) {
    const moduleContentEl = document.getElementById('module-content');
    
    // Depuración - ver qué moduleId está recibiendo
    console.log("Intentando cargar módulo:", moduleId);
    
    // Mostrar un indicador de carga
    moduleContentEl.innerHTML = '<div class="loading">Cargando...</div>';
    
    // Verificar si el usuario actual tiene permiso para este módulo
    const currentUser = auth.getCurrentUser();
    const isAdmin = currentUser && currentUser.roles.includes('Admin');
    
    // El resto de tu código actual para cargar el contenido del módulo
    switch (moduleId) {
        case 'dashboard':
            moduleContentEl.innerHTML = generateDashboardContent();
            loadDashboardStats();
            break;
        case 'usuarios':
            loadUsersModule(moduleContentEl);
            break;
        case 'roles':
            // Asegúrate de que esta función exista y esté implementada correctamente
            loadRolesModule(moduleContentEl);
            break;
        case 'modulos':
            loadModulesModule(moduleContentEl);
            break;
        case 'formularios':
            loadFormsModule(moduleContentEl);
            break;
        case 'permisos':
            loadPermissionsModule(moduleContentEl);
            break;
        case 'rolformpermisos':
            loadRolFormPermissionsModule(moduleContentEl);
            break;
        case 'formmodulos':
            loadFormModulesModule(moduleContentEl);
            break;
        case 'perfil':
            moduleContentEl.innerHTML = generateProfileContent(currentUser);
            setupProfileModule();
            break;
        default:
            moduleContentEl.innerHTML = `<h2 class="module-title">Módulo no encontrado: ${moduleId}</h2>
                                        <p>El módulo que intentas cargar no está disponible o no existe.</p>`;
            console.error(`Módulo no encontrado: ${moduleId}`);
    }
}

// Verificar que la función loadRolesModule esté correctamente implementada
function loadRolesModule(containerEl) {
    console.log("Cargando módulo de roles...");
    
    containerEl.innerHTML = `
        <h2 class="module-title">Gestión de Roles</h2>
        <button id="add-role-btn" class="btn btn-primary add-btn">Nuevo Rol</button>
        
        <table class="data-table">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Nombre</th>
                    <th>Descripción</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody id="roles-table-body">
                <tr>
                    <td colspan="4" class="loading-row">Cargando roles...</td>
                </tr>
            </tbody>
        </table>
    `;
    
    // Configurar el botón para añadir rol
    document.getElementById('add-role-btn').addEventListener('click', () => {
        showRoleForm();
    });
    
    // Cargar datos de roles desde la API
    loadRolesData();
}

// Función para cargar los datos de roles
async function loadRolesData() {
    const tableBody = document.getElementById('roles-table-body');
    
    try {
        const roles = await api.roles.getAll();
        
        if (roles && roles.length > 0) {
            // Generar filas de la tabla con los datos reales
            tableBody.innerHTML = roles.map(role => `
                <tr>
                    <td>${role.id}</td>
                    <td>${role.name}</td>
                    <td>${role.description || '-'}</td>
                    <td>
                        <button class="action-btn edit-role" data-id="${role.id}">Editar</button>
                        <button class="action-btn delete delete-role" data-id="${role.id}">Eliminar</button>
                    </td>
                </tr>
            `).join('');
            
            // Configurar eventos para los botones de editar y eliminar
            setupRolesEvents();
        } else {
            tableBody.innerHTML = '<tr><td colspan="4" class="empty-row">No hay roles registrados</td></tr>';
        }
    } catch (error) {
        console.error('Error al cargar roles:', error);
        tableBody.innerHTML = 
            `<tr><td colspan="4" class="error-row">Error al cargar roles: ${error.message}</td></tr>`;
    }
}

// Función para obtener los permisos del usuario
async function getUserPermissions(userData) {
    try {
        // Determinar el ID del rol del usuario
        const rolId = userData.roles.includes('Admin') ? 1 : 2; // Ajusta según tus IDs reales
        
        // Obtener las asignaciones de permisos para este rol
        const permissions = await api.rolFormPermissions.getByRolId(rolId);
        
        // Obtener todos los formularios
        const forms = await api.forms.getAll();
        
        // Crear un mapa de los formularios a los que el usuario tiene acceso
        const accessibleForms = {};
        
        permissions.forEach(permission => {
            const form = forms.find(f => f.id === permission.formId);
            if (form) {
                accessibleForms[form.code.toLowerCase()] = {
                    id: form.id,
                    name: form.name,
                    code: form.code,
                    canRead: permission.permissionId ? true : false, // Ajusta según tu estructura de permisos
                    canCreate: permission.permissionId ? true : false,
                    canUpdate: permission.permissionId ? true : false,
                    canDelete: permission.permissionId ? true : false
                };
            }
        });
        
        return accessibleForms;
    } catch (error) {
        console.error('Error al obtener permisos:', error);
        return {};
    }
}

// Generar contenido para el módulo Dashboard
function generateDashboardContent() {
    return `
        <h2 class="module-title">Dashboard</h2>
        <p>Bienvenido al panel de administración. Utiliza el menú lateral para navegar por los diferentes módulos.</p>
        
        <div class="dashboard-stats">
            <div class="stat-card">
                <h3>Usuarios</h3>
                <p class="stat-number" id="users-count">Cargando...</p>
            </div>
            <div class="stat-card">
                <h3>Roles</h3>
                <p class="stat-number" id="roles-count">Cargando...</p>
            </div>
            <div class="stat-card">
                <h3>Módulos</h3>
                <p class="stat-number" id="modules-count">Cargando...</p>
            </div>
            <div class="stat-card">
                <h3>Formularios</h3>
                <p class="stat-number" id="forms-count">Cargando...</p>
            </div>
        </div>
    `;
}

// Cargar y configurar el módulo de Usuarios
async function loadUsersModule(containerEl) {
    containerEl.innerHTML = `
        <h2 class="module-title">Gestión de Usuarios</h2>
        <button id="add-user-btn" class="btn btn-primary add-btn">Nuevo Usuario</button>
        
        <table class="data-table">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Nombre</th>
                    <th>Email</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody id="users-table-body">
                <tr>
                    <td colspan="4" class="loading-row">Cargando usuarios...</td>
                </tr>
            </tbody>
        </table>
    `;
    
    // Configurar el botón para añadir usuario
    document.getElementById('add-user-btn').addEventListener('click', () => {
        showUserForm();
    });
    
    // Cargar datos de usuarios desde la API
    try {
        const users = await api.users.getAll();
        const tableBody = document.getElementById('users-table-body');
        
        if (users && users.length > 0) {
            // Generar filas de la tabla con los datos reales
            tableBody.innerHTML = users.map(user => `
                <tr>
                    <td>${user.id}</td>
                    <td>${user.name}</td>
                    <td>${user.email}</td>
                    <td>
                        <button class="action-btn edit-user" data-id="${user.id}">Editar</button>
                        <button class="action-btn delete delete-user" data-id="${user.id}">Eliminar</button>
                    </td>
                </tr>
            `).join('');
            
            // Configurar eventos para los botones de editar y eliminar
            setupUsersEvents();
        } else {
            tableBody.innerHTML = '<tr><td colspan="4" class="empty-row">No hay usuarios registrados</td></tr>';
        }
    } catch (error) {
        console.error('Error al cargar usuarios:', error);
        document.getElementById('users-table-body').innerHTML = 
            `<tr><td colspan="4" class="error-row">Error al cargar usuarios: ${error.message}</td></tr>`;
    }
}

// Configurar eventos para los botones de editar y eliminar usuarios
function setupUsersEvents() {
    const editButtons = document.querySelectorAll('.edit-user');
    const deleteButtons = document.querySelectorAll('.delete-user');
    
    editButtons.forEach(btn => {
        btn.addEventListener('click', async () => {
            const userId = btn.getAttribute('data-id');
            try {
                // Obtener datos del usuario desde la API
                const user = await api.users.getById(userId);
                if (user) {
                    showUserForm(user);
                } else {
                    showErrorModal('Error', 'No se pudo encontrar el usuario');
                }
            } catch (error) {
                console.error('Error al obtener usuario:', error);
                showErrorModal('Error', `No se pudo cargar el usuario: ${error.message}`);
            }
        });
    });
    
    deleteButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const userId = btn.getAttribute('data-id');
            showConfirmModal(
                'Confirmar eliminación',
                `¿Está seguro de que desea eliminar el usuario con ID ${userId}?`,
                async () => {
                    try {
                        // Eliminar usuario a través de la API
                        await api.users.delete(userId);
                        // Recargar lista de usuarios
                        loadModuleContent('usuarios');
                    } catch (error) {
                        console.error('Error al eliminar usuario:', error);
                        showErrorModal('Error', `No se pudo eliminar el usuario: ${error.message}`);
                    }
                }
            );
        });
    });
}

// Cargar y configurar el módulo de Roles
async function loadRolesModule(containerEl) {
    containerEl.innerHTML = `
        <h2 class="module-title">Gestión de Roles</h2>
        <button id="add-role-btn" class="btn btn-primary add-btn">Nuevo Rol</button>
        
        <table class="data-table">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Nombre</th>
                    <th>Descripción</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody id="roles-table-body">
                <tr>
                    <td colspan="4" class="loading-row">Cargando roles...</td>
                </tr>
            </tbody>
        </table>
    `;
    
    // Configurar el botón para añadir rol
    document.getElementById('add-role-btn').addEventListener('click', () => {
        showRoleForm();
    });
    
    // Cargar datos de roles desde la API
    try {
        const roles = await api.roles.getAll();
        const tableBody = document.getElementById('roles-table-body');
        
        if (roles && roles.length > 0) {
            // Generar filas de la tabla con los datos reales
            tableBody.innerHTML = roles.map(role => `
                <tr>
                    <td>${role.id}</td>
                    <td>${role.name}</td>
                    <td>${role.description || '-'}</td>
                    <td>
                        <button class="action-btn edit-role" data-id="${role.id}">Editar</button>
                        <button class="action-btn delete delete-role" data-id="${role.id}">Eliminar</button>
                    </td>
                </tr>
            `).join('');
            
            // Configurar eventos para los botones de editar y eliminar
            setupRolesEvents();
        } else {
            tableBody.innerHTML = '<tr><td colspan="4" class="empty-row">No hay roles registrados</td></tr>';
        }
    } catch (error) {
        console.error('Error al cargar roles:', error);
        document.getElementById('roles-table-body').innerHTML = 
            `<tr><td colspan="4" class="error-row">Error al cargar roles: ${error.message}</td></tr>`;
    }
}

// Configurar eventos para los botones de editar y eliminar roles
function setupRolesEvents() {
    const editButtons = document.querySelectorAll('.edit-role');
    const deleteButtons = document.querySelectorAll('.delete-role');
    
    editButtons.forEach(btn => {
        btn.addEventListener('click', async () => {
            const roleId = btn.getAttribute('data-id');
            try {
                // Obtener datos del rol desde la API
                const role = await api.roles.getById(roleId);
                if (role) {
                    showRoleForm(role);
                } else {
                    showErrorModal('Error', 'No se pudo encontrar el rol');
                }
            } catch (error) {
                console.error('Error al obtener rol:', error);
                showErrorModal('Error', `No se pudo cargar el rol: ${error.message}`);
            }
        });
    });
    
    deleteButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const roleId = btn.getAttribute('data-id');
            showConfirmModal(
                'Confirmar eliminación',
                `¿Está seguro de que desea eliminar el rol con ID ${roleId}?`,
                async () => {
                    try {
                        // Eliminar rol a través de la API
                        await api.roles.delete(roleId);
                        // Recargar lista de roles
                        loadModuleContent('roles');
                    } catch (error) {
                        console.error('Error al eliminar rol:', error);
                        showErrorModal('Error', `No se pudo eliminar el rol: ${error.message}`);
                    }
                }
            );
        });
    });
}

// Cargar y configurar el módulo de Módulos
async function loadModulesModule(containerEl) {
    containerEl.innerHTML = `
        <h2 class="module-title">Gestión de Módulos</h2>
        <button id="add-module-btn" class="btn btn-primary add-btn">Nuevo Módulo</button>
        
        <table class="data-table">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Código</th>
                    <th>Activo</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody id="modules-table-body">
                <tr>
                    <td colspan="4" class="loading-row">Cargando módulos...</td>
                </tr>
            </tbody>
        </table>
    `;
    
    // Configurar el botón para añadir módulo
    document.getElementById('add-module-btn').addEventListener('click', () => {
        showModuleForm();
    });
    
    // Cargar datos de módulos desde la API
    try {
        const modules = await api.modules.getAll();
        const tableBody = document.getElementById('modules-table-body');
        
        if (modules && modules.length > 0) {
            // Generar filas de la tabla con los datos reales
            tableBody.innerHTML = modules.map(module => `
                <tr>
                    <td>${module.id}</td>
                    <td>${module.code}</td>
                    <td>${module.active ? 'Sí' : 'No'}</td>
                    <td>
                        <button class="action-btn edit-module" data-id="${module.id}">Editar</button>
                        <button class="action-btn delete delete-module" data-id="${module.id}">Eliminar</button>
                    </td>
                </tr>
            `).join('');
            
            // Configurar eventos para los botones de editar y eliminar
            setupModulesEvents();
        } else {
            tableBody.innerHTML = '<tr><td colspan="4" class="empty-row">No hay módulos registrados</td></tr>';
        }
    } catch (error) {
        console.error('Error al cargar módulos:', error);
        document.getElementById('modules-table-body').innerHTML = 
            `<tr><td colspan="4" class="error-row">Error al cargar módulos: ${error.message}</td></tr>`;
    }
}

// Configurar eventos para los botones de editar y eliminar módulos
function setupModulesEvents() {
    const editButtons = document.querySelectorAll('.edit-module');
    const deleteButtons = document.querySelectorAll('.delete-module');
    
    editButtons.forEach(btn => {
        btn.addEventListener('click', async () => {
            const moduleId = btn.getAttribute('data-id');
            try {
                // Obtener datos del módulo desde la API
                const module = await api.modules.getById(moduleId);
                if (module) {
                    showModuleForm(module);
                } else {
                    showErrorModal('Error', 'No se pudo encontrar el módulo');
                }
            } catch (error) {
                console.error('Error al obtener módulo:', error);
                showErrorModal('Error', `No se pudo cargar el módulo: ${error.message}`);
            }
        });
    });
    
    deleteButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const moduleId = btn.getAttribute('data-id');
            showConfirmModal(
                'Confirmar eliminación',
                `¿Está seguro de que desea eliminar el módulo con ID ${moduleId}?`,
                async () => {
                    try {
                        // Eliminar módulo a través de la API
                        await api.modules.delete(moduleId);
                        // Recargar lista de módulos
                        loadModuleContent('modulos');
                    } catch (error) {
                        console.error('Error al eliminar módulo:', error);
                        showErrorModal('Error', `No se pudo eliminar el módulo: ${error.message}`);
                    }
                }
            );
        });
    });
}

// Cargar y configurar el módulo de Formularios
async function loadFormsModule(containerEl) {
    containerEl.innerHTML = `
        <h2 class="module-title">Gestión de Formularios</h2>
        <button id="add-form-btn" class="btn btn-primary add-btn">Nuevo Formulario</button>
        
        <table class="data-table">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Nombre</th>
                    <th>Código</th>
                    <th>Activo</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody id="forms-table-body">
                <tr>
                    <td colspan="5" class="loading-row">Cargando formularios...</td>
                </tr>
            </tbody>
        </table>
    `;
    
    // Configurar el botón para añadir formulario
    document.getElementById('add-form-btn').addEventListener('click', () => {
        showFormForm();
    });
    
    // Cargar datos de formularios desde la API
    try {
        const forms = await api.forms.getAll();
        const tableBody = document.getElementById('forms-table-body');
        
        if (forms && forms.length > 0) {
            // Generar filas de la tabla con los datos reales
            tableBody.innerHTML = forms.map(form => `
                <tr>
                    <td>${form.id}</td>
                    <td>${form.name}</td>
                    <td>${form.code}</td>
                    <td>${form.active ? 'Sí' : 'No'}</td>
                    <td>
                        <button class="action-btn edit-form" data-id="${form.id}">Editar</button>
                        <button class="action-btn delete delete-form" data-id="${form.id}">Eliminar</button>
                    </td>
                </tr>
            `).join('');
            
            // Configurar eventos para los botones de editar y eliminar
            setupFormsEvents();
        } else {
            tableBody.innerHTML = '<tr><td colspan="5" class="empty-row">No hay formularios registrados</td></tr>';
        }
    } catch (error) {
        console.error('Error al cargar formularios:', error);
        document.getElementById('forms-table-body').innerHTML = 
            `<tr><td colspan="5" class="error-row">Error al cargar formularios: ${error.message}</td></tr>`;
    }
}

// Configurar eventos para los botones de editar y eliminar formularios
function setupFormsEvents() {
    const editButtons = document.querySelectorAll('.edit-form');
    const deleteButtons = document.querySelectorAll('.delete-form');
    
    editButtons.forEach(btn => {
        btn.addEventListener('click', async () => {
            const formId = btn.getAttribute('data-id');
            try {
                // Obtener datos del formulario desde la API
                const form = await api.forms.getById(formId);
                if (form) {
                    showFormForm(form);
                } else {
                    showErrorModal('Error', 'No se pudo encontrar el formulario');
                }
            } catch (error) {
                console.error('Error al obtener formulario:', error);
                showErrorModal('Error', `No se pudo cargar el formulario: ${error.message}`);
            }
        });
    });
    
    deleteButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const formId = btn.getAttribute('data-id');
            showConfirmModal(
                'Confirmar eliminación',
                `¿Está seguro de que desea eliminar el formulario con ID ${formId}?`,
                async () => {
                    try {
                        // Eliminar formulario a través de la API
                        await api.forms.delete(formId);
                        // Recargar lista de formularios
                        loadModuleContent('formularios');
                    } catch (error) {
                        console.error('Error al eliminar formulario:', error);
                        showErrorModal('Error', `No se pudo eliminar el formulario: ${error.message}`);
                    }
                }
            );
        });
    });
}

// Cargar y configurar el módulo de Permisos
async function loadPermissionsModule(containerEl) {
    containerEl.innerHTML = `
        <h2 class="module-title">Gestión de Permisos</h2>
        <button id="add-permission-btn" class="btn btn-primary add-btn">Nuevo Permiso</button>
        
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
            <tbody id="permissions-table-body">
                <tr>
                    <td colspan="6" class="loading-row">Cargando permisos...</td>
                </tr>
            </tbody>
        </table>
    `;
    
    // Configurar el botón para añadir permiso
    document.getElementById('add-permission-btn').addEventListener('click', () => {
        showPermissionForm();
    });
    
    // Cargar datos de permisos desde la API
    try {
        const permissions = await api.permissions.getAll();
        const tableBody = document.getElementById('permissions-table-body');
        
        if (permissions && permissions.length > 0) {
            // Generar filas de la tabla con los datos reales
            tableBody.innerHTML = permissions.map(permission => `
                <tr>
                    <td>${permission.id}</td>
                    <td>${permission.canRead ? 'Sí' : 'No'}</td>
                    <td>${permission.canCreate ? 'Sí' : 'No'}</td>
                    <td>${permission.canUpdate ? 'Sí' : 'No'}</td>
                    <td>${permission.canDelete ? 'Sí' : 'No'}</td>
                    <td>
                        <button class="action-btn edit-permission" data-id="${permission.id}">Editar</button>
                        <button class="action-btn delete delete-permission" data-id="${permission.id}">Eliminar</button>
                    </td>
                </tr>
            `).join('');
            
            // Configurar eventos para los botones de editar y eliminar
            setupPermissionsEvents();
        } else {
            tableBody.innerHTML = '<tr><td colspan="6" class="empty-row">No hay permisos registrados</td></tr>';
        }
    } catch (error) {
        console.error('Error al cargar permisos:', error);
        document.getElementById('permissions-table-body').innerHTML = 
            `<tr><td colspan="6" class="error-row">Error al cargar permisos: ${error.message}</td></tr>`;
    }
}

// Configurar eventos para los botones de editar y eliminar permisos
function setupPermissionsEvents() {
    const editButtons = document.querySelectorAll('.edit-permission');
    const deleteButtons = document.querySelectorAll('.delete-permission');
    
    editButtons.forEach(btn => {
        btn.addEventListener('click', async () => {
            const permissionId = btn.getAttribute('data-id');
            try {
                // Obtener datos del permiso desde la API
                const permission = await api.permissions.getById(permissionId);
                if (permission) {
                    showPermissionForm(permission);
                } else {
                    showErrorModal('Error', 'No se pudo encontrar el permiso');
                }
            } catch (error) {
                console.error('Error al obtener permiso:', error);
                showErrorModal('Error', `No se pudo cargar el permiso: ${error.message}`);
            }
        });
    });
    
    deleteButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const permissionId = btn.getAttribute('data-id');
            showConfirmModal(
                'Confirmar eliminación',
                `¿Está seguro de que desea eliminar el permiso con ID ${permissionId}?`,
                async () => {
                    try {
                        // Eliminar permiso a través de la API
                        await api.permissions.delete(permissionId);
                        // Recargar lista de permisos
                        loadModuleContent('permisos');
                    } catch (error) {
                        console.error('Error al eliminar permiso:', error);
                        showErrorModal('Error', `No se pudo eliminar el permiso: ${error.message}`);
                    }
                }
            );
        });
    });
}

// Cargar y configurar el módulo de RolFormPermissions
async function loadRolFormPermissionsModule(containerEl) {
    containerEl.innerHTML = `
        <h2 class="module-title">Gestión de Permisos por Rol y Formulario</h2>
        <button id="add-rolformpermission-btn" class="btn btn-primary add-btn">Nueva Asignación</button>
        
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
            <tbody id="rolformpermissions-table-body">
                <tr>
                    <td colspan="5" class="loading-row">Cargando asignaciones...</td>
                </tr>
            </tbody>
        </table>
    `;
    
    // Configurar el botón para añadir asignación
    document.getElementById('add-rolformpermission-btn').addEventListener('click', async () => {
        // Cargar roles, formularios y permisos para mostrar en el formulario
        try {
            const [roles, forms, permissions] = await Promise.all([
                api.roles.getAll(),
                api.forms.getAll(),
                api.permissions.getAll()
            ]);
            
            showRolFormPermissionForm(null, roles, forms, permissions);
        } catch (error) {
            console.error('Error al cargar datos para el formulario:', error);
            showErrorModal('Error', `No se pudieron cargar los datos necesarios: ${error.message}`);
        }
    });
    
    // Cargar datos de asignaciones desde la API
    try {
        // Primero necesitamos cargar las asignaciones
        const assignments = await api.rolFormPermissions.getAll();
        
        // Luego cargar roles, formularios y permisos para mostrar los nombres
        const [roles, forms, permissions] = await Promise.all([
            api.roles.getAll(),
            api.forms.getAll(),
            api.permissions.getAll()
        ]);
        
        // Crear un mapa para acceder rápidamente a los nombres
        const rolesMap = new Map(roles.map(role => [role.id, role.name]));
        const formsMap = new Map(forms.map(form => [form.id, form.name]));
        
        // Función para obtener los permisos en texto
        const getPermissionText = (permissionId) => {
            const permission = permissions.find(p => p.id === permissionId);
            if (!permission) return 'Desconocido';
            
            const permTypes = [];
            if (permission.canRead) permTypes.push('Lectura');
            if (permission.canCreate) permTypes.push('Creación');
            if (permission.canUpdate) permTypes.push('Actualización');
            if (permission.canDelete) permTypes.push('Eliminación');
            
            return permTypes.join(', ') || 'Ninguno';
        };
        
        const tableBody = document.getElementById('rolformpermissions-table-body');
        
        if (assignments && assignments.length > 0) {
            // Generar filas de la tabla con los datos reales
            tableBody.innerHTML = assignments.map(assignment => `
                <tr>
                    <td>${assignment.id}</td>
                    <td>${rolesMap.get(assignment.rolId) || `Rol ${assignment.rolId}`}</td>
                    <td>${formsMap.get(assignment.formId) || `Formulario ${assignment.formId}`}</td>
                    <td>${getPermissionText(assignment.permissionId)}</td>
                    <td>
                        <button class="action-btn edit-rolformpermission" data-id="${assignment.id}">Editar</button>
                        <button class="action-btn delete delete-rolformpermission" data-id="${assignment.id}">Eliminar</button>
                    </td>
                </tr>
            `).join('');
            
            // Configurar eventos para los botones de editar y eliminar
            setupRolFormPermissionsEvents(roles, forms, permissions);
        } else {
            tableBody.innerHTML = '<tr><td colspan="5" class="empty-row">No hay asignaciones registradas</td></tr>';
        }
    } catch (error) {
        console.error('Error al cargar asignaciones:', error);
        document.getElementById('rolformpermissions-table-body').innerHTML = 
            `<tr><td colspan="5" class="error-row">Error al cargar asignaciones: ${error.message}</td></tr>`;
    }
}

// Configurar eventos para los botones de editar y eliminar asignaciones
function setupRolFormPermissionsEvents(roles, forms, permissions) {
    const editButtons = document.querySelectorAll('.edit-rolformpermission');
    const deleteButtons = document.querySelectorAll('.delete-rolformpermission');
    
    editButtons.forEach(btn => {
        btn.addEventListener('click', async () => {
            const assignmentId = btn.getAttribute('data-id');
            try {
                // Obtener datos de la asignación desde la API
                const assignment = await api.rolFormPermissions.getById(assignmentId);
                if (assignment) {
                    showRolFormPermissionForm(assignment, roles, forms, permissions);
                } else {
                    showErrorModal('Error', 'No se pudo encontrar la asignación');
                }
            } catch (error) {
                console.error('Error al obtener asignación:', error);
                showErrorModal('Error', `No se pudo cargar la asignación: ${error.message}`);
            }
        });
    });
    
    deleteButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const assignmentId = btn.getAttribute('data-id');
            showConfirmModal(
                'Confirmar eliminación',
                `¿Está seguro de que desea eliminar la asignación con ID ${assignmentId}?`,
                async () => {
                    try {
                        // Eliminar asignación a través de la API
                        await api.rolFormPermissions.delete(assignmentId);
                        // Recargar lista de asignaciones
                        loadModuleContent('rolformpermisos');
                    } catch (error) {
                        console.error('Error al eliminar asignación:', error);
                        showErrorModal('Error', `No se pudo eliminar la asignación: ${error.message}`);
                    }
                }
            );
        });
    });
}

// Cargar y configurar el módulo de FormModules
async function loadFormModulesModule(containerEl) {
    containerEl.innerHTML = `
        <h2 class="module-title">Gestión de Formularios por Módulo</h2>
        <button id="add-formmodule-btn" class="btn btn-primary add-btn">Nueva Asignación</button>
        
        <table class="data-table">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Módulo</th>
                    <th>Formulario</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody id="formmodules-table-body">
                <tr>
                    <td colspan="4" class="loading-row">Cargando asignaciones...</td>
                </tr>
            </tbody>
        </table>
    `;
    
    // Configurar el botón para añadir asignación
    document.getElementById('add-formmodule-btn').addEventListener('click', async () => {
        // Cargar módulos y formularios para mostrar en el formulario
        try {
            const [modules, forms] = await Promise.all([
                api.modules.getAll(),
                api.forms.getAll()
            ]);
            
            showFormModuleForm(null, modules, forms);
        } catch (error) {
            console.error('Error al cargar datos para el formulario:', error);
            showErrorModal('Error', `No se pudieron cargar los datos necesarios: ${error.message}`);
        }
    });
    
    // Cargar datos de asignaciones desde la API
    try {
        // Primero necesitamos cargar las asignaciones
        const assignments = await api.formModules.getAll();
        
        // Luego cargar módulos y formularios para mostrar los nombres
        const [modules, forms] = await Promise.all([
            api.modules.getAll(),
            api.forms.getAll()
        ]);
        
        // Crear un mapa para acceder rápidamente a los nombres
        const modulesMap = new Map(modules.map(module => [module.id, module.code]));
        const formsMap = new Map(forms.map(form => [form.id, form.name]));
        
        const tableBody = document.getElementById('formmodules-table-body');
        
        if (assignments && assignments.length > 0) {
            // Generar filas de la tabla con los datos reales
            tableBody.innerHTML = assignments.map(assignment => `
                <tr>
                    <td>${assignment.id}</td>
                    <td>${modulesMap.get(assignment.moduleId) || `Módulo ${assignment.moduleId}`}</td>
                    <td>${formsMap.get(assignment.formId) || `Formulario ${assignment.formId}`}</td>
                    <td>
                        <button class="action-btn edit-formmodule" data-id="${assignment.id}">Editar</button>
                        <button class="action-btn delete delete-formmodule" data-id="${assignment.id}">Eliminar</button>
                    </td>
                </tr>
            `).join('');
            
            // Configurar eventos para los botones de editar y eliminar
            setupFormModulesEvents(modules, forms);
        } else {
            tableBody.innerHTML = '<tr><td colspan="4" class="empty-row">No hay asignaciones registradas</td></tr>';
        }
    } catch (error) {
        console.error('Error al cargar asignaciones:', error);
        document.getElementById('formmodules-table-body').innerHTML = 
            `<tr><td colspan="4" class="error-row">Error al cargar asignaciones: ${error.message}</td></tr>`;
    }
}

// Configurar eventos para los botones de editar y eliminar asignaciones
function setupFormModulesEvents(modules, forms) {
    const editButtons = document.querySelectorAll('.edit-formmodule');
    const deleteButtons = document.querySelectorAll('.delete-formmodule');
    
    editButtons.forEach(btn => {
        btn.addEventListener('click', async () => {
            const assignmentId = btn.getAttribute('data-id');
            try {
                // Obtener datos de la asignación desde la API
                const assignment = await api.formModules.getById(assignmentId);
                if (assignment) {
                    showFormModuleForm(assignment, modules, forms);
                } else {
                    showErrorModal('Error', 'No se pudo encontrar la asignación');
                }
            } catch (error) {
                console.error('Error al obtener asignación:', error);
                showErrorModal('Error', `No se pudo cargar la asignación: ${error.message}`);
            }
        });
    });
    
    deleteButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const assignmentId = btn.getAttribute('data-id');
            showConfirmModal(
                'Confirmar eliminación',
                `¿Está seguro de que desea eliminar la asignación con ID ${assignmentId}?`,
                async () => {
                    try {
                        // Eliminar asignación a través de la API
                        await api.formModules.delete(assignmentId);
                        // Recargar lista de asignaciones
                        loadModuleContent('formmodulos');
                    } catch (error) {
                        console.error('Error al eliminar asignación:', error);
                        showErrorModal('Error', `No se pudo eliminar la asignación: ${error.message}`);
                    }
                }
            );
        });
    });
}

// Generar contenido para el módulo de Perfil
function generateProfileContent(userData) {
    return `
        <h2 class="module-title">Mi Perfil</h2>
        <div class="profile-container">
            <div class="profile-info">
                <div class="form-row">
                    <label>Nombre:</label>
                    <p>${userData.name}</p>
                </div>
                <div class="form-row">
                    <label>Usuario:</label>
                    <p>${userData.username}</p>
                </div>
                <div class="form-row">
                    <label>Email:</label>
                    <p>${userData.email}</p>
                </div>
                <div class="form-row">
                    <label>Rol:</label>
                    <p>${userData.roles.join(', ')}</p>
                </div>
            </div>
            
            <div class="profile-actions">
                <button class="btn btn-primary" id="edit-profile-btn">Editar Perfil</button>
                <button class="btn btn-secondary" id="change-password-btn">Cambiar Contraseña</button>
            </div>
        </div>
    `;
}

// Configurar eventos para el módulo de Perfil
function setupProfileModule() {
    const editProfileBtn = document.getElementById('edit-profile-btn');
    const changePasswordBtn = document.getElementById('change-password-btn');
    
    if (editProfileBtn) {
        editProfileBtn.addEventListener('click', () => {
            const userData = auth.getCurrentUser();
            showUserProfileForm(userData);
        });
    }
    
    if (changePasswordBtn) {
        changePasswordBtn.addEventListener('click', () => {
            showChangePasswordForm();
        });
    }
}

// Mostrar formulario para editar perfil de usuario
function showUserProfileForm(userData) {
    const title = 'Editar Perfil';
    
    const modalContent = `
        <div class="form-container">
            <div class="form-row">
                <label for="profile-name">Nombre:</label>
                <input type="text" id="profile-name" name="name" value="${userData.name}" required>
            </div>
            <div class="form-row">
                <label for="profile-email">Email:</label>
                <input type="email" id="profile-email" name="email" value="${userData.email}" required>
            </div>
        </div>
    `;
    
    showModal(title, modalContent, async () => {
        try {
            const updatedData = {
                id: userData.id,
                name: document.getElementById('profile-name').value,
                email: document.getElementById('profile-email').value,
                // Mantener los datos que no se están modificando
                username: userData.username,
                password: userData.password // No enviamos la contraseña real
            };
            
            // Actualizar usuario a través de la API
            await api.users.update(updatedData);
            
            // Actualizar datos en localStorage
            const currentUser = auth.getCurrentUser();
            currentUser.name = updatedData.name;
            currentUser.email = updatedData.email;
            localStorage.setItem('user', JSON.stringify(currentUser));
            
            // Recargar el perfil
            loadModuleContent('perfil');
            closeModal();
        } catch (error) {
            console.error('Error al actualizar perfil:', error);
            showErrorModal('Error', `No se pudo actualizar el perfil: ${error.message}`);
        }
    });
}

// Mostrar formulario para cambiar contraseña
function showChangePasswordForm() {
    const title = 'Cambiar Contraseña';
    
    const modalContent = `
        <div class="form-container">
            <div class="form-row">
                <label for="current-password">Contraseña Actual:</label>
                <input type="password" id="current-password" name="currentPassword" required>
            </div>
            <div class="form-row">
                <label for="new-password">Nueva Contraseña:</label>
                <input type="password" id="new-password" name="newPassword" required>
            </div>
            <div class="form-row">
                <label for="confirm-password">Confirmar Contraseña:</label>
                <input type="password" id="confirm-password" name="confirmPassword" required>
            </div>
        </div>
    `;
    
    showModal(title, modalContent, async () => {
        const currentPassword = document.getElementById('current-password').value;
        const newPassword = document.getElementById('new-password').value;
        const confirmPassword = document.getElementById('confirm-password').value;
        
        // Validar que las contraseñas coincidan
        if (newPassword !== confirmPassword) {
            showErrorModal('Error', 'Las contraseñas no coinciden');
            return;
        }
        
        try {
            // Aquí iría la lógica para cambiar la contraseña a través de la API
            // Por ahora, solo simulamos el cambio
            const userData = auth.getCurrentUser();
            
            // En el modo demo, actualizar la contraseña en los usuarios mock
            auth.mockUsers.forEach(user => {
                if (user.id === userData.id) {
                    user.password = newPassword;
                }
            });
            
            closeModal();
            showErrorModal('Éxito', 'Contraseña cambiada correctamente');
        } catch (error) {
            console.error('Error al cambiar contraseña:', error);
            showErrorModal('Error', `No se pudo cambiar la contraseña: ${error.message}`);
        }
    });
}

// Funciones para mostrar formularios modales
function showUserForm(userData = null) {
    const title = userData ? 'Editar Usuario' : 'Nuevo Usuario';
    
    // Crear contenido del modal
    const modalContent = `
        <div class="form-container">
            <div class="form-row">
                <label for="user-name">Nombre:</label>
                <input type="text" id="user-name" name="name" required>
            </div>
            <div class="form-row">
                <label for="user-email">Email:</label>
                <input type="email" id="user-email" name="email" required>
            </div>
            <div class="form-row">
                <label for="user-password">Contraseña:</label>
                <input type="password" id="user-password" name="password" ${userData ? '' : 'required'}>
                ${userData ? '<small>Dejar en blanco para mantener la contraseña actual</small>' : ''}
            </div>
        </div>
    `;
    
    // Mostrar el modal
    showModal(title, modalContent, async () => {
        // Recoger los datos del formulario
        const name = document.getElementById('user-name').value;
        const email = document.getElementById('user-email').value;
        const password = document.getElementById('user-password').value;
        
        try {
            if (userData) {
                // Actualizar usuario existente
                const updatedData = {
                    id: userData.id,
                    name,
                    email
                };
                
                // Solo incluir la contraseña si se ha proporcionado una nueva
                if (password) {
                    updatedData.password = password;
                }
                
                await api.users.update(updatedData);
            } else {
                // Crear nuevo usuario
                await api.users.create({
                    name,
                    email,
                    password
                });
            }
            
            // Cerrar el modal y recargar la lista de usuarios
            closeModal();
            loadModuleContent('usuarios');
        } catch (error) {
            console.error('Error al guardar usuario:', error);
            showErrorModal('Error', `No se pudo guardar el usuario: ${error.message}`);
        }
    });
    
    // Si es una edición, rellenar el formulario con los datos del usuario
    if (userData) {
        document.getElementById('user-name').value = userData.name;
        document.getElementById('user-email').value = userData.email;
    }
}

function showRoleForm(roleData = null) {
    const title = roleData ? 'Editar Rol' : 'Nuevo Rol';
    
    const modalContent = `
        <div class="form-container">
            <div class="form-row">
                <label for="role-name">Nombre:</label>
                <input type="text" id="role-name" name="name" required>
            </div>
            <div class="form-row">
                <label for="role-description">Descripción:</label>
                <textarea id="role-description" name="description"></textarea>
            </div>
        </div>
    `;
    
    showModal(title, modalContent, async () => {
        // Recoger los datos del formulario
        const name = document.getElementById('role-name').value;
        const description = document.getElementById('role-description').value;
        
        try {
            if (roleData) {
                // Actualizar rol existente
                await api.roles.update({
                    id: roleData.id,
                    name,
                    description
                });
            } else {
                // Crear nuevo rol
                await api.roles.create({
                    name,
                    description
                });
            }
            
            // Cerrar el modal y recargar la lista de roles
            closeModal();
            loadModuleContent('roles');
        } catch (error) {
            console.error('Error al guardar rol:', error);
            showErrorModal('Error', `No se pudo guardar el rol: ${error.message}`);
        }
    });
    
    // Si es una edición, rellenar el formulario con los datos del rol
    if (roleData) {
        document.getElementById('role-name').value = roleData.name;
        document.getElementById('role-description').value = roleData.description || '';
    }
}

function showModuleForm(moduleData = null) {
    const title = moduleData ? 'Editar Módulo' : 'Nuevo Módulo';
    
    const modalContent = `
        <div class="form-container">
            <div class="form-row">
                <label for="module-code">Código:</label>
                <input type="text" id="module-code" name="code" required>
            </div>
            <div class="form-row">
                <label for="module-active">Activo:</label>
                <select id="module-active" name="active">
                    <option value="true">Sí</option>
                    <option value="false">No</option>
                </select>
            </div>
        </div>
    `;
    
    showModal(title, modalContent, async () => {
        // Recoger los datos del formulario
        const code = document.getElementById('module-code').value;
        const active = document.getElementById('module-active').value === 'true';
        
        try {
            if (moduleData) {
                // Actualizar módulo existente
                await api.modules.update({
                    id: moduleData.id,
                    code,
                    active
                });
            } else {
                // Crear nuevo módulo
                await api.modules.create({
                    code,
                    active
                });
            }
            
            // Cerrar el modal y recargar la lista de módulos
            closeModal();
            loadModuleContent('modulos');
        } catch (error) {
            console.error('Error al guardar módulo:', error);
            showErrorModal('Error', `No se pudo guardar el módulo: ${error.message}`);
        }
    });
    
    // Si es una edición, rellenar el formulario con los datos del módulo
    if (moduleData) {
        document.getElementById('module-code').value = moduleData.code;
        document.getElementById('module-active').value = moduleData.active.toString();
    }
}

function showFormForm(formData = null) {
    const title = formData ? 'Editar Formulario' : 'Nuevo Formulario';
    
    const modalContent = `
        <div class="form-container">
            <div class="form-row">
                <label for="form-name">Nombre:</label>
                <input type="text" id="form-name" name="name" required>
            </div>
            <div class="form-row">
                <label for="form-code">Código:</label>
                <input type="text" id="form-code" name="code" required>
            </div>
            <div class="form-row">
                <label for="form-active">Activo:</label>
                <select id="form-active" name="active">
                    <option value="true">Sí</option>
                    <option value="false">No</option>
                </select>
            </div>
        </div>
    `;
    
    showModal(title, modalContent, async () => {
        // Recoger los datos del formulario
        const name = document.getElementById('form-name').value;
        const code = document.getElementById('form-code').value;
        const active = document.getElementById('form-active').value === 'true';
        
        try {
            if (formData) {
                // Actualizar formulario existente
                await api.forms.update({
                    id: formData.id,
                    name,
                    code,
                    active
                });
            } else {
                // Crear nuevo formulario
                await api.forms.create({
                    name,
                    code,
                    active
                });
            }
            
            // Cerrar el modal y recargar la lista de formularios
            closeModal();
            loadModuleContent('formularios');
        } catch (error) {
            console.error('Error al guardar formulario:', error);
            showErrorModal('Error', `No se pudo guardar el formulario: ${error.message}`);
        }
    });
    
    // Si es una edición, rellenar el formulario con los datos
    if (formData) {
        document.getElementById('form-name').value = formData.name;
        document.getElementById('form-code').value = formData.code;
        document.getElementById('form-active').value = formData.active.toString();
    }
}

function showPermissionForm(permissionData = null) {
    const title = permissionData ? 'Editar Permiso' : 'Nuevo Permiso';
    
    const modalContent = `
        <div class="form-container">
            <div class="form-row">
                <div class="permission-grid">
                    <div class="permission-item">
                        <input type="checkbox" id="permission-read" name="canRead">
                        <label for="permission-read">Lectura</label>
                    </div>
                    <div class="permission-item">
                        <input type="checkbox" id="permission-create" name="canCreate">
                        <label for="permission-create">Creación</label>
                    </div>
                    <div class="permission-item">
                        <input type="checkbox" id="permission-update" name="canUpdate">
                        <label for="permission-update">Actualización</label>
                    </div>
                    <div class="permission-item">
                        <input type="checkbox" id="permission-delete" name="canDelete">
                        <label for="permission-delete">Eliminación</label>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    showModal(title, modalContent, async () => {
        // Recoger los datos del formulario
        const canRead = document.getElementById('permission-read').checked;
        const canCreate = document.getElementById('permission-create').checked;
        const canUpdate = document.getElementById('permission-update').checked;
        const canDelete = document.getElementById('permission-delete').checked;
        
        try {
            if (permissionData) {
                // Actualizar permiso existente
                await api.permissions.update({
                    id: permissionData.id,
                    canRead,
                    canCreate,
                    canUpdate,
                    canDelete
                });
            } else {
                // Crear nuevo permiso
                await api.permissions.create({
                    canRead,
                    canCreate,
                    canUpdate,
                    canDelete
                });
            }
            
            // Cerrar el modal y recargar la lista de permisos
            closeModal();
            loadModuleContent('permisos');
        } catch (error) {
            console.error('Error al guardar permiso:', error);
            showErrorModal('Error', `No se pudo guardar el permiso: ${error.message}`);
        }
    });
    
    // Si es una edición, rellenar el formulario con los datos
    if (permissionData) {
        document.getElementById('permission-read').checked = permissionData.canRead;
        document.getElementById('permission-create').checked = permissionData.canCreate;
        document.getElementById('permission-update').checked = permissionData.canUpdate;
        document.getElementById('permission-delete').checked = permissionData.canDelete;
    }
}

function showRolFormPermissionForm(permissionData = null, roles = [], forms = [], permissions = []) {
    const title = permissionData ? 'Editar Asignación de Permisos' : 'Nueva Asignación de Permisos';
    
    // Generar las opciones para los selects
    const rolesOptions = roles.map(role => `<option value="${role.id}">${role.name}</option>`).join('');
    const formsOptions = forms.map(form => `<option value="${form.id}">${form.name}</option>`).join('');
    const permissionsOptions = permissions.map(permission => {
        // Crear una descripción de los permisos
        const permTypes = [];
        if (permission.canRead) permTypes.push('Lectura');
        if (permission.canCreate) permTypes.push('Creación');
        if (permission.canUpdate) permTypes.push('Actualización');
        if (permission.canDelete) permTypes.push('Eliminación');
        
        const description = permTypes.join(', ') || 'Ninguno';
        
        return `<option value="${permission.id}">${description}</option>`;
    }).join('');
    
    const modalContent = `
        <div class="form-container">
            <div class="form-row">
                <label for="rolformpermission-rol">Rol:</label>
                <select id="rolformpermission-rol" name="rolId" required>
                    <option value="">Seleccionar rol</option>
                    ${rolesOptions}
                </select>
            </div>
            <div class="form-row">
                <label for="rolformpermission-form">Formulario:</label>
                <select id="rolformpermission-form" name="formId" required>
                    <option value="">Seleccionar formulario</option>
                    ${formsOptions}
                </select>
            </div>
            <div class="form-row">
                <label for="rolformpermission-permission">Permiso:</label>
                <select id="rolformpermission-permission" name="permissionId" required>
                    <option value="">Seleccionar permiso</option>
                    ${permissionsOptions}
                </select>
            </div>
        </div>
    `;
    
    showModal(title, modalContent, async () => {
        // Recoger los datos del formulario
        const rolId = parseInt(document.getElementById('rolformpermission-rol').value);
        const formId = parseInt(document.getElementById('rolformpermission-form').value);
        const permissionId = parseInt(document.getElementById('rolformpermission-permission').value);
        
        // Validar datos
        if (isNaN(rolId) || isNaN(formId) || isNaN(permissionId)) {
            showErrorModal('Error', 'Debe seleccionar valores válidos para todos los campos');
            return;
        }
        
        try {
            if (permissionData) {
                // Actualizar asignación existente
                await api.rolFormPermissions.update({
                    id: permissionData.id,
                    rolId,
                    formId,
                    permissionId
                });
            } else {
                // Crear nueva asignación
                await api.rolFormPermissions.create({
                    rolId,
                    formId,
                    permissionId
                });
            }
            
            // Cerrar el modal y recargar la lista
            closeModal();
            loadModuleContent('rolformpermisos');
        } catch (error) {
            console.error('Error al guardar asignación:', error);
            showErrorModal('Error', `No se pudo guardar la asignación: ${error.message}`);
        }
    });
    
    // Si es una edición, rellenar el formulario con los datos
    if (permissionData) {
        document.getElementById('rolformpermission-rol').value = permissionData.rolId;
        document.getElementById('rolformpermission-form').value = permissionData.formId;
        document.getElementById('rolformpermission-permission').value = permissionData.permissionId;
    }
}

function showFormModuleForm(assignmentData = null, modules = [], forms = []) {
    const title = assignmentData ? 'Editar Asignación de Formulario a Módulo' : 'Nueva Asignación de Formulario a Módulo';
    
    // Generar las opciones para los selects
    const modulesOptions = modules.map(module => `<option value="${module.id}">${module.code}</option>`).join('');
    const formsOptions = forms.map(form => `<option value="${form.id}">${form.name}</option>`).join('');
    
    const modalContent = `
        <div class="form-container">
            <div class="form-row">
                <label for="formmodule-module">Módulo:</label>
                <select id="formmodule-module" name="moduleId" required>
                    <option value="">Seleccionar módulo</option>
                    ${modulesOptions}
                </select>
            </div>
            <div class="form-row">
                <label for="formmodule-form">Formulario:</label>
                <select id="formmodule-form" name="formId" required>
                    <option value="">Seleccionar formulario</option>
                    ${formsOptions}
                </select>
            </div>
        </div>
    `;
    
    showModal(title, modalContent, async () => {
        // Recoger los datos del formulario
        const moduleId = parseInt(document.getElementById('formmodule-module').value);
        const formId = parseInt(document.getElementById('formmodule-form').value);
        
        // Validar datos
        if (isNaN(moduleId) || isNaN(formId)) {
            showErrorModal('Error', 'Debe seleccionar valores válidos para todos los campos');
            return;
        }
        
        try {
            if (assignmentData) {
                // Actualizar asignación existente
                await api.formModules.update({
                    id: assignmentData.id,
                    moduleId,
                    formId
                });
            } else {
                // Crear nueva asignación
                await api.formModules.create({
                    moduleId,
                    formId
                });
            }
            
            // Cerrar el modal y recargar la lista
            closeModal();
            loadModuleContent('formmodulos');
        } catch (error) {
            console.error('Error al guardar asignación:', error);
            showErrorModal('Error', `No se pudo guardar la asignación: ${error.message}`);
        }
    });
    
    // Si es una edición, rellenar el formulario con los datos
    if (assignmentData) {
        document.getElementById('formmodule-module').value = assignmentData.moduleId;
        document.getElementById('formmodule-form').value = assignmentData.formId;
    }
}

// Mostrar modal genérico
function showModal(title, content, confirmCallback) {
    // Crear contenedor del modal si no existe
    let modalBg = document.getElementById('modal-bg');
    
    if (!modalBg) {
        modalBg = document.createElement('div');
        modalBg.id = 'modal-bg';
        modalBg.className = 'modal-bg';
        document.body.appendChild(modalBg);
    }
    
    // Crear contenido del modal
    modalBg.innerHTML = `
        <div class="modal">
            <div class="modal-header">
                <h3>${title}</h3>
                <button class="modal-close">&times;</button>
            </div>
            <div class="modal-body">
                ${content}
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" id="modal-cancel-btn">Cancelar</button>
                <button class="btn btn-primary" id="modal-confirm-btn">Guardar</button>
            </div>
        </div>
    `;
    
    // Mostrar el modal
    modalBg.classList.add('active');
    
    // Configurar eventos
    document.querySelector('.modal-close').addEventListener('click', closeModal);
    document.getElementById('modal-cancel-btn').addEventListener('click', closeModal);
    document.getElementById('modal-confirm-btn').addEventListener('click', confirmCallback);
}

// Cerrar modal
function closeModal() {
    const modalBg = document.getElementById('modal-bg');
    if (modalBg) {
        modalBg.classList.remove('active');
    }
}

// Mostrar modal de confirmación
function showConfirmModal(title, message, confirmCallback) {
    const content = `<p>${message}</p>`;
    
    // Crear contenedor del modal si no existe
    let modalBg = document.getElementById('modal-bg');
    
    if (!modalBg) {
        modalBg = document.createElement('div');
        modalBg.id = 'modal-bg';
        modalBg.className = 'modal-bg';
        document.body.appendChild(modalBg);
    }
    
    // Crear contenido del modal
    modalBg.innerHTML = `
        <div class="modal">
            <div class="modal-header">
                <h3>${title}</h3>
                <button class="modal-close">&times;</button>
            </div>
            <div class="modal-body">
                ${content}
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" id="modal-cancel-btn">Cancelar</button>
                <button class="btn btn-primary delete" id="modal-confirm-btn">Eliminar</button>
            </div>
        </div>
    `;
    
    // Mostrar el modal
    modalBg.classList.add('active');
    
    // Configurar eventos
    document.querySelector('.modal-close').addEventListener('click', closeModal);
    document.getElementById('modal-cancel-btn').addEventListener('click', closeModal);
    document.getElementById('modal-confirm-btn').addEventListener('click', () => {
        confirmCallback();
        closeModal();
    });
}

// Mostrar modal de error
function showErrorModal(title, message) {
    const content = `<p>${message}</p>`;
    
    // Crear contenedor del modal si no existe
    let modalBg = document.getElementById('modal-bg');
    
    if (!modalBg) {
        modalBg = document.createElement('div');
        modalBg.id = 'modal-bg';
        modalBg.className = 'modal-bg';
        document.body.appendChild(modalBg);
    }
    
    // Crear contenido del modal
    modalBg.innerHTML = `
        <div class="modal">
            <div class="modal-header">
                <h3>${title}</h3>
                <button class="modal-close">&times;</button>
            </div>
            <div class="modal-body">
                ${content}
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" id="modal-confirm-btn">Aceptar</button>
            </div>
        </div>
    `;
    
    // Mostrar el modal
    modalBg.classList.add('active');
    
    // Configurar eventos
    document.querySelector('.modal-close').addEventListener('click', closeModal);
    document.getElementById('modal-confirm-btn').addEventListener('click', closeModal);
}