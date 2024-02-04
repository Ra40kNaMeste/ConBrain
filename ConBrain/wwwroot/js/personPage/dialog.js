const settingsButton = document.getElementById("settings")
const addPersonButton = document.getElementById("addPerson");
const dialogName = document.getElementById("name").textContent;

const messageBody = document.getElementById("messagesBody");
const textInput = document.getElementById("text");
const sendButton = document.getElementById("send");

const pattern = new RegExp("^(?<year>\w+)-<?{month}\w+>")

let messages = [];
let firstMessageId;s

const count = 10;

setInterval(fetchMessages, 500);

sendButton.addEventListener("click", async e => {
    console.log(`dialog/${dialogName}/messages?body=${textInput.value}`);
    const response = await fetch(`/dialog/${dialogName}/messages?body=${textInput.value}`, {
        method: "POST"
    });
    if (response.ok) {
        fetchMessages();
    }
});

async function fetchMessages() {
    const fetchStr = firstMessageId ? `/dialog/${dialogName}/messages?id=${firstMessageId}` : `/dialog/${dialogName}/messages?start=0&count=${count}`;
    const response = await fetch(fetchStr, {
        method: "GET"
    });
    if (response.ok) {
        
        const responeMessages = await response.json();
        for (rmes of responeMessages) {

            insertMessage(rmes, 0);
            firstMessageId = rmes.id;
        }
    }
}
function generateBodyMessage(message) {
    const root = document.createElement("div");
    const messageBlock = document.createElement("div");

    
    const datetime = document.createElement("div");
    datetime.classList.add("dates");

    const message = document.createElement("div");
    div.textContent = message.body;
    div.classList.add("message");
    messageBody.appendChild(div);
}

function convertDateTime(datetime) {

}