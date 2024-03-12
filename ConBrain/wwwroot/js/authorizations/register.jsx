import { FormTableWithPasswordValidation, FormTableItem } from "../../../../js/components/default-components/form-table.jsx"

ReactDOM.render(
    <FormTableWithPasswordValidation action="/register" method="POST" name="register" caption="Register">
        <FormTableItem name="nick" property="nick" type="text" min-length="5" isSend />
        <FormTableItem name="name" property="name" type="text" min-length="5" isSend />
        <FormTableItem name="family" property="family" type="text" min-length="5" isSend />
        <FormTableItem name="second name" property="secondname" type="text" isSend />
        <FormTableItem name="phone" property="phone" type="tel" isSend />
        <FormTableItem name="password" property="password" type="password" isSend />
        <FormTableItem name="repeat password" property="repeatpassword" type="password" />
    </FormTableWithPasswordValidation>,
    document.getElementById("content")
);