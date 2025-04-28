function ToggleWishlist(BookId) { 
    var button = $(`#wishlist-btn-${BookId}`);
    const isInWishlist = button.find('i').hasClass('fa-solid');

    $.ajax({
        type: "POST",
        url: isInWishlist ? removeFromWishlistUrl : addToWishlistUrl,
        data: { id: BookId },
        success: function (response) {
            if (response.success) {
                button.find('i')
                    .toggleClass('fa-solid fa-regular')
                    .parent().css('color', isInWishlist ? 'inherit' : 'red');
            }
        },
        error: function () {
            alert("An error occurred while adding the book to the wishlist.");
        }
    });
}
