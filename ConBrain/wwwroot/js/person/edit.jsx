import { FormTable, FormTableItem } from "./../../../js/components/default-components/form-table.jsx"
import { deleteCookie } from "./../../../js/extensions/cookie-extensions.jsx"
import {Avatar } from "./../components/default-components/avatar.jsx"
class AvatarInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            person: {
                avatar: ""
            }
        }
        this.loadPerson();
        this.setLoadedImageSize();
        this.createCanvas();
        this.fileInput = React.createRef();
    }

    createCanvas() {
        this.#canvas = document.createElement("canvas");
        this.#canvas.width = this.#loadedImageSize.width;
        this.#canvas.height = this.#loadedImageSize.height;
    }

    setLoadedImageSize() {
        this.#loadedImageSize = { height: 200, width: 200 };
    }

    async loadPerson() {
        const response = await fetch("./../authperson");
        if (response.ok === true) {
            const person = await response.json();
            console.log(person);
            this.setState({ person: person });
        }
    }

    loadImg() {
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
                if (response.ok === true) {
                    this.loadPerson();

                }
                else {
                    let message;
                    switch (response.status) {
                        case 400:
                            message = "Bad request data. Report to us please";
                            break;
                        case 401:
                            message = "Authorized time has expired. Log in again please";
                        default:
                            message = "Unknown error. Report to us please";
                    }
                    document.location.href = `/error?message=${message}`;
                }

            }
            img.src = rev.target.result;
        }
        reader.readAsDataURL(this.fileInput.current.files[0])
    }

    #canvas;
    #loadedImageSize;

    convertImageToJpg(image) {
        const context = this.#canvas.getContext("2d");
        context.drawImage(image, 0, 0, image.width, image.height, 0, 0, this.#canvas.width, this.#canvas.height);
        return this.#canvas.toDataURL("image/jpeg").replace(/^data:image\/jpeg;base64,/, "");
    }

    render() {
        const avatar = this.state.person.avatarId == null ? "" : this.state.person.avatarId;
        return <div className="rowstretchstackpanel">
            <Avatar avatar={avatar} className="bigavatar" />
            <input style={{ display: "none" }} ng-model="image" type="file" id="fileInput" ref={this.fileInput} onChange={() => this.loadImg()} />
            <button className="" onClick={()=>this.fileInput.current.click()}>Change avatar</button>
        </div>
    }
}

class PersonDataInput extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            person: {
                "nick": "",
                "name": "",
                "family": "",
                "secondName": "",
                "phone": "",
            }
        }
        this.loadPerson();
    }

    async loadPerson() {
        const response = await fetch("./../authperson");
        if (response.ok === true) {
            const person = await response.json();
            this.setState({ person: person });
        }
    }

    render() {
        return <FormTable action="/edit" method="POST" name="edit" caption="Edit profile" sendContent="Change">
            <FormTableItem name="Nick" value={this.state.person.nick} property="nick" type="text" min-length="5" isSend />
            <FormTableItem name="Name" value={this.state.person.name} property="name" type="text" min-length="5" isSend />
            <FormTableItem name="Family" value={this.state.person.family} property="family" type="text" min-length="5" isSend />
            <FormTableItem name="Second name" value={this.state.person.secondName} property="secondName" type="text" min-length="5" isSend />
            <FormTableItem name="Phone" value={this.state.person.phone} property="phone" type="tel" min-length="5" isSend />
        </FormTable>
    }
}

function DangerousZone(props) {

    const onExitAccountClick = () => {
        deleteCookie("token");
        location.href = "/Login";
    }

    const onChangePasswordClick = () => {
        document.location.href = `./changePassword`;
    }

    return <div className="stackpanel dangerousZone">
        <h3>Dangerous zone</h3>
        <button className="removeButton" onClick={onChangePasswordClick}>change password</button>
        <button className="removeButton" onClick={onExitAccountClick}>exit from the account</button>
    </div>
}

ReactDOM.render(
    <div className="columnnowrappanel scrollDiv fullSize">
        <AvatarInput/>

        <PersonDataInput />

        <DangerousZone/>
</div>,
    document.getElementById("content")
);