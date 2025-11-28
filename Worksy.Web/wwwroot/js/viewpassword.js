function togglePassword(inputId, iconId) {
    var input = document.getElementById(inputId);
    var icon = document.getElementById(iconId);
    if (input.type === "password") {
        input.type = "text";
        icon.src = "/img/ojo.png";
    } else {
        input.type = "password";
        icon.src = "/img/ojo.png";
    }
}
