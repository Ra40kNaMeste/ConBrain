import { PersonHeader } from "../components/person-page/view-person-page.jsx";
import { ImageBlock } from "../components/person-page/image-block.jsx";
    
ReactDOM.render(
    <div className="fullSize">
        <PersonHeader></PersonHeader>
        <ImageBlock></ImageBlock>
    </div>,
    document.getElementById("content")
);