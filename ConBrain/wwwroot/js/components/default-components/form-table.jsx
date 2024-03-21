import { fetchWithAddressString, saveToken } from "../../authorizations/authorization.js";
import { SelectItemListByUrl } from "./select-item-list.jsx"
import "../../../node_modules/imask/dist/imask.js";


export class FormTable extends React.Component {
    constructor(props) {
        super(props);
        this.validators = [];
        this.table = React.createRef();
    }

    validators;

    componentDidMount() {
        const table = this.table.current;

        const values = [].slice.call(document.getElementsByClassName("sendInput"));

        const phoneFields = values.filter(i => i.type == "tel");
        if (phoneFields)
            phoneFields.forEach(i => IMask(i, {
                mask: '+00(000)000-00-00'
            }))

        table.addEventListener("submit", async (e) => {
            e.preventDefault(e);
            for (const validator of this.validators)
                if (!validator(e))
                    return;

            const values = [].slice.call(document.getElementsByClassName("sendInput"));
            const response = await fetchWithAddressString(e.target, values);
            if (await saveToken(response) == false) {
                const data = await response.json();
                if (data != null) {
                    for (const result of data) {
                        for (const member of result.memberNames) {
                            const box = values.find(i => i.name.toLowerCase() == member);
                            box.setCustomValidity(result.errorMessage);
                            box.reportValidity();

                            box.addEventListener("input", (e) => {
                                box.setCustomValidity("");
                                box.reportValidity();
                            });

                        }
                    }
                }
                else {
                    alert("Unknow error log in");
                }
            }
            
        })
    }

    render() {
        return <form ref={this.table} name={this.props.name} action={this.props.action} method={this.props.method} className="form">
            <table>
                <caption>{this.props.caption }</caption>
                <tbody>
                    {this.props.children}
                </tbody>
            </table>
            <button>{this.props.sendContent}</button>
        </form>
    }

}

export class FormTableWithPasswordValidation extends FormTable {
    constructor(props) {
        super(props);
        this.validators.push(this.validate);
    }
    validate() {
        const secondPassword = document.getElementsByName("repeatpassword")[0];
        const password = document.getElementsByName("password")[0];
        
        if (secondPassword.value != password.value) {
            secondPassword.value = "";

            secondPassword.setCustomValidity("passwords not equals");
            secondPassword.reportValidity();

            secondPassword.addEventListener("input", (e) => {
                secondPassword.setCustomValidity("");
                secondPassword.reportValidity();
            });
            return false;
        }
        return true;
    }

}

export function FormTableItem(props) {
    let classes = props.isSend ? "sendInput" : "";
    classes += " valueForm";
    return <tr className="rowForm">
        <td className="nameForm">{props.name}</td>
        <td className="valueFormTd">
            <input autoComplete="on" className={classes} value={props.value} name={props.property} value={props.value} type={props.type} maxLength={props.maxLength} minLength={props.minLength} />
        </td>
    </tr>
}

export class FormTableAppendFriendItem extends React.Component {
    constructor(props) {
        super(props);
        this.state = { values: [], isSelected: false };
    }

    render() {
        let classes = "fromEmpty";
        if (this.props.isSend)
            classes += " sendInput";

        const value = this.state.values.map(i => i.id).join(`&${this.props.name}=`)

        let selectBox;
        if (this.state.isSelected) {
            
            const url = `/friends?${this.state.values.map(i => "ignores=" + i.id).join('&')}&`

            selectBox = <SelectItemListByUrl className="fullSize" x="" url={url} selected={(child) => {
                this.state.values.push(child);
                this.setState({ values: this.state.values, isSelected: false });
            }}>
            </SelectItemListByUrl>
        }
        else {
            selectBox = <div className="addItemDiv">
                <button className="addItemButton" onClick={() => this.setState({ isSelected: true })}>+</button>
            </div>;
        }

        return <tr className="rowForm">
            <td className="nameForm">{this.props.name}</td>
            <td className="valueFormTd rowwrapstackpanel">
                {this.state.values.map((o, e) =>
                    <div className="rownowrapstackpanel" onClick={() => this.setState({ isSelected: false })}>
                        <p>{o.nick}</p>
                        <input className={classes} name={this.props.property} value={o.nick} />
                        <button className="deleteItemButton" onClick={() => {
                            const val = this.state.values.filter(i=>i.nick != o.nick);
                            this.setState({ values: val, isSelected: false});
                        }}>x</button>
                    </div>)}
                {selectBox}
            </td>
        </tr>
    }
}