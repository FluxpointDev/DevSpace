const Utils = {
};

Object.defineProperty(window, 'Utils', {
    configurable: false,
    writable: false,
    value: Utils
});

Object.defineProperty(window.Utils, 'getUserTimezoneOffset', {
    configurable: false,
    writable: false,
    value: function () {
        return new Date().getTimezoneOffset();
    }
});

Object.defineProperty(window.Utils, 'testWebSocket', {
    configurable: false,
    writable: false,
    value: async function () {
        let response = await fetch('/api/test/websocket', {
            method: 'GET'
        });
        if (response.ok)
            return "yes";

        if (response.status == 412)
            return "no";

        return "fail";
    }
});

Object.defineProperty(window.Utils, 'loadQR', {
    configurable: false,
    writable: false,
    value: function () {
        new QRCode(document.getElementById("qrcode"), {
            text: document.getElementById('setupurl').textContent,
            width: 128,
            height: 128,
        });
    }
});