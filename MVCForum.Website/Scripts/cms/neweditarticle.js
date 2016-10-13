$('#SelectedTags')
            .select2({
                tags: true,
                tokenSeparators: [','],
                placeholder: "Tilføj tags her"
            });

$('#Category').select2();

$("#Image")
    .keyup(function () {
        $('#imgPreview').attr('src', $('#Image').val());
    });

$("#UploadImg").change(function () {
    var data = new FormData();
    var files = $("#UploadImg").get(0).files;
    if (files.length > 0) {
        data.append("MyImages", files[0]);
    }

    $.ajax({
        url: "/CMS/UploadFile",
        type: "POST",
        processData: false,
        contentType: false,
        data: data,
        success: function (response) {
            //code after success
            $("#Image").val('/Images/ArticleImages/' + response);
            $("#imgPreview").attr('src', '/Images/ArticleImages/' + response);
        },
        error: function (er) {
            alert(er);
        }

    });
});