import gensync from "gensync";
import loadFullConfig from "./full.js";
import { loadPartialConfig as loadPartialConfigImpl } from "./partial.js";
export { loadFullConfig as default };
import { createConfigItem as createConfigItemImpl } from "./item.js";
import { beginHiddenCallStack } from "../errors/rewrite-stack-trace.js";
const loadPartialConfigRunner = gensync(loadPartialConfigImpl);
export function loadPartialConfigAsync(...args) {
  return beginHiddenCallStack(loadPartialConfigRunner.async)(...args);
}
export function loadPartialConfigSync(...args) {
  return beginHiddenCallStack(loadPartialConfigRunner.sync)(...args);
}
export function loadPartialConfig(opts, callback) {
  if (callback !== undefined) {
    beginHiddenCallStack(loadPartialConfigRunner.errback)(opts, callback);
  } else if (typeof opts === "function") {
    beginHiddenCallStack(loadPartialConfigRunner.errback)(undefined, opts);
  } else {
    {
      throw new Error("Starting from Babel 8.0.0, the 'loadPartialConfig' function expects a callback. If you need to call it synchronously, please use 'loadPartialConfigSync'.");
    }
  }
}
function* loadOptionsImpl(opts) {
  const config = yield* loadFullConfig(opts);
  return config?.options ?? null;
}
const loadOptionsRunner = gensync(loadOptionsImpl);
export function loadOptionsAsync(...args) {
  return beginHiddenCallStack(loadOptionsRunner.async)(...args);
}
export function loadOptionsSync(...args) {
  return beginHiddenCallStack(loadOptionsRunner.sync)(...args);
}
export function loadOptions(opts, callback) {
  if (callback !== undefined) {
    beginHiddenCallStack(loadOptionsRunner.errback)(opts, callback);
  } else if (typeof opts === "function") {
    beginHiddenCallStack(loadOptionsRunner.errback)(undefined, opts);
  } else {
    {
      throw new Error("Starting from Babel 8.0.0, the 'loadOptions' function expects a callback. If you need to call it synchronously, please use 'loadOptionsSync'.");
    }
  }
}
const createConfigItemRunner = gensync(createConfigItemImpl);
export function createConfigItemAsync(...args) {
  return beginHiddenCallStack(createConfigItemRunner.async)(...args);
}
export function createConfigItemSync(...args) {
  return beginHiddenCallStack(createConfigItemRunner.sync)(...args);
}
export function createConfigItem(target, options, callback) {
  if (callback !== undefined) {
    beginHiddenCallStack(createConfigItemRunner.errback)(target, options, callback);
  } else if (typeof options === "function") {
    beginHiddenCallStack(createConfigItemRunner.errback)(target, undefined, callback);
  } else {
    {
      throw new Error("Starting from Babel 8.0.0, the 'createConfigItem' function expects a callback. If you need to call it synchronously, please use 'createConfigItemSync'.");
    }
  }
}

//# sourceMappingURL=index.js.map
