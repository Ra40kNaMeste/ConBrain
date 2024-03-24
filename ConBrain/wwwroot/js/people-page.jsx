import { LoadingDatesList } from "../../../js/components/loading-dates-list.jsx";

function redirectToPerson(nick){
    window.location.href = `../${nick}`;
}

let builder = (o)=>
    <div onClick={() => redirectToPerson(`${o.nick}`)} className="rowwrapstackpanel">
        <img className="middleavatar" src={`/${o.nick}/image?key=${o.avatarPath}` } />
        <p className="textBlock">{o.nick}</p>
        <p className="textBlock">{`${o.family} ${o.name}`}</p>
    </div>;

ReactDOM.render(
    <LoadingDatesList isShowSearch className="fullSize" url="/peopleList?" step="5" offset="1" direction="Down" builder={ builder }></LoadingDatesList>,
    document.getElementById("content")
);