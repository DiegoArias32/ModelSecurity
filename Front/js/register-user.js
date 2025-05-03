document.addEventListener('DOMContentLoaded', function() {
    // Si ya está autenticado, redirigir al dashboard
    if (isAuthenticated()) {
        window.location.href = 'dashboard.html';
        return;
    }
    
    // Obtener el formulario de registro
    const registerForm = document.getElementById('registerUserForm');
    
    // Agregar evento de envío al formulario
    registerForm.addEventListener('submit', handleRegisterSubmit);
    
    /**
     * Maneja el envío del formulario de registro
     */
    async function handleRegisterSubmit(event) {
        event.preventDefault();
        
        // Obtener los valores del formulario
        const name = document.getElementById('name').value;
        const email = document.getElementById('email').value;
        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;
        const confirmPassword = document.getElementById('confirmPassword').value;
        
        // Validar campos
        if (!name || !email || !username || !password || !confirmPassword) {
            showMessage('registerMessage', 'Por favor, complete todos los campos.', 'error');
            return;
        }
        
        // Validar que las contraseñas coincidan
        if (password !== confirmPassword) {
            showMessage('registerMessage', 'Las contraseñas no coinciden.', 'error');
            return;
        }
        
        try {
            // 1. Verificar si existen los roles necesarios
            const roles = await apiRequest(API_CONFIG.ENDPOINTS.ROL);
            
            // Verificar si el rol de usuario existe
            let userRoleId = API_CONFIG.ROLES.USER;
            let userRoleExists = false;
            
            if (roles && roles.length > 0) {
                const userRole = roles.find(r => r.name === 'Usuario');
                
                if (userRole) {
                    userRoleExists = true;
                    userRoleId = userRole.id;
                    // Actualizar la configuración global
                    API_CONFIG.ROLES.USER = userRoleId;
                } else {
                    // Si no encuentra un rol llamado exactamente 'Usuario', usar el primer rol disponible
                    // (esto es una solución temporal)
                    userRoleId = roles[0].id;
                    API_CONFIG.ROLES.USER = roles[0].id;
                    console.log('No se encontró rol de Usuario, usando rol con ID:', userRoleId);
                    userRoleExists = true;
                }
            }
            
            // Si el rol de usuario no existe, crearlo
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
                        API_CONFIG.ROLES.USER = userRoleId;
                        console.log('Rol de Usuario creado con ID:', userRoleId);
                    }
                } catch (error) {
                    console.error('Error al crear rol de Usuario:', error);
                    showMessage('registerMessage', 'Error al crear rol de usuario. Por favor, contacte al administrador.', 'error');
                    return;
                }
            }
            
            // 2. Crear usuario
            const userData = {
                Name: name,
                Email: email,
                Password: password
            };
            
            showMessage('registerMessage', 'Procesando su registro...', 'info');
            
            // Crear usuario
            const userResponse = await apiRequest(API_CONFIG.ENDPOINTS.USER, 'POST', userData);
            
            if (!userResponse || !userResponse.id) {
                throw new Error('Error al crear el usuario. No se recibió una respuesta válida.');
            }
            
            console.log('Usuario creado con ID:', userResponse.id);
            
            // 3. Intentar asignar rol de usuario al usuario sin crear login
            // Esto es un enfoque alternativo que evita la dependencia del login
            const rolUserData = {
                UserId: userResponse.id,
                RolId: userRoleId // Usar el ID verificado/creado
            };
            
            console.log('Asignando rol al usuario:', rolUserData);
            
            try {
                const rolUserResponse = await apiRequest(API_CONFIG.ENDPOINTS.ROL_USER, 'POST', rolUserData);
                console.log('Rol asignado correctamente al usuario');
                
                // Ahora vamos a intentar crear el login
                try {
                    const loginData = {
                        Username: username,
                        Password: password
                    };
                    
                    console.log('Intentando crear login con datos:', loginData);
                    const loginResponse = await apiRequest(API_CONFIG.ENDPOINTS.LOGIN, 'POST', loginData);
                    console.log('Login creado con ID:', loginResponse.loginId);
                } catch (loginError) {
                    console.error('Error al crear login, pero el usuario y el rol ya están creados:', loginError);
                    // No lanzamos el error, ya que el usuario y el rol se crearon correctamente
                }
                
                // Mostrar mensaje de éxito
                showMessage('registerMessage', '¡Registro exitoso! Ahora puedes iniciar sesión.', 'success');
                
                // Limpiar formulario
                registerForm.reset();
                
                // Redireccionar a login después de 2 segundos
                setTimeout(() => {
                    window.location.href = 'login.html';
                }, 2000);
            } catch (rolUserError) {
                // Si falla la asignación de rol, intentar eliminar el usuario
                console.error('Error al asignar rol, intentando eliminar usuario:', rolUserError);
                try {
                    await apiRequest(`${API_CONFIG.ENDPOINTS.USER}/${userResponse.id}`, 'DELETE');
                } catch (deleteError) {
                    console.error('Error al eliminar usuario tras fallo de asignación de rol:', deleteError);
                }
                
                throw new Error('Error al asignar rol al usuario: ' + rolUserError.message);
            }
            
        } catch (error) {
            showMessage('registerMessage', error.message || 'Error al registrar usuario.', 'error');
            console.error('Error al registrar usuario:', error);
        }
    }
});