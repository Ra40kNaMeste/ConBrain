import { LoadingDatesList } from "./../loading-dates-list.jsx"
export function ImageBlock({ target, step = 5, offset = 0 }) {
    const builder = (img) => {
        return <img className="bigavatar" src={`image?id=${img.id}`}></img>
    }

    return <div className="content scrollDiv columnnowrappanel">
        <h3>Images</h3>
        <LoadingDatesList url={`/images/get?nick=${target}&`} step={step} offset={offset} builder={builder} direction="TopWrap" className="imagecontainer" />
        <div className="rowstretchstackpanel">
            <button onClick={() => window.location.href = `/${target}/images`}>more images</button>
        </div>
    </div>
}