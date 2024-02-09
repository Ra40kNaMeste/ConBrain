import "../../../node_modules/imask/dist/imask.js";
const phoneFields = [].slice.call(document.getElementsByClassName("phone"));
console.log(phoneFields)
phoneFields.forEach(i => IMask(i, {
    mask: '+00(000)000-00-00'
}))

