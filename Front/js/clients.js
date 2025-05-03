/**
 * Gestión de Clientes
 */
import api, { isAdmin } from './api.js';

// Variables globales
let clients = [];
let currentPage = 1;
let itemsPerPage = 10;
let filteredClients = [];
let clientToDelete = null;

// Elementos del DOM
const clientsTable = document.getElementById('clients-table');
const paginationContainer = document.getElementById('pagination');
const totalClientsSpan = document.getElementById('total-clients');
const alertContainer = document.getElementById('alert-container');
const searchInput = document.getElementById('search-client');
const filterTypeSelect = document.getElementById('filter-client-type');
const applyFiltersBtn = document.getElementById('apply-filters-btn');

// Elementos del modal de cliente
const clientModal = document.getElementById('client-modal');
const clientForm = document.getElementById('client-form');
const modalTitle = document.getElementById('modal-title');
const clientIdInput = document.getElementById('client-id');
const firstNameInput = document.getElementById('first-name');
const lastNameInput = document.getElementById('last-name');
const identityDocumentInput = document.getElementById('identity-document');
const clientTypeSelect = document.getElementById('client-type');
const phoneInput = document.getElementById('phone');
const emailInput = document.getElementById('email');
const addressInput = document.getElementById('address');
const stratificationSelect = document.getElementById('socioeconomic-stratification');
const saveClientBtn = document.getElementById('save-client-btn');
const cancelBtn = document.getElementById('cancel-btn');
const closeModalBtn = document.getElementById('close-modal');
const newClientBtn = document.getElementById('new-client-btn');

// Elementos del modal de eliminación
const deleteModal = document.getElementById('delete-modal');
const deleteTypeSelect = document.getElementById('delete-type');
const confirmDeleteBtn = document.getElementById('confirm-delete-btn');
const cancelDeleteBtn = document.getElementById('cancel-delete-btn');
const closeDeleteModalBtn = document.getElementById('close-delete-modal');

// Validaciones de formulario
function validateName(name) {
    // Permite letras (incluyendo tildes), espacios y guiones
    const nameRegex = /^[A-Za-zÁáÉéÍíÓóÚúÑñ\s-]+$/;
    return nameRegex.test(name);
}

function validateEmail(email) {
    // Regex para validación de email
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function validatePhone(phone) {
    // Valida que sea un número de teléfono válido (solo dígitos)
    const phoneRegex = /^\d{7,15}$/;
    return phoneRegex.test(phone);
}

function validateIdentityDocument(document) {
    // Valida que sea un documento de identidad con solo números y longitud razonable
    const documentRegex = /^\d{6,12}$/;
    return documentRegex.test(document);
}

function validateClientForm() {
    let isValid = true;
    const errorMessages = [];

    // Validar nombre
    if (!firstNameInput.value.trim()) {
        errorMessages.push('El nombre es obligatorio');
        isValid = false;
    } else if (!validateName(firstNameInput.value)) {
        errorMessages.push('El nombre solo puede contener letras, espacios y guiones');
        isValid = false;
    }

    // Validar apellido
    if (!lastNameInput.value.trim()) {
        errorMessages.push('El apellido es obligatorio');
        isValid = false;
    } else if (!validateName(lastNameInput.value)) {
        errorMessages.push('El apellido solo puede contener letras, espacios y guiones');
        isValid = false;
    }

    // Validar documento de identidad
    if (!identityDocumentInput.value.trim()) {
        errorMessages.push('El documento de identidad es obligatorio');
        isValid = false;
    } else if (!validateIdentityDocument(identityDocumentInput.value)) {
        errorMessages.push('El documento de identidad debe ser un número válido (6-12 dígitos)');
        isValid = false;
    }

    // Validar email
    if (!emailInput.value.trim()) {
        errorMessages.push('El correo electrónico es obligatorio');
        isValid = false;
    } else if (!validateEmail(emailInput.value)) {
        errorMessages.push('Ingrese un correo electrónico válido');
        isValid = false;
    }

    // Validar teléfono
    if (!phoneInput.value.trim()) {
        errorMessages.push('El teléfono es obligatorio');
        isValid = false;
    } else if (!validatePhone(phoneInput.value)) {
        errorMessages.push('Ingrese un número de teléfono válido (7-15 dígitos)');
        isValid = false;
    }

    // Mostrar errores si los hay
    if (errorMessages.length > 0) {
        showAlert('error', errorMessages.join('<br>'));
    }

    return isValid;
}

// Inicializar la página
async function init() {
    // Cargar clientes
    await loadClients();
    
    // Configurar event listeners
    setupEventListeners();
}

// Cargar datos de clientes
async function loadClients() {
    try {
        // Mostrar spinner
        clientsTable.querySelector('tbody').innerHTML = `
            <tr>
                <td colspan="7" class="text-center">
                    <div class="spinner"></div>
                </td>
            </tr>
        `;
        
        // Obtener clientes desde la API
        clients = await api.getAll('client');
        
        // Aplicar filtros iniciales
        applyFilters();
        
        // Renderizar tabla
        renderClientsTable();
    } catch (error) {
        console.error('Error al cargar clientes', error);
        showAlert('error', 'Error al cargar la lista de clientes: ' + error.message);
    }
}

// Aplicar filtros a la lista de clientes
function applyFilters() {
    const searchTerm = searchInput.value.toLowerCase();
    const clientType = filterTypeSelect.value;
    
    filteredClients = clients.filter(client => {
        // Filtro por término de búsqueda
        const matchesSearch = searchTerm === '' || 
            client.firstName.toLowerCase().includes(searchTerm) || 
            client.lastName.toLowerCase().includes(searchTerm) || 
            client.email.toLowerCase().includes(searchTerm) || 
            client.identityDocument.toLowerCase().includes(searchTerm);
        
        // Filtro por tipo de cliente
        const matchesType = clientType === '' || client.clientType === clientType;
        
        return matchesSearch && matchesType;
    });
    
    // Actualizar contador
    totalClientsSpan.textContent = filteredClients.length;
    
    // Resetear a la primera página
    currentPage = 1;
    
    // Renderizar tabla y paginación
    renderClientsTable();
    renderPagination();
}

// Renderizar tabla de clientes
function renderClientsTable() {
    // Calcular el rango de clientes a mostrar según la paginación
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const displayedClients = filteredClients.slice(startIndex, endIndex);
    
    // Generar filas de la tabla
    let tableHTML = '';
    
    if (displayedClients.length === 0) {
        tableHTML = `
            <tr>
                <td colspan="7" class="text-center">No se encontraron clientes</td>
            </tr>
        `;
    } else {
        displayedClients.forEach(client => {
            tableHTML += `
                <tr>
                    <td>${client.clientId}</td>
                    <td>${client.firstName} ${client.lastName}</td>
                    <td>${client.identityDocument}</td>
                    <td>${client.clientType}</td>
                    <td>${client.email}</td>
                    <td>${client.phone}</td>
                    <td class="action-column">
                        <button class="btn btn-outline btn-sm edit-client" data-id="${client.clientId}">
                            Editar
                        </button>
                        <button class="btn btn-danger btn-sm delete-client" data-id="${client.clientId}">
                            Eliminar
                        </button>
                    </td>
                </tr>
            `;
        });
    }
    
    // Actualizar el tbody de la tabla
    clientsTable.querySelector('tbody').innerHTML = tableHTML;
    
    // Configurar event listeners para los botones de acción
    document.querySelectorAll('.edit-client').forEach(button => {
        button.addEventListener('click', () => openEditModal(button.dataset.id));
    });
    
    document.querySelectorAll('.delete-client').forEach(button => {
        button.addEventListener('click', () => openDeleteModal(button.dataset.id));
    });
}

// Renderizar paginación
function renderPagination() {
    const totalPages = Math.ceil(filteredClients.length / itemsPerPage);
    
    if (totalPages <= 1) {
        paginationContainer.innerHTML = '';
        return;
    }
    
    let paginationHTML = '';
    
    // Botón anterior
    paginationHTML += `
        <li class="pagination-item">
            <a href="#" class="pagination-link ${currentPage === 1 ? 'disabled' : ''}" data-page="prev">
                &laquo;
            </a>
        </li>
    `;
    
    // Números de página
    for (let i = 1; i <= totalPages; i++) {
        paginationHTML += `
            <li class="pagination-item">
                <a href="#" class="pagination-link ${currentPage === i ? 'active' : ''}" data-page="${i}">
                    ${i}
                </a>
            </li>
        `;
    }
    
    // Botón siguiente
    paginationHTML += `
        <li class="pagination-item">
            <a href="#" class="pagination-link ${currentPage === totalPages ? 'disabled' : ''}" data-page="next">
                &raquo;
            </a>
        </li>
    `;
    
    paginationContainer.innerHTML = paginationHTML;
    
    // Configurar event listeners para la paginación
    document.querySelectorAll('.pagination-link').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            
            const page = link.dataset.page;
            
            if (page === 'prev' && currentPage > 1) {
                currentPage--;
            } else if (page === 'next' && currentPage < totalPages) {
                currentPage++;
            } else if (page !== 'prev' && page !== 'next') {
                currentPage = parseInt(page);
            }
            
            renderClientsTable();
            renderPagination();
        });
    });
}

// Abrir modal para crear un nuevo cliente
function openCreateModal() {
    // Limpiar formulario
    clientForm.reset();
    clientIdInput.value = '';
    
    // Configurar modal
    modalTitle.textContent = 'Nuevo Cliente';
    
    // Mostrar modal
    clientModal.classList.add('show');
}

// Abrir modal para editar un cliente existente
async function openEditModal(clientId) {
    try {
        // Obtener cliente por ID
        const client = await api.getById('client', clientId);
        
        // Llenar formulario
        clientIdInput.value = client.clientId;
        firstNameInput.value = client.firstName;
        lastNameInput.value = client.lastName;
        identityDocumentInput.value = client.identityDocument;
        clientTypeSelect.value = client.clientType;
        phoneInput.value = client.phone;
        emailInput.value = client.email;
        addressInput.value = client.address || '';
        stratificationSelect.value = client.socioeconomicStratification || 1;
        
        // Configurar modal
        modalTitle.textContent = 'Editar Cliente';
        
        // Mostrar modal
        clientModal.classList.add('show');
    } catch (error) {
        console.error('Error al cargar cliente para editar', error);
        showAlert('error', 'Error al cargar los datos del cliente: ' + error.message);
    }
}

// Abrir modal de confirmación para eliminar cliente
function openDeleteModal(clientId) {
    // Guardar referencia al cliente a eliminar
    clientToDelete = clientId;
    
    // Mostrar modal
    deleteModal.classList.add('show');
}

// Guardar cliente (crear o actualizar)
async function saveClient() {
    // Validar formulario primero
    if (!validateClientForm()) {
        return;
    }

    try {
        // Obtener datos del formulario
        const clientData = {
            clientId: clientIdInput.value ? parseInt(clientIdInput.value) : 0,
            firstName: firstNameInput.value.trim(),
            lastName: lastNameInput.value.trim(),
            identityDocument: identityDocumentInput.value.trim(),
            clientType: clientTypeSelect.value,
            phone: parseInt(phoneInput.value),
            email: emailInput.value.trim(),
            address: addressInput.value.trim(),
            socioeconomicStratification: parseInt(stratificationSelect.value)
        };
        
        // Crear o actualizar según corresponda
        if (clientData.clientId === 0) {
            // Crear nuevo cliente
            await api.create('client', clientData);
            showAlert('success', 'Cliente creado exitosamente');
        } else {
            // Actualizar cliente existente
            await api.update('client', clientData);
            showAlert('success', 'Cliente actualizado exitosamente');
        }
        
        // Cerrar modal
        clientModal.classList.remove('show');
        
        // Recargar clientes
        await loadClients();
    } catch (error) {
        console.error('Error al guardar cliente', error);
        showAlert('error', 'Error al guardar cliente: ' + error.message);
    }
}

// Eliminar cliente
async function deleteClient() {
    if (!clientToDelete) return;
    
    try {
        const deleteType = deleteTypeSelect.value;
        
        if (deleteType === 'permanent' && isAdmin()) {
            // Eliminación permanente (solo admin)
            await api.permanentDelete('client', clientToDelete);
            showAlert('success', 'Cliente eliminado permanentemente');
        } else {
            // Eliminación lógica
            await api.delete('client', clientToDelete);
            showAlert('success', 'Cliente eliminado');
        }
        
        // Cerrar modal
        deleteModal.classList.remove('show');
        
        // Limpiar referencia
        clientToDelete = null;
        
        // Recargar clientes
        await loadClients();
    } catch (error) {
        console.error('Error al eliminar cliente', error);
        showAlert('error', 'Error al eliminar cliente: ' + error.message);
    }
}

// Mostrar alerta
function showAlert(type, message) {
    const alertClass = type === 'error' ? 'alert-danger' : 'alert-success';
    
    alertContainer.innerHTML = `
        <div class="alert ${alertClass}">
            ${message}
        </div>
    `;
    
    // Auto-ocultar después de 5 segundos
    setTimeout(() => {
        alertContainer.innerHTML = '';
    }, 5000);
}

// Configurar event listeners
function setupEventListeners() {
    // Botón para nuevo cliente
    newClientBtn.addEventListener('click', openCreateModal);
    
    // Botones del modal de cliente
    saveClientBtn.addEventListener('click', saveClient);
    cancelBtn.addEventListener('click', () => clientModal.classList.remove('show'));
    closeModalBtn.addEventListener('click', () => clientModal.classList.remove('show'));

    // Botones del modal de eliminación
    confirmDeleteBtn.addEventListener('click', deleteClient);
    cancelDeleteBtn.addEventListener('click', () => deleteModal.classList.remove('show'));
    closeDeleteModalBtn.addEventListener('click', () => deleteModal.classList.remove('show'));

    // Botones de filtro
    applyFiltersBtn.addEventListener('click', applyFilters);

    // Eventos de entrada para filtros rápidos
    searchInput.addEventListener('keyup', (e) => {
        if (e.key === 'Enter') {
            applyFilters();
        }
    });

    // Al enviar el formulario (para evitar recarga de página)
    clientForm.addEventListener('submit', (e) => {
        e.preventDefault();
        saveClient();
    });
}

// Iniciar la aplicación cuando se cargue el documento
document.addEventListener('DOMContentLoaded', init);