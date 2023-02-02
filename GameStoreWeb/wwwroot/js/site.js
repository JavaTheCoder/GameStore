// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// https://localhost:44316/Home/FilterGamesByGenre
// https://localhost:44316/Game/${id}
function imgClick(id) {
    if (window.location.href.includes("FilterGame"))
        window.location.href = `https://localhost:44316/Game/${id}`
        //window.location.href = window.location.href.slice(-22, -1).concat(`Game/${id}`);
    else 
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

function handleFilterByNameBtn() {
    var field = document.getElementById("filterbyname");
    if (field.style.display === "none") {
        field.style.display = "block";
    } else {
        field.style.display = "none";
    }
}
//here