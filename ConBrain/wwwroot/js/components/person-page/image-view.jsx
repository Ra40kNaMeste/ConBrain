import { FormTable, FormTableItem, FormTableSelectionItem } from "./../default-components/form-table.jsx"
export function ImageView({ onExit, image }) {
    return <div className="backgroundPresentContent" onClick={() => onExit()}>
        <div className="presentContent scrollDiv" onClick={e => e.stopPropagation()}>
            <img className="fullimage" src={`/image?id=${image.id}`}></img>
        </div>
    </div>
}