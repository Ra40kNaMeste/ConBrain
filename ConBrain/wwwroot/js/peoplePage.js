const peopleBody = document.getElementsByName("peopleBody")[0];
const search = document.getElementsByName("search")[0];
const step = 20;

let lastElementIndex = 0;
console.log(search.value);
UpdateItems();

async function UpdateItems() {
    let pattern = search.value;
    console.log('patte = ' + pattern);
    const response = await fetch("/peopleList" + `?offset=${lastElementIndex}&size=${step}&pattern=${pattern}`, {
        method: "GET",
    });
    //получаем ответ
    if (response.ok === true) {
        const data = await response.json();
        lastElementIndex += step;
        for (item of data) {
            const person = document.createElement("tr");
            

            const avatarTd = document.createElement("td");
            const avatar = document.createElement("img");
            avatar.classList.add("avatar");
            avatar.src = "/avatars/";
            if (item.hasOwnProperty("avatarPath") && item.avatarPath != null)
                avatar.src += item.avatarPath;
            else
                avatar.src += "default.svg";
           
            avatarTd.appendChild(avatar)
            
            const nick = document.createElement("td")
            if (item.hasOwnProperty("nick"))
                nick.appendChild(document.createTextNode(item.nick));
                
            const name = document.createElement("td")
            if (item.hasOwnProperty("family") && item.hasOwnProperty("name"))
                name.appendChild(document.createTextNode(`${item.family} ${item.name}`));

            person.appendChild(avatarTd);
            person.appendChild(nick);
            person.appendChild(name);

            peopleBody.appendChild(person);
        }
    }
}