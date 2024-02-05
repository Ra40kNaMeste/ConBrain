//определение внешних элементов упрвления
const settingsButton = document.getElementById("settings")
const addPersonButton = document.getElementById("addPerson");
const dialogName = document.getElementById("name").textContent;

//определение внешних элементов отправки текста
const messageBody = document.getElementById("messagesBody");
const textInput = document.getElementById("text");
const sendButton = document.getElementById("send");

let firstMessageId; //самое первое (старое) сообщение
let lastMessageId; //самое последнее (новое) сообщение

const count = 10; //Количество загружаемых за раз сообщений
const scrollOffset = 0; //смещение полосы прокрутки для подгрузки: при перемещении полосы проктрутки в начало начинается подгрузка старых сообщений
const updateTime = 500; //время обновления страницы

//настройка отправки сообщения
sendButton.addEventListener("click", async e => {
    const response = await fetch(`/dialog/${dialogName}/messages?body=${textInput.value}`, {
        method: "POST"  
    });
    if (response.ok) {
        await updateMessage();
        window.scroll(0, document.documentElement.scrollHeight);
        textInput.value = "";
    }
});

//Изначальная подгузка сообщений и создание таймера для обновления
updateMessage();
setInterval(updateMessage, updateTime);

//Добавление подгрузки старых сообщений
addEventListener("scroll", async (e) => {
    if (canStartScroll(scrollOffset)) {
        console.log("loading");
        if (firstMessageId)
            await fetchMessages(`/dialog/${dialogName}/oldmessages?id=${firstMessageId.id}&count=${count}`, pushBackMessage);
    }
})

//Функция обновления сообщений: если ни одно сообщение не добавлено, то загружается массив. Далее - только новые сообщения
async function updateMessage() {
    if (!lastMessageId) {
        await fetchMessages(`/dialog/${dialogName}/messages?start=0&count=${count}`, pushBackMessage)
        window.scroll(0, document.documentElement.scrollHeight);
    }
    else
        await fetchMessages(`/dialog/${dialogName}/newmessages?id=${lastMessageId.id}`, pushFrontMessage)
}

//Функция отправки запроса и обновления параметров сообщений. Path - путь запроса, appendAction - функция для записи сообщения
async function fetchMessages(path, appendAction) {
    const response = await fetch(path, {
        method: "GET"
    });
    if (response.ok) {
        let responeMessages = await response.json();
        console.log(responeMessages);
        responeMessages.forEach(i => {
            i.dateTime = new Date(i.dateTime)
            if (!firstMessageId || i.dateTime < firstMessageId.dateTime)
                firstMessageId = i;
            if (!lastMessageId || i.dateTime > lastMessageId.dateTime)
                lastMessageId = i;
        })

        console.log(path);
        console.log(responeMessages)
        for (rmes of responeMessages) {
            appendAction(rmes);
        }
    }
}

//Вставляет сообщение в конец списка (вниз)
function pushFrontMessage(message) {
    var block = generateBodyMessage(message);
    messageBody.appendChild(block);
}

//Вставляет сообщение в начало списка (наверх)
function pushBackMessage(message) {
    var block = generateBodyMessage(message);
    messageBody.insertBefore(block, messageBody.firstChild);
}

//генерирует тело сообщения
function generateBodyMessage(message) {
    const datetime = document.createElement("div");
    datetime.classList.add("dates");
    const date = new Date(message.dateTime);
    datetime.textContent = `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;

    const messageDiv = document.createElement("div");
    messageDiv.textContent = message.body;
    messageDiv.classList.add("message");

    const messageBlock = document.createElement("div");
    messageBlock.classList.add("messageblock")
    messageBlock.appendChild(datetime);
    messageBlock.appendChild(messageDiv);

    const avatar = document.createElement("div");
    avatar.classList.add("avatar");
    avatar.textContent = message.sender;

    const root = document.createElement("div");
    root.classList.add("rootmessageblock");
    root.appendChild(messageBlock);
    root.appendChild(avatar);
    return root;
}

//Условие начала прокрутки
function canStartScroll(offset) {
    return scrollY <= offset;
}