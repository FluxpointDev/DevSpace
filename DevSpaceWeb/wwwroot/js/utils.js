Blazor.registerCustomEventType('custompaste', {
    browserEventName: 'paste',
    createEventArgs: event => {
        let text = event.clipboardData.getData('text');
        return {
            text
        };
    }
});

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

Object.defineProperty(window.Utils, 'scrollLogsUp', {
    configurable: false,
    writable: false,
    value: function (url) {
        var element = document.getElementById('gridLogs').lastChild;
        element.scroll({ top: 0, behavior: 'smooth' });
    }

});

Object.defineProperty(window.Utils, 'scrollLogsDown', {
    configurable: false,
    writable: false,
    value: function (url) {
        var element = document.getElementById('gridLogs').lastChild;
        element.scroll({ top: 99999, behavior: 'smooth' });
    }

});

Object.defineProperty(window.Utils, 'loadQR', {
    configurable: false,
    writable: false,
    value: function (url) {
        new QRCode(document.getElementById("qrcode"), {
            text: url,
            width: 128,
            height: 128,
        });
    }
});