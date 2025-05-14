// Servicio de autenticación
const auth = {
    // Mock de usuarios para demostración (solo para desarrollo)
    mockUsers: [
        {
            id: 1,
            username: 'admin',
            password: 'admin123',
            name: 'Administrador',
            email: 'admin@example.com',
            roles: ['Admin']
        },
        {
            id: 2,
            username: 'user',
            password: 'user123',
            name: 'Usuario Regular',
            email: 'user@example.com',
            roles: ['Usuario']
        }
    ],
    
    // Validar credenciales en modo demo
    validateDemoCredentials(username, password) {
        const user = this.mockUsers.find(u => u.username === username && u.password === password);
        if (user) {
            // Crear token simulado
            const token = btoa(JSON.stringify({ id: user.id, username: user.username, roles: user.roles }));
            
            // Extraer solo los datos necesarios para almacenar en localStorage
            const userData = {
                id: user.id,
                username: user.username,
                name: user.name,
                email: user.email,
                roles: user.roles,
                token
            };
            
            return userData;
        }
        return null;
    },
    
    // Iniciar sesión en modo demo
    demoLogin(username, password) {
        const userData = this.validateDemoCredentials(username, password);
        
        if (userData) {
            // Guardar datos en localStorage
            localStorage.setItem('user', JSON.stringify(userData));
            localStorage.setItem('token', userData.token);
            return userData;
        }
        
        throw new Error('Credenciales inválidas');
    },
    
    // Iniciar sesión como un rol específico (demo)
    demoLoginAsRole(role) {
        let userData;
        
        if (role === 'Admin') {
            userData = this.mockUsers[0];
        } else if (role === 'User') {
            userData = this.mockUsers[1];
        } else {
            throw new Error('Rol no válido');
        }
        
        // Crear token simulado
        const token = btoa(JSON.stringify({ id: userData.id, username: userData.username, roles: userData.roles }));
        
        // Añadir token a los datos de usuario
        userData = {
            ...userData,
            token
        };
        
        // Guardar datos en localStorage
        localStorage.setItem('user', JSON.stringify(userData));
        localStorage.setItem('token', userData.token);
        
        return userData;
    },
    
    // Cerrar sesión
    logout() {
        localStorage.removeItem('user');
        localStorage.removeItem('token');
        window.location.reload();
    },
    
    // Verificar si el usuario está autenticado
    isAuthenticated() {
        return !!localStorage.getItem('token');
    },
    
    // Obtener usuario actual
    getCurrentUser() {
        const userJson = localStorage.getItem('user');
        return userJson ? JSON.parse(userJson) : null;
    },
    
    // Verificar si el usuario tiene un rol específico
    hasRole(role) {
        const user = this.getCurrentUser();
        return user ? user.roles.includes(role) : false;
    }
};