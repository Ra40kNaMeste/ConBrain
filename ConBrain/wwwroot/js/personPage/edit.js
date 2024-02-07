//Загрузка элементов со страницы
const fileInput = document.getElementById("fileInput");
const loadAvatarButton = document.getElementById("loadAvatarButton");
const accountExitButton = document.getElementById("accountExit");

const loadedImageSize = new Object();
loadedImageSize.width = 200;
loadedImageSize.height = 200;

////Настройка загрузки аватара
//loadAvatarButton.addEventListener("click", e => {
//    console.log(fileInput);
//    if (fileInput.files.length == 0) {
//        return;
//    }
//    console.log(fileInput.files[0]);
//    for (prop in fileInput.files[0]) {

//    }
//    var reader = new FileReader();

//    reader.onload = e => {
//        const img = document.createElement("img");

//        img.onload = e => {
//            console.log(e);
//        }

//        img.src = e.target.result;
//        const formData = new FormData();
//        const targetimg = convertImageToJpg(img);

//        formData.append("avatar", convertImageToJpg(img), "avatar.jpg");
//    }
//    reader.readAsDataURL(fileInput.files[0])

//});


//const canvas = document.createElement("canvas");
//canvas.width = loadedImageSize.width;
//canvas.height = loadedImageSize.height;
//function convertImageToJpg(image) {
//    canvas.getContext("2d").drawImage(image, 0, 0, image.width, image.height, 0, 0, canvas.width, canvas.height);
//    image.src = canvas.toDataURL();
//    return image;
//}

accountExitButton.addEventListener("click", e => {
    delete_cookie("token");
    location.href = "/Login";
});

function delete_cookie(name) {
    const regex = new RegExp(name + "=.*");
    const value = document.cookie.split(';').find(i => i.match(regex) != null);
    document.cookie = document.cookie.replace(value, value + ";max-age=-1");
}