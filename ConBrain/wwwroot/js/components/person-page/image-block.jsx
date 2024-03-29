import { LoadingDatesList } from "./../loading-dates-list.jsx"

class ImageCollage extends React.Component {
    constructor(props) {
        super(props);
        this.state = { images: [] };
        this.loadImage();
    }

    #size = 6;

    async loadImage() {
        const response = await fetch(`/images/get?nick=${this.props.nick}&size=${this.#size}`);
        if (response.ok === true) {
            const images = await response.json();
            this.setState({ images: images });
        }
    }

    render() {
        return <div className="collegeContainer">
            {this.state.images.map(o => <img className="collegeImage" src={`/image?id=${o.id}`} onClick={() => this.props.onImageClick(o)}></img>) }
        </div>
    }
}

export function ImageBlock({ target, onImageClick, step = 5, offset = 0 }) {

    return <div className="content scrollDiv columnnowrappanel">
        <h3>Images</h3>
        <ImageCollage nick={target} onImageClick={img => onImageClick(img)} />
        <div className="rowstretchstackpanel">
            <button onClick={() => window.location.href = `/${target}/images`}>more images</button>
        </div>
    </div>
}