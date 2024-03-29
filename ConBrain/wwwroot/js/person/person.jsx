import { PersonHeader } from "../components/person-page/view-person-page.jsx";
import { ImageBlock } from "../components/person-page/image-block.jsx";
import { ImageView } from "../components/person-page/image-view.jsx"

class PersonView extends React.Component {
    constructor(props) {
        super(props);
        this.state = { image: undefined };
    }
    render() {
        const imageView = this.state.image ? <ImageView image={this.state.image} onExit={() => this.setState({ image: undefined })}></ImageView> : undefined;

        return <div className="fullSize scrollDiv">
            <PersonHeader personNick={this.props.nick}></PersonHeader>
            <ImageBlock target={this.props.nick} onImageClick={img => this.setState({image:img})}></ImageBlock>
            {imageView}
        </div>
    }
}

const re = new RegExp("[^/]+$");

const person = window.location.href.match(re);

ReactDOM.render(<PersonView nick={person[0]} />, document.getElementById("content"));