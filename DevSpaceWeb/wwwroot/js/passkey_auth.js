Object.defineProperty(window.Passkey, 'passkeyStartAuth', {
    configurable: false,
    writable: false,
    value: async function (requestId) {
        // send to server for registering
        let makeAssertionOptions;
        try {
            var res = await fetch('/auth/passkey/confirm/assertionOptions', {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'RequestId': requestId
                }
            });

            makeAssertionOptions = await res.json();
        } catch (e) {
            console.log('Request to server failed');
            if (window.Passkey.debug) {
                console.log(e);
            }
            return false;
        }

        if (window.Passkey.debug) {
            console.log("Assertion Options Object", makeAssertionOptions);
        }


        // show options error to user
        if (makeAssertionOptions.status === "error") {
            console.log("Error creating assertion options");
            console.log(makeAssertionOptions.errorMessage);
            return false;
        }

        makeAssertionOptions.challenge = window.Passkey.coerceToArrayBuffer(makeAssertionOptions.challenge);
        makeAssertionOptions.allowCredentials.forEach(function (listItem) {
            listItem.id = window.Passkey.coerceToArrayBuffer(listItem.id);
        });

        if (window.Passkey.debug) {
            console.log("Assertion options", makeAssertionOptions);
        }

        // ask browser for credentials (browser will ask connected authenticators)
        let credential;
        try {
            credential = await navigator.credentials.get({ publicKey: makeAssertionOptions })
        } catch (err) {
            console.log(err.message ? err.message : err);
            return false;
        }

        try {
            await window.Passkey.verifyAssertionWithServer(credential, requestId);
        } catch (e) {
            console.log('Could not verify assertion');
            if (window.Passkey.debug) {
                console.log(e);
            }

            return false;
        }

        return true;
    }
});


Object.defineProperty(window.Passkey, 'verifyAssertionWithServer', {
    configurable: false,
    writable: false,
    value: async function (assertedCredential, requestId) {

        let authData = new Uint8Array(assertedCredential.response.authenticatorData);
        let clientDataJSON = new Uint8Array(assertedCredential.response.clientDataJSON);
        let rawId = new Uint8Array(assertedCredential.rawId);
        let sig = new Uint8Array(assertedCredential.response.signature);

        const data = {
            id: assertedCredential.id,
            rawId: window.Passkey.coerceToBase64Url(rawId),
            type: assertedCredential.type,
            extensions: assertedCredential.getClientExtensionResults(),
            response: {
                authenticatorData: window.Passkey.coerceToBase64Url(authData),
                clientDataJson: window.Passkey.coerceToBase64Url(clientDataJSON),
                signature: window.Passkey.coerceToBase64Url(sig)
            }
        };

        let response;
        try {
            let res = await fetch("/auth/passkey/confirm/makeAssertion", {
                method: 'POST',
                body: JSON.stringify(data),
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'RequestId': requestId
                }
            });

            response = await res.json();
        } catch (e) {
            console.log("Request to server failed");
            throw e;
        }
        if (window.Passkey.debug) {
            console.log("Assertion Object", response);
        }

        // show error
        if (response.status === "error") {
            console.log("Error doing assertion");
            console.log(response.errorMessage);
            return;
        }

        return response;
    }
});
