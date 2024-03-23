import { PersonHeader } from "../../../js/components/view-person-page.jsx";

const re = new RegExp("[^/]+$");

const person = window.location.href.match(re);
    
ReactDOM.render(
    <PersonHeader personNick={person[0]}></PersonHeader>,
    document.getElementById("content")
);