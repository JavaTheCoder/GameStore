// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function imgClick(id) {
    window.location.href = window.location.href.slice(0, -1).concat(`/${id}`);
}

//document.getElementById("reply").addEventListener("click", () => {
//    document.getElementById("textfield").focus();
//});

function handleFilterBtn() {
    var field = document.getElementById("filtergames");
    if (field.style.display === "none") {
        field.style.display = "block";
    } else {
        field.style.display = "none";
    }
}
//here