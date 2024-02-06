const peopleBodies = document.getElementsByClassName("peopleBody");

peopleBodies.forEach(body => {
    for (tbody of body.children) {
        for (row of tbody.children) {
            row.addEventListener("click", (e) => onPersonClick(row.children[1].innerText));
        }
        
    }
})
function onPersonClick(nick) {
    window.location.href = `../id=${nick}`;
}