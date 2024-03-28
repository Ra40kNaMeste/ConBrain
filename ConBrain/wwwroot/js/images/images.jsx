import { LoadingDatesList } from "../components/loading-dates-list.jsx"
import { ImageEdit } from "./image-edit.jsx"
import { convertImageToJpg } from "./../authorizations/image-converter.js"

class ImageView extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            image: undefined
        };

        this.dates = React.createRef();

        this.fileInput = React.createRef();
        this.loadPerson();
    }

    async loadPerson() {

        const authresponse = await fetch("/authperson");
        if (authresponse.ok === true) {
            const authPerson = await authresponse.json();
            this.setState({ person: authPerson });
        }
    }

    async loadImg() {
        if (this.fileInput.current.files.length == 0) {
            return;
        }
        const reader = new FileReader();

        reader.onload = rev => {
            const img = document.createElement("img");
            img.onload = async () => {
                const formData = new FormData();
                const targetimg = convertImageToJpg(img);
                let blobImg = new Blob([targetimg], { type: "image/jpg" });

                formData.append("file", blobImg, "avatar.jpg");
                formData.append("key", "avatar.jpg");
                const response = await fetch("/image", {
                    method: "POST",
                    body: formData
                });
                if (response.ok === true) {
                    const imgData = await response.json();
                    this.setState({ image: imgData });
                }

            }
            img.src = rev.target.result;
        }
        reader.readAsDataURL(this.fileInput.current.files[0])
    }

    async changeImage() {
        this.dates.current.update();
        this.setState({ image: undefined });
    }

    render() {
        const isEdit = this.state.person && this.props.nick == this.state.person.nick;

        const builder = (o) => {
            const date = new Date(Date.parse(o.date));
            return <div className="viewImageItem" onClick={() => {
                if (isEdit)
                    this.setState({ image: o })
            }}>
                <img className="bigAvatar" src={`/image?id=${o.id}`}></img>
                <p>{o.name}</p>
                <p>{date.toISOString().split('T')[0]}</p>
            </div>
        }

        const content = 
            <div className="fullSize scrollDiv">
                <LoadingDatesList ref={this.dates} isSearch className="" url={`/images/get?nick=${this.props.nick}&`} step={this.props.step} offset={this.props.offset} builder={builder} direction="TopWrap" >
                </LoadingDatesList>
                <input style={{ display: "none" }} ng-model="image" type="file" id="fileInput" ref={this.fileInput} onChange={() => this.loadImg()} />
                {isEdit === true ? <button className="addItemButton" onClick={() => this.fileInput.current.click()}>+</button>:undefined }
                {this.state.image ? <ImageEdit image={this.state.image} onExit={() => this.setState({image:undefined})} onChange={() => this.changeImage()} /> : undefined}
            </div>;

        return content;
    }
}

const regex = /[/](?<nick>[^/]+)[/]images/gm;
const nick = regex.exec(window.location.href).groups.nick;


ReactDOM.render(
    <ImageView nick={nick} step="30" offset="0"></ImageView>, document.getElementById("content")
);