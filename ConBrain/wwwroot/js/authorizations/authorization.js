export async function fetchWithAddressString(e) {
    const values = [].slice.call(document.getElementsByClassName("sendInput"));
    const target = e.currentTarget;

    //Собираем тело для отправки
    var formBody = [];
    for (var value of values) {
        var encodedKey = encodeURIComponent(value.name);
        var encodedValue = encodeURIComponent(value.value);
        formBody.push(encodedKey + "=" + encodedValue);
    }
    formBody = formBody.join("&");
    console.log(formBody)
    //отправляем
    const response = await fetch(target.action, {
        method: target.method,
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: formBody
    });
    return response;
}
export async function saveToken(response) {
    //получаем ответ
    if (response.ok === true) {
        const data = await response.json();
        document.cookie = "token = " + data.token;
        window.location.replace("../home")
        return true;
    }
    return false;
}

