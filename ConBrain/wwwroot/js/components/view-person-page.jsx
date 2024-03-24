export class PersonHeader extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            person: {
                "nick": "",
                "name": "",
                "family": "",
                "secondName": "",
                "phone": "",
                "avatarPath": ""
            },
            friends: []
        }
        this.loadPerson();
        this.loadFriends();
    }

    async loadPerson() {
        let authPerson;
        let authFriends;
        const authresponse = await fetch("./../authperson");
        if (authresponse.ok === true) {
            authPerson = await authresponse.json();
            authFriends = await this.loadFriends(authPerson);
        }

        let person;
        let personFriends;
        if (this.props.personNick) {
            const response = await fetch(`./../person?nick=${this.props.personNick}`);
            if (response.ok === true) {
                person = await response.json();
                personFriends = await this.loadFriends(person);
            }
        }

        let isEdit = authPerson && person && authPerson.nick == person.nick;
        if (person == undefined)
            isEdit = true;

        this.setState({
            person: person ? person : authPerson,
            friends: person ? personFriends : authFriends,
            isFriendControl: authFriends && person && authPerson.nick != person.nick ? authFriends.find(i => i.nick == person.nick) != undefined : undefined,
            isEdit: isEdit
        });

        
    }

    async loadFriends(person, friend) {
        if (!person)
            return;
        const response = await fetch(`./../${person.nick}/friends`, {
            method:"GET"
        });
        if (response.ok === true) {
            const friends = await response.json();
            return friends;
        }
        return undefined;
    }

    #addremoveFlag = false;

    async goodButtonOnClick() {
        if (this.#addremoveFlag === true)
            return
        this.#addremoveFlag = true;
        const response = await fetch(`/friends?nick=${this.state.person.nick}`, {
            method: "PUT",
        });
        if (response.ok === true) {
            this.setState({ isFriendControl: true });
        }
        this.#addremoveFlag = false;
    }

    async poorlyButtonOnClick() {
        if (this.#addremoveFlag === true)
            return
        this.#addremoveFlag = true;
        const response = await fetch(`/friends?nick=${this.state.person.nick}`, {
            method: "DELETE",
        });
        if (response.ok === true) {
            this.setState({ isFriendControl: false });
        }
        this.#addremoveFlag = false;
    }

    getFrinedsString() {
        return this.state.friends.length == 1 ? "friend" : "friends";
    }

    render() {
        const editButton = this.state.isEdit ? <a href="./edit">edit</a> : undefined;

        const friendControl = this.state.isFriendControl != undefined ? <div className="rowstretchstackpanel">
            {this.state.isFriendControl == false ? <button className="goodButton" onClick={() => this.goodButtonOnClick()}>add to friends</button> :
                <button className="poorlyButton" onClick={() => this.poorlyButtonOnClick()}>remove from friends</button>}
        </div> : undefined;

        return <div className="fullSize">
            <div className="columnnowrappanel scrollDiv content">
                <div className="rownowrapstackpanel">
                    <img src={`./${this.state.person.nick}/image?key=${this.state.person.avatarPath}`} className="bigavatar"></img>
                    <div className="columnnowrappanel">
                        <div className="rowstretchstackpanel">
                            <p>{this.state.person.nick}</p>
                            {editButton}
                        </div>
                        <h2>{`${this.state.person.family} ${this.state.person.name} ${this.state.person.secondName}`}</h2>
                        <div className="rownowrapstackpanel">
                            <p>{`${this.state.friends.length} ${this.getFrinedsString()}: `}</p>
                            {this.state.friends.slice(0, 3).map(i => <img className="smallavatar" src={`./${i.nick}/image?key=${i.avatarPath}`}></img>)}
                            <button onClick={() => window.location.href = `./friends/${this.state.person.nick}`}>more</button>
                        </div>
                    </div>
                </div>
            </div>
            {friendControl}
            <div className="rowstretchstackpanel">
            </div>
        </div>

    }
}
