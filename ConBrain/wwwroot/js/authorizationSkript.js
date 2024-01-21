﻿const form = document.getElementsByName("loginform")[0];
const values = document.getElementsByClassName("sendInput");
console.log('Hello world!')
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

    //отправляем
    const response = await fetch(target.action, {
        method: target.method,
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: formBody
    });
    //получаем ответ
    if (response.ok === true) {
        const data = await response.json();
        document.cookie = "nick = " + data.nick;
        document.cookie = "token = " + data.token;
    }
    else {

    }
}
form.addEventListener("submit", saveToken);
