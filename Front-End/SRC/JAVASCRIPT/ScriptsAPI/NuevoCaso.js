/* NuevoCaso.js
   Flujo:
   - Elegir tipo servicio (1|2) -> cargar servicios via GET Api/ServiciosMecanicos/ListarPorTipo/{tipo}
   - Seleccionar checkboxes -> generar string "1,2,3" (ids)
   - Buscar cliente -> GET Api/Clientes/Buscar?nombre=...&correo=...
   - Ver vehiculos -> GET Api/Vehiculos/ListarPorCliente/{idCliente}
   - Seleccionar vehiculo -> guardar idVehiculo
   - Crear orden -> POST Api/OrdenesServicio/NuevaOrden { TipoServicio, diagnosticoInicial (ids CSV), costoInicial, idVehiculo }
   - Al obtener orden -> POST Api/Casos/CrearCaso { idOrdenServicio, idUsuario }
*/

document.addEventListener("DOMContentLoaded", () => {
    const API_BASE = "https://localhost:7292/Api"; 
    const serviciosContainer = document.getElementById("serviciosContainer");
    const tipoRadios = document.getElementsByName("tipoServicio");
    const diagnosticoVisual = document.getElementById("diagnosticoVisual");
    const costoInicialInput = document.getElementById("costoInicial");
    const btnCrearOrden = document.getElementById("btnCrearOrden");
    const btnCancelar = document.getElementById("btnCancelar");
    const mensajeEstado = document.getElementById("mensajeEstado");

    // Cliente / Vehículos
    const inputNombre = document.getElementById("inputNombre");
    const inputCorreo = document.getElementById("inputCorreo");
    const btnBuscarCliente = document.getElementById("btnBuscarCliente");
    const clientesList = document.getElementById("clientesList");
    const vehiculosList = document.getElementById("vehiculosList");
    const vehiculoSelText = document.getElementById("vehiculoSelText");

    let serviciosCargados = [];     // array de {idServicio, servicio, precio}
    let seleccionadoVehiculoId = null;
    let clienteSeleccionadoId = null;

    // Obtener idUsuario desde sessionStorage
    const idUsuario = sessionStorage.getItem("idUsuario") || null;

    // --- utilidades ---
    function setMensaje(text, tipo = "") {
        mensajeEstado.textContent = text;
        mensajeEstado.className = "info-line " + (tipo === "error" ? "error" : tipo === "success" ? "success" : "");
    }

    function clearMensaje() { mensajeEstado.textContent = ""; mensajeEstado.className = "info-line"; }

    // --- cargar servicios por tipo ---
    async function cargarServicios(tipo) {
        serviciosContainer.innerHTML = "<div>Cargando servicios...</div>";
        serviciosCargados = [];

        try {
            const res = await fetch(`${API_BASE}/ServiciosMecanicos/ListarPorTipo/${tipo}`);
            if (!res.ok) {
                // si 404 o No Content -> mostrar mensaje
                const txt = await res.text();
                serviciosContainer.innerHTML = `<div class="error">No hay servicios: ${txt}</div>`;
                return;
            }
            const lista = await res.json();
            serviciosCargados = lista; // array de DTO ServiciosPorTipoDTO
            renderServicios(lista);
        } catch (err) {
            serviciosContainer.innerHTML = `<div class="error">Error cargando servicios: ${err.message}</div>`;
        }
    }

    function renderServicios(lista) {
        if (!lista || lista.length === 0) {
            serviciosContainer.innerHTML = `<div>No hay servicios disponibles.</div>`;
            return;
        }
        serviciosContainer.innerHTML = "";
        lista.forEach(s => {
            // cada checkbox contiene data-id, pero mostramos solo el nombre
            const div = document.createElement("div");
            div.className = "servicio-item";
            div.innerHTML = `
                <label style="display:flex; align-items:center; gap:8px; width:100%;">
                    <input type="checkbox" class="chk-servicio" data-id="${s.idServicio}" data-nombre="${s.servicio}" />
                    <span>${s.servicio}</span>
                </label>
            `;
            serviciosContainer.appendChild(div);
        });

        // agregar listeners a los checkboxes
        const chks = Array.from(document.querySelectorAll(".chk-servicio"));
        chks.forEach(c => c.addEventListener("change", actualizarDiagnosticoVisual));
        actualizarDiagnosticoVisual();
    }

    function actualizarDiagnosticoVisual() {
        const seleccionados = Array.from(document.querySelectorAll(".chk-servicio:checked"));
        const nombres = seleccionados.map(s => s.dataset.nombre);
        diagnosticoVisual.value = nombres.join(", ");
    }

    // --- evento radio change ---
    tipoRadios.forEach(r => {
        r.addEventListener("change", (e) => {
            const tipo = e.target.value;
            cargarServicios(tipo);
        });
    });

    // cargar por defecto tipo 1 al inicio
    const radioActivo = Array.from(tipoRadios).find(r => r.checked) || tipoRadios[0];
    if (radioActivo) cargarServicios(radioActivo.value);

    // --- buscar cliente ---
    btnBuscarCliente.addEventListener("click", async () => {
        clientesList.innerHTML = "Buscando...";
        vehiculosList.innerHTML = "";
        clienteSeleccionadoId = null;
        seleccionadoVehiculoId = null;
        vehiculoSelText.textContent = "Ninguno";
        clearMensaje();

        const nombre = inputNombre.value.trim();
        const correo = inputCorreo.value.trim();

        if (!nombre && !correo) {
            clientesList.innerHTML = `<div class="error">Ingresa nombre o correo para buscar</div>`;
            return;
        }

        try {
            const url = new URL(`${API_BASE}/Clientes/Buscar`);
            if (nombre) url.searchParams.append("nombre", nombre);
            if (correo) url.searchParams.append("correo", correo);

            const res = await fetch(url.toString());
            const text = await res.text();
            if (!res.ok) {
                clientesList.innerHTML = `<div class="error">Error: ${text}</div>`;
                return;
            }
            const lista = JSON.parse(text);
            renderClientes(lista);
        } catch (err) {
            clientesList.innerHTML = `<div class="error">Error: ${err.message}</div>`;
        }
    });

    function renderClientes(lista) {
        clientesList.innerHTML = "";
        if (!lista || lista.length === 0) {
            clientesList.innerHTML = `<div>No se encontró ningún cliente</div>`;
            return;
        }

        lista.forEach(c => {
            const div = document.createElement("div");
            div.className = "cliente-item";
            div.innerHTML = `
                <div>
                    <div style="font-weight:700;">${c.nombreCliente}</div>
                    <div style="font-size:0.95rem;">${c.telefonoCliente || ''} — ${c.correoCliente || ''}</div>
                </div>
                <div style="display:flex; gap:8px;">
                    <button class="small-btn btn-vehiculos" data-id="${c.idCliente}">Vehículos del Cliente</button>
                </div>
            `;
            clientesList.appendChild(div);
        });

        // listeners para ver vehículos
        const btns = Array.from(document.querySelectorAll(".btn-vehiculos"));
        btns.forEach(b => b.addEventListener("click", async (e) => {
            const idCliente = e.currentTarget.dataset.id;
            clienteSeleccionadoId = parseInt(idCliente);
            await cargarVehiculosCliente(idCliente);
        }));
    }

    // --- cargar vehiculos por cliente ---
    async function cargarVehiculosCliente(idCliente) {
        vehiculosList.innerHTML = "Cargando vehículos...";
        try {
            const res = await fetch(`${API_BASE}/Vehiculos/ListarPorCliente/${idCliente}`);
            if (!res.ok) {
                const txt = await res.text();
                vehiculosList.innerHTML = `<div class="error">No hay vehículos: ${txt}</div>`;
                return;
            }
            const json = await res.json();
            // controlador retorna { mensaje, data } según tu spec
            const lista = json.data || json;
            if (!lista || lista.length === 0) {
                vehiculosList.innerHTML = `<div>No hay vehículos.</div>`;
                return;
            }
            renderVehiculos(lista);
        } catch (err) {
            vehiculosList.innerHTML = `<div class="error">Error: ${err.message}</div>`;
        }
    }

    function renderVehiculos(lista) {
        vehiculosList.innerHTML = "";
        lista.forEach(v => {
            const div = document.createElement("div");
            div.className = "vehiculo-item";
            div.innerHTML = `
                <div>
                    <div style="font-weight:700;">${v.placa} — ${v.marca} ${v.modelo} (${v.anio})</div>
                    <div style="font-size:0.9rem;">Chasis: ${v.numeroChasis}</div>
                </div>
                <div>
                    <button class="small-btn btn-seleccionar" data-id="${v.idVehiculo}" data-text="${v.placa}">Seleccionar vehículo</button>
                </div>
            `;
            vehiculosList.appendChild(div);
        });

        const btns = Array.from(document.querySelectorAll(".btn-seleccionar"));
        btns.forEach(b => b.addEventListener("click", (e) => {
            const id = parseInt(e.currentTarget.dataset.id, 10);
            const text = e.currentTarget.dataset.text;
            seleccionadoVehiculoId = id;
            vehiculoSelText.textContent = text + ` (id: ${id})`;
            setMensaje(`Vehículo ${text} seleccionado.`, "success");
        }));
    }

    // --- Crear orden + crear caso ---
    btnCrearOrden.addEventListener("click", async () => {
        clearMensaje();

        // obtener tipo servicio seleccionado
        const tipo = Array.from(tipoRadios).find(r => r.checked)?.value;
        if (!tipo) { setMensaje("Selecciona un tipo de servicio", "error"); return; }

        // tomar checkboxes seleccionados -> ids CSV
        const seleccionados = Array.from(document.querySelectorAll(".chk-servicio:checked"));
        if (seleccionados.length === 0) {
            setMensaje("Selecciona al menos un servicio.", "error");
            return;
        }
        const ids = seleccionados.map(s => s.dataset.id.trim());
        const idsCSV = ids.join(", ");

        if (!seleccionadoVehiculoId) {
            setMensaje("Selecciona un vehículo antes de crear la orden.", "error");
            return;
        }

        // preparar DTO
        const dto = {
            TipoServicio: parseInt(tipo, 10),
            diagnosticoInicial: idsCSV, // tu backend acepta string con ids: "1,2,3"
            idVehiculo: seleccionadoVehiculoId
        };

        // desactivar boton
        btnCrearOrden.disabled = true;
        btnCrearOrden.textContent = "Creando...";

        try {
            // 1) Crear Orden
            const res = await fetch(`${API_BASE}/OrdenesServicio/NuevaOrden`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(dto)
            });

            const text = await res.text();
            if (!res.ok) {
                btnCrearOrden.disabled = false;
                btnCrearOrden.textContent = "Crear Orden y Caso";
                setMensaje(`Error creando orden: ${text}`, "error");
                return;
            }
            const orden = JSON.parse(text);
            const idOrden = orden.idOrden || orden.idOrdenServicio || orden.id; // intenta varias propiedades
            // Si la respuesta devuelve el objeto OrdenesServicio, usa su id (verifica en tu API real)
            // Pero tu controlador devuelve la orden completa: buscamos la propiedad idOrden o idOrdenServicio.
            const ordenIdReal = orden.idOrden ?? orden.idOrden ?? orden.id ?? orden.idOrden; 

            // 2) Crear Caso con idOrden y idUsuario (sessionStorage)
            const crearCasoDto = {
                idOrdenServicio: orden.idOrden || ordenIdReal || orden.idOrdenServicio || orden.id
                // idUsuario se agrega abajo
            };

            // si no tenemos idOrden claro, tratar de leer la propiedad más probable
            if (!crearCasoDto.idOrdenServicio) {
                // intentar encontrar entero en el objeto
                for (const k of Object.keys(orden)) {
                    if (typeof orden[k] === "number" && orden[k] > 0) {
                        crearCasoDto.idOrdenServicio = orden[k];
                        break;
                    }
                }
            }

            // añadir idUsuario (puede ser null si no hay)
            crearCasoDto.idUsuario = idUsuario ? parseInt(idUsuario, 10) : null;

            // Llamada para crear caso
            const res2 = await fetch(`${API_BASE}/Casos/CrearCaso`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(crearCasoDto)
            });

            const text2 = await res2.text();
            if (!res2.ok) {
                setMensaje(`Orden creada (id: ${crearCasoDto.idOrdenServicio}), pero error creando caso: ${text2}`, "error");
                btnCrearOrden.disabled = false;
                btnCrearOrden.textContent = "Crear Orden y Caso";
                return;
            }
            const caso = JSON.parse(text2);

            setMensaje("Orden y Caso creados correctamente.", "success");
            // opcional: redirigir a la lista de casos después de 1.2s
            setTimeout(() => {
                window.location.href = "../PaginasMecanicos/CasosMecanico.html";
            }, 900);

        } catch (err) {
            setMensaje("Error en el proceso: " + err.message, "error");
            console.error(err);
            btnCrearOrden.disabled = false;
            btnCrearOrden.textContent = "Crear Orden y Caso";
        }
    });

    // cancelar -> volver a lista de casos
    btnCancelar.addEventListener("click", () => {
        window.location.href = "../PaginasMecanicos/CasosMecanico.html";
    });
});