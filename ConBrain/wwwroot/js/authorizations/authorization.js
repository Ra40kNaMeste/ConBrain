export async function fetchWithAddressString(target, values) {
    //Собираем тело для отправки
    var formBody = [];
    for (var value of values) {
        console.log(value.name);
        console.log(value.value);
        var encodedKey = encodeURIComponent(value.name);
        var encodedValue = encodeURIComponent(value.value);
        formBody.push(encodedKey + "=" + encodedValue);
    }
    formBody = formBody.join("&");
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

