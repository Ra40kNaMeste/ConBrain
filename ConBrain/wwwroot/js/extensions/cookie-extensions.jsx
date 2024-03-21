export function deleteCookie(name) {
    const regex = new RegExp(name + "=.*");
    const value = document.cookie.split(';').find(i => i.match(regex) != null);
    document.cookie = document.cookie.replace(value, value + ";max-age=-1");
}