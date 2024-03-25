export function Avatar({avatar, className }) {
    const url = avatar && avatar != "" ? `/image?id=${avatar}` : `/images/default.svg`;
    return <img src={url} className={className }/>
}