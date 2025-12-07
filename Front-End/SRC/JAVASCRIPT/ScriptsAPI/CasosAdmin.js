document.addEventListener("DOMContentLoaded", async () => {

    const idMecanico = sessionStorage.getItem("idUsuario");
    const contenedor = document.getElementById("casos-container");
    const filtro = document.getElementById("filtroEstado");

    if (!idMecanico) {
        contenedor.innerHTML = "<p>No se encontró el ID del mecánico.</p>";
        return;
    }

    let listaCasos = [];

    // ============================================
    // 1. Llamar API – TU RUTA REAL DEL PROYECTO
    // ============================================
    async function cargarCasos() {
        try {
            const response = await fetch(`https://localhost:7292/api/Casos/Mecanico/${idMecanico}`);

            if (!response.ok) throw new Error("Error al obtener los casos");

            listaCasos = await response.json();
            mostrarCasos(listaCasos);

        } catch (error) {
            contenedor.innerHTML = `<p>Error: ${error.message}</p>`;
        }
    }

    // ============================================
    // 2. Mostrar Casos en Tarjetas
    // ============================================
    function mostrarCasos(casos) {
        contenedor.innerHTML = "";

        casos.forEach(caso => {
            const tarjeta = document.createElement("div");
            tarjeta.classList.add("caso-card");

            // Fechas
            const fechaInicio = new Date(caso.fechaInicio).toLocaleDateString();
            const fechaFin = caso.fechaFin ? new Date(caso.fechaFin).toLocaleDateString() : "—";

            // Estado visual
            const estadoInfo = obtenerEstado(caso.estadoCaso);

            // HTML tarjeta
            tarjeta.innerHTML = `
                <h3>Caso #${caso.idCaso}</h3>

                <p><strong>Cliente:</strong> ${caso.cliente || "—"}</p>
                <p><strong>Fecha Inicio:</strong> ${fechaInicio}</p>
                <p><strong>Fecha Fin:</strong> ${fechaFin}</p>
                <p><strong>Horas trabajadas:</strong> ${caso.horasTrabajadas}</p>
                <p><strong>Total:</strong> $${caso.totalCaso.toFixed(2)}</p>

                <div class="estado-linea">
                    <span class="estado-punto" style="background:${estadoInfo.color}"></span>
                    <span class="estado-texto" style="color:${estadoInfo.color}">${estadoInfo.texto}</span>
                </div>

                ${estadoInfo.botonHTML}
            `;

            contenedor.appendChild(tarjeta);
        });
    }

    // ============================================
    // 3. Estado → color, texto y botón
    // ============================================
    function obtenerEstado(estado) {
        switch (estado) {
            case "noEmpezado":
                return {
                    color: "#ff3b3b",
                    texto: "No Empezado",
                    botonHTML: `<button class="btn-accion comenzar">Comenzar</button>`
                };

            case "enProceso":
                return {
                    color: "#ffc107",
                    texto: "En Proceso",
                    botonHTML: `<button class="btn-accion continuar">Continuar</button>`
                };

            case "terminado":
                return {
                    color: "#28a745",
                    texto: "Terminado",
                    botonHTML: ""
                };

            default:
                return {
                    color: "#999",
                    texto: estado,
                    botonHTML: ""
                };
        }
    }

    // ============================================
    // 4. Filtrar por estado
    // ============================================
    filtro.addEventListener("change", () => {
        const value = filtro.value;

        if (value === "todos") {
            mostrarCasos(listaCasos);
        } else {
            const filtrados = listaCasos.filter(c => c.estadoCaso === value);
            mostrarCasos(filtrados);
        }
    });

    // Inicializar
    cargarCasos();
});
