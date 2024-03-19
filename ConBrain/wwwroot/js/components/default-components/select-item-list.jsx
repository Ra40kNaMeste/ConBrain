import { LoadingDatesList } from "./../loading-dates-list.jsx"

export function SelectItemList({ values, selected }) {

    return <div className="selectblock columnnowrapstackpanel">
        {values.map((o, e) => <div onClick={ ()=>selected(o)}>{o}</div>)}
    </div>
}

export class SelectItemListByUrl extends React.Component {

    constructor(props) {
        super(props);
        this.root = React.createRef();
        this.window = React.createRef();
    }

    componentDidMount() {
        const window = this.window.current;
        const root = this.root.current;
        
        var rect = root.getBoundingClientRect();
        window.style.top = rect.top;
        window.style.left = rect.left;

    }

    render() {
        const builder = (o) => <div className="item" onClick={() => this.props.selected(o)}>{`${o.nick} ${o.name}`}</div>;
        return <div ref={this.root} className="empty">
            <div ref={this.window} className="selectblock columnnowrapstackpanel">
                <LoadingDatesList url={this.props.url} step="10" offset="0" builder={builder} direction="Top" >
                </LoadingDatesList>
            </div>
        </div>
    }
}