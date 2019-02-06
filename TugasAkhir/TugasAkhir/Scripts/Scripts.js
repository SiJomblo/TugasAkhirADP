function ShowImagePreview(imageUploader, previewImage) {
    if (imageUploader.files && imageUploader.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewImage).attr('src', e.target.result);
        }
        reader.readAsDataURL(imageUploader.files[0]);
    }
}

function jQueryAjaxPost(form) {
    $.validator.unobtrusive.parse(form);
    if ($(form).valid()) {
        var ajaxConfig = {
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            success: function (response) {
                if (response.success) {
                    $("#tab1").hide();
                    $("#list").html(response.html);
                    refreshAddNewTab($(form).attr('data-restUrl'), true);
                    $.notify(response.message, "success");
                    if (typeof activatejQueryTable !== 'undefined' && $.isFunction(activatejQueryTable)) {
                        activatejQueryTable();
                    }
                }
                else {
                    $.notify(response.message, "error");
                }
            }
        }
        if ($(form).attr('enctype') == "multipart/form-data") {
            ajaxConfig["contentType"] = false;
            ajaxConfig["processData"] = false;
        }
        $.ajax(ajaxConfig);
    }
    return false;
}

function refreshAddNewTab(resetUrl, showViewTab) {
    $.ajax({
        type: 'GET',
        url: resetUrl,
        success: function (response) {
            $("#AddorEdit").html(response);
            $('ul.nav.nav-tabs a:eq(1)').html('Add New');
            if (showViewTab)
                $('ul.nav.nav-tabs a:eq(0)').tab('show');
        }
    });
}

function Edit(url) {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (response) {
            $("#AddorEdit").html(response);
            $('ul.nav.nav-tabs a:eq(1)').html('Edit');
            $('ul.nav.nav-tabs a:eq(1)').tab('show');
        }
    });
}

function Details(url) {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (response) {
            $('#tab1').show();
            $("#Details").html(response);
            $('ul.nav.nav-tabs a:eq(2)').html('Detail');
            $('ul.nav.nav-tabs a:eq(2)').tab('show');
        }
    });
}


function Delete(url) {
    Swal.fire({
        title: 'Apakah Anda Yakin?',
        text: "Data karyawan ini akan dihapus!",
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Ya',
        cancelButtonText: 'Tidak'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: 'POST',
                url: url,
                success: function (response) {
                    if (response.success) {
                        $("#list").html(response.html);
                        $.notify(response.message, "warn");
                        if (typeof activatejQueryTable !== 'undefined' && $.isFunction(activatejQueryTable)) {
                            activatejQueryTable();
                        }
                    }
                    else {
                        $.notify(response.message, "error");
                    }
                }
            });
        }
    });
}
// punya karyawan
function refreshEdit(resetUrl, showViewTab) {
    $.ajax({
        type: 'GET',
        url: resetUrl,
        success: function (response) {
            $("#Edit").html(response);
            if (showViewTab)
                $('ul.nav.nav-tabs a:eq(0)').tab('show');
        }
    });
}



function EditSuratA(url) {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (response) {
            $('#tab1').show();
            $("#editsurat").html(response);
            $('ul.nav.nav-tabs a:eq(3)').tab('show');
        }
    });
}


