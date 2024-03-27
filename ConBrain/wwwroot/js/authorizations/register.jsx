import { FormTableWithPasswordValidation, FormTableItem } from "../../../../js/components/default-components/form-table.jsx"

ReactDOM.render(
    <FormTableWithPasswordValidation isSaveToken action="/register" method="POST" name="register" caption="Registration" sendContent="Register">
        <FormTableItem name="Nick" property="nick" type="text" min-length="5" isSend />
        <FormTableItem name="Name" property="name" type="text" min-length="5" isSend />
        <FormTableItem name="Family" property="family" type="text" min-length="5" isSend />
        <FormTableItem name="Second name" property="secondname" type="text" isSend />
        <FormTableItem name="Phone" property="phone" type="tel" isSend />
        <FormTableItem name="Password" property="password" type="password" isSend />
        <FormTableItem name="Repeat password" property="repeatpassword" type="password" />
    </FormTableWithPasswordValidation>,
    document.getElementById("content")
);