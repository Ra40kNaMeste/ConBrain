import { fetchWithAddressString, saveToken } from "./authorization.js";
const form = document.getElementsByName("loginform")[0];
const values = [].slice.call(document.getElementsByClassName("sendInput"));

form.addEventListener("submit", async e => {
    e.preventDefault();
    const response = await fetchWithAddressString(e);
    if (await saveToken(response) == false) {
        console.log(values);
        values.forEach(value => {
            value.value = "";
            value.setCustomValidity("incorrect login or password");
            value.reportValidity();
        })
    }
});