
const Auth = {
};

Object.defineProperty(window, 'Auth', {
    configurable: false,
    writable: false,
    value: Auth
});

Object.defineProperty(window.Auth, 'accountLogin', {
    configurable: false,
    writable: false,
    value: async function (email, password, rememberMe, requestId) {
        var formData = new FormData();
        formData.append('email', email);
        formData.append('password', password);
        formData.append('rememberMe', rememberMe)

        try {
            let response = await fetch('/auth/login', {
                method: 'POST',
                body: formData,
                headers: {
                    'Accept': 'application/json',
                    'RequestId': requestId
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
});

Object.defineProperty(Auth, 'accountLoginExternal', {
    configurable: false,
    writable: false,
    value: async function (provider, rememberMe, requestId) {
        var formData = new FormData();
        formData.append('provider', provider);
        formData.append('rememberMe', rememberMe)

        try {
            let response = await fetch('/auth/login/external', {
                method: 'POST',
                body: formData,
                headers: {
                    'Accept': 'application/json',
                    'RequestId': requestId
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
});

Object.defineProperty(Auth, 'getSessionInfo', {
    configurable: false,
    writable: false,
    value: function () {
        console.log(window.Auth.getBrowser());
        return {
            IsMobile: navigator.userAgent.match(/Mobi|Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i) !== null,
            Country: window.Auth.getCountry(),
            BrowserType: window.Auth.getBrowser()
        };
    }
});

Object.defineProperty(Auth, 'getCountry', {
    configurable: false,
    writable: false,
    value: function () {
        var countryName;
        try {
            countryName = Intl.DateTimeFormat().resolvedOptions().timeZone.split('/')[0];
        }
        catch { }
        if (!countryName)
            return 'Unknown';
        return countryName;
    }
});

Object.defineProperty(Auth, 'getBrowser', {
    configurable: false,
    writable: false,
    value: function () {
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
});

Object.defineProperty(Auth, 'accountChangePassword', {
    configurable: false,
    writable: false,
    value: async function (email, password, emailToken, requestId) {
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
});

