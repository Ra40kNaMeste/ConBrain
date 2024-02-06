//определение внешних элементов упрвления
const settingsButton = document.getElementById("settings")
const addPersonButton = document.getElementById("addPerson");
const dialogName = document.getElementById("title").textContent;
const contentDiv = document.getElementById("mainDiv");

//определение внешних элементов отправки текста
const messageBody = document.getElementById("messagesBody");
const textInput = document.getElementById("text");
const sendButton = document.getElementById("send");

let firstMessageId; //самое первое (старое) сообщение
let lastMessageId; //самое последнее (новое) сообщение

const count = 20; //Количество загружаемых за раз сообщений
const scrollOffset = 0; //смещение полосы прокрутки для подгрузки: при перемещении полосы проктрутки в начало начинается подгрузка старых сообщений
const updateTime = 500; //время обновления страницы

class PersonManager {
    constructor() {
        this.#personData = new Map();
    }
    #personData;
    async #loadpersonfromserver(nick) {
        const response = await fetch(`/person?nick=${nick}`, {
            method: "GET"
        });
        if (response.ok === true) {
            const person = await response.json();
            
            let path = "../avatars/";
            console.log(path);
            path += person.avatarPath != null ? person.avatarPath : "default.svg";
            console.log(path);
            this.#personData.set(nick, path);
            return path;
        }
        return null;
    }

    async getPerson(nick) {
        const res = this.#personData.get(nick);
        if (res == null)
            return await this.#loadpersonfromserver(nick);
        return res;
    }
}

//Менеджер пользователей
const personManager = new PersonManager();


//настройка отправки сообщения
sendButton.addEventListener("click", async e => {
    awaitsendMessage();
});

textInput.addEventListener("keypress", async e => {
    if (e.key == "Enter")
        await sendMessage();
})



//Изначальная подгузка сообщений и создание таймера для обновления
updateMessage();
setInterval(updateMessage, updateTime);

//Добавление подгрузки старых сообщений
contentDiv.addEventListener("scroll", async (e) => {
    if (canStartScroll(scrollOffset)) {
        const oldScrollHeight = contentDiv.scrollHeight;
        if (firstMessageId)
            await fetchMessages(`/dialog/${dialogName}/oldmessages?id=${firstMessageId.id}&count=${count}`, pushBackMessage);
        contentDiv.scroll(0, contentDiv.scrollHeight - oldScrollHeight);
    }
})

//Функция отправки сообщения на сервер
async function sendMessage() {
    if (textInput.value.length == 0)
        return;
    const response = await fetch(`/dialog/${dialogName}/messages?body=${textInput.value}`, {
        method: "POST"
    });
    if (response.ok) {
        await updateMessage();
        contentDiv.scroll(0, document.documentElement.scrollHeight);
        textInput.value = "";
    }
}

//Функция обновления сообщений: если ни одно сообщение не добавлено, то загружается массив. Далее - только новые сообщения
async function updateMessage() {
    if (!lastMessageId) {
        await fetchMessages(`/dialog/${dialogName}/messages?start=0&count=${count}`, pushBackMessage)
        contentDiv.scroll(0, contentDiv.scrollHeight);
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
        responeMessages.forEach(i => {
            i.dateTime = new Date(i.dateTime)
            if (!firstMessageId || i.dateTime < firstMessageId.dateTime)
                firstMessageId = i;
            if (!lastMessageId || i.dateTime > lastMessageId.dateTime)
                lastMessageId = i;
        })
        for (rmes of responeMessages) {
            await appendAction(rmes);
        }
    }
}

//Вставляет сообщение в конец списка (вниз)
async function pushFrontMessage(message) {
    var block = await generateBodyMessage(message);
    console.log(block);
    messageBody.appendChild(block);
}

//Вставляет сообщение в начало списка (наверх)
async function pushBackMessage(message) {
    var block = await generateBodyMessage(message);
    console.log(block);
    messageBody.insertBefore(block, messageBody.firstChild);
}

//генерирует тело сообщения
async function generateBodyMessage(message) {
    const avatar = document.createElement("img");
    avatar.classList.add("smallavatar");
    console.log(message);
    avatar.src = await personManager.getPerson(message.sender);

    const nick = document.createElement("p");
    nick.classList.add("nick");
    nick.textContent = message.sender;

    const datetime = document.createElement("div");
    datetime.classList.add("dates");
    const date = new Date(message.dateTime);
    datetime.textContent = `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;

    const metadataBlock = document.createElement("div");
    metadataBlock.classList.add("rowstackpanel")
    metadataBlock.appendChild(avatar);
    metadataBlock.appendChild(nick);
    metadataBlock.appendChild(datetime);

    const messageBlock = document.createElement("div");
    messageBlock.textContent = message.body;
    messageBlock.classList.add("message");
 
    const root = document.createElement("div");
    root.classList.add("rootmessageblock");
    root.appendChild(metadataBlock);
    root.appendChild(messageBlock);
    return root;
}

//Условие начала прокрутки
function canStartScroll(offset) {
    return contentDiv.scrollTop <= offset;
}

