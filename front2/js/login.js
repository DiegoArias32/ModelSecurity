document.addEventListener('DOMContentLoaded', function() {
    // Si ya está autenticado, redirigir al dashboard
    if (isAuthenticated()) {
        window.location.href = 'dashboard.html';
        return;
    }
    
    // Obtener el formulario de login
    const loginForm = document.getElementById('loginForm');
    
    // Agregar evento de envío al formulario
    loginForm.addEventListener('submit', handleLoginSubmit);
    
    /**
     * Maneja el envío del formulario de login
     */
    async function handleLoginSubmit(event) {
        event.preventDefault();
        
        // Obtener los valores del formulario
        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;
        
        // Validar campos
        if (!username || !password) {
            showMessage('loginMessage', 'Por favor, complete todos los campos.', 'error');
            return;
        }
        
        try {
            showMessage('loginMessage', 'Iniciando sesión...', 'info');
            
            // Obtener usuarios para buscar usuario por nombre de usuario
            const users = await apiRequest(API_CONFIG.ENDPOINTS.USER);
            
            if (!users || users.length === 0) {
                throw new Error('No se encontraron usuarios registrados.');
            }
            
            // Buscar el usuario por nombre o email
            const user = users.find(u => 
                (u.name && u.name.toLowerCase() === username.toLowerCase()) || 
                (u.email && u.email.toLowerCase() === username.toLowerCase())
            );
            
            if (!user) {
                throw new Error('Usuario no encontrado. Verifique su nombre de usuario o correo electrónico.');
            }
            
            // Verificar contraseña (esto normalmente lo haría el backend de forma segura)
            if (user.password !== password) {
                throw new Error('Contraseña incorrecta.');
            }
            
            // Buscar el rol del usuario
            let rolId = API_CONFIG.ROLES.USER; // Rol por defecto
            let rolName = 'Usuario';
            
            try {
                // Obtener roles del usuario
                const rolUsers = await apiRequest(API_CONFIG.ENDPOINTS.ROL_USER);
                const userRol = rolUsers.find(ru => ru.userId === user.id);
                
                if (userRol) {
                    rolId = userRol.rolId;
                    
                    // Obtener detalles del rol
                    const roles = await apiRequest(API_CONFIG.ENDPOINTS.ROL);
                    const rol = roles.find(r => r.id === rolId);
                    
                    if (rol) {
                        rolName = rol.name;
                    }
                }
            } catch (roleError) {
                console.error('Error al obtener el rol del usuario:', roleError);
                // No fallamos el login por un error en la obtención del rol
            }
            
            // Crear objeto de usuario con la información necesaria
            const userObj = {
                id: user.id,
                username: username,
                name: user.name || username,
                email: user.email || '',
                rolId: rolId,
                rolName: rolName,
                token: 'fake-token-' + Math.random().toString(36).substring(2)
            };
            
            // Guardar datos de usuario
            saveToStorage('user', userObj);
            
            // Mostrar mensaje de éxito
            showMessage('loginMessage', '¡Inicio de sesión exitoso! Redirigiendo...', 'success');
            
            // Redirigir al dashboard
            setTimeout(() => {
                window.location.href = 'dashboard.html';
            }, 1000);
            
        } catch (error) {
            showMessage('loginMessage', error.message || 'Error al iniciar sesión. Inténtelo de nuevo.', 'error');
            console.error('Error al iniciar sesión:', error);
        }
    }
});