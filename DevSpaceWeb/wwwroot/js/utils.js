function GetUserTimezoneOffset() {
    return new Date().getTimezoneOffset();
}

async function TestWebSocket() {
    let response = await fetch('/api/test/websocket', {
        method: 'GET'
    });
    if (response.ok)
        return "yes";

    if (response.status == 412)
        return "no";

    return "fail";
}

function LoadQR() {
    new QRCode(document.getElementById("qrcode"), {
        text: document.getElementById('setupurl').textContent,
        width: 128,
        height: 128,
    });
}