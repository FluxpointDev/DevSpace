
try { let Debug = true; }
catch { }

async function passkeyStartRegister(name, requestId) {

    // possible values: true,false

    // send to server for registering
    let makeCredentialOptions;
    try {
        makeCredentialOptions = await fetchMakeCredentialOptions(requestId);

    } catch (e) {
        if (Debug) {
            console.log(e);
        }
        console.log("Something went wrong with making credentials");
        return false;
    }

    if (Debug) {
        console.log("Credential Options Object", makeCredentialOptions);
    }



    if (makeCredentialOptions.status !== "ok") {
        console.log("Error creating credential options");
        console.log(makeCredentialOptions.errorMessage);
        return false;
    }

    // Turn the challenge back into the accepted format of padded base64
    makeCredentialOptions.challenge = coerceToArrayBuffer(makeCredentialOptions.challenge);
    // Turn ID into a UInt8Array Buffer for some reason
    makeCredentialOptions.user.id = coerceToArrayBuffer(makeCredentialOptions.user.id);

    makeCredentialOptions.excludeCredentials = makeCredentialOptions.excludeCredentials.map((c) => {
        c.id = coerceToArrayBuffer(c.id);
        return c;
    });

    if (makeCredentialOptions.authenticatorSelection.authenticatorAttachment === null)
        makeCredentialOptions.authenticatorSelection.authenticatorAttachment = undefined;


    if (Debug) {
        console.log("Credential Options Formatted", makeCredentialOptions);
    }


    //Swal.fire({
    //    title: 'Registering...',
    //    text: 'Tap your security key to finish registration.',
    //    imageUrl: getFolder() + "/images/securitykey.min.svg",
    //    showCancelButton: true,
    //    showConfirmButton: false,
    //    focusConfirm: false,
    //    focusCancel: false
    //});


    console.log("Creating PublicKeyCredential...");

    let newCredential;
    try {
        newCredential = await navigator.credentials.create({
            publicKey: makeCredentialOptions
        });
    } catch (e) {
        var msg = "Could not create credentials in browser. Probably because the username is already registered with your authenticator. Please change username or authenticator."
        console.error(msg);
        if (Debug) {
            console.log(e);
        }
        return false;
    }

    if (Debug) {
        console.log("PublicKeyCredential Created", newCredential);
    }



    try {
        registerNewCredential(name, newCredential, requestId);

    } catch (e) {
        console.log(e.message ? e.message : e);

        return false;
    }

    return true;
}

async function fetchMakeCredentialOptions(requestId) {

    let response = await fetch('/auth/passkey/register/makeCredentialOptions', {
        method: 'POST', // or 'PUT'
        headers: {
            'Accept': 'application/json',
            'RequestVerificationToken': requestId
        }
    });

    let data = await response.json();

    return data;
}


// This should be used to verify the auth data with the server
async function registerNewCredential(name, newCredential, requestId) {
    // Move data into Arrays incase it is super long
    let attestationObject = new Uint8Array(newCredential.response.attestationObject);
    let clientDataJSON = new Uint8Array(newCredential.response.clientDataJSON);
    let rawId = new Uint8Array(newCredential.rawId);

    const data = {
        name: name,
        data: {
            id: newCredential.id,
            rawId: coerceToBase64Url(rawId),
            type: newCredential.type,
            extensions: newCredential.getClientExtensionResults(),
            response: {
                AttestationObject: coerceToBase64Url(attestationObject),
                clientDataJson: coerceToBase64Url(clientDataJSON)
            }
        }
    };

    let response;
    try {
        response = await registerCredentialWithServer(data, requestId);
    } catch (e) {
        console.log("Failed to register credentials");
        if (Debug) {
            console.log(e);
        }
        return;
    }

    if (Debug) {
        console.log("Credential Object", response);
    }


    // show error
    if (response.status !== "ok") {
        console.log("Error creating credential");
        console.log(response.errorMessage);
        return;
    }

    // show success 
    //Swal.fire({
    //    title: 'Registration Successful!',
    //    text: 'You\'ve registered successfully.',
    //    // type: 'success',
    //    timer: 2000
    //});

    // redirect to dashboard?
    //window.location.href = "/dashboard/" + state.user.displayName;
}

async function registerCredentialWithServer(formData, requestId) {
    let response = await fetch('/auth/passkey/register/makeCredential', {
        method: 'POST',
        body: JSON.stringify(formData),
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'RequestVerificationToken': requestId
        }
    });

    let data = await response.json();

    return data;
}
