//Загрузка элементов со страницы
const fileInput = document.getElementById("fileInput");
const loadAvatarButton = document.getElementById("loadAvatarButton");
const accountExitButton = document.getElementById("accountExit");

const loadedImageSize = new Object();
loadedImageSize.width = 200;
loadedImageSize.height = 200;

//Настройка загрузки аватара
fileInput.addEventListener("change", e => {
    if (fileInput.files.length == 0) {
        return;
    }
    for (prop in fileInput.files[0]) {

    }
    var reader = new FileReader();

    reader.onload = rev => {
        
        const img = document.createElement("img");
        img.onload = async () => {
            const formData = new FormData();
            const targetimg = convertImageToJpg(img);
            let blobImg = new Blob([targetimg], { type: "image/jpg" });
            //const source = new File(targetimg, "avatar.jpg");
            
            formData.append("file", blobImg, "avatar.jpg");
            formData.append("key", "avatar.jpg");
            const response = await fetch("./image", {
                method: "POST",
                body: formData
            });
            if (response.ok === true) {
                window.location.reload();
            }
        }
        img.src = rev.target.result;
    }
    reader.readAsDataURL(fileInput.files[0])
});


const canvas = document.createElement("canvas");
canvas.width = loadedImageSize.width;
canvas.height = loadedImageSize.height;
function convertImageToJpg(image) {
    const context = canvas.getContext("2d");
    context.drawImage(image, 0, 0, image.width, image.height, 0, 0, canvas.width, canvas.height);
    return canvas.toDataURL("image/jpeg").replace(/^data:image\/jpeg;base64,/, "");
}

accountExitButton.addEventListener("click", e => {
    delete_cookie("token");
    location.href = "/Login";
});

function delete_cookie(name) {
    const regex = new RegExp(name + "=.*");
    const value = document.cookie.split(';').find(i => i.match(regex) != null);
    document.cookie = document.cookie.replace(value, value + ";max-age=-1");
}