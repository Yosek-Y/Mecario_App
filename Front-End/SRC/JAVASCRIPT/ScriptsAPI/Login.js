document.addEventListener("DOMContentLoaded", () => {

    const form = document.getElementById("loginForm");

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        // Valores ingresados
        const usuario = document.getElementById("usuario").value.trim();
        const contrasena = document.getElementById("password").value.trim();

        // Objeto DTO
        const dto = {
            usuario: usuario,
            contrasena: contrasena
        };

        // El div donde pondremos el mensaje de error si algo falla
        let errorDiv = document.getElementById("loginError");

        if (!errorDiv) {
            errorDiv = document.createElement("p");
            errorDiv.id = "loginError";
            errorDiv.style.color = "red";
            errorDiv.style.marginTop = "10px";
            errorDiv.style.fontSize = "0.9rem";
            form.appendChild(errorDiv);
        }

        try {
            const response = await fetch("https://localhost:7292/Api/Usuarios/IniciarSesion", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(dto)
            });

            if (!response.ok) {
                const errorMsg = await response.text();
                errorDiv.textContent = errorMsg || "Error al iniciar sesión.";
                return;
            }

            const data = await response.json();

            // Guardar en sessionStorage
            sessionStorage.setItem("idUsuario", data.idUsuario);
            sessionStorage.setItem("tipoUsuario", data.tipoUsuario);

            // Redirecciones según el tipo de usuario
            if (data.tipoUsuario === "Admin") {
                window.location.href = "../PaginasAdmin/CasosAdmin.html";
            } else if (data.tipoUsuario === "Mecanico") {
                window.location.href = "../PaginasMecanicos/CasosMecanico.html"; // Cambiar a la página correspondiente para mecánicos
            }
        } catch (error) {
            errorDiv.textContent = "Error de conexión con el servidor.";
        }
    });
});