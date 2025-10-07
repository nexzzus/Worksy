validateAuthentication();

function getHeaders() {
    return {
        'Authorization': 'Bearer ' + localStorage.token
    };
}

async function validateAuthentication() {
    const response = await fetch("/validateAuthentication", {
        method: 'GET',
        headers: getHeaders()
    });

    if (response.ok) {
        var url = document.URL || document.location.href;
        var htmlFileName = url.substring(url.lastIndexOf('/') + 1);
        var nombreArchivo = htmlFileName.replace(".html", "");

        console.log(nombreArchivo);

        if (nombreArchivo.includes("Secured")) {
            actualizarUsernameDelUsuario();
        }else if(nombreArchivo === ""){
            location.href = "indexSecured.html";
        }else{
            var securedHtmlFileName = htmlFileName.replace(".html", "Secured.html");
            location.href = securedHtmlFileName;
        }
    } else {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'No tienes acceso a esta pestaña',
            showConfirmButton: false,
            timer: 1000
        }).then(() => {
            location.href = "index.html";
        });
    }
}

function actualizarUsernameDelUsuario() {
    document.getElementById("userDropdown").outerHTML = localStorage.username;
}

async function logout() {
    localStorage.clear();
    Swal.fire({
        icon: 'success',
        title: 'Sesión cerrada correctamente',
        showConfirmButton: false,
        timer: 1000
    }).then(() => {
        location.href = "index.html";
    });
}