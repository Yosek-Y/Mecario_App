document.addEventListener("DOMContentLoaded", async () => {
    const idCaso = sessionStorage.getItem("idCasoAbrir"); 
    const timerDisplay = document.getElementById("timerDisplay");
    const serviciosContainer = document.getElementById("serviciosContainer");
    const listaPiezasUsadas = document.getElementById("listaPiezasUsadas");
    const listaTareas = document.getElementById("listaTareas");
    const btnFinalizar = document.getElementById("btnFinalizar");
    const btnContinuarDespues = document.getElementById("btnContinuarDespues");
    const btnRegistrarPieza = document.getElementById("btnRegistrarPieza");
    const inputCodigoPieza = document.getElementById("inputCodigoPieza");
    const inputCantidadPieza = document.getElementById("inputCantidadPieza");
    const tituloCaso = document.getElementById("tituloCaso");
    const infoOrden = document.getElementById("infoOrden");

    if (!idCaso) {
        alert("No se encontró el ID del caso.");
        return;
    }

    let tiempoTrabajado = 0; // en segundos
    let timerInterval;

    // =========================
    // 1. Iniciar timer
    // =========================
    function iniciarTimer() {
        timerInterval = setInterval(() => {
            tiempoTrabajado++;
            const hrs = String(Math.floor(tiempoTrabajado / 3600)).padStart(2, "0");
            const mins = String(Math.floor((tiempoTrabajado % 3600) / 60)).padStart(2, "0");
            const secs = String(tiempoTrabajado % 60).padStart(2, "0");
            timerDisplay.textContent = `${hrs}:${mins}:${secs}`;
        }, 1000);
    }

    // =========================
    // 2. Traer servicios del caso
    // =========================
    async function cargarServicios() {
        try {
            const res = await fetch(`https://localhost:7292/Api/Casos/ServiciosDeCaso/${idCaso}`);
            if (!res.ok) throw new Error("Error al obtener los servicios");
            const data = await res.json();

            tituloCaso.textContent = `Caso #${idCaso}`;
            infoOrden.textContent = `Orden: ${data.idOrden} - Diagnóstico: ${data.diagnosticoInicial}`;

            serviciosContainer.innerHTML = "";
            data.servicios.forEach(servicio => {
                const div = document.createElement("div");
                div.classList.add("servicio-item");
                div.dataset.idServicio = servicio.idServicio;
                div.dataset.nombreServicio = servicio.servicio;
                div.innerHTML = `
                    <span>${servicio.servicio} - $${servicio.precio}</span>
                    <button class="accion-btn btn-hecho">Hecho</button>
                `;
                serviciosContainer.appendChild(div);
            });

            await cargarTareasRegistradas();
            habilitarFinalizarSiListo();
        } catch (error) {
            serviciosContainer.innerHTML = `<p>Error: ${error.message}</p>`;
        }
    }

    // =========================
    // 3. Registrar tarea hecha
    // =========================
    serviciosContainer.addEventListener("click", async (e) => {
        if (e.target.classList.contains("btn-hecho")) {
            const item = e.target.closest(".servicio-item");
            const tarea = item.dataset.nombreServicio;

            const dto = {
                idCaso: parseInt(idCaso),
                hora: new Date(),
                tareaRealizada: tarea
            };

            try {
                const res = await fetch(`https://localhost:7292/api/DetallesCaso/RegistrarTarea`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(dto)
                });
                if (!res.ok) throw new Error("No se pudo registrar la tarea");
                item.classList.add("done");
                e.target.disabled = true;

                await cargarTareasRegistradas();
                habilitarFinalizarSiListo();
            } catch (err) {
                alert(err.message);
            }
        }
    });

    // =========================
    // 4. Cargar tareas ya hechas
    // =========================
    async function cargarTareasRegistradas() {
        try {
            const res = await fetch(`https://localhost:7292/api/DetallesCaso/${idCaso}`);
            if (!res.ok) throw new Error("Error al obtener detalles de tareas");
            const tareas = await res.json();
            listaTareas.innerHTML = "";
            tareas.forEach(t => {
                const div = document.createElement("div");
                div.classList.add("servicio-item", "done");
                div.innerHTML = `${t.tareaRealizada} - <span style="font-size:0.85rem;color:#555;">${new Date(t.hora).toLocaleTimeString()}</span>`;
                listaTareas.appendChild(div);

                // marcar también en serviciosContainer como hecho
                const servicioDiv = Array.from(serviciosContainer.children)
                    .find(s => s.dataset.nombreServicio === t.tareaRealizada);
                if (servicioDiv) {
                    servicioDiv.classList.add("done");
                    servicioDiv.querySelector("button").disabled = true;
                }
            });
        } catch (err) {
            listaTareas.innerHTML = `<p>Error: ${err.message}</p>`;
        }
    }

    // =========================
    // 5. Piezas usadas
    // =========================
    async function cargarPiezasUsadas() {
        try {
            const res = await fetch(`https://localhost:7292/api/DetallesPiezas/${idCaso}`);
            if (!res.ok) throw new Error("Error al obtener piezas");
            const piezas = await res.json();
            listaPiezasUsadas.innerHTML = "";
            piezas.forEach(p => {
                const div = document.createElement("div");
                div.classList.add("servicio-item");
                div.textContent = `${p.nombrePieza} x${p.cantidad} - $${p.subtotal.toFixed(2)}`;
                listaPiezasUsadas.appendChild(div);
            });
        } catch (err) {
            listaPiezasUsadas.innerHTML = `<p>Error: ${err.message}</p>`;
        }
    }

    btnRegistrarPieza.addEventListener("click", async () => {
        const codigo = inputCodigoPieza.value.trim();
        const cantidad = parseInt(inputCantidadPieza.value);

        if (!codigo || cantidad <= 0) return alert("Código o cantidad inválidos");

        const dto = { idCaso: parseInt(idCaso), codigoPieza: codigo, cantidad: cantidad };

        try {
            const res = await fetch(`https://localhost:7292/api/DetallesPiezas/RegistrarPieza`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(dto)
            });
            if (!res.ok) throw new Error("No se pudo registrar la pieza");
            inputCodigoPieza.value = "";
            inputCantidadPieza.value = 1;
            await cargarPiezasUsadas();
        } catch (err) {
            alert(err.message);
        }
    });

    // =========================
    // 6. Finalizar / Continuar
    // =========================
    btnFinalizar.addEventListener("click", async () => {
        clearInterval(timerInterval);
        const dto = {
            idCaso: parseInt(idCaso),
            fechaFin: new Date(),
            horasTrabajadas: tiempoTrabajado / 3600
        };
        try {
            const res = await fetch(`https://localhost:7292/Api/Casos/CerrarCaso`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(dto)
            });
            if (!res.ok) throw new Error("Error al finalizar el caso");
            alert("Caso finalizado correctamente");
            window.location.href = "CasosMecanico.html"; // volver al listado
        } catch (err) {
            alert(err.message);
        }
    });

    btnContinuarDespues.addEventListener("click", async () => {
        clearInterval(timerInterval);
        const dto = {
            idCaso: parseInt(idCaso),
            horasTrabajadas: tiempoTrabajado / 3600
        };
        try {
            const res = await fetch(`https://localhost:7292/Api/Casos/ContinuarCaso`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(dto)
            });
            if (!res.ok) throw new Error("Error al continuar el caso");
            alert("Caso actualizado y continuado más tarde");
            window.location.href = "CasosMecanico.html"; // volver al listado
        } catch (err) {
            alert(err.message);
        }
    });

    // =========================
    // 7. Habilitar botón Finalizar
    // =========================
    function habilitarFinalizarSiListo() {
        const todosHechos = Array.from(serviciosContainer.children).every(s => s.classList.contains("done"));
        btnFinalizar.disabled = !todosHechos;
    }

    // =========================
    // Inicializar
    // =========================
    cargarServicios();
    cargarPiezasUsadas();
    iniciarTimer();
});