// ================================
//    MECANICOS ADMIN - FRONTEND
// ================================

// URL del endpoint back-end
const API_URL = "https://localhost:7292/Api/Usuarios/AgregarMecanico";
const API_LISTAR_MECANICOS = "https://localhost:7292/Api/Usuarios/Mecanicos";
const API_LISTAR_CASOS_MECANICO = "https://localhost:7292/Api/Casos/Mecanico/Casos_"; // + idMecanico

// ===============================================
//          MENÚ DE SECCIONES
// ===============================================
document.addEventListener("DOMContentLoaded", () => {

    const botones = document.querySelectorAll(".menu-btn");
    const secciones = document.querySelectorAll(".section");

    botones.forEach(boton => {
        boton.addEventListener("click", (e) => {
            e.preventDefault();

            let target = boton.getAttribute("href");
            let seccion = document.querySelector(target);

            // Oculta todas
            secciones.forEach(s => s.classList.add("hidden"));

            // Muestra solo la seleccionada
            seccion.classList.remove("hidden");
            seccion.scrollIntoView({ behavior: "smooth" });

            // Cargar mecánicos cuando se abre "Ver"
            if (target === "#ver") {
                cargarTablaMecanicos();
            }

            // Cargar dropdown cuando se abre "Casos por Status"
            if (target === "#casos") {
                cargarDropdownMecanicos();
            }
        });
    });

    configurarRegistroMecanico();
    configurarBuscarCasos();
});

// ===============================================
//    REGISTRAR NUEVO MECÁNICO
// ===============================================
function configurarRegistroMecanico() {

    const form = document.querySelector("#agregar .formulario");

    form.addEventListener("submit", async (event) => {
        event.preventDefault();

        const data = {
            nombreUsuario: document.getElementById("Nombre_Mec").value,
            telefonoUsuario: document.getElementById("Telefono_Mec").value,
            correoUsuario: document.getElementById("Email_Mec").value,
            direccionUsuario: "Sin dirección agregada desde el front",
            userName: document.getElementById("Usuario_Mec").value,
            userPassword: document.getElementById("Contraseña_Mec").value
        };

        try {
            const response = await fetch(API_URL, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data)
            });

            const result = await response.json();

            if (response.ok) {
                alert("Mecánico agregado exitosamente");
                form.reset();
            } else {
                alert(result.error);
            }

        } catch (error) {
            console.error("Error al conectar con el servidor:", error);
            alert("No se pudo conectar con el servidor");
        }
    });
}

// ===============================================
//    LISTAR MECÁNICOS EN LA TABLA
// ===============================================
async function cargarTablaMecanicos() {
    const tablaBody = document.getElementById("tablaMecanicos");
    tablaBody.innerHTML = "<tr><td colspan='5'>Cargando...</td></tr>";

    try {
        const res = await fetch(API_LISTAR_MECANICOS);
        const data = await res.json();

        if (!res.ok) {
            tablaBody.innerHTML = "<tr><td colspan='5'>Error al cargar los mecánicos.</td></tr>";
            return;
        }

        const lista = data.mecanicos;

        if (!lista || lista.length === 0) {
            tablaBody.innerHTML = "<tr><td colspan='5'>No hay mecánicos registrados.</td></tr>";
            return;
        }

        tablaBody.innerHTML = "";
        lista.forEach(m => {
            tablaBody.innerHTML += `
                <tr>
                    <td>${m.idUsuario}</td>
                    <td>${m.nombreUsuario}</td>
                    <td>${m.telefonoUsuario}</td>
                    <td>${m.correoUsuario}</td>
                    <td>${m.userName}</td>
                </tr>
            `;
        });

    } catch (error) {
        console.error("Error al obtener mecánicos:", error);
        tablaBody.innerHTML = "<tr><td colspan='5'>Error de conexión.</td></tr>";
    }
}

// ===============================================
//    LLENAR DROPDOWN DE MECÁNICOS
// ===============================================
async function cargarDropdownMecanicos() {
    const select = document.getElementById("Id_Mecanico_Status");
    select.innerHTML = `<option>Cargando...</option>`;

    try {
        const res = await fetch(API_LISTAR_MECANICOS);
        const data = await res.json();

        const lista = data.mecanicos;
        select.innerHTML = "";

        lista.forEach(m => {
            select.innerHTML += `<option value="${m.idUsuario}">${m.nombreUsuario}</option>`;
        });

    } catch (error) {
        console.error("Error al cargar mecánicos en dropdown:", error);
        select.innerHTML = `<option>Error al cargar</option>`;
    }
}

// ===============================================
//    BUSCAR CASOS POR MECÁNICO Y STATUS
// ===============================================
function configurarBuscarCasos() {

    document.querySelector("#casos .btn").addEventListener("click", async () => {

        const idMec = document.getElementById("Id_Mecanico_Status").value;
        const estado = document.getElementById("Status_Caso").value;
        const tabla = document.getElementById("tablaCasos");

        tabla.innerHTML = `<tr><td colspan="3">Buscando...</td></tr>`;

        try {
            const res = await fetch(API_LISTAR_CASOS_MECANICO + idMec);
            const lista = await res.json();

            if (!res.ok) {
                tabla.innerHTML = `<tr><td colspan="3">${lista.error}</td></tr>`;
                return;
            }

            // Filtro si NO se seleccionó "Todos"
            let filtrados = lista;
            if (estado !== "Todos") {
                filtrados = lista.filter(c => c.estadoCaso === estado);
            }

            if (filtrados.length === 0) {
                tabla.innerHTML = `<tr><td colspan="3">El mecánico no tiene casos asignados</td></tr>`;
                return;
            }

            tabla.innerHTML = "";
            filtrados.forEach(c => {

                // Formato de fechas
                let inicio = (c.fechaInicio === "0001-01-01T00:00:00") ? "Pendiente" : c.fechaInicio.split("T")[0];
                let fin = (!c.fechaFin) ? "Pendiente" : c.fechaFin.split("T")[0];

                // Texto más bonito para estado
                let estadoTexto = "";
                if (c.estadoCaso === "noEmpezado") estadoTexto = "No Empezado";
                if (c.estadoCaso === "enProceso") estadoTexto = "En Proceso";
                if (c.estadoCaso === "terminado") estadoTexto = "Terminado";

                tabla.innerHTML += `
                <tr>
                    <td>${c.idCaso}</td>
                    <td>Inicio: ${inicio}<br>Fin: ${fin}<br>Horas: ${c.horasTrabajadas}<br>Total: $${c.totalCaso}</td>
                    <td class="${c.estadoCaso === 'noEmpezado' ? 'estado-noempezado' :
                        c.estadoCaso === 'enProceso' ? 'estado-proceso' :
                            'estado-terminado'}">${estadoTexto}</td>
                </tr>
            `;
            });


        } catch (error) {
            console.error("Error:", error);
            tabla.innerHTML = `<tr><td colspan="3">Error de conexión</td></tr>`;
        }
    });
}