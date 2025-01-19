
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
                credentials: 'same-origin',
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

Object.defineProperty(window.Auth, 'accountLoginExternal', {
    configurable: false,
    writable: false,
    value: async function () {
        document.getElementById('login-external-form').submit();
    }
});

Object.defineProperty(window.Auth, 'getSessionInfo', {
    configurable: false,
    writable: false,
    value: async function () {
        console.log(window.Auth.getBrowser());
        var Country = await window.Auth.getCountry();
        return {
            IsMobile: navigator.userAgent.match(/Mobi|Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i) !== null,
            Country: Country,
            BrowserType: window.Auth.getBrowser()
        };
    }
});

Object.defineProperty(window.Auth, 'getCountry', {
    configurable: false,
    writable: false,
    value: async function () {
        var countryName;

        try {
            let response = await fetch('https://ip-check-perf.radar.cloudflare.com', {
                method: 'GET',
                headers: {
                    'Accept': 'application/json'
                }
            });
            if (response.ok) {
                var json = await response.json();
                countryName = json.region + " (" + json.country + ")";
            }
            
        }
        catch { }

        if (!countryName) {
            try {
                countryName = Intl.DateTimeFormat().resolvedOptions().timeZone.split('/')[0];
            }
            catch { }
        }

        
        if (!countryName)
            return 'Unknown';
        return countryName;
    }
});

Object.defineProperty(window.Auth, 'getBrowser', {
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

Object.defineProperty(window.Auth, 'accountChangePassword', {
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

