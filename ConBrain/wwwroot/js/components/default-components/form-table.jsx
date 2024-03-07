import * from "../../../../node_modules/react/index";
import * from  "../../../../node_modules/react-dom/index";

class FormTable extends Component {
    constructor(name, controller, action, sendContent, ...parameters) {
        this.state["params"] = parameters;
        this.state["name"] = name;
        this.state["sendContent"] = sendContent;

        this.props["controller"] = controller;
        this.props["action"] = action;
    }
    render() {
        return
        <form asp-controller={this.props["controller"]} asp-action={this.props["action"]} method="POST">
            <table>
                <caption>{this.state["name"]}</caption>
                {
                    this.state["params"].map((o, i) =>
                        <tr class="rowForm">
                            <td class="nameForm">{o.name}</td>
                            <td class="valueFormTd">
                                <input name={o.property} value={o.value} type={o.type} maxlength={o.maxLength} minlength={o.minLength} />
                            </td>
                        </tr>)
                }
            </table>
            <button name="send">{this.state["sendContent"]}</button>
        </form>
    }
}