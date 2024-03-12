import { FormTable, FormTableItem } from "../../../../js/components/default-components/form-table.jsx"

ReactDOM.render(
    <div className="fullSize">
        <FormTable action="/login" method="POST" name="loginform" caption="Sign in">
            <FormTableItem name="login" property="login" type="text" min-length="5" isSend="true" />
            <FormTableItem name="password" property="password" type="password" min-length="5" isSend="true" />
        </FormTable>
        <a href="/register" className="btn">register</a>
    </div>
,
    document.getElementById("content")
);