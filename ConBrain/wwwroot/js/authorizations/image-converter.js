

export function convertImageToJpg(image) {
    const canvas = document.createElement("canvas");
    canvas.width = 200;
    canvas.height = 200;

    const context = canvas.getContext("2d");
    context.drawImage(image, 0, 0, image.width, image.height, 0, 0, canvas.width, canvas.height);
    return canvas.toDataURL("image/jpeg").replace(/^data:image\/jpeg;base64,/, "");
}