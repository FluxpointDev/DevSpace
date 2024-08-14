let Debug = true;
async function passkeyStartAuth(requestId) {

    // send to server for registering
    let makeAssertionOptions;
    try {
        var res = await fetch('/auth/passkey/confirm/assertionOptions', {
            method: 'POST', // or 'PUT'
            headers: {
                'Accept': 'application/json',
                'RequestVerificationToken': requestId
            }
        });

        makeAssertionOptions = await res.json();
    } catch (e) {
        console.log('Request to server failed');
        if (Debug) {
            console.log(e);
        }
        return false;
    }

    if (Debug) {
        console.log("Assertion Options Object", makeAssertionOptions);
    }


    // show options error to user
    if (makeAssertionOptions.status !== "ok") {
        console.log("Error creating assertion options");
        console.log(makeAssertionOptions.errorMessage);
        return false;
    }

    // todo: switch this to coercebase64
    const challenge = makeAssertionOptions.challenge.replace(/-/g, "+").replace(/_/g, "/");
    makeAssertionOptions.challenge = Uint8Array.from(atob(challenge), c => c.charCodeAt(0));

    // fix escaping. Change this to coerce
    makeAssertionOptions.allowCredentials.forEach(function (listItem) {
        var fixedId = listItem.id.replace(/\_/g, "/").replace(/\-/g, "+");
        listItem.id = Uint8Array.from(atob(fixedId), c => c.charCodeAt(0));
    });

    if (Debug) {
        console.log("Assertion options", makeAssertionOptions);
    }


    //Swal.fire({
    //    title: 'Logging In...',
    //    text: 'Tap your security key to login.',
    //    imageUrl: getFolder() + "/images/securitykey.min.svg",
    //    showCancelButton: true,
    //    showConfirmButton: false,
    //    focusConfirm: false,
    //    focusCancel: false
    //});

    // ask browser for credentials (browser will ask connected authenticators)
    let credential;
    try {
        credential = await navigator.credentials.get({ publicKey: makeAssertionOptions })
    } catch (err) {
        console.log(err.message ? err.message : err);

        return false;
    }

    try {
        await verifyAssertionWithServer(credential, requestId);
    } catch (e) {
        console.log('Could not verify assertion');
        if (Debug) {
            console.log(e);
        }

        return false;
    }

    return true;
}

async function verifyAssertionWithServer(assertedCredential, requestId) {

    // Move data into Arrays incase it is super long
    let authData = new Uint8Array(assertedCredential.response.authenticatorData);
    let clientDataJSON = new Uint8Array(assertedCredential.response.clientDataJSON);
    let rawId = new Uint8Array(assertedCredential.rawId);
    let sig = new Uint8Array(assertedCredential.response.signature);
    let userHandle = new Uint8Array(assertedCredential.response.userHandle);
    const data = {
        id: assertedCredential.id,
        rawId: coerceToBase64Url(rawId),
        type: assertedCredential.type,
        extensions: assertedCredential.getClientExtensionResults(),
        response: {
            authenticatorData: coerceToBase64Url(authData),
            clientDataJson: coerceToBase64Url(clientDataJSON),
            userHandle: userHandle !== null && userHandle.length > 0 ? coerceToBase64Url(userHandle) : null,
            signature: coerceToBase64Url(sig)
        }
    };

    let response;
    try {
        let res = await fetch("/auth/passkey/confirm/makeAssertion", {
            method: 'POST', // or 'PUT'
            body: JSON.stringify(data), // data can be `string` or {object}!
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'RequestVerificationToken': requestId
            }
        });

        response = await res.json();
    } catch (e) {
        console.log("Request to server failed");
        throw e;
    }
    if (Debug) {
        console.log("Assertion Object", response);
    }


    // show error
    if (response.status !== "ok") {
        console.log("Error doing assertion");
        console.log(response.errorMessage);
        return;
    }

    return response;

}
