import { fetchWithAddressString, saveToken } from "../../authorizations/authorization.js";


export class FormTable extends React.Component {
    constructor(props) {
        super(props);
        this.table = React.createRef();
    }
    componentDidMount() {
        const table = this.table.current;
        table.addEventListener("submit", async (e) => {
            e.preventDefault();
            const values = [].slice.call(document.getElementsByClassName("sendInput"));
            const response = await fetchWithAddressString(e.target, values);
            if (await saveToken(response) == false) {
                values.forEach(value => {
                    value.setCustomValidity("incorrect login or password");
                    value.reportValidity();
                })
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
            <button>sign in</button>
        </form>
    }

}


export function FormTableItem(props) {

    return <tr className="rowForm">
        <td className="nameForm">{props.name}</td>
        <td className="valueFormTd">
            <input autoComplete="on" className={props.isSend ?"sendInput":"" } name={props.property} value={props.value} type={props.type} maxLength={props.maxLength} minLength={props.minLength} />
        </td>
    </tr>
}
