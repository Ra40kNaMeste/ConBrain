import { LoadingDatesList } from "./../components/loading-dates-list.jsx"

const builder = o => <div className="dialog" onClick={() => window.location.href = `./dialog/${o.name}`}>{o.name}</div>;

ReactDOM.render(
    <div className="fullSize">
        <a className="dialogcommandsdiv stretchbutton" href="dialogs/build">Create dialog</a>
        <LoadingDatesList className="dialogdiv" url={`./../dialogs/get?`} step="10" offset="0" builder={builder} direction="Down"/>
    </div>, document.getElementById("content")
);
