// Esperar que el DOM esté cargado
document.addEventListener("DOMContentLoaded", () => {

    // ------------------ CLIENTE ------------------ //
    const nombreInput = document.getElementById("nombreCliente");
    const telefonoInput = document.getElementById("telefonoCliente");
    const correoInput = document.getElementById("correoCliente");
    const direccionInput = document.getElementById("direccionCliente");
    const btnAgregar = document.getElementById("btnAgregarCliente");
    const mensaje = document.getElementById("mensajeResultado");
    const panelFormulario = document.getElementById("panelFormulario");

    let idClienteActual = null; // Guardamos el ID del cliente recién agregado

    // Función para limpiar mensaje
    function limpiarMensaje() {
        mensaje.textContent = "";
        mensaje.style.color = "#333";
    }

    // Evento al hacer clic en "Agregar Cliente"
    btnAgregar.addEventListener("click", async () => {
        limpiarMensaje();

        // Crear DTO con los datos del formulario
        const dto = {
            nombreCliente: nombreInput.value.trim(),
            telefonoCliente: telefonoInput.value.trim(),
            correoCliente: correoInput.value.trim(),
            direccionCliente: direccionInput.value.trim()
        };

        try {
            // Petición POST a la API
            const res = await fetch("https://localhost:7292/Api/Clientes/NuevoCliente", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(dto)
            });

            const data = await res.json();

            if (!res.ok) {
                // Mostrar mensaje de error
                mensaje.textContent = data.error || "Error al agregar cliente";
                mensaje.style.color = "red";
                return;
            }

            // Guardar ID del cliente recién agregado
            idClienteActual = data.data.idCliente;

            // Mostrar mensaje de éxito
            mensaje.textContent = `Cliente "${data.data.nombreCliente}" agregado correctamente. Ahora puede registrar vehículos.`;
            mensaje.style.color = "green";

            // Limpiar campos del cliente
            nombreInput.value = "";
            telefonoInput.value = "";
            correoInput.value = "";
            direccionInput.value = "";

            // Mostrar formulario de vehículo
            mostrarFormularioVehiculo();

        } catch (err) {
            mensaje.textContent = "Error de conexión con el servidor";
            mensaje.style.color = "red";
            console.error(err);
        }
    });

    // ------------------ VEHÍCULO ------------------ //
    function mostrarFormularioVehiculo() {
        // Crear un contenedor para el formulario de vehículos
        const vehiculoHTML = `
            <hr style="margin:12px 0; border:none; border-top:1px solid #eee;">
            <h2>Registrar Vehículo para este cliente</h2>
            <div class="field">
                <label for="placaVehiculo">Placa</label>
                <input type="text" id="placaVehiculo" placeholder="Ej: PZ1234">
            </div>
            <div class="field">
                <label for="marcaVehiculo">Marca</label>
                <input type="text" id="marcaVehiculo" placeholder="Ej: Toyota">
            </div>
            <div class="field">
                <label for="modeloVehiculo">Modelo</label>
                <input type="text" id="modeloVehiculo" placeholder="Ej: Corolla">
            </div>
            <div class="field">
                <label for="anioVehiculo">Año</label>
                <input type="number" id="anioVehiculo" placeholder="Ej: 2020" min="1900" max="${new Date().getFullYear() + 1}">
            </div>
            <div class="field">
                <label for="colorVehiculo">Color</label>
                <input type="text" id="colorVehiculo" placeholder="Ej: Rojo">
            </div>
            <div class="field">
                <label for="chasisVehiculo">Número de chasis</label>
                <input type="text" id="chasisVehiculo" placeholder="Ej: 1122334455667788">
            </div>
            <div style="margin-top:12px; display:flex; gap:12px;">
                <button id="btnAgregarVehiculo" class="btn-accion">Agregar Vehículo</button>
                <button class="btn-cancelar" onclick="window.location.href='../PaginasMecanicos/CasosMecanico.html'">Finalizar</button>
            </div>
            <div id="mensajeVehiculo" style="margin-top:10px; font-size:0.95rem;"></div>
        `;

        // Insertar el formulario debajo del formulario de cliente
        panelFormulario.insertAdjacentHTML("beforeend", vehiculoHTML);

        // Referencias a los campos del vehículo
        const placaInput = document.getElementById("placaVehiculo");
        const marcaInput = document.getElementById("marcaVehiculo");
        const modeloInput = document.getElementById("modeloVehiculo");
        const anioInput = document.getElementById("anioVehiculo");
        const colorInput = document.getElementById("colorVehiculo");
        const chasisInput = document.getElementById("chasisVehiculo");
        const btnAgregarVehiculo = document.getElementById("btnAgregarVehiculo");
        const mensajeVehiculo = document.getElementById("mensajeVehiculo");

        // Función para limpiar mensaje
        function limpiarMensajeVehiculo() {
            mensajeVehiculo.textContent = "";
            mensajeVehiculo.style.color = "#333";
        }

        // Evento para agregar vehículo
        btnAgregarVehiculo.addEventListener("click", async () => {
            limpiarMensajeVehiculo();

            // --- VALIDACIONES ANTES DE ENVIAR ---

            // 1. Número de chasis debe tener 16 caracteres
            const chasis = chasisInput.value.trim();
            if (chasis.length !== 16) {
                mensajeVehiculo.textContent = "El número de chasis debe tener exactamente 16 caracteres.";
                mensajeVehiculo.style.color = "red";
                return;
            }

            // 2. Año válido
            const anio = parseInt(anioInput.value);
            if (isNaN(anio) || anio < 1900 || anio > new Date().getFullYear() + 1) {
                mensajeVehiculo.textContent = "El año ingresado no es válido.";
                mensajeVehiculo.style.color = "red";
                return;
            }

            // 3. Cliente existente
            if (!idClienteActual) {
                mensajeVehiculo.textContent = "No se puede agregar vehículo: cliente no válido.";
                mensajeVehiculo.style.color = "red";
                return;
            }

            // --- CREAR DTO Y ENVIAR ---
            const vehiculoDTO = {
                placa: placaInput.value.trim(),
                marca: marcaInput.value.trim(),
                modelo: modeloInput.value.trim(),
                anio: anio,
                color: colorInput.value.trim(),
                numeroChasis: chasis,
                idCliente: idClienteActual
            };

            try {
                // Petición POST a la API de vehículos
                const res = await fetch("https://localhost:7292/Api/Vehiculos/AgregarVehiculo", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(vehiculoDTO)
                });

                const data = await res.json();

                if (!res.ok) {
                    mensajeVehiculo.textContent = data.error || "Error al agregar vehículo";
                    mensajeVehiculo.style.color = "red";
                    return;
                }

                mensajeVehiculo.textContent = data.mensaje;
                mensajeVehiculo.style.color = "green";

                // Limpiar campos para poder agregar otro vehículo
                placaInput.value = "";
                marcaInput.value = "";
                modeloInput.value = "";
                anioInput.value = "";
                colorInput.value = "";
                chasisInput.value = "";
            } catch (err) {
                mensajeVehiculo.textContent = "Error de conexión con el servidor";
                mensajeVehiculo.style.color = "red";
                console.error(err);
            }
        });
    }
});