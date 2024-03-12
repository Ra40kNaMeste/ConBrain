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
    scroll(old, root) {
        return {top:root.scrollTop, left:root.scrollLeft}
    }
    append(old, append) {
        return [...old, ...append]
    }
}

class TopLoaidingDatesListFunctions {
    style = "downstackpanel";
    condition(root, offset) {
        return root.scrollTop <= offset;
    }
    scroll(old, root) {
        console.log(old.scrollHeight)
        return { top: root.scrollHeight + old.scrollTop - old.scrollHeight, left: root.scrollLeft }
    }
    append(old, append) {
        return [...append.reverse(), ...old]
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

    //Подгружает элементы, если дожли до конца
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
        const root = this.rootdiv.current;
        root.addEventListener('resize', (e) => this.scroll());

        while (this.functions.condition(root, parseInt(this.props["offset"]))) {
            let old = this.copyScrollPosition(root);
            await this.load();
            root.scroll(this.functions.scroll(old, root));
        }
    }


    //Загружает данные с сервера
    async load() {
        this.setState({ loading: true });

        const loadingData = await this.#data.load();
        
        this.setState({ dates: this.functions.append(this.state["dates"], loadingData) });

        this.setState({ loading: false });
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
    
    render() {
        const divClassNames = `${this.functions.style} scrollDiv`;
        return <div className={divClassNames} ref={this.rootdiv} onScroll={ ()=>this.scroll() }>
            {this.state["dates"].map((o, e) => this.props["builder"](o))}
            {this.state["loading"] && <img src="/images/load.gif" className="middleicon loadingimage" />}
        </div>
    }
}
