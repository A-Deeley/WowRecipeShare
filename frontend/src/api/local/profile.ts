import { apiRoot } from "../apiconfig";
import { GetSessionStorageToken } from "../blizzard/profile";

async function getProfile(): Promise<string> {
  const response = await fetch(`${apiRoot}/Profile`);

  if (!response.ok) throw new Error(response.statusText);

  return response.json();
}

async function login(code: string):Promise<LoginToken> {
    const response = await fetch(`${apiRoot}/Profile`, { method: 'POST', body: JSON.stringify({ code }), headers: { 'Content-type': 'application/json'}});

    return response.json();
}

async function update(data: UpdateAccount):Promise<void> {
  const token = GetSessionStorageToken();

  if (token === null)
    throw new Error("No session token found. Please log in again.");

  await fetch(`${apiRoot}/Profile`, { method: 'PATCH', body: JSON.stringify(data), headers: { 'Content-type': 'application/json', "X-RecipeShare-SessionId": token.SessionId}});
  token.PreferredAccountId = data.PreferredAccountId;
  token.PreferredRealmId = data.PreferredRealmId;
  sessionStorage.setItem("token", JSON.stringify(token));
}

type UpdateAccount = {
  Id: number,
  PreferredRealmId?: number,
  PreferredAccountId?: number
}

type LoginToken = {
    SessionId: string,
    IdToken: string,
    ExpiresOn: Date,
    LastSyncedOn: Date,
    PreferredRealmId?: number,
    PreferredAccountId?: number
}


export type { LoginToken, UpdateAccount }

export { getProfile, login, update }