document.addEventListener("DOMContentLoaded", async () => {

    const contenedor = document.getElementById("clientes-container");
    const filtro = document.getElementById("filtroCliente");

    let listaClientes = [];

    // ===================
    // 1. Llamar API
    // ===================
    async function cargarClientes() {
        try {
            const response = await fetch("https://localhost:7082/api/Clientes");

            if (!response.ok) throw new Error("Error al obtener los clientes");

            listaClientes = await response.json();
            mostrarClientes(listaClientes);

        } catch (error) {
            contenedor.innerHTML = `<p>Error: ${error.message}</p>`;
        }
    }

    // ===================
    // 2. Mostrar Clientes
    // ===================
    function mostrarClientes(clientes) {
        contenedor.innerHTML = "";

        clientes.forEach(cliente => {
            const tarjeta = document.createElement("div");
            tarjeta.classList.add("cliente-card");

            tarjeta.innerHTML = `
                <h3>Cliente #${cliente.id}</h3>

                <p><strong>Nombre:</strong> ${cliente.nombre} ${cliente.apellido}</p>
                <p><strong>Teléfono:</strong> ${cliente.telefono}</p>
                <p><strong>Correo:</strong> ${cliente.correo}</p>
                <p><strong>Dirección:</strong> ${cliente.direccion}</p>

                <div class="acciones-cliente">
                    <button class="btn-editar" data-id="${cliente.id}">Editar</button>
                    <button class="btn-eliminar" data-id="${cliente.id}">Eliminar</button>
                </div>
            `;

            contenedor.appendChild(tarjeta);
        });

        activarBotones();
    }

    // ===================
    // 3. Activar botones Editar / Eliminar
    // ===================
    function activarBotones() {

        // EDITAR
        document.querySelectorAll(".btn-editar").forEach(btn => {
            btn.addEventListener("click", () => {
                const id = btn.getAttribute("data-id");
                editarCliente(id);
            });
        });

        // ELIMINAR
        document.querySelectorAll(".btn-eliminar").forEach(btn => {
            btn.addEventListener("click", () => {
                const id = btn.getAttribute("data-id");
                eliminarCliente(id);
            });
        });
    }

    // ===================
    // 4. Editar Cliente
    // ===================
    async function editarCliente(id) {

        try {
            const response = await fetch(`https://localhost:7082/api/Clientes/${id}`);

            if (!response.ok) throw new Error("No se pudo obtener el cliente");

            const cliente = await response.json();

            // Llenar formulario
            document.getElementById("idCliente").value = id;
            document.getElementById("nombre").value = cliente.nombre;
            document.getElementById("apellido").value = cliente.apellido;
            document.getElementById("telefono").value = cliente.telefono;
            document.getElementById("correo").value = cliente.correo;
            document.getElementById("direccion").value = cliente.direccion;

        } catch (error) {
            alert("Error al editar: " + error.message);
        }
    }

    // ===================
    // 5. Guardar (Crear o Actualizar)
    // ===================
    document.getElementById("formCliente").addEventListener("submit", async (e) => {
        e.preventDefault();

        const id = document.getElementById("idCliente").value;

        const datos = {
            nombre: document.getElementById("nombre").value,
            apellido: document.getElementById("apellido").value,
            telefono: document.getElementById("telefono").value,
            correo: document.getElementById("correo").value,
            direccion: document.getElementById("direccion").value
        };

        try {
            let response;

            // ACTUALIZAR
            if (id) {
                response = await fetch(`https://localhost:7082/api/Clientes/${id}`, {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(datos)
                });
            }
            // CREAR
            else {
                response = await fetch("https://localhost:7082/api/Clientes", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(datos)
                });
            }

            if (!response.ok) throw new Error("Error al guardar");

            limpiarFormulario();
            cargarClientes();

        } catch (error) {
            alert("Error: " + error.message);
        }
    });

    // ===================
    // 6. Eliminar Cliente
    // ===================
    async function eliminarCliente(id) {
        if (!confirm("¿Seguro que desea eliminar a este cliente?")) return;

        try {
            const response = await fetch(`https://localhost:7082/api/Clientes/${id}`, {
                method: "DELETE"
            });

            if (!response.ok) throw new Error("No se pudo eliminar");

            cargarClientes();

        } catch (error) {
            alert("Error: " + error.message);
        }
    }

    // ===================
    // 7. Filtro por nombre (ejemplo)
    // ===================
    filtro.addEventListener("input", () => {
        const texto = filtro.value.toLowerCase();

        const filtrados = listaClientes.filter(c =>
            c.nombre.toLowerCase().includes(texto) ||
            c.apellido.toLowerCase().includes(texto)
        );

        mostrarClientes(filtrados);
    });

    // ===================
    // 8. Limpiar formulario
    // ===================
    function limpiarFormulario() {
        document.getElementById("formCliente").reset();
        document.getElementById("idCliente").value = "";
    }

    // Iniciar
    cargarClientes();
});
