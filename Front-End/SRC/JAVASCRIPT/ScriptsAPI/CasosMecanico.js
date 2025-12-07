document.addEventListener("DOMContentLoaded", async () => {

    const idMecanico = sessionStorage.getItem("idUsuario");
    const contenedor = document.getElementById("casos-container");
    const filtro = document.getElementById("filtroEstado");

    if (!idMecanico) {
        contenedor.innerHTML = "<p>No se encontró el ID del mecánico.</p>";
        return;
    }

    let listaCasos = [];

    //Llamada a la API
    async function cargarCasos() {
        try {
            const response = await fetch(`https://localhost:7292/Api/Casos/Mecanico/Casos_${idMecanico}`);

            const data = await response.json();

            //Si el json retorna "El mecánico no tiene casos asignados." muestra el mensaje personalizado
            if (!response.ok) {
                if (data.error === "El mecánico no tiene casos asignados.") {
                    contenedor.innerHTML = "<p>No tienes casos asignados, crea un caso o dile al administrador que te asigne un caso.</p>";
                } else {
                    contenedor.innerHTML = `<p>Error: ${data.error || "Error desconocido"}</p>`;
                }
                return;
            }

            listaCasos = data;
            mostrarCasos(listaCasos);


        } catch (error) {
            contenedor.innerHTML = `<p>Error: ${error.message}</p>`;
        }
    }

    //Muestra los casos del mecanico 
    function mostrarCasos(casos) {
        contenedor.innerHTML = "";

        casos.forEach(caso => {
            const tarjeta = document.createElement("div");
            tarjeta.classList.add("caso-card");

            // Convertir fecha a formato corto
            const fechaInicio = new Date(caso.fechaInicio).toLocaleDateString();
            const fechaFin = caso.fechaFin ? new Date(caso.fechaFin).toLocaleDateString() : "—";

            // Estado → color + punto
            const estadoInfo = obtenerEstado(caso.estadoCaso);

            // Crear HTML interno
            tarjeta.innerHTML = `
                <h3>Caso #${caso.idCaso}</h3>

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
            contenedor.appendChild(tarjeta);

            // Agregar listeners a botones que acabamos de insertar
            // Botón "Comenzar a trabajar"
            const btnComenzar = tarjeta.querySelector(".comenzar");
            if (btnComenzar) {
                btnComenzar.addEventListener("click", async () => {
                    const dto = {
                        idCaso: caso.idCaso,
                        fechaInicio: new Date() // fecha actual
                    };

                    try {
                        const res = await fetch("https://localhost:7292/Api/Casos/AbrirCaso", {
                            method: "PUT",
                            headers: { "Content-Type": "application/json" },
                            body: JSON.stringify(dto)
                        });

                        if (!res.ok) throw new Error("No se pudo abrir el caso");

                        const casoActualizado = await res.json();

                        // guardamos idCaso para la nueva página
                        sessionStorage.setItem("idCasoAbrir", casoActualizado.idCaso);

                        // redirigimos a la página de trabajo del caso
                        window.location.href = "../PaginasMecanicos/AbrirCaso.html";
                    } catch (err) {
                        alert(err.message);
                    }
                });
            }

            // Botón "Continuar Trabajando" (si quieres que haga algo parecido)
            const btnContinuar = tarjeta.querySelector(".continuar");
            if (btnContinuar) {
                btnContinuar.dataset.id = caso.idCaso;
                btnContinuar.addEventListener("click", () => {
                    // igual que comenzar, abrimos la página de trabajo (en caso de reanudar)
                    sessionStorage.setItem("idCasoAbrir", caso.idCaso);
                    window.location.href = "../PaginasMecanicos/AbrirCaso.html";
                });
            }

        });
    }

    // Informacion visual de los estados de los casos
    function obtenerEstado(estado) {
        switch (estado) {
            case "noEmpezado":
                return {
                    color: "#ff3b3b",
                    texto: "No Empezado",
                    botonHTML: `<button class="btn-accion comenzar">Comenzar a trabajar</button>`
                };

            case "enProceso":
                return {
                    color: "#ffc107",
                    texto: "En Proceso",
                    botonHTML: `<button class="btn-accion continuar">Continuar Trabajando</button>`
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

    //Filtrar por dropdown
    filtro.addEventListener("change", () => {
        const value = filtro.value;

        if (value === "todos") {
            mostrarCasos(listaCasos);
        } else {
            const filtrados = listaCasos.filter(c => c.estadoCaso === value);
            mostrarCasos(filtrados);
        }
    });

    cargarCasos();
});