document.addEventListener('DOMContentLoaded', function() {
    // Si ya está autenticado, redirigir al dashboard
    if (isAuthenticated()) {
        window.location.href = 'dashboard.html';
        return;
    }
    
    // Obtener el formulario de registro
    const registerForm = document.getElementById('registerAdminForm');
    
    // Agregar evento de envío al formulario
    registerForm.addEventListener('submit', handleRegisterSubmit);
    
    /**
     * Maneja el envío del formulario de registro de administrador
     */
    async function handleRegisterSubmit(event) {
        event.preventDefault();
        
        // Obtener los valores del formulario
        const name = document.getElementById('name').value;
        const email = document.getElementById('email').value;
        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;
        const confirmPassword = document.getElementById('confirmPassword').value;
        const companyCode = document.getElementById('companyCode').value;
        
        // Validar campos
        if (!name || !email || !username || !password || !confirmPassword || !companyCode) {
            showMessage('registerMessage', 'Por favor, complete todos los campos.', 'error');
            return;
        }
        
        // Validar que las contraseñas coincidan
        if (password !== confirmPassword) {
            showMessage('registerMessage', 'Las contraseñas no coinciden.', 'error');
            return;
        }
        
        // Validar el código de empresa (esto ahora sería validado en el backend)
        const validCompanyCodes = ['ADMIN123', 'EMPRESA2023', 'SYSADMIN2025'];
        if (!validCompanyCodes.includes(companyCode)) {
            showMessage('registerMessage', 'Código de empresa no válido.', 'error');
            return;
        }
        
        try {
            // 1. Verificar si existen los roles necesarios
            const roles = await apiRequest(API_CONFIG.ENDPOINTS.ROL);
            
            // Verificar si el rol de administrador existe
            let adminRoleId = API_CONFIG.ROLES.ADMIN;
            let adminRoleExists = false;
            
            if (roles && roles.length > 0) {
                const adminRole = roles.find(r => r.name === 'Administrador');
                
                if (adminRole) {
                    adminRoleExists = true;
                    adminRoleId = adminRole.id;
                    // Actualizar la configuración global
                    API_CONFIG.ROLES.ADMIN = adminRoleId;
                }
            }
            
            // Si el rol de administrador no existe, crearlo
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
                        API_CONFIG.ROLES.ADMIN = adminRoleId;
                        console.log('Rol de Administrador creado con ID:', adminRoleId);
                    }
                } catch (error) {
                    console.error('Error al crear rol de Administrador:', error);
                    showMessage('registerMessage', 'Error al crear rol de administrador. Por favor, contacte al soporte técnico.', 'error');
                    return;
                }
            }
            
            showMessage('registerMessage', 'Procesando su registro...', 'info');
            
            // 2. Crear trabajador primero
            const workerData = {
                FirstName: name.split(' ')[0], // Tomar el primer nombre
                LastName: name.includes(' ') ? name.substring(name.indexOf(' ') + 1) : '', // Tomar el apellido si hay espacio
                IdentityDocument: companyCode, // Usar el código de empresa como documento de identidad
                JobTitle: 'Administrator',
                Email: email,
                Phone: 12345678, // Valor por defecto
                HireDate: new Date().toISOString()
            };
            
            const workerResponse = await apiRequest(API_CONFIG.ENDPOINTS.WORKER, 'POST', workerData);
            
            if (!workerResponse || !workerResponse.workerId) {
                throw new Error('Error al crear el trabajador. No se recibió una respuesta válida.');
            }
            
            console.log('Trabajador creado con ID:', workerResponse.workerId);
            
            // 3. Crear login para el trabajador
            const loginData = {
                Username: username,
                Password: password
            };
            
            const loginResponse = await apiRequest(API_CONFIG.ENDPOINTS.LOGIN, 'POST', loginData);
            
            if (!loginResponse || !loginResponse.loginId) {
                // Si falla, intentar eliminar el trabajador creado
                try {
                    await apiRequest(`${API_CONFIG.ENDPOINTS.WORKER}/${workerResponse.workerId}`, 'DELETE');
                } catch (deleteError) {
                    console.error('Error al eliminar trabajador tras fallo de login:', deleteError);
                }
                
                throw new Error('Error al crear el login para el administrador.');
            }
            
            console.log('Login creado con ID:', loginResponse.loginId);
            
            // 4. Crear relación entre trabajador y login
            const workerLoginData = {
                LoginId: loginResponse.loginId,
                WorkerId: workerResponse.workerId,
                Username: username,
                Password: password,
                Status: true
            };
            
            const workerLoginResponse = await apiRequest(API_CONFIG.ENDPOINTS.WORKER_LOGIN, 'POST', workerLoginData);
            
            if (!workerLoginResponse) {
                // Si falla, intentar eliminar los recursos creados
                try {
                    await apiRequest(`${API_CONFIG.ENDPOINTS.LOGIN}/${loginResponse.loginId}`, 'DELETE');
                    await apiRequest(`${API_CONFIG.ENDPOINTS.WORKER}/${workerResponse.workerId}`, 'DELETE');
                } catch (deleteError) {
                    console.error('Error al eliminar recursos tras fallo de asignación de login:', deleteError);
                }
                
                throw new Error('Error al vincular login con el trabajador.');
            }
            
            console.log('Relación Worker-Login creada correctamente');
            
            // 5. Crear usuario para el administrador (para integrarlo en el sistema de roles)
            const userData = {
                Name: name,
                Email: email,
                Password: password,
                WorkerId: workerResponse.workerId
            };
            
            const userResponse = await apiRequest(API_CONFIG.ENDPOINTS.USER, 'POST', userData);
            
            if (!userResponse || !userResponse.id) {
                // Si falla, intentar eliminar los recursos creados
                try {
                    await apiRequest(`${API_CONFIG.ENDPOINTS.WORKER_LOGIN}/${workerLoginResponse.id}`, 'DELETE');
                    await apiRequest(`${API_CONFIG.ENDPOINTS.LOGIN}/${loginResponse.loginId}`, 'DELETE');
                    await apiRequest(`${API_CONFIG.ENDPOINTS.WORKER}/${workerResponse.workerId}`, 'DELETE');
                } catch (deleteError) {
                    console.error('Error al eliminar recursos tras fallo de creación de usuario:', deleteError);
                }
                
                throw new Error('Error al crear el usuario para el administrador.');
            }
            
            console.log('Usuario creado con ID:', userResponse.id);
            
            // 6. Asignar rol de administrador al usuario
            const rolUserData = {
                UserId: userResponse.id,
                RolId: adminRoleId // Usar el ID verificado/creado
            };
            
            console.log('Asignando rol al usuario:', rolUserData);
            
            const rolUserResponse = await apiRequest(API_CONFIG.ENDPOINTS.ROL_USER, 'POST', rolUserData);
            
            if (!rolUserResponse) {
                // Si falla, intentar eliminar los recursos creados
                try {
                    await apiRequest(`${API_CONFIG.ENDPOINTS.USER}/${userResponse.id}`, 'DELETE');
                    await apiRequest(`${API_CONFIG.ENDPOINTS.WORKER_LOGIN}/${workerLoginResponse.id}`, 'DELETE');
                    await apiRequest(`${API_CONFIG.ENDPOINTS.LOGIN}/${loginResponse.loginId}`, 'DELETE');
                    await apiRequest(`${API_CONFIG.ENDPOINTS.WORKER}/${workerResponse.workerId}`, 'DELETE');
                } catch (deleteError) {
                    console.error('Error al eliminar recursos tras fallo de asignación de rol:', deleteError);
                }
                
                throw new Error('Error al asignar rol de administrador.');
            }
            
            console.log('Rol de administrador asignado correctamente');
            
            // Mostrar mensaje de éxito
            showMessage('registerMessage', '¡Registro de administrador exitoso! Ahora puedes iniciar sesión.', 'success');
            
            // Limpiar formulario
            registerForm.reset();
            
            // Redireccionar a login después de 2 segundos
            setTimeout(() => {
                window.location.href = 'login.html';
            }, 2000);
            
        } catch (error) {
            showMessage('registerMessage', error.message || 'Error al registrar administrador.', 'error');
            console.error('Error al registrar administrador:', error);
        }
    }
});