class DataStore {
    constructor(url, step) {
        this.#url = url;
        this.#step = step;
        this.#ignores = [];
    }
    #ignores;
    #url;
    #step;
    async load() {
        const res = [];

        const response = await fetch(this.#url + `size=${this.#step}&${this.#ignores.map(i => "ignores=" + i).join('&')}`);
        if (response.ok === true) {
            const loadingData = await response.json();
            
            for (const data of loadingData) {
                res.push(data);
                this.#ignores.push(data.id);
            }
        }
        return res;
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
        console.log(this.props.direction);
        this.#condition = this.getFunction(this.props["direction"]);
        this.#style = this.getStyle(this.props["direction"]);  

        this.rootdiv = React.createRef();

        this.state = {
            loading: false,
            dates: []
        };
        this.init();
    }
    #style;
    #condition;
    #data;

    async init() {
        this.#condition = this.getFunction(this.props["direction"]);
        for (let i = 0; i < 3; i++)
            await this.load()
    }

    async scroll() {
        if (this.#condition(this.props["offset"])) {
            await this.load();
        }
    }

    async load() {
        this.setState({ loading: true });
        const loadingData = await this.#data.load();
        this.setState({ dates: [...this.state["dates"], ...loadingData] });

        this.setState({ loading: false });
        console.log(this.state["loading"])
    }

    getFunction(direction) {
        switch (direction) {
            case "Down":
                return this.downscroll;
            case "Left":
                return this.leftscroll;
            case "Right":
                return this.rightscroll;
            default:
                return this.topscroll;
        }
    }

    getStyle(direction) {
        switch (direction) {
            case "Down":
                return "downstackpanel";
            case "Left":
                return "leftstackpanel";
            case "Right":
                return "rightstackpanel";
            default:
                return "topstackpanel";
        }
    }

    topscroll(offset) {
        console.log(this.rootdiv)
        rd = this.rootdiv;
        if (rd == null)
            return false;
        console.log("gg")
        return rd.current.scrolltop < offset;
    }

    downscroll(offset) {
        return rd.scrolly + rd.clientheight + offset >= rd.scrollheight;
    }

    leftscroll(offset) {
        return rd.scrollleft < offset;
    }
    rightscroll(offset) {
        return rd.scrolly + rd.clientheight + offset >= rd.current.scrollheight;
    }

    render() {
        return <div className={this.#style} ref={this.rootdiv }>
            {this.state["dates"].map((o, e) => this.props["builder"](o))}
            {this.state["loading"] && <img src="/images/load.gif" className="middleicon loadingimage" />}
        </div>
    }
}