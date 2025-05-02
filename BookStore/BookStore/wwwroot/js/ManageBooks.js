document.addEventListener("DOMContentLoaded", function () {
    const deleteButtons = document.querySelectorAll(".btn-delete");

    deleteButtons.forEach(button => {
        button.addEventListener("click", function () {
            const bookTitle = this.getAttribute("data-title");
            if (confirm(`Are you sure you want to delete "${bookTitle}"?`)) {
                alert("Book deleted (simulation)");
            }
        });
    });
});
