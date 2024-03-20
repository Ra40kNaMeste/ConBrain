import "../../../node_modules/@microsoft/signalr/dist/browser/signalr.js";
import { LoadingDatesList } from "./../components/loading-dates-list.jsx"

//определение внешних элементов упрвления
const dialogName = document.getElementById("title").textContent;

class Dialog extends React.Component {
    constructor(props) {
        super(props);

        this.messageList = React.createRef();
        this.textInput = React.createRef();

        this.#hubConnection = this.buildHubConnection();

        this.state = {
            avatarPath: ""
        };

        this.bindDialogHub();
        this.setCurrentPerson();
        this.startHub(this.#hubConnection);
    }

    #hubConnection;

    buildHubConnection(){
        return new signalR.HubConnectionBuilder()
            .withUrl("../../message")
            .build();
    }

    async startHub(hubConnection) {
        //Подключение к диалогу
        await hubConnection.start();
        await hubConnection.invoke("Subscribe", this.props.dialogName);
    }

    async setCurrentPerson() {
        const response = await fetch("./../../authperson");
        

        if (response.ok === true) {
            const person = await response.json();
            this.setState({
                avatarPath: person.avatarPath
            });
        }
    }

    async sendMessage() {
        if (this.textInput.current.value === "")
            return;

        await this.#hubConnection.invoke("Send", this.textInput.current.value, this.props.dialogName);
        this.textInput.current.value = "";
    }

    bindDialogHub() {
        //Настройка приёма сообщений
        this.#hubConnection.on("Message", mess => {
            this.messageList.current.push([mess]);
        });
    }
    render() {
        const builder = o => <div className="rootmessageblock">
                <div className="rownowrapstackpanel">
                    <img className="smallavatar" src={`./../image?key=${o.sender.avatarPath}&person=${o.sender.nick}`} />
                    <p>{o.nick}</p>
                </div>
                    <div>{o.body}</div>
            </div>

        return <div className="fullSize">
            <LoadingDatesList className="dialogdiv" ref={this.messageList} url={`./../dialog/${this.props.dialogName}/messages?`} step={this.props.step} offset={this.props.offset} builder={builder} direction="Top" >
            </LoadingDatesList>

            <div className="dialogcommandsdiv rownowrapstackpanel">
                <img className="smallavatar" src={`./../image?key=${this.state.avatarPath}`}/>
                <input ref={this.textInput} className="valueForm" id="text" type="text" />
                <img className="smallicon sendbutton" onClick={()=>this.sendMessage()} id="send" src="/images/arrow.svg"></img>
                <img id="settings" src="/images/settings.svg" className="middleicon"></img>
                <img id="addPerson" src="/images/add_person.svg" className="middleicon"></img>
            </div>
        </div>

    }
}

ReactDOM.render(
    <Dialog dialogName={dialogName} step="5" offset="0"/>, document.getElementById("content")
);