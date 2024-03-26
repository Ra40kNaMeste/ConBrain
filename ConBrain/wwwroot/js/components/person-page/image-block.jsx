import { LoadingDatesList } from "./../loading-dates-list.jsx"
export function ImageBlock({ target, step = 5, offset = 0 }) {
    const builder = (img) => {
        console.log(img);
        return <img className="bigavatar" src={`image?id=${img.id}`}></img>
    }

    const url = target ? `/images?nick=${target}&` : "/images?";

    return <div className="content scrollDiv columnnowrappanel">
        <h3>Images</h3>
        <LoadingDatesList url={url} step={step} offset={offset} builder={builder} direction="Left" />
        <div className="rowstretchstackpanel">
            <button>more images</button>
            <button>add image</button>
        </div>
    </div>
}