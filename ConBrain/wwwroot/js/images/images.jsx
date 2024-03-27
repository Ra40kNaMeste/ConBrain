import { LoadingDatesList } from "../components/loading-dates-list.jsx"
import { ImageEdit } from "./image-edit.jsx"
class ImageView extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            image: undefined
        };

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
                const targetimg = this.convertImageToJpg(img);
                let blobImg = new Blob([targetimg], { type: "image/jpg" });

                formData.append("file", blobImg, "avatar.jpg");
                formData.append("key", "avatar.jpg");
                const response = await fetch("/edit/avatar", {
                    method: "POST",
                    body: formData
                });

            }
            //img.src = rev.target.result;
            console.log(rev.target);
        }
        reader.readAsDataURL(this.fileInput.current.files[0])
    }


    render() {
        const builder = (o) => {
            const date = new Date(Date.parse(o.date));
            return <div className="viewImageItem" onClick={() => this.setState({ image: o })}>
                <img className="bigAvatar" src={`/image?id=${o.id}`}></img>
                <p>{o.name}</p>
                <p>{date.toISOString().split('T')[0]}</p>
            </div>
        }
        console.log(`/images?nick=${this.props.nick}&`)

        const content = this.state.image ? <ImageEdit image={this.state.image} onExit={() => this.setState({ image: undefined })} />
            : <div className="fullSize">
                <LoadingDatesList isSearch className="" url={`/images/get?nick=${this.props.nick}&`} step={this.props.step} offset={this.props.offset} builder={builder} direction="TopWrap" >
                </LoadingDatesList>
                <input style={{ display: "none" }} ng-model="image" type="file" id="fileInput" ref={this.fileInput} onChange={() => this.loadImg()} />
                <button className="addItemButton" >+</button>

            </div>;

        return content;
    }
}

const regex = /[/](?<nick>[^/]+)[/]images/gm;
const nick = regex.exec(window.location.href).groups.nick;


ReactDOM.render(
    <ImageView nick={nick} step="30" offset="0"></ImageView>, document.getElementById("content")
);