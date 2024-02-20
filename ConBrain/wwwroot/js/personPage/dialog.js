//import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import "../../../node_modules/@microsoft/signalr/dist/browser/signalr.js";

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

const count = 20; //Количество загружаемых за раз сообщений
const scrollOffset = 0; //смещение полосы прокрутки для подгрузки: при перемещении полосы проктрутки в начало начинается подгрузка старых сообщений
const timeoutloading = 5000;

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
            console.log(person)
            let path = `../${person.nick}/image?key=${person.avatarPath}`;
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

const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("../../message")
    .build();

//Настройка приёма сообщений
hubConnection.on("Message", async mess =>{
    await pushFrontMessage(mess);
    contentDiv.scroll({
        top: contentDiv.scrollHeight,
        behavior: "instant"
    });
});

await startHub(hubConnection);

//Подключение к диалогу
await hubConnection.invoke("Subscribe", dialogName);

//настройка отправки сообщения
sendButton.addEventListener("click", async e => {
    await sendMessage();
});

textInput.addEventListener("keypress", async e => {
    if (e.key == "Enter")
        await sendMessage();
})
//Настройка отправки сообщений

updateMessage();

//Добавление подгрузки старых сообщений
contentDiv.addEventListener("scroll", async (e) => {
    if (canStartScroll(scrollOffset)) {
        const oldScrollHeight = contentDiv.scrollHeight;
        if (firstMessageId)
            await fetchMessages(`/dialog/${dialogName}/oldmessages?id=${firstMessageId.id}&count=${count}`, pushBackMessage);
        contentDiv.scroll({
            top: contentDiv.scrollHeight - oldScrollHeight,
            behavior: "instant"
        });
    }
})


async function updateMessage() {
    await fetchMessages(`/dialog/${dialogName}/messages?start=0&count=${count}`, pushBackMessage)
    contentDiv.scroll({
        top: contentDiv.scrollHeight,
        behavior: "instant"
    });
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
        })
        for (let rmes of responeMessages) {
            await appendAction(rmes);
        }
    }
}

//Вставляет сообщение в конец списка (вниз)
async function pushFrontMessage(message) {
    var block = await generateBodyMessage(message);
    messageBody.appendChild(block);
}

//Вставляет сообщение в начало списка (наверх)
async function pushBackMessage(message) {
    var block = await generateBodyMessage(message);
    messageBody.insertBefore(block, messageBody.firstChild);
}

//генерирует тело сообщения
async function generateBodyMessage(message) {
    const avatar = document.createElement("img");
    avatar.classList.add("smallavatar");
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

async function sendMessage(){
    if (textInput.value === "")
        return;

    await hubConnection.invoke("Send", textInput.value, dialogName);
    textInput.value = "";
}

async function startHub(hubConnection) {
    await hubConnection.start()
        .catch((p) => {
            const message = document.createElement("p");
            message.textContent = "Error loading messages. Start reload..";
            messageBody.appendChild(message);
            setTimeout(() => {
                messageBody.removeChild(message);
                startHub(hubConnection);
            }, timeoutloading)
        });
}

//Условие начала прокрутки
function canStartScroll(offset) {
    return contentDiv.scrollTop <= offset;
}

