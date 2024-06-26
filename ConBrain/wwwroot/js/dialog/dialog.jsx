﻿import "../../../node_modules/@microsoft/signalr/dist/browser/signalr.js";
import { LoadingDatesList } from "./../components/loading-dates-list.jsx"
import { Avatar } from "./../components/default-components/avatar.jsx"

//определение внешних элементов упрвления
const dialogName = document.getElementById("title").textContent;

class Dialog extends React.Component {
    constructor(props) {
        super(props);

        this.messageList = React.createRef();
        this.textInput = React.createRef();

        this.#hubConnection = this.buildHubConnection();

        this.state = {
            person: {
                avatarId:""
            }
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
                person: person
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
                <Avatar className="smallavatar" avatar={o.sender.data.avatarId} onClick={() => window.location.href = `./../id=${o.sender.nick}`}/>
                    <p>{o.nick}</p>
                </div>
                    <div>{o.body}</div>
            </div>

        const keyPressHandler = e => {
            if (e.key == "Enter" && !e.shiftKey && !e.altKey)
                this.sendMessage();
        } 

        return <div className="fullSize">
            <LoadingDatesList className="dialogdiv" ref={this.messageList} url={`./../dialog/${this.props.dialogName}/messages?`} step={this.props.step} offset={this.props.offset} builder={builder} direction="Top" >
            </LoadingDatesList>

            <div className="dialogcommandsdiv rownowrapstackpanel">
                <Avatar className="smallavatar" avatar={this.state.person.avatarId}/>
                <input ref={this.textInput} autoFocus className="inputTextBox" id="text" type="text" onKeyDown={keyPressHandler} />
                <img className="smallicon sendbutton" onClick={()=>this.sendMessage()} id="send" src="/images/arrow.svg"></img>
                <img id="settings" src="/images/settings.svg" className="middleicon" onClick={()=>window.location.href+="/edit"}></img>
                <img id="addPerson" src="/images/add_person.svg" className="middleicon"></img>
            </div>
        </div>

    }
}

ReactDOM.render(
    <Dialog dialogName={dialogName} step="5" offset="0"/>, document.getElementById("content")
);