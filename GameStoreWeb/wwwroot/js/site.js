// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
function imgClick(id) {
    if (window.location.href.includes("FilterGame")) {
        window.location.href = `https://localhost:44316/Game/${id}`
    }
    else {
        window.location.href = window.location.href.slice(0, -1).concat(`/${id}`);
    }
}

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

function hideCommentOnDelete() {
    var hideBtn = document.getElementById("comment");
    var restoreBtn = document.getElementById("restore-btn");
    if (restoreBtn.style.display === "none") {
        hideBtn.style.display = "none";
        restoreBtn.style.display = "block";
    } else {
        hideBtn.style.display = "block";
        restoreBtn.style.display = "none";
    }
}
