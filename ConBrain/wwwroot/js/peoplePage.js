//Нахождение внешних опорных элементов
const peopleBody = document.getElementsByName("peopleBody")[0];
const search = document.getElementsByName("search")[0];

//Создание гифки загрузки
const loadImg = document.createElement("img");
loadImg.src = "/images/load.gif";
loadImg.classList.add("middleicon")

const step = 10;
const scrollOffset = 1;

ResetItems();

addEventListener("scroll", async(e) => {    
    //Загрузка данных с сервера
    let temp = 0
    while (canEndScroll(scrollOffset)) {
        temp = await UpdateItemsAsync(lastElementIndex, step);
        lastElementIndex += temp;
        if (temp < step)
            break;
    }
})

search.addEventListener("input", ResetItems);

//Сброс всех данных
async function ResetItems() {
    //Сбрасываем начальный элемент
    lastElementIndex = 0;
    //Удаляем все элементы
    while (peopleBody.firstChild) {
        peopleBody.removeChild(peopleBody.firstChild);
    }

    //Добавляем элементы пока они не закончатся или пока не достигнем конца экрана
    while (canEndScroll(scrollOffset)) {
        let len = await UpdateItemsAsync(lastElementIndex, step);
        if (len < step)
            break;
    }

}

//Добавление новых пользователей на сервер. Возвращает количество загруженных пользователей
async function UpdateItemsAsync(lastElementIndex, step) {
    //Добавляем анимацию загрузки данных с сервера
    peopleBody.appendChild(loadImg);
    //Запрашиваем данные с сервера
    const dates = await LoadPeopleByServer(lastElementIndex, step);
    
    //Удаляем анимацию загрузки данных с сервера
    peopleBody.removeChild(loadImg);
    
    //Добавляем загруженные данные в таблицу
    if (dates != null) {
        let res = 0;
        for (data of dates) {
            appendPersonInTable(data);
            res++;
        }
        return res
    }
    return 0;
}

//Функция загрузки пользователей с сервера
async function LoadPeopleByServer(lastElementIndex, step) {
    //Запрашиваем данные с сервера
    let pattern = search.value;
    const response = await fetch("/peopleList" + `?offset=${lastElementIndex}&size=${step}&pattern=${pattern}`, {
        method: "GET",
    });

    //получаем ответ
    if (response.ok === true)
        return await response.json();
    else
        return null;
}

//Функция добавление пользователя в список
function appendPersonInTable(data) {
    const person = document.createElement("tr");


    const avatarTd = document.createElement("td");
    const avatar = document.createElement("img");
    avatar.classList.add("avatar");
    avatar.src = "/avatars/";
    if (data.hasOwnProperty("avatarPath") && data.avatarPath != null)
        avatar.src += data.avatarPath;
    else
        avatar.src += "default.svg";

    avatarTd.appendChild(avatar)

    const nick = document.createElement("td")
    if (data.hasOwnProperty("nick"))
        nick.appendChild(document.createTextNode(data.nick));

    const name = document.createElement("td")
    if (data.hasOwnProperty("family") && data.hasOwnProperty("name"))
        name.appendChild(document.createTextNode(`${data.family} ${data.name}`));

    person.appendChild(avatarTd);
    person.appendChild(nick);
    person.appendChild(name);

    peopleBody.appendChild(person);
}

function canEndScroll(offset) {
    return scrollY + document.body.clientHeight + offset >= document.documentElement.scrollHeight;
}

