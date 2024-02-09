const form = document.getElementsByName("loginform")[0];
const values = [].slice.call(document.getElementsByClassName("sendInput"));
async function saveToken(e) {
    e.preventDefault();
    const target = e.currentTarget;

    //Собираем тело для отправки
    var formBody = [];
    for (var value of values) {
        var encodedKey = encodeURIComponent(value.name);
        var encodedValue = encodeURIComponent(value.value);
        formBody.push(encodedKey + "=" + encodedValue);
    }
    formBody = formBody.join("&");
    console.log(formBody);
    //отправляем
    const response = await fetch(target.action, {
        method: target.method,
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: formBody
    });
    const data = await response.json();
    console.log(data);
    //получаем ответ
    if (response.ok === true) {
        document.cookie = "token = " + data.token;
        window.location.replace("../home")
    }
    else {
        if (data != null) {
            for (result of data) {
                for (member of result.memberNames) {
                    const box = values.find(i => i.name.toLowerCase() == str);
                    box.setCustomValidity(result.errorMessage);
                    box.reportValidity();
                }
            }
        }
    }
}
form.addEventListener("submit", saveToken);
