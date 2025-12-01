//Display del Login
// Referencia al modal
const modal = document.getElementById("modalLogin");

// Función para abrir el modal de Login
function abrirLogin() {
    modal.style.display = "flex";
}

// Función para cerrar el modal
function cerrarLogin() {
    modal.style.display = "none";
}

// Cerrar modal al hacer clic fuera del contenido
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
};

//Resaltar el enlace activo en la navegación al hacer scroll
document.addEventListener("DOMContentLoaded", () => {

    // Referencias
    const header = document.querySelector('header');         // Header sticky
    const navLinks = document.querySelectorAll('nav a');     // Todos los enlaces del nav

    // Función que calcula la altura real del header (incluye padding, border)
    function alturaHeader() {
        return header ? Math.ceil(header.getBoundingClientRect().height) : 0;
    }

    // Scroll suave hacia un elemento (compensando el header)
    function scrollAElemento(id) {
        const target = document.querySelector(id);
        if (!target) return;

        const headerAlt = alturaHeader();
        // Coordenada Y absoluta del elemento
        const topAbsolute = target.getBoundingClientRect().top + window.scrollY;
        // Posición final que queremos (elemento al inicio, compensando header)
        const destino = topAbsolute - headerAlt;

        // Usamos scrollTo con comportamiento suave
        window.scrollTo({
            top: destino,
            behavior: 'smooth'
        });
    }

    // Interceptar clicks en los links del nav para usar nuestro scroll
    navLinks.forEach(link => {
        // Solo afectamos links internos que comienzan con '#'
        const href = link.getAttribute('href') || '';
        if (!href.startsWith('#')) return;

        link.addEventListener('click', (e) => {
            e.preventDefault();          // Evita el comportamiento por defecto del navegador
            const id = href;             // ej. "#inicio"
            scrollAElemento(id);

            // Opcional: cerrar menus móviles aquí si tienes uno
            // ejemplo: document.querySelector('.menu').classList.remove('open');
        });
    });

    // Ajuste extra: si la página carga con hash en la URL (ej. al recargar),
    // hacemos el scroll controlado una vez que todo está cargado.
    if (window.location.hash) {
        // Pequeño delay para asegurar que todo el layout esté listo
        setTimeout(() => {
            scrollAElemento(window.location.hash);
        }, 50);
    }

    // Si cambias tamaño de ventana (ej. rotación móvil), recalculamos posible desfase
    // y opcionalmente corregimos si la URL tiene hash.
    window.addEventListener('resize', () => {
        if (window.location.hash) {
            // Pequeño debounce básico
            clearTimeout(window._scrollResizeTimeout);
            window._scrollResizeTimeout = setTimeout(() => {
                scrollAElemento(window.location.hash);
            }, 120);
        }
    });

    // Detectar y resaltar el enlace activo según scroll
    const secciones = document.querySelectorAll("section[id], footer[id]");

    function actualizarLinkActivo() {
        const headerAlt = alturaHeader();

        let indexActivo = -1;

        secciones.forEach((sec, index) => {
            const top = sec.offsetTop - headerAlt - 50;
            const bottom = top + sec.offsetHeight;

            if (window.scrollY >= top && window.scrollY < bottom) {
                indexActivo = index;
            }
        });

        navLinks.forEach(link => link.classList.remove("active"));

        if (indexActivo !== -1) {
            const idActivo = "#" + secciones[indexActivo].id;
            const linkActivo = document.querySelector(`nav a[href="${idActivo}"]`);
            if (linkActivo) linkActivo.classList.add("active");
        }
    }

    window.addEventListener("scroll", actualizarLinkActivo);
});