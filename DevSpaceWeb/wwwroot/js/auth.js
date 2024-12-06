async function accountLogin(email, password, rememberMe, requestId) {
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