
//// возвращает куки с указанным name,
//// или undefined, если ничего не найдено
//function getCookie(name) {
//    let matches = document.cookie.match(new RegExp(
//        "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
//    ));
//    return matches ? decodeURIComponent(matches[1]) : undefined;
//}
//token = getCookie('token');
//if (token) {
//    const authLinks = document.getElementsByClassName("auth");
//    async function openLink(e) {
//        e.preventDefault();
//        target = e.currentTarget;
//        console.log(token);

//        const response = await fetch(target.href, {
//            method: target.method,
//            headers: { 'Authorization': 'Bearer ' + token }
//        });
//        if (response.ok === true) {
///*            location.assign(target['href']);*/
//            console.log(response.text());
//            document.write(response.text());
//        }
//    }
//    for (var link of authLinks) {
//        link.addEventListener("click", openLink)

//    }
//}
XMLHttpRequest.