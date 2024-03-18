import { FormTable, FormTableItem, FormTableAppendFriendItem } from "../../../../js/components/default-components/form-table.jsx"

ReactDOM.render(
    <div className="columnnowrapstackpanel scrollDiv">
        <FormTable action="/dialogs/build" method="POST" name="createDialogForm" caption="Creating dialog" sendContent="Create">
            <FormTableItem name="Name" property="name" type="text" min-length="5" isSend="true" />
            <FormTableAppendFriendItem name="Password" property="password" type="password" min-length="5" isSend="true" />
        </FormTable>
        <a href="/register" className="registerBtn">register</a>
    </div>
    ,
    document.getElementById("content")
);