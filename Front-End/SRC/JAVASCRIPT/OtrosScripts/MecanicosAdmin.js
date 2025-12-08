  document.addEventListener("DOMContentLoaded", () => {

        // Selecciono todos los botones del menú interno
        const botones = document.querySelectorAll(".menu-btn");

        // Selecciono todas las secciones del módulo
        const secciones = document.querySelectorAll(".section");

        // Recorro cada botón del menú
        botones.forEach(boton => {
            boton.addEventListener("click", (e) => {
                e.preventDefault(); // Evita el salto brusco del HTML

                // Obtiene el id de la sección a la que debe ir (#agregar, #ver...)
                let target = boton.getAttribute("href");
                let seccion = document.querySelector(target);

                // Oculto todas las secciones
                secciones.forEach(s => s.classList.add("hidden"));

                // Muestro solo la sección seleccionada
                seccion.classList.remove("hidden");

                // Scroll suave hasta esa sección
                seccion.scrollIntoView({ behavior: "smooth" });
            });
        });
    });