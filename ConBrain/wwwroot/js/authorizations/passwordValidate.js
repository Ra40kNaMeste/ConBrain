function validate(e) {
    const secondPassword = document.getElementsByName("repeatpassword")[0];

    if (secondPassword.value != document.getElementsByName("password")[0].value) {
        secondPassword.value = "";
        secondPassword.setAttribute("class", "incorrectInput");
        e.preventDefault();
    }
    
}

const sendBtn = document.getElementsByName("send")[0];

sendBtn.addEventListener("click", validate);
