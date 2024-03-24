import { LoadingDatesList } from "../../../js/components/loading-dates-list.jsx";

const re = new RegExp("[^/]+$");
const person = window.location.href.match(re);

function redirectToPerson(nick) {
    window.location.href = `../${nick}`;
}

let builder = (o) =>
    <div onClick={() => redirectToPerson(`${o.nick}`)} className="rowwrapstackpanel">
        <img className="middleavatar" src={`/${o.nick}/image?key=${o.avatarPath}`} />
        <p className="textBlock">{o.nick}</p>
        <p className="textBlock">{`${o.family} ${o.name}`}</p>
    </div>;

ReactDOM.render(
    <LoadingDatesList className="fullSize" url={`./../friends?nick=${person[0]}&`} step="5" offset="1" direction="Down" builder={builder}></LoadingDatesList>,
    document.getElementById("content")
);