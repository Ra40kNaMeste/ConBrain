import { FormTable, FormTableItem, FormTableSelectionItem } from "./../../../js/components/default-components/form-table.jsx"
export class ImageEdit extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        const securityValues = [
            { key: 0, value: "public" },
            { key: 1, value: "only friends" },
            { key: 2, value: "private" },
        ];
        console.log(this.props.image.id);
        return <div className="backgroundPresentContent" onClick={() => this.props.onExit()}>
            <div className="fullSize presentContent" onClick={e=>e.stopPropagation()}>
                <img className="fullimage" src={`/image?id=${this.props.image.id}`}></img>
                <FormTable action={`/image/edit?id=${this.props.image.id}`} method="POST" name="edit" caption="Edit image" sendContent="Change" onSend={() => this.props.onExit()}>
                    <FormTableItem name="Name" value={this.props.image.name} property="name" type="text" min-length="5" isSend />
                    <FormTableItem name="Description" value={this.props.image.description} property="description" type="text" min-length="5" isSend />
                    <FormTableSelectionItem name="Security" value={this.props.image.securityLevel} property="level" values={securityValues} isSend />
                </FormTable>
            </div>
        </div>

    }
}