let currentPage = 1;
const postPerPage = 6;
let totalPages = 1;


function cargarPosts() {
    return fetch(`/getPublicaciones?page=${currentPage}&pageSize=${postPerPage}`, {
        method: 'GET',
    })
        .then((response) => {
            if (!response.ok) {
                throw new Error("No se pudo obtener la lista de usuarios");
            }
            // Actualizar el número de página actual
            document.querySelector("#paginaActual").innerText = currentPage;

            // Obtener el número total de páginas del encabezado personalizado
            const lastPageHeader = response.headers.get('X-Last-Page');
            totalPages = parseInt(lastPageHeader);

            // Habilitar o deshabilitar los botones de paginación
            const prevButton = document.querySelector("#btn-prev");
            const nextButton = document.querySelector("#btn-next");
            prevButton.disabled = currentPage === 1;
            nextButton.disabled = currentPage === totalPages;
            return response.json();
        })
        .then((posts) => {

            let listadoHtml = '';
            for (let post of posts) {
                let postHtml =
                    ` 
                            <div class="col-lg-4">
                                <div class="card border-none my-card">
                                    <div class="card-image">
                                        <img src="/assets/img/service-details.jpg" alt="Servicio" class="img-fluid"/>
                                    </div>
                                    <div class="card-body text-uppercase">
                                        <div class="card-meta text-muted">
                                            <span class="meta-date">${post.fecha}</span>
                                            <span class="meta-category">${post.ubicacion}</span>
                                        </div>
                                        <h3 class="card-title">
                                            <a href="#">${post.header}</a>
                                        </h3>
                                    </div>
                                </div>
                            </div>`;
                listadoHtml += postHtml;
            }

            document.querySelector("#publicaciones main").innerHTML = listadoHtml;
        })
        .catch((error) => {
            console.error("Error al cargar las publicaciones:", error);
        });
}

cargarPosts()
    .then(() => {
    })
    .catch((error) => {
        console.error("Error en el manejo de la promesa cargarPosts():", error);
    });



function paginaAnterior() {
    if (currentPage > 1) {
        currentPage--;
        cargarPosts().then(() => {});
    }
}

function paginaSiguiente() {
    if (currentPage < totalPages) {
        currentPage++;
        cargarPosts().then(() => {});
    }
}