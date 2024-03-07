import toPrimitive from "./toPrimitive.js";
export default function toPropertyKey(arg) {
  var key = toPrimitive(arg, "string");
  return typeof key === "symbol" ? key : String(key);
}

//# sourceMappingURL=toPropertyKey.js.map
