//  LISTAR CLIENTES
document.getElementById("btnListarClientes").addEventListener("click", async () => {
    const tablaBody = document.querySelector("#tablaClientes tbody");
    tablaBody.innerHTML = "<tr><td colspan='5'>Cargando...</td></tr>";

    try {
        const res = await fetch("https://localhost:7292/Api/Clientes/ListarClientes");
        const json = await res.json();
        const data = json.data ?? [];

        tablaBody.innerHTML = "";

        data.forEach(cliente => {
            const fila = document.createElement("tr");
            fila.innerHTML = `
                <td>${cliente.nombreCliente}</td>
                <td>${cliente.telefonoCliente}</td>
                <td>${cliente.correoCliente}</td>
                <td>${cliente.direccionCliente}</td>
                <td>
                    <button class="btn-tabla" onclick="mostrarVehiculos(${cliente.idCliente})">Ver Vehículos</button>
                    <button class="btn-tabla btn-add-veh" onclick="mostrarFormularioVehiculo(${cliente.idCliente})">
                    Añadir Nuevo Vehículo</button>
                </td>
            `;
            tablaBody.appendChild(fila);
        });
    } catch (err) {
        console.error(err);
        tablaBody.innerHTML = "<tr><td colspan='5'>Error al cargar clientes</td></tr>";
    }
});

//  MOSTRAR VEHÍCULOS DE UN CLIENTE
async function mostrarVehiculos(idCliente) {
    const tablaBody = document.querySelector("#tablaVehiculos tbody");
    tablaBody.innerHTML = "<tr><td colspan='6'>Cargando vehículos...</td></tr>";

    try {
        const res = await fetch(`https://localhost:7292/Api/Vehiculos/ListarPorCliente/${idCliente}`);

        if (res.status === 404) {
            tablaBody.innerHTML = "<tr><td colspan='6'>El cliente no tiene vehículos registrados.</td></tr>";
            return;
        }

        const json = await res.json();
        const data = json.data ?? [];

        tablaBody.innerHTML = "";

        data.forEach(v => {
            const fila = document.createElement("tr");
            fila.innerHTML = `
                <td>${v.placa}</td>
                <td>${v.marca}</td>
                <td>${v.modelo}</td>
                <td>${v.anio}</td>
                <td>${v.color}</td>
                <td>${v.numeroChasis}</td>
            `;
            tablaBody.appendChild(fila);
        });

    } catch (err) {
        console.error(err);
        tablaBody.innerHTML = "<tr><td colspan='6'>Error al cargar vehículos</td></tr>";
    }
}

//  VARIABLE GLOBAL PARA GUARDAR EL CLIENTE ACTUAL
let clienteSeleccionado = 0;

//  EVENTO AGREGAR NUEVO CLIENTE
document.getElementById("formAgregarCliente").addEventListener("submit", async (e) => {
    e.preventDefault();

    // Capturar datos
    const dto = {
        nombreCliente: document.getElementById("cl_nombre").value.trim(),
        telefonoCliente: document.getElementById("cl_telefono").value.trim(),
        correoCliente: document.getElementById("cl_correo").value.trim(),
        direccionCliente: document.getElementById("cl_direccion").value.trim()
    };

    const msg = document.getElementById("msgAgregarCliente");

    try {
        const res = await fetch("https://localhost:7292/Api/Clientes/NuevoCliente", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(dto)
        });

        const data = await res.json();

        if (!res.ok) {
            msg.innerText = data.error || "Error al guardar el cliente.";
            return;
        }

        msg.innerText = data.mensaje;

        // Guardar ID del cliente creado
        clienteSeleccionado = data.data.idCliente;

        // Mostrar formulario de vehículo
        document.getElementById("formAgregarVehiculo").style.display = "grid";
        document.getElementById("formAgregarVehiculo").reset();

    } catch (error) {
        console.error(error);
        msg.innerText = "Error de conexión con el servidor.";
    }
});

//  MOSTRAR FORMULARIO DE VEHÍCULO (
function mostrarFormularioVehiculo(idCliente) {
    clienteSeleccionado = idCliente;
    document.getElementById("formAgregarVehiculo").style.display = "grid";
    document.getElementById("msgAgregarVehiculo").innerText = "";
    document.getElementById("formAgregarVehiculo").reset();
}

//  EVENTO GUARDAR VEHÍCULO
document.getElementById("formAgregarVehiculo").addEventListener("submit", async (e) => {
    e.preventDefault();

    const placa = document.getElementById("veh_placa").value.trim();
    const marca = document.getElementById("veh_marca").value.trim();
    const modelo = document.getElementById("veh_modelo").value.trim();
    const anio = parseInt(document.getElementById("veh_anio").value);
    const color = document.getElementById("veh_color").value.trim();
    const numeroChasis = document.getElementById("veh_chasis").value.trim();

    const msg = document.getElementById("msgAgregarVehiculo");

    if (!placa || !marca || !modelo || !anio || !color || !numeroChasis) {
        msg.innerText = "Todos los campos son obligatorios.";
        return;
    }

    const dto = {
        placa,
        marca,
        modelo,
        anio,
        color,
        numeroChasis,
        idCliente: clienteSeleccionado
    };

    try {
        const res = await fetch("https://localhost:7292/Api/Vehiculos/AgregarVehiculo", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(dto)
        });

        const data = await res.json();

        if (!res.ok) {
            msg.innerText = data.error || "Error al guardar el vehículo.";
            return;
        }

        msg.innerText = data.mensaje;
        mostrarVehiculos(clienteSeleccionado);
        e.target.reset();

    } catch (error) {
        console.error(error);
        msg.innerText = "Error de conexión con el servidor.";
    }
});
