var loc;
var stl;
var vid;
var isedit = false;
var block = 0;

$(document).on('click', '#reguser', function () {
    $("#regdiv").addClass('hide-block');
    $("#submit").click();
});
$(document).on('click', '.radio-container', function () {

    if ($(this).prop('id').split('-')[0] == "payment") {
        var pay = $(this).prop('id').split("-")[1];
        $("#PaymentType").val(pay);
        if (pay == 1) {
            $("#ip").removeClass('hide-block').addClass('hide-block');
            $("#ip").removeClass('simle-block').removeClass('show');
            $("#company").removeClass('hide-block').addClass('hide-block');
            $("#company").removeClass('simle-block').removeClass('show');
        }
        if (pay == 2)
        {
            $("#ip").removeClass('hide-block');
            $("#ip").addClass('simle-block').addClass('show');
            $("#company").removeClass('hide-block').addClass('hide-block');
        }
        if (pay == 3) {
            $("#ip").removeClass('hide-block').addClass('hide-block');
            $("#ip").removeClass('simle-block').removeClass('show');
            $("#company").removeClass('hide-block');
            $("#company").addClass('simle-block').addClass('show');
        }



        var items = document.getElementsByClassName('radio');
        for (var i = 0; i < items.length; ++i) {
            if ($(items[i]).prop('id').split('-')[1] == pay) {
                $(items[i]).removeClass('uncheck').removeClass('check').addClass('check')

            }
            else {
                $(items[i]).removeClass('uncheck').removeClass('check').addClass('uncheck')
            }

        }
    }



    if ($(this).prop('id').split('-')[0] == "stylesel")
    {
        stl = $(this).prop('id').split("-")[1];
        $("#StyleId").val(stl);
        $.ajax({
            type: 'GET',
            url: '/Order/GetStyle',
            data_ajax: "true",
            data: { "Id": stl },
            dataType: 'text',
            success: function (data) {
                var sss = JSON.parse(data);
                if (sss.price == 0) {
                    $('#StyleDescription').val('');
                    $('#StyleDescription').attr('disabled', true);
                    $('#stylAarea').removeClass('hide').addClass('hide');
                    $('#styleTable').removeClass('hide').addClass('hide');

                    block = 1;

                }
                else {
                    $('#StyleDescription').removeAttr('disabled');
                    $('#stylAarea').removeClass('hide');
                    $('#styleTable').removeClass('hide');
                    block = 0;
                }

            },
        });
        var items = document.getElementsByClassName('radiost');
        for (var i = 0; i < items.length; ++i) {
            if ($(items[i]).prop('id').split('-')[1] == stl) {
                $(items[i]).removeClass('uncheck').removeClass('check').addClass('check')

            }
            else {
                $(items[i]).removeClass('uncheck').removeClass('check').addClass('uncheck')
            }

        }
    }
    if ($(this).prop('id').split('-')[0] == "videos")
    {
        vid = $(this).prop('id').split("-")[1];
   
        $("#VideoId").val(vid);
        var items = document.getElementsByClassName('radiovid');
        for (var i = 0; i < items.length; ++i) {
            if ($(items[i]).prop('id').split('-')[1] == vid) {
                $(items[i]).removeClass('uncheck').removeClass('check').addClass('check')

            }
            else {
                $(items[i]).removeClass('uncheck').removeClass('check').addClass('uncheck')
            }

        }
    }
    if ($(this).prop('id').split('-')[0] == "locsel")
    {
        loc = $(this).prop('id').split("-")[1];
        $("#LocId").val(loc);
        var items = document.getElementsByClassName('radio');
        for (var i = 0; i < items.length; ++i) {
            if ($(items[i]).prop('id').split('-')[1] == loc) {
                $(items[i]).removeClass('uncheck').removeClass('check').addClass('check')

            }
            else {
                $(items[i]).removeClass('uncheck').removeClass('check').addClass('uncheck')
            }

        }
    }

    
 

});
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
});
$(document).on('focus', '#TelegramId', function () {

    $(this).inputFilter(function (value) {
        return /^[a-zA-Z0-9]+$/.test(value)
    });

});



var count;
$(document).on('click', '.group-select', function () {

    count = $(this).prop('id').split("-")[1];

    $("#NumberPhotos").val(count);

    var items = document.getElementsByClassName('group-select');
    var images = document.getElementsByClassName('group-image');
    for (var i = 0; i < items.length; ++i)
    {
        if ($(items[i]).prop('id').split("-")[1] <= count) {
            $(items[i]).removeClass('check').addClass('check');
            $(images[i]).removeClass('unact');
        }
        else
        {
            $(items[i]).removeClass('check');
            $(images[i]).removeClass('unact').addClass('unact');

        }
    }

});

$(document).on('click', '#addOrderRow', function () {

    $("#param").removeAttr('hidden');

});

$(document).on('click', '.remove-row', function () {

    var id = $(this).prop('id').split("-")[1];

    $.ajax({
        type: 'GET',
        url: '/Order/RemoveOrderItem',
        data_ajax: "true",
        //data_ajax_method: "GET",
        //data_ajax_mode: "replace",
        //data_ajax_update: "#products",
        data: { "Id": id },
        dataType: 'text',
        success: function (data) {
            var sss = JSON.parse(data);
            document.getElementById('goods-count').textContent = sss.goodsCount;
            document.getElementById('photos-count').textContent = sss.photosCount;
            document.getElementById('videos-count').textContent = sss.videosCount;
            document.getElementById('ammount-order').textContent = sss.itog;
            var row = document.getElementById("tr-" + id);
            row.parentElement.removeChild(row);
            $.ajax({
                type: 'GET',
                url: '/Order/GetTable',
                data_ajax: "true",
                data_ajax_method: "GET",
                data_ajax_mode: "replace",
                data_ajax_update: "#orderTable",
                data: { "Id": id },
                dataType: 'text',
                success: function (data) {
                    if (data != "") {
                        $('#orderTable').html(data);
                    }
                },
            });
            $.ajax({
                type: 'GET',
                url: '/Order/GetFooter',
                data_ajax: "true",
                data_ajax_method: "GET",
                data_ajax_mode: "replace",
                data_ajax_update: "#footerGoods",
                data: { "Id": id },
                dataType: 'text',
                success: function (data) {
                    if (data != "") {
                        $('#footerGoods').html(data);
                    }
                },
            });

        },
    });

    

});

$(document).on('click', '.copy-row', function () {

    var id = $(this).prop('id').split("-")[1];
    $.ajax({
        type: 'GET',
        url: '/Order/CopyRow',
        data_ajax: "true",
        //data_ajax_method: "GET",
        //data_ajax_mode: "replace",
        //data_ajax_update: "#products",
        data: { "Id": id },
        dataType: 'text',
        success: function (data) {
            var sss = JSON.parse(data);
            document.getElementById('goods-count').textContent = sss.goodsCount;
            document.getElementById('photos-count').textContent = sss.photosCount;
            document.getElementById('videos-count').textContent = sss.videosCount;
            document.getElementById('ammount-order').textContent = sss.itog;
            $.ajax({
                type: 'GET',
                url: '/Order/GetTable',
                data_ajax: "true",
                //data_ajax_method: "GET",
                //data_ajax_mode: "replace",
                //data_ajax_update: "#products",
                data: { "Id": id },
                dataType: 'text',
                success: function (data) {
                    if (data != "") {
                        $('#orderTable').html(data);
                    }
                },
            });
            $.ajax({
                type: 'GET',
                url: '/Order/GetFooter',
                data_ajax: "true",
                data_ajax_method: "GET",
                data_ajax_mode: "replace",
                data_ajax_update: "#footerGoods",
                data: { "Id": id },
                dataType: 'text',
                success: function (data) {
                    if (data != "") {
                        $('#footerGoods').html(data);
                    }
                },
            });






        },


    });
   
});


$(document).on('change', '#category', function () {
    var sel = document.getElementById("category").options.selectedIndex;
    var id = document.getElementById("category").options[sel].value;
    $.ajax({
        type: 'GET',
        url: '/Order/GetProducts',
        data_ajax: "true",
        data_ajax_method: "GET",
        data_ajax_mode: "replace",
        data_ajax_update: "#products",
        data: { "Id": id },
        dataType: 'text',
        success: function (data) {
            if (data != "") {
                $('#products').html(data);
            }
        },


    });
});

$(document).on('click', '#SaveRow', function () {

    $("#submit").click();
});

function receieveResponse(response) {

    if (response != null) {
        for (var i = 0; i < response.length; i++) {
            alert(response[i].property1);
        }
    }
}

$(document).on('click', '#FillRow', function () {
    var sel = document.getElementById("CatalogId").options.selectedIndex;

    var id = document.getElementById("CatalogId").options[sel].value;

    $.ajax({
        type: 'GET',
        url: '/Order/FillRow',
        data_ajax: "true",
        data_ajax_method: "GET",
        data_ajax_mode: "replace",
        data_ajax_update: "#orderItem",
        data: {"Id": id, "Row": $(document.getElementById("CurrentItem")).val()},
        dataType: 'text',
        success: function (data) {
            if (data != "") {
                $('#orderItem').html(data);
            
                if (isedit == true) {

                    $("locsel-"+$(document.getElementById("LocId")).val()).click();
                    $("stylesel-"+$(document.getElementById("StyleId")).val()).click();
                    $("videos-"+$(document.getElementById("VideoId")).val()).click();
                }
 
                document.getElementById('anchor1').scrollIntoView(true);
 

            }
        },
    });

});

$(document).on('keyup', '#StyleDescription', function () {

    $("#StyleDescriptionlabel").text("Текст " + $(this).val().length + "/256");

});
$(document).on('change', '#StyleDescription', function () {
    $("#StyleDescriptionlabel").text("Текст " + $(this).val().length + "/256");
});
$(document).on('keyup', '#FooterDescription', function () {

    $("#FooterDescriptionlabel").text("Текст " + $(this).val().length + "/256");

});
$(document).on('change', '#FooterDescription', function () {
    $("#FooterDescriptionlabel").text("Текст " + $(this).val().length + "/256");
});


var numPhoto;
$(document).on('click', '#imageChange-0', function () {
    numPhoto = 0;
    $("#myphotos").click();
});
$(document).on('click', '.add-image', function () {
    numPhoto = $(this).prop('id').split("-")[1];

    if (block == 1 && (numPhoto == 1 || numPhoto == 2)) {
        var f = 23;
    }
    else {
        if (numPhoto > 0) { $("#myphotos").click(); }
    }
   
    
});

$(document).on('change', '#myphotos', function () {

  


    var formData = new FormData();
    var input = document.getElementById("myphotos");
    var im = input.files[0];

    formData.append("im", im);
    var Id = document.getElementById('CurrentItem');
    var d = $(Id).val();
    formData.append("itemId", d);
    formData.append("photoNum", numPhoto);

    $.ajax({
        type: 'POST',
        url: '/Order/AddOrderItemImage',
        data: formData,
        processData: false,
        contentType: false,
        beforeSend: function () {

            $('#tableImage-' + (numPhoto)).addClass("hide");
            $('#progress-' + (numPhoto)).removeClass("hide");
        },
        success: function (data) {
            if (data != "") {
                $('#myImage-' + (numPhoto)).html(data);

                //$('#tableImage-' + (numPhoto + 1)).addClass("hide");
                //$('#progress-' + (numPhoto + 1)).removeClass("hide");
            }
        },
    });



});


window.addEventListener("DOMContentLoaded", function () {
    [].forEach.call(document.querySelectorAll('.tel'), function (input) {
        var keyCode;
        function mask(event) {
            event.keyCode && (keyCode = event.keyCode);
            var pos = this.selectionStart;
            if (pos < 3) event.preventDefault();
            var matrix = "+7 (___) ___ ____",
                i = 0,
                def = matrix.replace(/\D/g, ""),
                val = this.value.replace(/\D/g, ""),
                new_value = matrix.replace(/[_\d]/g, function (a) {
                    return i < val.length ? val.charAt(i++) || def.charAt(i) : a
                });
            i = new_value.indexOf("_");
            if (i != -1) {
                i < 5 && (i = 3);
                new_value = new_value.slice(0, i)
            }
            var reg = matrix.substr(0, this.value.length).replace(/_+/g,
                function (a) {
                    return "\\d{1," + a.length + "}"
                }).replace(/[+()]/g, "\\$&");
            reg = new RegExp("^" + reg + "$");
            if (!reg.test(this.value) || this.value.length < 5 || keyCode > 47 && keyCode < 58) this.value = new_value;
            if (event.type == "blur" && this.value.length < 5) this.value = ""
        }

        input.addEventListener("input", mask, false);
        input.addEventListener("focus", mask, false);
        input.addEventListener("blur", mask, false);
        input.addEventListener("keydown", mask, false)

    });

});