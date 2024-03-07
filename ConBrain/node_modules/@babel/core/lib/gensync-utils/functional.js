import { isAsync, waitFor } from "./async.js";
export function once(fn) {
  let result;
  let resultP;
  let promiseReferenced = false;
  return function* () {
    if (!result) {
      if (resultP) {
        promiseReferenced = true;
        return yield* waitFor(resultP);
      }
      if (!(yield* isAsync())) {
        try {
          result = {
            ok: true,
            value: yield* fn()
          };
        } catch (error) {
          result = {
            ok: false,
            value: error
          };
        }
      } else {
        let resolve, reject;
        resultP = new Promise((res, rej) => {
          resolve = res;
          reject = rej;
        });
        try {
          result = {
            ok: true,
            value: yield* fn()
          };
          resultP = null;
          if (promiseReferenced) resolve(result.value);
        } catch (error) {
          result = {
            ok: false,
            value: error
          };
          resultP = null;
          if (promiseReferenced) reject(error);
        }
      }
    }
    if (result.ok) return result.value;else throw result.value;
  };
}

//# sourceMappingURL=functional.js.map
