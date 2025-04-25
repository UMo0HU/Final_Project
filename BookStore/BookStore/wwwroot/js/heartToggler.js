document.addEventListener("DOMContentLoaded", () => {
    const wishlistIcons = document.querySelectorAll('.book-details-page-wishlist');

    wishlistIcons.forEach((wishlistIcon) => {
        const wishlistText = wishlistIcon.querySelector('i');

        wishlistIcon.addEventListener('click', () => {
            if (wishlistText.classList.contains('fa-regular')) {
                wishlistText.classList.remove('fa-regular');
                wishlistText.classList.add('fa-solid');
                wishlistText.style.color = "red";
            } else {
                wishlistText.classList.remove('fa-solid');
                wishlistText.classList.add('fa-regular');
                wishlistText.style.color = "";
            }
        });
    });
});