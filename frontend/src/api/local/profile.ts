const apiRoot = import.meta.env.DEV ? 'https://localhost:7034' : 'https://api.recipeshare.kuronai.dev';

async function getProfile(): Promise<string> {
  const response = await fetch(`${apiRoot}/Profile`);

  if (!response.ok) throw new Error(response.statusText);

  return response.json();
}

async function login(code: string):Promise<LoginToken> {
    const response = await fetch(`${apiRoot}/Profile`, { method: 'POST', body: JSON.stringify({ code }), headers: { 'Content-type': 'application/json'}});

    return response.json();
}

type LoginToken = {
    session_id: string,
    id_token: string,
    expires_in: number
}

export type { LoginToken }

export { getProfile, login }