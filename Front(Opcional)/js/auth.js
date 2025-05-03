/**
 * Manejo de autenticación y permisos
 */
import api, { isAuthenticated, isAdmin } from './api.js';

// Redirige a la página de login si el usuario no está autenticado
const requireAuth = () => {
  if (!isAuthenticated()) {
    window.location.href = '../html/login.html';
    return false;
  }
  return true;
};

// Redirige a la página principal si el usuario intenta acceder a login estando ya autenticado
const redirectIfAuthenticated = () => {
  if (isAuthenticated()) {
    window.location.href = '../html/index.html';
    return true;
  }
  return false;
};

// Verificar y aplicar permisos de administrador
const applyAdminPermissions = () => {
  const adminElements = document.querySelectorAll('.admin-only');
  
  if (isAdmin()) {
    document.body.classList.add('is-admin');
    adminElements.forEach(element => {
      element.classList.remove('hidden');
    });
  } else {
    document.body.classList.remove('is-admin');
    adminElements.forEach(element => {
      element.classList.add('hidden');
    });
  }
};

// Inicializar el sistema de autenticación
const initAuth = () => {
  // Configura el menú de navegación según el usuario
  updateNavigation();
  
  // Aplica permisos de administrador
  applyAdminPermissions();
  
  // Configura el botón de logout si existe
  const logoutButton = document.getElementById('logout-btn');
  if (logoutButton) {
    logoutButton.addEventListener('click', (e) => {
      e.preventDefault();
      api.logout();
      window.location.href = '../html/login.html';
    });
  }
};

// Actualizar la navegación según el usuario autenticado
const updateNavigation = () => {
  const navbar = document.querySelector('.navbar-menu');
  if (!navbar) return;
  
  // Si hay un usuario autenticado, mostrar el menú completo
  if (isAuthenticated()) {
    const adminLinks = document.querySelectorAll('.admin-nav-link');
    
    // Mostrar/ocultar links de administrador
    if (isAdmin()) {
      adminLinks.forEach(link => link.classList.remove('hidden'));
    } else {
      adminLinks.forEach(link => link.classList.add('hidden'));
    }
    
    // Actualizar el nombre de usuario si existe el elemento
    const userNameSpan = document.getElementById('user-name');
    if (userNameSpan) {
      const user = getCurrentUser();
      userNameSpan.textContent = user ? user.name : 'Usuario';
    }
    
    // Mostrar las opciones de sesión
    const authSection = document.querySelector('.auth-section');
    const noAuthSection = document.querySelector('.no-auth-section');
    
    if (authSection) authSection.classList.remove('hidden');
    if (noAuthSection) noAuthSection.classList.add('hidden');
  } else {
    // Si no hay usuario, mostrar solo los links públicos
    const authSection = document.querySelector('.auth-section');
    const noAuthSection = document.querySelector('.no-auth-section');
    
    if (authSection) authSection.classList.add('hidden');
    if (noAuthSection) noAuthSection.classList.remove('hidden');
  }
};

// Manejar el formulario de login
const handleLogin = async (e) => {
  e.preventDefault();
  
  const username = document.getElementById('username').value;
  const password = document.getElementById('password').value;
  const errorMessage = document.getElementById('login-error');
  
  // Ocultar mensaje de error anterior
  if (errorMessage) {
    errorMessage.classList.add('hidden');
  }
  
  try {
    // Mostrar spinner de carga
    const spinner = document.querySelector('.spinner');
    if (spinner) spinner.classList.remove('hidden');
    
    const response = await api.login({ username, password });
    
    // Redirigir a la página principal
    window.location.href = '../html/index.html';
  } catch (error) {
    // Mostrar mensaje de error
    if (errorMessage) {
      errorMessage.textContent = error.message || 'Error de autenticación';
      errorMessage.classList.remove('hidden');
    }
  } finally {
    // Ocultar spinner
    const spinner = document.querySelector('.spinner');
    if (spinner) spinner.classList.add('hidden');
  }
};

// Inicializar formulario de login
const initLoginForm = () => {
  const loginForm = document.getElementById('login-form');
  if (loginForm) {
    loginForm.addEventListener('submit', handleLogin);
  }
};

export {
  requireAuth,
  redirectIfAuthenticated,
  applyAdminPermissions,
  initAuth,
  updateNavigation,
  initLoginForm
};