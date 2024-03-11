import { LoadingDatesList } from "../../../js/components/loading-dates-list.jsx";

function redirectToPerson(nick){
    window.location.href = `../id=${nick}`;
}

let builder = (o)=>
    <tr onClick={() => redirectToPerson(`${o.nick}`)}>
        <td>
            <img className="middleavatar" src={`/${o.nick}/image?key=${o.avatarPath}` } />
        </td>
        <td>{o.nick}</td>
        <td>{`${o.family} ${o.name}`}</td>
    </tr>;


let dates = [];
dates.push("one");
dates.push("two");

ReactDOM.render(
    <LoadingDatesList url="/peopleList?" step="5" offset="1" direction="Down" builder={ builder }></LoadingDatesList>,
    document.getElementById("content")
);