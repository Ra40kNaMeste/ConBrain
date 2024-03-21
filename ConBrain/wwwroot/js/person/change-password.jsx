import { FormTableWithPasswordValidation, FormTableItem } from "../../../../js/components/default-components/form-table.jsx"

ReactDOM.render(
    <FormTableWithPasswordValidation action="/changepassword" method="POST" name="change password" caption="Change password" sendContent="Change">
        <FormTableItem name="Old password" property="oldPassword" type="password" min-length="5" isSend />
        <FormTableItem name="Password" property="password" type="password" isSend />
        <FormTableItem name="Repeat password" property="repeatpassword" type="password" />
    </FormTableWithPasswordValidation>,
    document.getElementById("content")
);