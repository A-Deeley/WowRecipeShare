import { apiRoot, RequestException } from "../apiconfig";
import { GetSessionStorageToken } from "../blizzard/profile";



async function getProfessions(
    id: string | number
): Promise<ApiCharRef | null> {
    const token = GetSessionStorageToken();

    if (token === null)
      throw new Error("No session token found. Please log in again.");
    
    const response = await fetch(`${apiRoot}/Character/${id}/Profession`);

    if (response.status === 404) return null;

    return response.json();
}

async function updateProfessions(
  id: string | number,
  file: File
): Promise<undefined | RequestException> {
  const token = GetSessionStorageToken();

  if (token === null)
    throw new Error("No session token found. Please log in again.");
  const formData = new FormData();
  formData.append("file", file);
  const response = await fetch(
    `${apiRoot}/Character/${id}/Profession`,
    {
      method: "post",
      headers: {
        "X-RecipeShare-SessionId": token.session_id
      },
      body: formData
    }
  );

  try {
    const body: RequestException = await response.json()
    console.log(body)
    if (body) return body;
  }
  catch{
    return undefined;
  }
}

async function GetAllCharacters(): Promise<CharacterList[]> {
  //const token = GetSessionStorageToken();

  // if (token === null)
  //   throw new Error("No session token found. Please log in again.");

  const response = await fetch(`${apiRoot}/Character`)

  return response.json();
}

async function GetCharacter(id: string | number): Promise<ApiCharRef> {
  // const token = GetSessionStorageToken();

  // if (token === null)
  //   throw new Error("No session token found. Please log in again.");

  const response = await fetch(`${apiRoot}/Character/${id}`)

  return response.json();
}

export interface CharacterList {
  character: string,
  realm: string,
  id: number
}

export interface CharacterInfo {
  GuildName?: string,
  Name: string,
  RealmSlug: string,
  CharacterId: number
}

export interface ApiCharRef {
  Professions?: ProfessionSkills,
  CharInfo: CharacterInfo
}

export interface ProfessionSkills {
  TradeSkills:  Tradeskill[],
  CraftSkills:  Craftskill[]
}

export interface Tradeskill {
    Name:       string;
    CurrentExp: number;
    MaxExp:     number;
    Items:      Item[];
    SubSpecialisation?: string;
}

export interface Craftskill {
  Name:       string;
  CurrentExp: number;
  MaxExp:     number;
  Items:      CraftItem[];
}

export interface CraftItem {
  Name:         string;
  ItemId:       number;
  Difficulty:   string;
  Reagents:     Reagent[];
}

export interface ItemCooldown {
  CooldownEnd: string,
  CooldownDuration: string
}

export interface Item {
    Name:       string;
    ItemId:     number;
    HeaderName: string;
    Difficulty: number;
    Reagents:   Reagent[];
    Cooldown?:  ItemCooldown
}

export interface Reagent {
    Name:   string;
    ItemId: number;
    Count:  number;
}




export { updateProfessions, getProfessions, GetAllCharacters, GetCharacter }