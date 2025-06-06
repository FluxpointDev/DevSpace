﻿Object.defineProperty(window.Passkey, 'passkeyStartRegister', {
    configurable: false,
    writable: false,
    value: async function (name, requestId) {
        // possible values: true,false

        // send to server for registering
        let makeCredentialOptions;
        try {
            makeCredentialOptions = await window.Passkey.fetchMakeCredentialOptions(requestId);

        } catch (e) {
            if (window.Passkey.debug) {
                console.log(e);
            }
            console.log("Something went wrong with making credentials");
            return false;
        }

        if (window.Passkey.debug) {
            console.log("Credential Options Object", makeCredentialOptions);
        }



        if (makeCredentialOptions.status === "error") {
            console.log("Error creating credential options");
            console.log(makeCredentialOptions.errorMessage);
            return false;
        }

        // Turn the challenge back into the accepted format of padded base64
        makeCredentialOptions.challenge = window.Passkey.coerceToArrayBuffer(makeCredentialOptions.challenge);
        // Turn ID into a UInt8Array Buffer for some reason
        makeCredentialOptions.user.id = window.Passkey.coerceToArrayBuffer(makeCredentialOptions.user.id);

        makeCredentialOptions.excludeCredentials = makeCredentialOptions.excludeCredentials.map((c) => {
            c.id = window.Passkey.coerceToArrayBuffer(c.id);
            return c;
        });

        if (makeCredentialOptions.authenticatorSelection.authenticatorAttachment === null)
            makeCredentialOptions.authenticatorSelection.authenticatorAttachment = undefined;


        if (window.Passkey.debug) {
            console.log("Credential Options Formatted", makeCredentialOptions);
        }


        console.log("Creating PublicKeyCredential...");

        let newCredential;
        try {
            newCredential = await navigator.credentials.create({
                publicKey: makeCredentialOptions
            });
        } catch (e) {
            var msg = "Could not create credentials in browser. Probably because the username is already registered with your authenticator. Please change username or authenticator."
            console.error(msg);
            if (window.Passkey.debug) {
                console.log(e);
            }
            return false;
        }

        if (window.Passkey.debug) {
            console.log("PublicKeyCredential Created", newCredential);
        }



        try {
            await window.Passkey.registerNewCredential(name, newCredential, requestId);

        } catch (e) {
            console.log(e.message ? e.message : e);

            return false;
        }
        console.log('Success Request');
        return true;
    }
});


Object.defineProperty(window.Passkey, 'fetchMakeCredentialOptions', {
    configurable: false,
    writable: false,
    value: async function (requestId) {
        let response = await fetch('/auth/passkey/register/makeCredentialOptions', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'RequestId': requestId
            }
        });

        let data = await response.json();

        return data;
    }
});


Object.defineProperty(window.Passkey, 'registerNewCredential', {
    configurable: false,
    writable: false,
    value: async function (name, newCredential, requestId) {
        // Move data into Arrays incase it is super long
        let attestationObject = new Uint8Array(newCredential.response.attestationObject);
        let clientDataJSON = new Uint8Array(newCredential.response.clientDataJSON);
        let rawId = new Uint8Array(newCredential.rawId);

        const data = {
            name: name,
            data: {
                id: newCredential.id,
                rawId: window.Passkey.coerceToBase64Url(rawId),
                type: newCredential.type,
                extensions: newCredential.getClientExtensionResults(),
                response: {
                    AttestationObject: window.Passkey.coerceToBase64Url(attestationObject),
                    clientDataJson: window.Passkey.coerceToBase64Url(clientDataJSON),
                    transports: newCredential.response.getTransports()
                }
            }
        };

        let response;
        try {
            response = await window.Passkey.registerCredentialWithServer(data, requestId);
        } catch (e) {
            console.log("Failed to register credentials");
            if (window.Passkey.debug) {
                console.log(e);
            }
            return;
        }

        if (window.Passkey.debug) {
            console.log("Credential Object", response);
        }


        // show error
        if (response.status === "error") {
            console.log("Error creating credential");
            console.log(response.errorMessage);
            return;
        }
    }
});

Object.defineProperty(window.Passkey, 'registerCredentialWithServer', {
    configurable: false,
    writable: false,
    value: async function (formData, requestId) {
        let response = await fetch('/auth/passkey/register/makeCredential', {
            method: 'POST',
            body: JSON.stringify(formData),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'RequestId': requestId
            }
        });

        let data = await response.json();

        return data;
    }
});
