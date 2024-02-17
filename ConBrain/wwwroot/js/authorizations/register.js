import { fetchWithAddressString, saveToken } from "./authorization.js";
const form = document.getElementsByName("loginform")[0];
const values = [].slice.call(document.getElementsByClassName("sendInput"));

form.addEventListener("submit", async e => {
    console.log("gg")
    e.preventDefault();
    const response = await fetchWithAddressString(e);
    if (await saveToken(response) == false) {   
        const data = await response.json();
        if (data != null) {
            for (result of data) {
                for (member of result.memberNames) {
                    const box = values.find(i => i.name.toLowerCase() == str);
                    box.setCustomValidity(result.errorMessage);
                    box.reportValidity();
                }
            }
        }
    }
});
