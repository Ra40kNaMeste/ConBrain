import assertClassBrand from "./assertClassBrand.js";
export default function _classPrivateGetter(privateMap, receiver, getter) {
  return getter(assertClassBrand(privateMap, receiver));
}

//# sourceMappingURL=classPrivateGetter.js.map
