const dialogs = document.getElementsByClassName("dialog");


for (dialog of dialogs) {
    console.log(dialog.textContent);
    const value = dialog.textContent;
    dialog.addEventListener("click", e => {
        window.location.href = `dialog/${value}`;
    });
}