function GetUserTimezoneOffset() {
    return new Date().getTimezoneOffset();
}

function LoadQR() {
    new QRCode(document.getElementById("qrcode"), {
        text: document.getElementById('setupurl').textContent,
        width: 128,
        height: 128,
    });
}