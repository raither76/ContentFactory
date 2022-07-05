jQuery(function ($) {
    $.fn.inputFilter = function (inputFilter) {
        return this.on("input keydown keyup mousedown mouseup select contextmenu drop", function () {
            if (inputFilter(this.value)) {
                this.oldValue = this.value;
                this.oldSelectionStart = this.selectionStart;
                this.oldSelectionEnd = this.selectionEnd;
            } else if (this.hasOwnProperty("oldValue")) {
                this.value = this.oldValue;
                this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
            }
        });
    };

    // Custom
    var stickyToggle = function (sticky, stickyWrapper, offset, scrollElement) {
        var stickyHeight = sticky.outerHeight();
        var stickyTop = stickyWrapper.offset().top + offset;
        if (scrollElement.scrollTop() >= stickyTop) {
            stickyWrapper.height(stickyHeight);
            sticky.addClass("is-sticky");
        }
        else {
            sticky.removeClass("is-sticky");
            stickyWrapper.height('auto');
        }
    };




    // Find all data-toggle="sticky-onscroll" elements
    $('[data-toggle="sticky-onscroll"]').each(function () {
        var sticky = $(this);

        var stickyWrapper = $('<div>').addClass('sticky-wrapper'); // insert hidden element to maintain actual top offset on page
        sticky.before(stickyWrapper);
        sticky.addClass('sticky');

        // Scroll & resize events
        $(window).on('scroll.sticky-onscroll resize.sticky-onscroll', function () {
            stickyToggle(sticky, stickyWrapper, 0, $(this));

        });
        $(window).bind("load resize scroll", function () {
            stickyToggle(sticky, stickyWrapper, 0, $(this));
        });


        // On page load
        stickyToggle(sticky, stickyWrapper, $(window));
    });
    $('[data-toggle="sticky-onscroll2"]').each(function () {
        var sticky = $(this);
        var stickyWrapper = $('<div>').addClass('sticky-wrapper'); // insert hidden element to maintain actual top offset on page
        sticky.before(stickyWrapper);
        sticky.addClass('sticky');

        // Scroll & resize events
        $(window).on('scroll.sticky-onscroll resize.sticky-onscroll', function () {
            stickyToggle(sticky, stickyWrapper, 55, $(this));

        });

        // On page load
        stickyToggle(sticky, stickyWrapper, 0, $(window));
    });




});
$(document).on('click', '.removeOrder', function () {

    id = $(this).prop('id').split("-")[1];

    $.ajax({
        type: 'GET',
        url: '/Admin/RemoveOrder',
        data: { "Id": id },
        dataType: 'text'
        //success: function (data) {
        //    if (data != "") {

        //    }
        //},
    });
    $(this).closest('tr').remove();

});
$(document).ready(function () {
    $('.carousel').slick({
        slidesToShow: 3,
        dots: true,
        centerMode: true,
    });
});

// Slick version 1.5.8
jQuery(function ($) {
    $.fn.dashboard = function (args) {
        var defaults = {
            width: 768,
            sidebar: {
                collapsedClass: 'sidebar-collapsed',
                fixedClass: 'sidebar-fixed',
                beforeCollapse: function () { },
                afterCollapse: function () { },
            }
        };
        var settings = $.extend(true, defaults, args);

        init();

        function init() {
            $('[data-toggle-sidebar]').click(function (e) {
                e.preventDefault();
                toggleSidebar();
            });

            $('[data-toggle-fixed]').click(function (e) {
                e.preventDefault();
                toggleFixed();
            });

            $(window).bind("load resize scroll", function () {
                width = window.innerWidth;
                height = window.innerHeight;
                var menu = document.getElementById('stickynav');
                var footer = document.getElementById('mainfooter');
                var top = menu.getBoundingClientRect();
                var bottom = footer.getBoundingClientRect();
                $(".main-sidebar").css("top", top.bottom);

                //$(".main-sidebar").css("bottom", height -18);
                if (width < settings.width)
                    collapseSidebar();
            });
        };

        function toggleSidebar() {
            if ($('body').hasClass(settings.sidebar.collapsedClass)) {
                expandSidebar();

            } else {
                collapseSidebar();
            }
        }

        function collapseSidebar() {
            settings.sidebar.beforeCollapse();
            $('body').addClass(settings.sidebar.collapsedClass);
            settings.sidebar.afterCollapse();
        }

        function expandSidebar() {
            $('body').removeClass(settings.sidebar.collapsedClass);
        }

        function toggleFixed() {
            $('body').toggleClass(settings.sidebar.fixedClass);
        }
    };
});

jQuery(function ($) {
    var args = {
        sidebar: {
            beforeCollapse: function () {
                $('.main-sidebar .collapse').collapse('hide');
            }
        }
    };
    $('body').dashboard(args);
});


$(document).on('change', '#images', function () {
    uploadimages();
});
$(document).on('change', '#modelimages', function () {
    uploadmodelimages();
});
$(document).on('change', '#changeImage', function () {
    addlocation();
});

$(document).on('change', '#change-Image', function () {
    
    changeImLoc();
});


$(document).on('change', '#changeStyleImage', function () {
    addStyle();
});

$(document).on('change', '#change-StyleImage', function () {

    changeImStyle();
});
$(document).on('click', '.select-video', function () {

    var i = $(this).prop('id').split("-")[1];
    $("#videofile-"+i).click();
});
$(document).on('change', '.vid', function () {

    var i = $(this).prop('id').split("-")[1];

    $("#s-" + i).removeAttr('hidden');
   
});

$(document).on('click', '.save-desc', function () {
    var i = $(this).prop('id').split("-")[1];

    var model = $('#videocontent-' + i).serialize();
    $.ajax({
        type: 'POST',
        url: '/Admin/SaveVideoDesc',
        data_ajax: "true",
        data_ajax_method: "POST",
        data_ajax_mode: "replace",
        data_ajax_update: "#video-" + i,
        data: model,
        data_ajax: "true",

    });



});


$(document).on('click', '.save-video', function () {

    var i = $(this).prop('id').split("-")[1];
    var formData = new FormData($('#videocontent-' + i)[0]);
    var model = $('#videocontent-' + i).serialize();

    var input = document.getElementById("videofile-" + i);
    var vid = input.files[0];
    formData.append("video", vid);

 
  

/*     Ajax-запрос на сервер*/
    $.ajax({
        type: 'POST',
        url: '/Admin/UpdateVideo',
        data_ajax: "true",
        data_ajax_method: "POST",
        data_ajax_mode: "replace",
        data_ajax_update: "#video-"+i,
        data: formData,
        dataType: "text",
        processData: false,
        contentType: false,
      /*  data_ajax: "true",*/
        //beforeSend: function () {
 

        //    // Перед загрузкой файла удалить старые ошибки и показать индикатор
        //    $('.error').text('').hide();
        //    $('#progress'+i).show();

        //    // Установить прогресс-бар на 0
        //    $('#progress-bar'+i).css('width', '0');
        //    $('#progress-value'+i).text('0 %');
        //},
        success: function (data) {
            $('#video-'+i).html(data);
        },
        xhrFields: { // Отслеживаем процесс загрузки файлов
            onprogress: function (e) {
                if (e.lengthComputable) {
                    // Отображение процентов и длины прогресс бара
                    var perc = e.loaded / 100 * e.total;
                    $('#progress-bar'+i).css('width', perc + '%');
                    $('#progress-value'+i).text(perc + ' %');
                }
            }
        }


    });









});

$(document).on('click', '.add-image', function () {
   
    $("#changeImage").click();
});
$(document).on('click', '.add-style-image', function () {

    $("#changeStyleImage").click();
});

var id;
var styleId;
$(document).on('click', '.change-image', function () {
    id = $(this).prop('id').split("-")[1];
    $("#change-Image").click();
});
$(document).on('click', '.change-Style-image', function () {
    styleId = $(this).prop('id').split("-")[1];
    $("#change-StyleImage").click();
});


$(document).on('click', '.saveLoc', function () {


    var id = $(this).prop('id').split("-")[1];
   
    var orderJson = {
        Id: id,
        Name: $(document.getElementById("Name-" + id)).val(),
        Description: $(document.getElementById("Description-" + id)).val(),
        Price: $(document.getElementById("Price-" + id)).val(),
        Color: $(document.getElementById("Color-" + id)).val()

    };


    $.ajax({
        type: 'POST',
        url: '/Admin/SaveLocation',
        data: orderJson,
        dataType: "json",
        data_ajax: "true",
        success: null
    });

});
$(document).on('click', '.saveStyle', function () {


    var id = $(this).prop('id').split("-")[1];

    var orderJson = {
        Id: id,
        Name: $(document.getElementById("Name-" + id)).val(),
        Description: $(document.getElementById("Description-" + id)).val(),
        Price: $(document.getElementById("Price-" + id)).val()

    };


    $.ajax({
        type: 'POST',
        url: '/Admin/SaveStyle',
        data: orderJson,
        dataType: "json",
        data_ajax: "true",
        success: null
    });

});
$(document).on('click', '.deltr', function () {

    $(this).closest("tr").remove();
});

$(document).on('click', '.up', function () {

    var row = $(this).parents("tr:first");

    row.insertBefore(row.prev());

});
$(document).on('click', '.down', function () {

    var row = $(this).parents("tr:first");

    row.insertAfter(row.next());

});


$(document).on('focus', '.dbl', function () {

    $(this).inputFilter(function (value) {
        return /^-?\d*[,]?\d*$/.test(value)
    });

});

$(document).on('change', '#changeroot', function () {
    $("#categoryName").prop("disabled", !$("#changeroot").prop('checked'));
    $("#parentName").prop("disabled", !$("#changeroot").prop('checked'));

});

$(document).on('change', '.changelogo', function () {
 
    var d = $(this).prop('checked');
    var curr = $(this).prop('id');

    var items = document.getElementsByClassName('changelogo');

    if (d == true)
    {
        for (var i = 0; i < items.length; ++i)
        {

            var fff = $(items[i]).prop('id');
            if (fff != curr)
            {
                document.getElementById(fff).checked = false;
            }
        }
        var id = $(this).prop('alt');
        $.ajax({
            type: 'POST',
            dataType: 'html', 
            url: '/Admin/ChangeFaceImage',
            data: {'id': id }

    });
   }
});





function uploadimages() {
    var input = document.getElementById("images");
    var files = input.files;
    var formData = new FormData();
    for (var i = 0; i != files.length; i++) {
        formData.append("files", files[i]);
    }
    var Id = document.getElementById('CatId');
    formData.append("CatId", $(Id).val());
    $.ajax({
        type: 'POST',
        url: '/Admin/Upload',
        data: formData,
        data_ajax: "true",
        processData: false,
        contentType: false,
        beforeSend: function () {
            //$('#fileUpload').removeClass('dd');

            // Перед загрузкой файла удалить старые ошибки и показать индикатор
            $('.error').text('').hide();
            $('.progress').show();

            // Установить прогресс-бар на 0
            $('.progress-bar').css('width', '0');
            $('.progress-value').text('0 %');
        },
        success: function (data) {
            $('.progress-bar').css('width', '100%');
            $('.progress-value').text('100 %');
            $('#productimages').html(data);
            //$('.progress').hide();
        },
        //error: function (xhr, status, p3) {
        //    alert(xhr.responseText);
        //},
        xhrFields: { // Отслеживаем процесс загрузки файлов
            onprogress: function (e) {
                if (e.lengthComputable) {
                    // Отображение процентов и длины прогресс бара
                    var perc = e.loaded / 100 * e.total;
                    $('.progress-bar').css('width', perc + '%');
                    $('.progress-value').text(perc + ' %');
                }
            }
        }
    });
};


function changeImLoc() {

    var input = document.getElementById("change-Image");
    var im = input.files[0];
    var formData = new FormData();
    formData.append("image", im);

    formData.append("Id", id);
    var str = $("#locim-" + id).prop("src");

    $.ajax({
        type: 'POST',
        url: '/Admin/ChangeImLoc',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {

            //$("#change-" + id).append.html(data);
           // $(this).html(data);

        },
    });
};
function changeImStyle() {

    var input = document.getElementById("change-StyleImage");
    var im = input.files[0];
    var formData = new FormData();
    formData.append("image", im);

    formData.append("Id", styleId);
    var str = $("#styleim-" + id).prop("src");

    $.ajax({
        type: 'POST',
        url: '/Admin/changeImStyle',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {

            //$("#change-" + id).append.html(data);
            // $(this).html(data);

        },
    });
};





function addlocation() {
 
    var input = document.getElementById("changeImage");
    var im = input.files[0];
    var formData = new FormData();
    formData.append("image", im);
    var Desc = document.getElementById('newLocation');
    formData.append("Desc", $(Desc).val());

    $.ajax({
        type: 'POST',
        url: '/Admin/AddLocation',
        data: formData,
        data_ajax: "true",
        processData: false,
        contentType: false,

    });
};

function addStyle() {

    var input = document.getElementById("changeStyleImage");
    var im = input.files[0];
    var formData = new FormData();
    formData.append("image", im);
    var Desc = document.getElementById('newStyle');
    formData.append("Desc", $(Desc).val());

    $.ajax({
        type: 'POST',
        url: '/Admin/AddStyle',
        data: formData,
        data_ajax: "true",
        processData: false,
        contentType: false,

    });
};

function uploadmodelimages() {
    var input = document.getElementById("modelimages");
    var files = input.files;
    var formData = new FormData();
    for (var i = 0; i != files.length; i++) {
        formData.append("files", files[i]);
    }
    var Id = document.getElementById('ModelId');
    formData.append("Id", $(Id).val());
    $.ajax({
        type: 'POST',
        url: '/Admin/UploadModel',
        data: formData,
        data_ajax: "true",
        processData: false,
        contentType: false,
        beforeSend: function () {
            //$('#fileUpload').removeClass('dd');

            // Перед загрузкой файла удалить старые ошибки и показать индикатор
            $('.error').text('').hide();
            $('.progress').show();

            // Установить прогресс-бар на 0
            $('.progress-bar').css('width', '0');
            $('.progress-value').text('0 %');
        },
        success: function (data) {
            $('.progress-bar').css('width', '100%');
            $('.progress-value').text('100 %');

            $('#modelimg').html(data);

            //$('.progress').hide();
        },
        //error: function (xhr, status, p3) {
        //    alert(xhr.responseText);
        //},
        xhrFields: { // Отслеживаем процесс загрузки файлов
            onprogress: function (e) {
                if (e.lengthComputable) {
                    // Отображение процентов и длины прогресс бара
                    var perc = e.loaded / 100 * e.total;
                    $('.progress-bar').css('width', perc + '%');
                    $('.progress-value').text(perc + ' %');
                }
            }
        }
    });
};


