//JAVASCRIPT: Funcionalidad del "Ojito" para ver/ocultar la contraseña
// Obtiene referencias al input de contraseña y al icono
const togglePassword = document.getElementById('togglePassword');
const passwordInput = document.getElementById('password');

// Agrega un listener de evento al icono
togglePassword.addEventListener('click', function () {
    // Verifica el tipo de input actual
    const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';

    // Cambia el tipo (Muestra u oculta los caracteres)
    passwordInput.setAttribute('type', type);

    // Cambia el icono visualmente (Cierra o abre el ojo)
    this.classList.toggle('fa-eye'); // Quita el ojo cerrado
    this.classList.toggle('fa-eye-slash'); // Pone el ojo abierto
});