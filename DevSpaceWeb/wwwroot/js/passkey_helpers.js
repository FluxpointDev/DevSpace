const Passkey = {
};

Object.defineProperty(window, 'Passkey', {
    configurable: false,
    writable: false,
    value: Passkey
});

Object.defineProperty(Passkey, 'debug', {
    configurable: false,
    writable: false,
    value: false
});

Object.defineProperty(Passkey, 'coerceToArrayBuffer', {
    configurable: false,
    writable: false,
    value: function (thing, name) {
        if (typeof thing === "string") {
        // base64url to base64
        thing = thing.replace(/-/g, "+").replace(/_/g, "/");

        // base64 to Uint8Array
        var str = window.atob(thing);
        var bytes = new Uint8Array(str.length);
        for (var i = 0; i < str.length; i++) {
            bytes[i] = str.charCodeAt(i);
        }
        thing = bytes;
    }

    // Array to Uint8Array
    if (Array.isArray(thing)) {
        thing = new Uint8Array(thing);
    }

    // Uint8Array to ArrayBuffer
    if (thing instanceof Uint8Array) {
        thing = thing.buffer;
    }

    // error if none of the above worked
    if (!(thing instanceof ArrayBuffer)) {
        throw new TypeError("could not coerce '" + name + "' to ArrayBuffer");
    }

    return thing;
    }
});

Object.defineProperty(Passkey, 'coerceToBase64Url', {
    configurable: false,
    writable: false,
    value: function (thing) {
// Array or ArrayBuffer to Uint8Array
    if (Array.isArray(thing)) {
        thing = Uint8Array.from(thing);
    }

    if (thing instanceof ArrayBuffer) {
        thing = new Uint8Array(thing);
    }

    // Uint8Array to base64
    if (thing instanceof Uint8Array) {
        var str = "";
        var len = thing.byteLength;

        for (var i = 0; i < len; i++) {
            str += String.fromCharCode(thing[i]);
        }
        thing = window.btoa(str);
    }

    if (typeof thing !== "string") {
        throw new Error("could not coerce to string");
    }

    // base64 to base64url
    // NOTE: "=" at the end of challenge is optional, strip it off here
    thing = thing.replace(/\+/g, "-").replace(/\//g, "_").replace(/=*$/g, "");

    return thing;
    }
});

Object.defineProperty(Passkey, 'isPasskeySupported', {
    configurable: false,
    writable: false,
    value: function () {
if (window.PublicKeyCredential === undefined ||
        typeof window.PublicKeyCredential !== "function") {
        return false;
    }
    return true;
    }
});


Object.defineProperty(Passkey, 'findGetParameter', {
    configurable: false,
    writable: false,
    value: function (parameterName) {
        var result = null,
            tmp = [];
        location.search
            .substring(1)
            .split("&")
            .forEach(function (item) {
                tmp = item.split("=");
                if (tmp[0] === parameterName) result = decodeURIComponent(tmp[1]);
            });
        return result;
    }
});