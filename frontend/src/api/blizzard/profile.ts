import { ProfessionSkills } from "../local/character";
import { LoginToken } from "../local/profile";

const clientId = 'ef6da78b517e49c2a609812b995abc8e';
const queryParams = {
  response_type: 'code',
  scope: 'openid wow.profile offline',
  state: 'abcde',
  redirect_uri: import.meta.env.DEV ? 'http://localhost:5173/login-bnet' : 'https://recipeshare.kuronai.dev/login-bnet',
  client_id: clientId
}

const baseUrl: string = import.meta.env.DEV ? 'https://localhost:7034' : 'https://api.recipeshare.kuronai.dev';

const authorizeUrl = `https://oauth.battle.net/authorize?${new URLSearchParams(queryParams).toString()}`;

async function GetProfileAccountSummary(): Promise<AccountProfileSummary> {

  const token = GetSessionStorageToken();

  if (token === null) throw new Error('No session token found. Please log in again.');
  
  const response = await fetch(`${baseUrl}/WowUser`, { headers: { 'X-RecipeShare-SessionId': token.session_id}});

  if (!response.ok) throw new Error(response.statusText);

  return response.json();
}

async function GetProtectedCharacter(id: string | number): Promise<ProfileCharacter> {
  const token = GetSessionStorageToken();

  if (token === null) throw new Error('No session token found. Please log in again.');

  const response = await fetch(`${baseUrl}/WowUser/${id}`, { headers: { 'X-RecipeShare-SessionId': token.session_id}});

  if (!response.ok) throw new Error(response.statusText);

  return response.json();
}

function GetSessionStorageToken(): LoginToken | null {

  const tokenStorageValue = sessionStorage.getItem("token");

  if (tokenStorageValue == null) return null;
  const token: LoginToken = JSON.parse(tokenStorageValue);

  return token;
}

interface ProfileCharacter {
  id: number,
  name: string,
  race: Link,
  character_class: Link,
  realm: Realm,
  level: number,
  professions: ProfessionSkills | null
}

interface AccountProfileSummary {
  id: number;
  wow_accounts: WowAccount[];
}

interface WowAccount {
  id: number;
  characters: Characters[];
}

interface Characters {
  id: string | number,
  level: number;
  name: string;
  gender: Link;
  faction: Link;
  playable_class: Link;
  playable_race: Link;
  realm: Realm;
}

interface Realm {
  id: number;
  name: string;
  slug: string;
}

interface Link {
  name: string;
}

export { authorizeUrl, GetProfileAccountSummary, GetProtectedCharacter, GetSessionStorageToken };

export type { Link, Realm, Characters, WowAccount, AccountProfileSummary, ProfileCharacter };
