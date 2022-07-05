$(document).ready(function () {
    //do work

    var multipleCardCarousel = document.querySelector(
        "#carouselModel"

    );

    if (window.matchMedia("(min-width: 768px)").matches) {
        var carousel = new bootstrap.Carousel(multipleCardCarousel, {
            interval: false,
        });

        var carouselWidth = $(".carousel-inner")[0].scrollWidth;
        var cardWidth = $(".carousel-item").width();
        var scrollPosition = 0;
        $("#carouselModel .carousel-control-next").on("click", function () {
            if (scrollPosition < carouselWidth - cardWidth * 4) {
                scrollPosition += cardWidth;
                $("#carouselModel .carousel-inner").animate(
                    { scrollLeft: scrollPosition },
                    900
                );
            }
        });
        $("#carouselModel .carousel-control-prev").on("click", function () {
            if (scrollPosition > 0) {
                scrollPosition -= cardWidth;
                $("#carouselModel .carousel-inner").animate(
                    { scrollLeft: scrollPosition },
                    900
                );
            }
        });
    } else {
        /*  $(multipleCardCarousel).addClass("slide");*/
    }

});




