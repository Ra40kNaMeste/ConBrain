const nameDialog = document.getElementsByName("dialogName")[0]; //поле ввода имени диалога
const memberBody = document.getElementById("membersBody"); //Блок для записи списка собеседников
const addMemberButton = document.getElementById("appendFriend"); //Кнопка для добавления собеседника
const createButton = document.getElementById("create"); //кнопка создания диалога

const members = []; //Собеседники (ники)
const memberList = document.createElement("div"); //Список друзей

fillMemberList(); //Заполнение списка друзей

//Настройка кнопки addMemberButton
addMemberButton.parentElement.insertBefore(memberList, addMemberButton)
memberList.classList.add("unshow");
addMemberButton.classList.add("show");

addMemberButton.addEventListener("click", e => {
    memberList.classList.remove("unshow");
    memberList.classList.add("show");

    addMemberButton.classList.remove("show");
    addMemberButton.classList.add("unshow");
})

//Настройка кнопки создания диалога
createButton.addEventListener("click", async e => {
    if (await fetchDialog())
        window.location.href = `./${nameDialog.textContent}`;
    else
        window.location.href = `../../error?message=Oops. Error create dialog. Maybe dialog with there name already exists`
});

//Функция для добавления собеседника
function addMember(member) {
    members.push(member.value);
    memberBody.insertBefore(createMemberElement(member), memberList);
    memberList.removeChild(member);
}

//Функция для удаления собеседника
function removeMember(member, block) {
    removeElement(members, member.value);
    memberBody.removeChild(block);
    memberList.appendChild(member);
}

function removeElement(list, element) {
    const index = list.indexOf(element);
    if (index != -1)
        list.splice(index, 1);
}

//Функция отправки нового диалога на сервер
async function fetchDialog() {
    fetchStr = `/dialogs/build?name=${nameDialog.value}`;
    for (friend of members) {
        fetchStr += `&people=${friend}`;
    }
    const response = await fetch(fetchStr, {
        method: "POST",
    });
    return response.ok === true;
}

//Функция для заполнения выпадающего списка друзей
async function fillMemberList() {
    let friends = [];
    const response = await fetch(`/person/friends`, {
        method: "GET"
    });
    if (response.ok === true) {
        friends = await response.json();
        for (friend of friends) {
            const member = document.createElement("p");
            member.value = friend;
            member.textContent = friend;
            member.classList.add("sublistelement");
            member.addEventListener("click", (e) => {
                addMember(member);
                memberList.classList.remove("show");
                memberList.classList.add("unshow");
                
                if (memberList.children.length != 0) {
                    addMemberButton.classList.remove("unshow");
                    addMemberButton.classList.add("show");
                }
                    
            });
            memberList.appendChild(member);
        }
    }
}
function createMemberElement(member) {
    const div = document.createElement("div");
    div.classList.add("rowForm");

    const name = document.createElement("p");
    name.classList.add("centralText");
    name.textContent = member.value;

    //Добавление кнопки удаления
    const deleteBtn = document.createElement("button");
    deleteBtn.textContent = "x";
    deleteBtn.classList.add("removeButton");
    deleteBtn.addEventListener("click", e => {
        removeMember(member, div);
        addMemberButton.classList.add("show");
    })

    div.appendChild(name);
    div.appendChild(deleteBtn);
    return div;
}