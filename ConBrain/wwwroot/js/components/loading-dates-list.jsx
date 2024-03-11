class DataStore {
    constructor(url, step) {
        this.#url = url;
        this.#step = step;
        this.#ignores = [];
    }
    #ignores;
    #url;
    #step;
    #flag = false;
    async load() {
        const res = [];
        if (this.#flag)
            return res;
        this.#flag = true;
        console.log(this.#url + `size=${this.#step}&${this.#ignores.map(i => "ignores=" + i).join('&')}`);
        const response = await fetch(this.#url + `size=${this.#step}&${this.#ignores.map(i => "ignores=" + i).join('&')}`);
        if (response.ok === true) {
            const loadingData = await response.json();
            console.log(loadingData);
            for (const data of loadingData) {
                res.push(data);
                this.#ignores.push(data.id);
            }
        }
        this.#flag = false;
        return res;
    }
}

class DownLoaidingDatesListFunctions {
    style = "downstackpanel";
    condition(root, offset) {
        return root.scrollTop + root.clientHeight + offset >= root.scrollHeight;
    }
    scroll(root, offset) {
        return {top:root.scrollTop, left:root.scrollLeft}
    }
    append(old, append) {
        return [...old, ...append]
    }
}

///Компонент для создания подгружаемых данных в зависимости от прокрутки. Обязательные свойства:
///url - адрес сервака. Дописывает last=последний_элементы&step=количество_подгружаемых_элементов
///step - количество подгружаемых элементов за 1 раз
///offset - отступ от конца при скроллинге
///builder - строитель блока от объекта (возвращает html-компонент из объекта)
///необязательные
///direction - направление рендеринга из перечисления Direction
///dates - данные. Можно изменять
export class LoadingDatesList extends React.Component
{
    constructor(props) {
        super(props);

        this.#data = new DataStore(this.props["url"], this.props["step"]);

        this.rootdiv = React.createRef();

        this.state = {
            loading: false,
            dates: []
        };
        this.init();
    }
    functions;
    #data;

    async init() {
        this.functions = this.getFunctions(this.props["direction"]);
    }

    async scroll() {
        console.log("scrolling");
        console.log();
        const root = this.rootdiv.current;
        console.log(this.functions.condition(root, parseInt(this.props["offset"])));

        if (this.functions.condition(root, parseInt(this.props["offset"]))) {
            const old = {
                scrollTop: root.scrollTop,
                scrollLeft: root.scrollLeft,
                scrollHeight: root.scrollHeight,
                scrollWidth: root.scrollWidth
            };
            console.log("loading scroll");
            await this.load();
            root.scroll(this.functions.scroll(old, root));
        }
    }

    async componentDidMount() {
        console.log("gg");
        const root = this.rootdiv.current;
        if (this.functions.condition(root, parseInt(this.props["offset"]))) {
            console.log("loading");
            const old = {
                scrollTop: root.scrollTop,
                scrollLeft: root.scrollLeft,
                scrollHeight: root.scrollHeight,
                scrollWidth: root.scrollWidth,

            };
            await this.load();
            root.scroll(this.functions.scroll(old, root));
        }
    }

    async load() {
        this.setState({ loading: true });
        const loadingData = await this.#data.load();
        this.setState({ dates: this.functions.append(this.state["dates"], loadingData) });

        this.setState({ loading: false });
        console.log(this.state["loading"])
    }

    getFunctions(direction) {
        switch (direction) {
            case "Down":
                return new DownLoaidingDatesListFunctions();
            case "Left":
                return this.leftScrollCondition;
            case "Right":
                return this.rightScrollCondition;
            default:
                return this.topScrollCondition;
        }
    }

    //topScrollCondition(root, offset) {
    //    console.log(root.scrollTop)
    //    return root.scrollTop <= offset;
    //}
    
    //leftScrollCondition(root, offset) {
    //    return rd.scrollleft < offset;
    //}
    //rightScrollCondition(root, offset) {
    //    return rd.scrolly + rd.clientheight + offset >= rd.current.scrollheight;
    //}
    
    render() {
        const divClassNames = `${this.functions.style} scrollDiv`;
        return <div className={divClassNames} ref={this.rootdiv} onScroll={ ()=>this.scroll() }>
            {this.state["dates"].map((o, e) => this.props["builder"](o))}
            {this.state["loading"] && <img src="/images/load.gif" className="middleicon loadingimage" />}
        </div>
    }
}