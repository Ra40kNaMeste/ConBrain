const settingsButton = document.getElementById("settings")
const addPersonButton = document.getElementById("addPerson");
const dialogName = document.getElementById("name").textContent;

const messageBody = document.getElementById("messagesBody");
const textInput = document.getElementById("text");
const sendButton = document.getElementById("send");

let messages = [];
let ids = [];

const count = 5;

setInterval(fetchMessages, 1000);

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
    const response = await fetch(`/dialog/${dialogName}/messages?start=0&count=${count}`, {
        method: "GET"
    });
    if (response.ok) {
        
        const responeMessages = await response.json();
        let ids = messages.slice();
        ids = ids.map(i => i.id);
        for (rmes of responeMessages) {
            if (!ids.includes(rmes.id))
                messages.push(rmes);
        }
        updateMessages();
    }
}
function updateMessages() {
    
    while (messageBody.firstChild)
        messageBody.removeChild(messageBody.firstChild);
    console.log(messages);
    for (message of messages) {
        const div = document.createElement("div");
        div.textContent = message.body;
        
        div.classList.add("message");
        messageBody.appendChild(div);
    }
}