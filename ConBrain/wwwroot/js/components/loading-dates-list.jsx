import * from "../../../../node_modules/react/index";
import * from  "../../../../node_modules/react-dom/index";

class DataManager {
    constructor(requestItem) {
        this.#dates = new Map();
        this.#requestItem = requestItem;
    }
    #dates;
    #requestItem;
    async #loadpersonfromserver(nick) {
        let item = await this.#requestItem(nick);
        this.#dates.set(nick, item);
        return item;
    }

    async getPerson(nick) {
        const res = this.#dates.get(nick);
        if (res == null)
            return await this.#loadpersonfromserver(nick);
        return res;
    }
}

class LoadingDatesList extends Component
{
    constructor(props, direction, builder) {
        super(props);
        this.props = new {
            style: direction + "stackpanel",
            builder:builder
        };
        this.state = new {
            loading: true,
            dates: []
        };
    }

    render() {
        return <div className={this.props.style}>
            {this.state["dates"].map((o, e) => props["builder"](p))}
            {this.state["loading"] && <img src="/images/load.gif" />}
        </div>
    }
}


//constructor(props, url, pattern, childComponentBuilder, scrollOffset = 1, timeoutLoading = 5000, direction = "down") {
//    super(props);

//    this.rootDiv = React.createRef();

//    let canScroll;
//    switch (direction) {
//        case "down":
//            canScroll = this.canDownScroll;
//            break;
//        case "left":
//            canScroll = this.canLeftScroll;
//            break;
//        case "right":
//            canScroll = this.canRightScroll;
//            break;
//        default:
//            canScroll = this.canTopScroll;
//            break;
//    }

//    this.props = new {
//        step: step,
//        scrollOffset: scrollOffset,
//        timeout: timeoutLoading,
//        style: direction + "stackpanel",
//        builder: childComponentBuilder,
//        canScroll: canScroll,
//        url: url
//    };
//    this.state = new {
//        pattern: pattern,
//        elements: []
//    };
//}

//canTopScroll(offset) {
//    return this.rootDiv.scrollTop < offset;
//}

//canDownScroll(offset) {
//    return this.rootDiv.scrollY + this.rootDiv.clientHeight + offset >= this.rootDiv.scrollHeight;
//}

//canLeftScroll(offset) {
//    return this.rootDiv.scrollLeft < offset;
//}
//canRightScroll(offset) {
//    return this.rootDiv.scrollY + this.rootDiv.clientHeight + offset >= this.rootDiv.scrollHeight;
//}

//    async scroll() {
//    //Загрузка данных с сервера
//    let temp = 0
//    while (!this.props.canScroll(scrollOffset)) {
//        temp = await UpdateItemsAsync(lastElementIndex, step);
//        lastElementIndex += temp;
//        if (temp < step)
//            break;
//    }
//}
//    //Добавление новых пользователей на сервер. Возвращает количество загруженных пользователей
//    async append() {
//    this.state["load"] = true;
//    //Запрашиваем данные с сервера
//    const dates = await loadByServer();

//    //Добавляем загруженные данные в таблицу
//    if (dates != null) {
//        for (data of dates) {
//            this.state.elements.add(this.props.childComponentBuilder(data));
//        }
//    }
//    this.state["load"] = false;
//    return 0;
//}
//    async loadByServer() {
//    //Запрашиваем данные с сервера
//    const response = await fetch(this.props.url + `?start=${this.props.last}&size=${this.props.step}&pattern=${this.state.pattern}`, {
//        method: "GET",
//    });

//    //получаем ответ
//    return response.ok === true ? await response.json() : null;
//}