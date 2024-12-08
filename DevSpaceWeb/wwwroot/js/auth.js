async function accountLogin(email, password, rememberMe, requestId) {
    var formData = new FormData();
    formData.append('email', email);
    formData.append('password', password);
    formData.append('rememberMe', rememberMe)

    var isMobile = false;
    if (navigator.userAgent.match(/Mobi|Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i))
        isMobile = true;

    formData.append('isMobile', isMobile);
    formData.append('browser', getBrowser());
    formData.append('country', getCountry());

    try {
        let response = await fetch('/auth/login', {
            method: 'POST',
            body: formData,
            headers: {
                'Accept': 'application/json',
                'requestId': requestId
            }
        });
        if (response.ok)
            return "ok";

        if (response.status == 429)
            return "ratelimit";

        return "fail";
    }
    catch {
        return "exception";
    }

    return "fail";
}

function getCountry() {
    try {
        return Intl.DateTimeFormat().resolvedOptions().timeZone.split('/')[0];
    }
    catch { }
    return 'Unknown';
}

function getBrowser() {
    if ((navigator.userAgent.indexOf("Opera") || navigator.userAgent.indexOf('OPR')) != -1) {
        return 6;
    } else if (navigator.userAgent.indexOf("Edg") != -1) {
        return 5;
    } else if (navigator.userAgent.indexOf("Vivaldi") != -1) {
        return 7;
    } else if (navigator.userAgent.indexOf("Safari") != -1) {
        return 4;
    } else if (navigator.userAgent.indexOf("Firefox") != -1) {
        return 2;
    } else if ((navigator.userAgent.indexOf("MSIE") != -1) || (!!document.documentMode == true)) //IF IE > 10
    {
        return 1;
    } else if (navigator.userAgent.indexOf("Chrome") != -1) {
        return 3;
    } else {
        return 0;
    }
}

async function accountChangePassword(email, password, emailToken, requestId) {
    var formData = new FormData();
    formData.append('email', email);
    formData.append('password', password);
    formData.append('emailToken', emailToken);

    try {
        let response = await fetch('/auth/account/changePassword', {
            method: 'POST',
            body: formData,
            headers: {
                'Accept': 'application/json',
                'requestId': requestId
            }
        });
        return response.ok;
    }
    catch {
    }

    return false;
}