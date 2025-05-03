/**
 * Este archivo complementa al dashboard.js con funcionalidades específicas
 * para la gestión de módulos y formularios
 */
document.addEventListener('DOMContentLoaded', function() {
    // Verificar si el usuario está autenticado
    requireAuth();
    
    // Solo continuar si es administrador
    if (!isAdmin()) return;
    
    // Verificar que estamos en la página correcta
    if (!document.getElementById('modulesSection') && !document.getElementById('formsSection')) return;
    
    // Agregar enlaces de menú para módulos y formularios si no existen
    addAdminMenuItems();
    
    // Cargar datos iniciales
    loadModulesData();
    loadFormsData();
    loadFormModulesData();
    
    // Agregar eventos para formularios
    setupFormHandlers();
    
    /**
     * Agrega elementos al menú de navegación para administrar módulos y formularios
     */
    function addAdminMenuItems() {
        const menu = document.querySelector('.menu');
        if (!menu) return;
        
        // Verificar si los elementos ya existen
        if (!document.querySelector('a[data-section="modules"]')) {
            const modulesItem = document.createElement('li');
            modulesItem.classList.add('admin-only');
            modulesItem.innerHTML = '<a href="#" data-section="modules">Módulos</a>';
            menu.appendChild(modulesItem);
            
            // Agregar sección de módulos al contenido si no existe
            if (!document.getElementById('modulesSection')) {
                const modulesSection = document.createElement('div');
                modulesSection.id = 'modulesSection';
                modulesSection.className = 'section-content hidden';
                modulesSection.innerHTML = `
                    <div class="table-container">
                        <div class="table-header">
                            <h3>Lista de Módulos</h3>
                            <button id="addModuleBtn" class="button primary">Agregar Módulo</button>
                        </div>
                        
                        <table class="data-table">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Código</th>
                                    <th>Activo</th>
                                    <th>Acciones</th>
                                </tr>
                            </thead>
                            <tbody id="modulesTableBody">
                                <!-- Se cargará dinámicamente -->
                            </tbody>
                        </table>
                    </div>
                `;
                document.querySelector('.content').appendChild(modulesSection);
            }
        }
        
        if (!document.querySelector('a[data-section="forms"]')) {
            const formsItem = document.createElement('li');
            formsItem.classList.add('admin-only');
            formsItem.innerHTML = '<a href="#" data-section="forms">Formularios</a>';
            menu.appendChild(formsItem);
            
            // Agregar sección de formularios al contenido si no existe
            if (!document.getElementById('formsSection')) {
                const formsSection = document.createElement('div');
                formsSection.id = 'formsSection';
                formsSection.className = 'section-content hidden';
                formsSection.innerHTML = `
                    <div class="table-container">
                        <div class="table-header">
                            <h3>Lista de Formularios</h3>
                            <button id="addFormBtn" class="button primary">Agregar Formulario</button>
                        </div>
                        
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
                            <tbody id="formsTableBody">
                                <!-- Se cargará dinámicamente -->
                            </tbody>
                        </table>
                    </div>
                    
                    <div class="table-container">
                        <div class="table-header">
                            <h3>Asignación de Formularios a Módulos</h3>
                            <button id="addFormModuleBtn" class="button primary">Asignar Formulario</button>
                        </div>
                        
                        <table class="data-table">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Módulo</th>
                                    <th>Formulario</th>
                                    <th>Acciones</th>
                                </tr>
                            </thead>
                            <tbody id="formModulesTableBody">
                                <!-- Se cargará dinámicamente -->
                            </tbody>
                        </table>
                    </div>
                `;
                document.querySelector('.content').appendChild(formsSection);
            }
        }
        
        // Actualizar los eventos para cambiar entre secciones
        document.querySelectorAll('.menu a[data-section]').forEach(link => {
            link.addEventListener('click', function(e) {
                e.preventDefault();
                
                // Obtener la sección a mostrar
                const sectionToShow = this.getAttribute('data-section');
                
                // Actualizar clases activas en menú
                document.querySelectorAll('.menu a').forEach(l => l.classList.remove('active'));
                this.classList.add('active');
                
                // Actualizar título de la sección
                document.getElementById('sectionTitle').textContent = this.textContent;
                
                // Ocultar todas las secciones
                document.querySelectorAll('.section-content').forEach(section => section.classList.add('hidden'));
                
                // Mostrar la sección seleccionada
                document.getElementById(sectionToShow + 'Section').classList.remove('hidden');
            });
        });
    }
    
    /**
     * Carga la lista de módulos
     */
    async function loadModulesData() {
        try {
            const modules = await apiRequest(API_CONFIG.ENDPOINTS.MODULE);
            
            if (!modules || modules.length === 0) {
                document.getElementById('modulesTableBody').innerHTML = '<tr><td colspan="4">No hay módulos registrados.</td></tr>';
                return;
            }
            
            const modulesTableBody = document.getElementById('modulesTableBody');
            modulesTableBody.innerHTML = '';
            
            modules.forEach(module => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${module.id}</td>
                    <td>${module.code}</td>
                    <td>${module.active ? 'Sí' : 'No'}</td>
                    <td>
                        <div class="action-buttons">
                            <button class="button primary btn-edit-module" data-id="${module.id}">Editar</button>
                            <button class="button danger btn-delete-module" data-id="${module.id}">Eliminar</button>
                            <button class="button danger btn-permanent-delete-module" data-id="${module.id}">Eliminar Permanente</button>
                        </div>
                    </td>
                `;
                modulesTableBody.appendChild(row);
            });
            
            // Agregar eventos a los botones
            document.querySelectorAll('.btn-edit-module').forEach(btn => {
                btn.addEventListener('click', handleEditModule);
            });
            
            document.querySelectorAll('.btn-delete-module').forEach(btn => {
                btn.addEventListener('click', handleDeleteModule);
            });
            
            document.querySelectorAll('.btn-permanent-delete-module').forEach(btn => {
                btn.addEventListener('click', handlePermanentDeleteModule);
            });
            
        } catch (error) {
            console.error('Error al cargar módulos:', error);
            document.getElementById('modulesTableBody').innerHTML = '<tr><td colspan="4">Error al cargar módulos.</td></tr>';
        }
    }
    
    /**
     * Carga la lista de formularios
     */
    async function loadFormsData() {
        try {
            const forms = await apiRequest(API_CONFIG.ENDPOINTS.FORM);
            
            if (!forms || forms.length === 0) {
                document.getElementById('formsTableBody').innerHTML = '<tr><td colspan="5">No hay formularios registrados.</td></tr>';
                return;
            }
            
            const formsTableBody = document.getElementById('formsTableBody');
            formsTableBody.innerHTML = '';
            
            forms.forEach(form => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${form.id}</td>
                    <td>${form.name}</td>
                    <td>${form.code}</td>
                    <td>${form.active ? 'Sí' : 'No'}</td>
                    <td>
                        <div class="action-buttons">
                            <button class="button primary btn-edit-form" data-id="${form.id}">Editar</button>
                            <button class="button danger btn-delete-form" data-id="${form.id}">Eliminar</button>
                            <button class="button danger btn-permanent-delete-form" data-id="${form.id}">Eliminar Permanente</button>
                        </div>
                    </td>
                `;
                formsTableBody.appendChild(row);
            });
            
            // Agregar eventos a los botones
            document.querySelectorAll('.btn-edit-form').forEach(btn => {
                btn.addEventListener('click', handleEditForm);
            });
            
            document.querySelectorAll('.btn-delete-form').forEach(btn => {
                btn.addEventListener('click', handleDeleteForm);
            });
            
            document.querySelectorAll('.btn-permanent-delete-form').forEach(btn => {
                btn.addEventListener('click', handlePermanentDeleteForm);
            });
            
        } catch (error) {
            console.error('Error al cargar formularios:', error);
            document.getElementById('formsTableBody').innerHTML = '<tr><td colspan="5">Error al cargar formularios.</td></tr>';
        }
    }
    
    /**
     * Carga las asignaciones de formularios a módulos
     */
    async function loadFormModulesData() {
        try {
            const formModules = await apiRequest(API_CONFIG.ENDPOINTS.FORM_MODULE);
            
            if (!formModules || formModules.length === 0) {
                document.getElementById('formModulesTableBody').innerHTML = '<tr><td colspan="4">No hay asignaciones registradas.</td></tr>';
                return;
            }
            
            // Obtener todos los módulos para mostrar su código
            const modules = await apiRequest(API_CONFIG.ENDPOINTS.MODULE);
            const moduleMap = new Map();
            modules.forEach(module => moduleMap.set(module.id, module));
            
            // Obtener todos los formularios para mostrar su nombre
            const forms = await apiRequest(API_CONFIG.ENDPOINTS.FORM);
            const formMap = new Map();
            forms.forEach(form => formMap.set(form.id, form));
            
            const formModulesTableBody = document.getElementById('formModulesTableBody');
            formModulesTableBody.innerHTML = '';
            
            formModules.forEach(formModule => {
                const module = moduleMap.get(formModule.moduleId);
                const form = formMap.get(formModule.formId);
                
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${formModule.id}</td>
                    <td>${module ? module.code : 'Desconocido'}</td>
                    <td>${form ? form.name : 'Desconocido'}</td>
                    <td>
                        <div class="action-buttons">
                            <button class="button danger btn-delete-form-module" data-id="${formModule.id}">Eliminar</button>
                            <button class="button danger btn-permanent-delete-form-module" data-id="${formModule.id}">Eliminar Permanente</button>
                        </div>
                    </td>
                `;
                formModulesTableBody.appendChild(row);
            });
            
            // Agregar eventos a los botones
            document.querySelectorAll('.btn-delete-form-module').forEach(btn => {
                btn.addEventListener('click', handleDeleteFormModule);
            });
            
            document.querySelectorAll('.btn-permanent-delete-form-module').forEach(btn => {
                btn.addEventListener('click', handlePermanentDeleteFormModule);
            });
            
        } catch (error) {
            console.error('Error al cargar asignaciones de formularios a módulos:', error);
            document.getElementById('formModulesTableBody').innerHTML = '<tr><td colspan="4">Error al cargar asignaciones.</td></tr>';
        }
    }
    
    /**
     * Configura los manejadores de eventos para los formularios
     */
    function setupFormHandlers() {
        // Botón para agregar módulo
        const addModuleBtn = document.getElementById('addModuleBtn');
        if (addModuleBtn) {
            addModuleBtn.addEventListener('click', showAddModuleModal);
        }
        
        // Botón para agregar formulario
        const addFormBtn = document.getElementById('addFormBtn');
        if (addFormBtn) {
            addFormBtn.addEventListener('click', showAddFormModal);
        }
        
        // Botón para asignar formulario a módulo
        const addFormModuleBtn = document.getElementById('addFormModuleBtn');
        if (addFormModuleBtn) {
            addFormModuleBtn.addEventListener('click', showAddFormModuleModal);
        }
    }
    
    /**
     * Muestra un modal para agregar un nuevo módulo
     */
    function showAddModuleModal() {
        // Crear modal
        const modal = document.createElement('div');
        modal.className = 'modal';
        modal.id = 'addModuleModal';
        
        modal.innerHTML = `
            <div class="modal-content">
                <span class="close-modal">&times;</span>
                <h3>Agregar Nuevo Módulo</h3>
                
                <form id="addModuleForm">
                    <div class="form-group">
                        <label for="moduleCode">Código:</label>
                        <input type="text" id="moduleCode" name="code" required>
                    </div>
                    
                    <div class="form-group">
                        <label>Estado:</label>
                        <div class="checkbox-group">
                            <input type="checkbox" id="moduleActive" name="active" checked>
                            <label for="moduleActive">Activo</label>
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <button type="submit" class="button primary">Guardar Módulo</button>
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
        modal.querySelector('#addModuleForm').addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const code = document.getElementById('moduleCode').value;
            const active = document.getElementById('moduleActive').checked;
            
            try {
                const moduleData = {
                    Code: code,
                    Active: active
                };
                
                const response = await apiRequest(API_CONFIG.ENDPOINTS.MODULE, 'POST', moduleData);
                
                if (response) {
                    // Cerrar modal y recargar datos
                    modal.remove();
                    loadModulesData();
                    alert('Módulo creado correctamente.');
                } else {
                    alert('Error al crear el módulo.');
                }
            } catch (error) {
                console.error('Error al crear módulo:', error);
                alert(`Error al crear módulo: ${error.message || 'Error desconocido'}`);
            }
        });
    }
    
    /**
     * Muestra un modal para agregar un nuevo formulario
     */
    function showAddFormModal() {
        // Crear modal
        const modal = document.createElement('div');
        modal.className = 'modal';
        modal.id = 'addFormModal';
        
        modal.innerHTML = `
            <div class="modal-content">
                <span class="close-modal">&times;</span>
                <h3>Agregar Nuevo Formulario</h3>
                
                <form id="addFormForm">
                    <div class="form-group">
                        <label for="formName">Nombre:</label>
                        <input type="text" id="formName" name="name" required>
                    </div>
                    
                    <div class="form-group">
                        <label for="formCode">Código:</label>
                        <input type="text" id="formCode" name="code" required>
                    </div>
                    
                    <div class="form-group">
                        <label>Estado:</label>
                        <div class="checkbox-group">
                            <input type="checkbox" id="formActive" name="active" checked>
                            <label for="formActive">Activo</label>
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <button type="submit" class="button primary">Guardar Formulario</button>
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
        modal.querySelector('#addFormForm').addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const name = document.getElementById('formName').value;
            const code = document.getElementById('formCode').value;
            const active = document.getElementById('formActive').checked;
            
            try {
                const formData = {
                    Name: name,
                    Code: code,
                    Active: active
                };
                
                const response = await apiRequest(API_CONFIG.ENDPOINTS.FORM, 'POST', formData);
                
                if (response) {
                    // Cerrar modal y recargar datos
                    modal.remove();
                    loadFormsData();
                    alert('Formulario creado correctamente.');
                } else {
                    alert('Error al crear el formulario.');
                }
            } catch (error) {
                console.error('Error al crear formulario:', error);
                alert(`Error al crear formulario: ${error.message || 'Error desconocido'}`);
            }
        });
    }
    
    /**
     * Muestra un modal para asignar un formulario a un módulo
     */
    async function showAddFormModuleModal() {
        try {
            // Obtener módulos y formularios disponibles
            const modules = await apiRequest(API_CONFIG.ENDPOINTS.MODULE);
            const forms = await apiRequest(API_CONFIG.ENDPOINTS.FORM);
            
            if (!modules || modules.length === 0) {
                alert('No hay módulos disponibles para asignar formularios.');
                return;
            }
            
            if (!forms || forms.length === 0) {
                alert('No hay formularios disponibles para asignar.');
                return;
            }
            
            // Crear modal
            const modal = document.createElement('div');
            modal.className = 'modal';
            modal.id = 'addFormModuleModal';
            
            // Generar opciones para el select de módulos
            let moduleOptions = '';
            modules.forEach(module => {
                moduleOptions += `<option value="${module.id}">${module.code}</option>`;
            });
            
            // Generar opciones para el select de formularios
            let formOptions = '';
            forms.forEach(form => {
                formOptions += `<option value="${form.id}">${form.name}</option>`;
            });
            
            modal.innerHTML = `
                <div class="modal-content">
                    <span class="close-modal">&times;</span>
                    <h3>Asignar Formulario a Módulo</h3>
                    
                    <form id="addFormModuleForm">
                        <div class="form-group">
                            <label for="moduleId">Módulo:</label>
                            <select id="moduleId" name="moduleId" required>
                                <option value="">Seleccione un módulo</option>
                                ${moduleOptions}
                            </select>
                        </div>
                        
                        <div class="form-group">
                            <label for="formId">Formulario:</label>
                            <select id="formId" name="formId" required>
                                <option value="">Seleccione un formulario</option>
                                ${formOptions}
                            </select>
                        </div>
                        
                        <div class="form-group">
                            <button type="submit" class="button primary">Guardar Asignación</button>
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
            modal.querySelector('#addFormModuleForm').addEventListener('submit', async (e) => {
                e.preventDefault();
                
                const moduleId = parseInt(document.getElementById('moduleId').value);
                const formId = parseInt(document.getElementById('formId').value);
                
                try {
                    const formModuleData = {
                        ModuleId: moduleId,
                        FormId: formId
                    };
                    
                    const response = await apiRequest(API_CONFIG.ENDPOINTS.FORM_MODULE, 'POST', formModuleData);
                    
                    if (response) {
                        // Cerrar modal y recargar datos
                        modal.remove();
                        loadFormModulesData();
                        alert('Asignación creada correctamente.');
                    } else {
                        alert('Error al crear la asignación.');
                    }
                } catch (error) {
                    console.error('Error al crear asignación:', error);
                    alert(`Error al crear asignación: ${error.message || 'Error desconocido'}`);
                }
            });
        } catch (error) {
            console.error('Error al preparar el modal de asignación:', error);
            alert('Error al cargar los datos necesarios para la asignación.');
        }
    }
    
    /**
     * Maneja la edición de un módulo
     */
    async function handleEditModule(event) {
        const moduleId = event.target.getAttribute('data-id');
        
        try {
            // Obtener detalles del módulo
            const module = await apiRequest(`${API_CONFIG.ENDPOINTS.MODULE}/${moduleId}`);
            
            if (!module) {
                alert('No se pudo obtener la información del módulo.');
                return;
            }
            
            // Crear modal
            const modal = document.createElement('div');
            modal.className = 'modal';
            modal.id = 'editModuleModal';
            
            modal.innerHTML = `
                <div class="modal-content">
                    <span class="close-modal">&times;</span>
                    <h3>Editar Módulo</h3>
                    
                    <form id="editModuleForm">
                        <input type="hidden" id="editModuleId" value="${module.id}">
                        
                        <div class="form-group">
                            <label for="editModuleCode">Código:</label>
                            <input type="text" id="editModuleCode" name="code" value="${module.code}" required>
                        </div>
                        
                        <div class="form-group">
                            <label>Estado:</label>
                            <div class="checkbox-group">
                                <input type="checkbox" id="editModuleActive" name="active" ${module.active ? 'checked' : ''}>
                                <label for="editModuleActive">Activo</label>
                            </div>
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
            modal.querySelector('#editModuleForm').addEventListener('submit', async (e) => {
                e.preventDefault();
                
                const id = parseInt(document.getElementById('editModuleId').value);
                const code = document.getElementById('editModuleCode').value;
                const active = document.getElementById('editModuleActive').checked;
                
                try {
                    const moduleData = {
                        Id: id,
                        Code: code,
                        Active: active
                    };
                    
                    const response = await apiRequest(`${API_CONFIG.ENDPOINTS.MODULE}/${id}`, 'PUT', moduleData);
                    
                    if (response !== null) {
                        // Cerrar modal y recargar datos
                        modal.remove();
                        loadModulesData();
                        alert('Módulo actualizado correctamente.');
                    } else {
                        alert('Error al actualizar el módulo.');
                    }
                } catch (error) {
                    console.error('Error al actualizar módulo:', error);
                    alert(`Error al actualizar módulo: ${error.message || 'Error desconocido'}`);
                }
            });
        } catch (error) {
            console.error('Error al editar módulo:', error);
            alert('Error al obtener los datos del módulo.');
        }
    }
    
    /**
     * Maneja la eliminación de un módulo
     */
    async function handleDeleteModule(event) {
        const moduleId = event.target.getAttribute('data-id');
        
        if (confirm('¿Está seguro de que desea eliminar este módulo? Esta acción se puede revertir.')) {
            try {
                // Intentar eliminar el módulo
                const deleted = await apiRequest(`${API_CONFIG.ENDPOINTS.MODULE}/${moduleId}`, 'DELETE');
                
                if (deleted) {
                    // Recargar la lista de módulos
                    loadModulesData();
                    alert('Módulo eliminado correctamente.');
                } else {
                    alert('No se pudo eliminar el módulo.');
                }
            } catch (error) {
                console.error('Error al eliminar módulo:', error);
                alert(`Error al eliminar módulo: ${error.message || 'Error desconocido'}`);
            }
        }
    }
    
    /**
     * Maneja la eliminación permanente de un módulo
     */
    async function handlePermanentDeleteModule(event) {
        const moduleId = event.target.getAttribute('data-id');
        
        if (confirm('¿Está seguro de que desea eliminar permanentemente este módulo? Esta acción NO se puede deshacer.')) {
            try {
                // Intentar eliminar permanentemente el módulo
                const deleted = await apiRequest(`${API_CONFIG.ENDPOINTS.MODULE}/permanent/${moduleId}`, 'DELETE');
                
                if (deleted) {
                    // Recargar la lista de módulos
                    loadModulesData();
                    alert('Módulo eliminado permanentemente.');
                } else {
                    alert('No se pudo eliminar permanentemente el módulo.');
                }
            } catch (error) {
                console.error('Error al eliminar permanentemente módulo:', error);
                alert(`Error al eliminar permanentemente módulo: ${error.message || 'Error desconocido'}`);
            }
        }
    }
    
    /**
     * Maneja la edición de un formulario
     */
    async function handleEditForm(event) {
        const formId = event.target.getAttribute('data-id');
        
        try {
            // Obtener detalles del formulario
            const form = await apiRequest(`${API_CONFIG.ENDPOINTS.FORM}/${formId}`);
            
            if (!form) {
                alert('No se pudo obtener la información del formulario.');
                return;
            }
            
            // Crear modal
            const modal = document.createElement('div');
            modal.className = 'modal';
            modal.id = 'editFormModal';
            
            modal.innerHTML = `
                <div class="modal-content">
                    <span class="close-modal">&times;</span>
                    <h3>Editar Formulario</h3>
                    
                    <form id="editFormForm">
                        <input type="hidden" id="editFormId" value="${form.id}">
                        
                        <div class="form-group">
                            <label for="editFormName">Nombre:</label>
                            <input type="text" id="editFormName" name="name" value="${form.name}" required>
                        </div>
                        
                        <div class="form-group">
                            <label for="editFormCode">Código:</label>
                            <input type="text" id="editFormCode" name="code" value="${form.code}" required>
                        </div>
                        
                        <div class="form-group">
                            <label>Estado:</label>
                            <div class="checkbox-group">
                                <input type="checkbox" id="editFormActive" name="active" ${form.active ? 'checked' : ''}>
                                <label for="editFormActive">Activo</label>
                            </div>
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
            modal.querySelector('#editFormForm').addEventListener('submit', async (e) => {
                e.preventDefault();
                
                const id = parseInt(document.getElementById('editFormId').value);
                const name = document.getElementById('editFormName').value;
                const code = document.getElementById('editFormCode').value;
                const active = document.getElementById('editFormActive').checked;
                
                try {
                    const formData = {
                        Id: id,
                        Name: name,
                        Code: code,
                        Active: active
                    };
                    
                    const response = await apiRequest(`${API_CONFIG.ENDPOINTS.FORM}`, 'PUT', formData);
                    
                    if (response !== null) {
                        // Cerrar modal y recargar datos
                        modal.remove();
                        loadFormsData();
                        alert('Formulario actualizado correctamente.');
                    } else {
                        alert('Error al actualizar el formulario.');
                    }
                } catch (error) {
                    console.error('Error al actualizar formulario:', error);
                    alert(`Error al actualizar formulario: ${error.message || 'Error desconocido'}`);
                }
            });
        } catch (error) {
            console.error('Error al editar formulario:', error);
            alert('Error al obtener los datos del formulario.');
        }
    }
    
    /**
     * Maneja la eliminación de un formulario
     */
    async function handleDeleteForm(event) {
        const formId = event.target.getAttribute('data-id');
        
        if (confirm('¿Está seguro de que desea eliminar este formulario? Esta acción se puede revertir.')) {
            try {
                // Intentar eliminar el formulario
                const deleted = await apiRequest(`${API_CONFIG.ENDPOINTS.FORM}/${formId}`, 'DELETE');
                
                if (deleted) {
                    // Recargar la lista de formularios
                    loadFormsData();
                    alert('Formulario eliminado correctamente.');
                } else {
                    alert('No se pudo eliminar el formulario.');
                }
            } catch (error) {
                console.error('Error al eliminar formulario:', error);
                alert(`Error al eliminar formulario: ${error.message || 'Error desconocido'}`);
            }
        }
    }
    
    /**
     * Maneja la eliminación permanente de un formulario
     */
    async function handlePermanentDeleteForm(event) {
        const formId = event.target.getAttribute('data-id');
        
        if (confirm('¿Está seguro de que desea eliminar permanentemente este formulario? Esta acción NO se puede deshacer.')) {
            try {
                // Intentar eliminar permanentemente el formulario
                const deleted = await apiRequest(`${API_CONFIG.ENDPOINTS.FORM}/permanent/${formId}`, 'DELETE');
                
                if (deleted) {
                    // Recargar la lista de formularios
                    loadFormsData();
                    alert('Formulario eliminado permanentemente.');
                } else {
                    alert('No se pudo eliminar permanentemente el formulario.');
                }
            } catch (error) {
                console.error('Error al eliminar permanentemente formulario:', error);
                alert(`Error al eliminar permanentemente formulario: ${error.message || 'Error desconocido'}`);
            }
        }
    }
    
    /**
     * Maneja la eliminación de una asignación de formulario a módulo
     */
    async function handleDeleteFormModule(event) {
        const formModuleId = event.target.getAttribute('data-id');
        
        if (confirm('¿Está seguro de que desea eliminar esta asignación? Esta acción se puede revertir.')) {
            try {
                // Intentar eliminar la asignación
                const deleted = await apiRequest(`${API_CONFIG.ENDPOINTS.FORM_MODULE}/${formModuleId}`, 'DELETE');
                
                if (deleted) {
                    // Recargar la lista de asignaciones
                    loadFormModulesData();
                    alert('Asignación eliminada correctamente.');
                } else {
                    alert('No se pudo eliminar la asignación.');
                }
            } catch (error) {
                console.error('Error al eliminar asignación:', error);
                alert(`Error al eliminar asignación: ${error.message || 'Error desconocido'}`);
            }
        }
    }
    
    /**
     * Maneja la eliminación permanente de una asignación de formulario a módulo
     */
    async function handlePermanentDeleteFormModule(event) {
        const formModuleId = event.target.getAttribute('data-id');
        
        if (confirm('¿Está seguro de que desea eliminar permanentemente esta asignación? Esta acción NO se puede deshacer.')) {
            try {
                // Intentar eliminar permanentemente la asignación
                const deleted = await apiRequest(`${API_CONFIG.ENDPOINTS.FORM_MODULE}/permanent/${formModuleId}`, 'DELETE');
                
                if (deleted) {
                    // Recargar la lista de asignaciones
                    loadFormModulesData();
                    alert('Asignación eliminada permanentemente.');
                } else {
                    alert('No se pudo eliminar permanentemente la asignación.');
                }
            } catch (error) {
                console.error('Error al eliminar permanentemente asignación:', error);
                alert(`Error al eliminar permanentemente asignación: ${error.message || 'Error desconocido'}`);
            }
        }
    }
});