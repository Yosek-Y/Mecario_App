// ===============================
// 🌐 CONFIGURACIÓN DE API
// ===============================
const API_URL = "https://localhost:7292/Api";

// Variables globales
let idClienteActual = null;

// ===============================
// 🔎 EVENTO: BUSCAR CLIENTE
// ===============================
document.getElementById("btnBuscarCliente").addEventListener("click", async () => {
    const nombre = document.getElementById("buscarNombreCliente").value.trim();
    const correo = document.getElementById("buscarCorreoCliente").value.trim();
    const mensajes = document.getElementById("mensajeCliente");
    const contenedor = document.getElementById("resultadosClientes");

    mensajes.textContent = "";
    contenedor.innerHTML = "";

    if (nombre === "" && correo === "") {
        mensajes.textContent = "Debe ingresar un nombre o un correo para buscar.";
        mensajes.style.color = "red";
        return;
    }

    try {
        const response = await fetch(`${API_URL}/Clientes/Buscar?nombre=${nombre}&correo=${correo}`);
        const data = await response.json();

        if (!response.ok) {
            mensajes.textContent = data;
            mensajes.style.color = "red";
            return;
        }

        if (data.length === 0) {
            mensajes.textContent = "No se encontraron clientes.";
            mensajes.style.color = "red";
            return;
        }

        // Renderizar clientes encontrados
        data.forEach(cliente => {
            const div = document.createElement("div");
            div.classList.add("item-cliente");

            div.innerHTML = `
                <p><strong>${cliente.nombreCliente}</strong></p>
                <p>Correo: ${cliente.correoCliente}</p>
                <p>Telefono: ${cliente.telefonoCliente}</p>
                <p>Dirección: ${cliente.direccionCliente}</p>
                <button class="btn-buscar btn-seleccionar" style="margin-top:6px; width:100%;">Agregar Vehículo</button>
            `;

            // Evento botón seleccionar cliente
            div.querySelector(".btn-seleccionar").addEventListener("click", () => {
                seleccionarCliente(cliente.idCliente);
            });

            contenedor.appendChild(div);
        });

    } catch (error) {
        mensajes.textContent = "Error de conexión con el servidor.";
        mensajes.style.color = "red";
        console.error(error);
    }
});

// ===============================
// ✔ FUNCIÓN: SELECCIONAR CLIENTE
// ===============================
function seleccionarCliente(id) {
    idClienteActual = id;
    document.getElementById("idClienteSeleccionado").value = id;
    document.getElementById("mensajeCliente").textContent = "Cliente seleccionado correctamente.";
    document.getElementById("mensajeCliente").style.color = "green";
}

// ===============================
// 🚗 EVENTO: GUARDAR VEHÍCULO
// ===============================
document.getElementById("btnGuardarVehiculo").addEventListener("click", async () => {
    const mensajeVehiculo = document.getElementById("mensajeVehiculo");
    mensajeVehiculo.textContent = "";
    mensajeVehiculo.style.color = "red";

    if (!idClienteActual) {
        mensajeVehiculo.textContent = "Debe seleccionar un cliente primero.";
        return;
    }

    // Obtener datos del formulario
    const marca = document.getElementById("vehiculoMarca").value.trim();
    const modelo = document.getElementById("vehiculoModelo").value.trim();
    const anio = parseInt(document.getElementById("vehiculoAnno").value);
    const placa = document.getElementById("vehiculoMatricula").value.trim();
    const chasis = document.getElementById("vehiculoChasis").value.trim();

    // ===============================
    // 📌 VALIDACIONES
    // ===============================
    if (!marca) return mostrarError("La marca es obligatoria.");
    if (!modelo) return mostrarError("El modelo es obligatorio.");
    if (!placa) return mostrarError("La matrícula/placa es obligatoria.");
    if (isNaN(anio) || anio < 1900 || anio > new Date().getFullYear() + 1)
        return mostrarError("El año ingresado no es válido.");
    if (!chasis || chasis.length !== 16)
        return mostrarError("El número de chasis debe tener 16 caracteres.");

    // ===============================
    // 📤 ENVIAR A API
    // ===============================
    const vehiculoDTO = {
        placa: placa,
        marca: marca,
        modelo: modelo,
        anio: anio,
        color: "",               // No hay campo color en el HTML, se envía vacío
        numeroChasis: chasis,
        idCliente: idClienteActual
    };

    try {
        const res = await fetch(`${API_URL}/Vehiculos/AgregarVehiculo`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(vehiculoDTO)
        });

        const data = await res.json();

        if (!res.ok) {
            mensajeVehiculo.textContent = data.error || "Error al registrar vehículo.";
            mensajeVehiculo.style.color = "red";
            return;
        }

        mensajeVehiculo.style.color = "green";
        mensajeVehiculo.textContent = data.mensaje;

        // Limpiar campos
        document.getElementById("vehiculoMarca").value = "";
        document.getElementById("vehiculoModelo").value = "";
        document.getElementById("vehiculoAnno").value = "";
        document.getElementById("vehiculoMatricula").value = "";
        document.getElementById("vehiculoChasis").value = "";

    } catch (err) {
        mensajeVehiculo.textContent = "Error de conexión con el servidor.";
        console.error(err);
    }
});

// ===============================
// ⚠ UTILIDAD: MOSTRAR ERRORES
// ===============================
function mostrarError(msg) {
    const mensajeVehiculo = document.getElementById("mensajeVehiculo");
    mensajeVehiculo.textContent = msg;
    mensajeVehiculo.style.color = "red";
}