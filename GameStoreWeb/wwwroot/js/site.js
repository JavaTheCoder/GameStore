// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
function imgClick(id) {
    if (window.location.href.endsWith("FilterGames")) {
        window.location.href = window.location.href.replace("Game/FilterGames", `Details/${id}`);
    } else if (window.location.href.endsWith("GetAllCartItems")) {
        window.location.href = window.location.href.replace("CartItem/GetAllCartItems", `Details/${id}`);
    } else {
        window.location.href = window.location.href.replace("Games", `Details/${id}`);
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
