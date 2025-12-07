document.addEventListener("DOMContentLoaded", async () => {
    // =========================
    // VARIABLES PARA ASIGNAR
    // =========================
    let casoSeleccionado = null;
    let mecanicoSeleccionado = null;

    const contCasos = document.getElementById("listaCasosSinMecanico");
    const contMecanicos = document.getElementById("listaMecanicos");
    const btnConfirmar = document.getElementById("btnConfirmarAsignacion");
    const msgAsignar = document.getElementById("msgAsignarCaso");

    // =========================
    // 1. CARGAR CASOS SIN MECÁNICO
    // =========================
    async function cargarCasosSinMecanico() {
        contCasos.innerHTML = "<p>Cargando...</p>";
        try {
            const r = await fetch("https://localhost:7292/api/Casos/NoAsignados");
            const data = await r.json();
            if (!r.ok) return contCasos.innerHTML = `<p>${data.mensaje}</p>`;

            contCasos.innerHTML = "";
            data.forEach(c => {
                contCasos.innerHTML += `
                <div class="item-select">
                    <span><strong>#${c.idCaso}</strong> | ${c.fechaInicio.split("T")[0]} | ${c.estadoCaso}</span>
                    <button class="btn-mini" onclick="seleccionarCaso(${c.idCaso})">Seleccionar</button>
                </div>`;
            });
        } catch (e) {
            contCasos.innerHTML = "<p>Error al cargar datos.</p>";
        }
    }

    // =========================
    // 2. LISTAR MECÁNICOS
    // =========================
    async function cargarMecanicos() {
        contMecanicos.innerHTML = "<p>Cargando...</p>";
        try {
            const r = await fetch("https://localhost:7292/api/Usuarios/Mecanicos");
            const data = await r.json();

            contMecanicos.innerHTML = "";
            data.mecanicos.forEach(m => {
                contMecanicos.innerHTML += `
                <div class="item-select">
                    <span>${m.nombreUsuario}</span>
                    <button class="btn-mini" onclick="seleccionarMecanico(${m.idUsuario})">Asignar</button>
                </div>`;
            });
        } catch (e) {
            contMecanicos.innerHTML = "<p>Error al cargar mecanicos.</p>";
        }
    }

    // =========================
    // SELECCIÓN Y CONFIRMACIÓN
    // =========================
    window.seleccionarCaso = function (id) {
        casoSeleccionado = id;
        validarAsignacion();
    };

    window.seleccionarMecanico = function (id) {
        mecanicoSeleccionado = id;
        validarAsignacion();
    };

    function validarAsignacion() {
        btnConfirmar.disabled = !(casoSeleccionado && mecanicoSeleccionado);
    }

    // =========================
    // 3. CONFIRMAR ASIGNACIÓN
    // =========================
    btnConfirmar.addEventListener("click", async () => {
        try {
            const r = await fetch("https://localhost:7292/api/Casos/AsignarCaso", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    idCaso: casoSeleccionado,
                    idMecanico: mecanicoSeleccionado
                })
            });
            const data = await r.json();
            msgAsignar.textContent = data.mensaje || data.error;

            casoSeleccionado = null;
            mecanicoSeleccionado = null;
            validarAsignacion();
            cargarCasosSinMecanico(); // recargar lista
        } catch (e) {
            msgAsignar.textContent = "Error al asignar."
        }
    });

    // CARGA INICIAL
    cargarCasosSinMecanico();
    cargarMecanicos();

});

// VARIABLES GLOBALES 
let listaCasosAdmin = [];

// CARGAR TODOS LOS CASOS 
document.getElementById("btnVerTodos").addEventListener("click", async () => {
    const contenedor = document.getElementById("cardsTodosCasos");
    contenedor.innerHTML = "<p>Cargando...</p>";

    try {
        const res = await fetch("https://localhost:7292/Api/Casos/AllCasos");
        listaCasosAdmin = await res.json(); // Guardar lista

        mostrarCasosAdmin(listaCasosAdmin);

    } catch (error) {
        contenedor.innerHTML = "<p>Error al cargar los casos.</p>";
        console.error(error);
    }
});

// FUNCIÓN PARA MOSTRAR TARJETAS
function mostrarCasosAdmin(data) {
    const contenedor = document.getElementById("cardsTodosCasos");
    contenedor.innerHTML = "";

    data.forEach(caso => {
        const card = document.createElement("div");
        card.className = "card-caso-admin";

        // Asignación visual del estado
        let estadoClase = "";
        let estadoTexto = "";

        if (caso.estadoCaso === "noEmpezado") {
            estadoClase = "estado-noempezado";
            estadoTexto = "No Empezado";
        }
        if (caso.estadoCaso === "enProceso") {
            estadoClase = "estado-proceso";
            estadoTexto = "En Proceso";
        }
        if (caso.estadoCaso === "terminado") {
            estadoClase = "estado-terminado";
            estadoTexto = "Terminado";
        }

        card.innerHTML = `
            <h3>Caso #${caso.idCaso}</h3>
            <p><strong>Inicio:</strong> ${caso.fechaInicio === "0001-01-01T00:00:00" ? "Pendiente" : caso.fechaInicio}</p>
            <p><strong>Fin:</strong> ${caso.fechaFin ?? "Pendiente"}</p>
            <p><strong>Horas:</strong> ${caso.horasTrabajadas}</p>
            <p><strong>SubTotal Aprox:</strong> $${caso.totalCaso || "0.00"}</p>
            <p><strong>Mecánico:</strong> ${caso.nombreMecanico ?? "Sin asignar"}</p>

            <div class="estado-linea-admin">
                <div class="estado-punto-admin ${estadoClase}"></div>
                <span><strong>${estadoTexto}</strong></span>
            </div>
        `;

        contenedor.appendChild(card);
    });
}

// FILTRAR POR ESTADO 
document.getElementById("filtroEstadoAdmin").addEventListener("change", () => {
    const filtro = document.getElementById("filtroEstadoAdmin").value;

    if (filtro === "todos") {
        mostrarCasosAdmin(listaCasosAdmin);
    } else {
        const filtrados = listaCasosAdmin.filter(caso => caso.estadoCaso === filtro);
        mostrarCasosAdmin(filtrados);
    }
});

//FACTURAS 
document.getElementById("btnVerFacturas").addEventListener("click", async () => {
    const tablaBody = document.querySelector("#tablaFacturas tbody");
    tablaBody.innerHTML = "<tr><td colspan='7'>Cargando...</td></tr>";

    try {
        const res = await fetch("https://localhost:7292/Api/Casos/ListarTodasLasFacturas");
        const json = await res.json();

        const data = json.data ?? []; // lista de facturas
        tablaBody.innerHTML = ""; // limpiar

        let sumaTotal = 0;

        data.forEach(f => {
            sumaTotal += f.totalCaso;

            const fila = document.createElement("tr");
            fila.innerHTML = `
                <td>${f.idCaso}</td>
                <td>$${f.totalCaso.toFixed(2)}</td>
                <td>${f.fechaInicio === "0001-01-01T00:00:00" ? "Pendiente" : f.fechaInicio}</td>
                <td>${f.fechaFin ?? "Pendiente"}</td>
                <td>${f.horasTrabajadas}</td>
                <td>${f.idUsuario}</td>
                <td>${f.nombreMecanico}</td>
            `;
            tablaBody.appendChild(fila);
        });

        // Mostrar total de facturación
        document.getElementById("totalFacturacionBox").innerText =
            `Facturación total: $${sumaTotal.toFixed(2)}`;

        // Si no hay elementos
        if (data.length === 0) {
            tablaBody.innerHTML = "<tr><td colspan='7'>No hay facturas disponibles</td></tr>";
        }

    } catch (err) {
        tablaBody.innerHTML = "<tr><td colspan='7'>Error al cargar facturas</td></tr>";
        console.error(err);
    }
});