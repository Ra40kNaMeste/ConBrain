﻿import { FormTable, FormTableItem, FormTableSelectionItem } from "./../../../js/components/default-components/form-table.jsx"
export class ImageEdit extends React.Component {
    constructor(props) {
        super(props);
    }

    async deleteImage() {
        const response = await fetch(`/image?id=${this.props.image.id}`, {method: "DELETE"});
        if (response.ok === true) {
            this.props.onChange();
        }
    }

    render() {
        const securityValues = [
            { key: 0, value: "public" },
            { key: 1, value: "only friends" },
            { key: 2, value: "private" },
        ];
        return <div className="backgroundPresentContent" onClick={() => this.props.onExit()}>
            <div className="presentContent scrollDiv" onClick={e=>e.stopPropagation()}>
                <img className="fullimage" src={`/image?id=${this.props.image.id}`}></img>
                <FormTable action={`/image/edit?id=${this.props.image.id}`} method="POST" name="edit" caption="Edit image" sendContent="Change" onSend={() => this.props.onChange()}>
                    <FormTableItem name="Name" value={this.props.image.name} property="name" type="text" min-length="5" isSend />
                    <FormTableItem name="Description" value={this.props.image.description} property="description" type="text" min-length="5" isSend />
                    <FormTableSelectionItem name="Security" value={this.props.image.securityLevel} property="level" values={securityValues} isSend />
                </FormTable>
                <button className="removeButton" onClick={()=>this.deleteImage()}>Delete</button>
            </div>
        </div>
    }
}