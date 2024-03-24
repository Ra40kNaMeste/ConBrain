class DataStore {
    constructor(url, step, searchStr) {
        this.#url = url;
        this.#step = step;
        this.#ignores = [];
        this.#searchStr = searchStr;
    }
    #ignores;
    #url;
    #step;
    #searchStr;
    #flag = false;
    async load() {
        const res = [];
        if (this.#flag)
            return res;
        this.#flag = true;
        const response = await fetch(this.#url + `size=${this.#step}&pattern=${this.#searchStr}&${this.#ignores.map(i => "ignores=" + i).join('&')}`);
        if (response.ok === true) {
            const loadingData = await response.json();
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
    scroll(old, root) {
        return {top:root.scrollTop, left:root.scrollLeft}
    }
    append(old, append) {
        return [...old, ...append]
    }
    push(old, pushed) {
        return [...pushed.reverse(), ...old];
    }
}

class TopLoaidingDatesListFunctions {
    style = "downstackpanel";
    condition(root, offset) {
        return root.scrollTop <= offset;
    }
    scroll(old, root) {
        return { top: root.scrollHeight + old.scrollTop - old.scrollHeight, left: root.scrollLeft }
    }
    append(old, append) {
        return [...append.reverse(), ...old]
    }
    push(old, pushed) {
        return [...old, ...pushed]
    }
}

class LeftLoaidingDatesListFunctions {
    style = "leftstackpanel";
    condition(root, offset) {
        return root.scrollLeft + root.clientWidth + offset >= root.scrollWidth;
    }
    scroll(old, root) {
        return { top: root.scrollTop, left: root.scrollLeft }
    }
    append(old, append) {
        return [...old, ...append]
    }
    push(old, pushed) {
        return [...pushed.reverse(), ...old];
    }
}

class RightLoaidingDatesListFunctions {
    style = "leftstackpanel";
    condition(root, offset) {
        return root.scrollLeft <= offset;
    }
    scroll(old, root) {
        return { top: root.scrollTop, left: root.scrollWidth + old.scrollLeft - old.scrollWidth }
    }
    append(old, append) {
        return [...append.reverse(), ...old]
    }
    push(old, pushed) {
        return[...old, ...pushed]
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

        this.#data = new DataStore(this.props["url"], this.props["step"], "");

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

    //Подгружает элементы, если дошли до конца
    async scroll() {
        const root = this.rootdiv.current;

        if (this.functions.condition(root, parseInt(this.props["offset"]))) {
            let old = this.copyScrollPosition(root);
            await this.load();
            root.scroll(this.functions.scroll(old, root));
        }
    }

    //Проверяет нужно ли добавлять элементы после визуализирования
    async componentDidMount() {
        await this.fillView()
    }

    async fillView() {
        const root = this.rootdiv.current;
        root.addEventListener('resize', () => this.scroll());

        while (this.functions.condition(root, parseInt(this.props["offset"]))) {
            const old = this.copyScrollPosition(root);
            const count = await this.load();
            root.scroll(this.functions.scroll(old, root));
            if (count == 0)
                return;
        }
    }



    //Загружает данные с сервера
    async load() {
        this.setState({ loading: true });

        const loadingData = await this.#data.load();
        
        this.setState({ dates: this.functions.append(this.state["dates"], loadingData), loading: false });

        return loadingData.length;
    }

    push(dates) {
        const root = this.rootdiv.current;
        const old = this.copyScrollPosition(root);
        this.setState({ dates: this.functions.push(this.state["dates"], dates) });
        
        root.scroll(this.functions.scroll(old, root));
    }

    //Возвращает набор действий для направления
    getFunctions(direction) {
        switch (direction) {
            case "Down":
                return new DownLoaidingDatesListFunctions();
            case "Left":
                return new LeftLoaidingDatesListFunctions();
            case "Right":
                return new RightLoaidingDatesListFunctions();
            default:
                return new TopLoaidingDatesListFunctions();
        }
    }

    //Вынужденная мера т.к. не копируется в REACT с помошью Object.assign
    copyScrollPosition(root) {
        return {
            scrollTop: root.scrollTop,
            scrollHeight: root.scrollHeight,
            scrollClientHeight: root.scrollClientHeight,
            scrollLeft: root.scrollLeft,
            scrollWidth: root.scrollWidth,
            scrollClientWidth: root.scrollClientWidth
        }
    }

    async handleChangeSearch(searchStr) {
        this.#data = new DataStore(this.props["url"], this.props["step"], searchStr);
        this.setState({ dates: [] });
        await this.load();
        await this.fillView();
    }
    
    render() {
        const divClassNames = `${this.functions.style} scrollDiv ${this.props.className}`;
        return <div className={this.props.className}>
            {
                this.props.isShowSearch ? <div className="rowstretchstackpanel">
                    <input ref={this.search} className="inputTextBox" onChange={e => this.handleChangeSearch(e.target.value)} />
                    <img src="./../../images/search.svg" className="smallicon" />

                </div> : undefined
            }
            <div className={divClassNames} ref={this.rootdiv} onScroll={() => this.scroll()}>
                {this.state["dates"].map((o, e) => this.props["builder"](o))}
                {this.state["loading"] && <img src="/images/load.gif" className="middleicon loadingimage" />}
            </div>
        </div>

    }
}
