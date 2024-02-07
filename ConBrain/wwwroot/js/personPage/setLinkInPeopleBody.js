const peopleBodies = document.getElementsByClassName("peopleBody")[0];
peopleBodies.childNodes.forEach(tbody => {
    if (tbody.nodeName != "TBODY")
        return;
    for (row of tbody.children) {
        row.addEventListener("click", (e) => onPersonClick(row.children[1].innerText));
    }
})
function onPersonClick(nick) {
    window.location.href = `../id=${nick}`;
}