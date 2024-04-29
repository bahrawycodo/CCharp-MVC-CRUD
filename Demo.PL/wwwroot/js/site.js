function fileToImageSource(fileInput, imageElement) {
    if (fileInput.files.length > 0) {
        var file = fileInput.files[0];

        var reader = new FileReader();
        reader.onload = function (e) {
            var base64String = e.target.result;
            imageElement.attr('src', base64String).show();
        };

        reader.readAsDataURL(file);
    }
}
