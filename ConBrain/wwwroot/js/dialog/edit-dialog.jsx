import { FormTable, FormTableItem, FormTableAppendFriendItem } from "../../../../js/components/default-components/form-table.jsx"

class DialogEditor extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            dialog: {
                "name": "",
                "members": []
            },
            person: {
                id: null
            }
        }
        this.loadDialog();
        this.loadPerson();
    }

    async loadDialog() {
        const response = await fetch("./get");
        if (response.ok === true) {
            
            const dialog = await response.json();
            this.setState({ dialog: dialog });
        }
    }

    async loadPerson() {
        const response = await fetch("/authperson");
        if (response.ok === true) {
            const person = await response.json();
            this.setState({ person: person });
        }
    }

    render() {
        return <div className="columnnowrapstackpanel scrollDiv">
            <FormTable action="./edit" method="POST" name="editDialog" caption="Edit dialog" sendContent="Apply">
                <FormTableItem name="Name" value={this.state.dialog.name} property="name" type="text" min-length="5" isSend="true" />
                <FormTableAppendFriendItem name="Members" value={this.state.dialog.members.map(i=>i.data).filter(i => i.id != this.state.person.id)} property="people" isSend="true" />
            </FormTable>
        </div>
    }
}

ReactDOM.render(
    <DialogEditor/>,
    document.getElementById("content")
);