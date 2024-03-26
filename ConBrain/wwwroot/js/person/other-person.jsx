import { PersonHeader } from "../components/person-page/view-person-page.jsx";
import { ImageBlock } from "../components/person-page/image-block.jsx";

const re = new RegExp("[^/]+$");

const person = window.location.href.match(re);
    
ReactDOM.render(
    <div className="fullSize">
        <PersonHeader personNick={person[0]}></PersonHeader>
        <ImageBlock target={person[0]}></ImageBlock>
    </div>,
    document.getElementById("content")
);