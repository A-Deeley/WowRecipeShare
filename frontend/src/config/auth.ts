import { User } from "oidc-client-ts"

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const onSigninCallback = (_user: User | void): void => {
    window.history.replaceState( {}, document.title, window.location.pathname )
}

const oidcConfig = {
    authority: 'https://oauth.battle.net',
    disablePKCE: true,
    scope: 'openid wow.profile',
    redirect_uri: (import.meta.env.DEV ? 'http://localhost:5173' : 'http://localhost:4173'),
    client_id: 'ef6da78b517e49c2a609812b995abc8e',
    client_secret: 'RIl6iLmjR0kr9sx0bzP4K3kktHNj5vFw',
    automaticSilentRenew: true,
    onSigninCallback
}

function getUser() {
    const oidcStorage = sessionStorage.getItem(`oidc.user:${oidcConfig.authority}:${oidcConfig.client_id}`)
    if (!oidcStorage) {
        return null;
    }

    return User.fromStorageString(oidcStorage);
}

function parseJwt (token: string) {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function(c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
}

export {
    oidcConfig,
    getUser,
    parseJwt
}