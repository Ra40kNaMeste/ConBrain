import { FormTable, FormTableItem } from "../../../../js/components/default-components/form-table.jsx"

ReactDOM.render(
    <div className="columnnowrapstackpanel">
        <FormTable isSaveToken action="/login" method="POST" name="loginform" caption="Authorization" sendContent="Sign in">
            <FormTableItem name="Login" property="login" type="text" min-length="5" isSend="true" />
            <FormTableItem name="Password" property="password" type="password" min-length="5" isSend="true" />
        </FormTable>
        <a href="/register" className="registerBtn">register</a>
    </div>
,
    document.getElementById("content")
);