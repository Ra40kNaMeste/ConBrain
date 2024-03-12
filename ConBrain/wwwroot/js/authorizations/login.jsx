import { FormTable, FormTableItem } from "../../../../js/components/default-components/form-table.jsx"

ReactDOM.render(
    <FormTable action="/login" method="POST" name="loginform" caption="Sign in">
        <FormTableItem name="login" property="login" type="text" min-length="5" isSend = "true"/>
        <FormTableItem name="password" property="password" type="password" min-length="5" isSend="true"/>
    </FormTable>,
    document.getElementById("content")
);