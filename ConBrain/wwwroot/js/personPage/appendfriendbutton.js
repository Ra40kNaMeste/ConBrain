const area = document.getElementById("addFriendArea");
const currentPerson = window.location.href.match("id=(?<name>\\w+)").groups["name"];
UpdateFriendButton();

class ButtonBuilder {
    constructor(id, content, onclick) {
        this.#id = id;
        this.#onclick = onclick;
        this.#content = content;
    }
    #id; 
    #onclick;
    #content;

    #button;
    getButton() {
        if (this.#button == null) {
            this.#button = document.createElement("button");
            this.#button.id = this.#id;
            this.#button.addEventListener("click", this.#onclick);
            this.#button.appendChild(document.createTextNode(this.#content))
        }
        return this.#button;
    }
}

const addBtn = new ButtonBuilder("addFriendButton", "append on friends", async e => {
    const response = await fetch(`/person/friends?nick=${currentPerson}`, {
        method: "PUT",
    });
    if (response.ok === true)
        await UpdateFriendButton();
    else
        AppendErrorMessage("Unknow error add friend");
})

const removeBtn = new ButtonBuilder("removeFriendButton", "remove from friends", async e => {
    const response = await fetch(`/person/friends?nick=${currentPerson}`, {
        method: "DELETE",
        body: currentPerson
    });
    if (response.ok === true)
        await UpdateFriendButton();
    else
        AppendErrorMessage("Unknow error remove friend");
})

async function UpdateFriendButton() {
    //Удаляем все элементы
    while (area.firstChild) {
        area.removeChild(area.firstChild);
    }
    const response = await fetch("/person/friends", {
        method: "GET"
    });
    if (response.ok === true) {
        var data = await response.json();
        let btn = data.includes(currentPerson) ? removeBtn.getButton() : addBtn.getButton();
        area.appendChild(btn);
    }
}
function AppendErrorMessage(message) {
    while (area.firstChild != area.lastChild)
        area.removeChild(area.lastChild);
    const mess = document.createElement("p");
    mess.textContent = message;
    area.appendChild(mess);
}