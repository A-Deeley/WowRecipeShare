import { createFileRoute, Link } from "@tanstack/react-router";
import {
  authorizeUrl,
  Characters,
  GetProfileAccountSummary,
  Realm,
  WowAccount
} from "../api/blizzard/profile";
import '../wow.css';
import { useEffect, useState } from "react";
import { LoginToken, update } from "../api/local/profile";
import { useQuery } from "@tanstack/react-query";
import { parseJwt } from "../config/auth";
import { CharacterList } from "../CharacterList/CharacterList";

export const Route = createFileRoute("/")({
  component: Index,
});

function GetRealms(account: WowAccount): Realm[] {
  const realmList: Realm[] = [];
  account.characters.forEach(character => {
    if (!realmList.find(r => r.id === character.realm.id))
      realmList.push(character.realm)
  });
  return realmList;
}

function Index() {

  const tryGetLoginToken = (): LoginToken | null => {
    const loginToken = sessionStorage.getItem("token");

    if (loginToken === null) return null;

    return JSON.parse(loginToken);
  }
  const [auth, setAuth] = useState<LoginToken | null>(tryGetLoginToken());
  const isAuthenticated = auth !== null;
  const btag = auth && parseJwt(auth?.IdToken).battle_tag;


  const { data, isPending, isError } = useQuery({ queryKey: ['wowaccount'], queryFn: GetProfileAccountSummary, enabled: isAuthenticated})
  const [wowAccount, setWowAccount] = useState<WowAccount | undefined>();
  const [realms, setRealms] = useState<Realm[] | undefined>();
  const [realm, setRealm] = useState<Realm | undefined>();

  useEffect(() => {
    if (data)
      setWowAccount(data.wow_accounts[0]);
  }, [data])

  useEffect(() => {
    if (wowAccount)
      setRealms(GetRealms(wowAccount));
    else
      setRealms(undefined);
  }, [wowAccount, data])

  useEffect(() => {
    const favoriteRealm = realms?.find(r => r.id === auth?.PreferredRealmId);
    if (favoriteRealm)
      setRealm(favoriteRealm)
    else
      setRealm(undefined);
  }, [auth?.PreferredRealmId, realms])

  const handleFavoriteRealmChanged = async (realmId: number) => {
    await update({ Id: data!.id, PreferredRealmId: realmId, PreferredAccountId: wowAccount?.id});
    setAuth({...auth!, PreferredAccountId: wowAccount?.id, PreferredRealmId: realmId});
  }

  if (isAuthenticated && !isPending && !isError) {
    return (
      <div style={{ padding: '1rem', display: 'flex', flexDirection: 'column', gap: 10, alignItems: 'start', width: '30%'}}>
        <h3>Welcome {btag}</h3>
        <Link to="/addon">How do I upload my character's professions?</Link>
        {data?.wow_accounts && data.wow_accounts.length > 1 && 
        <>
        <span>Wow Account: </span>
        <select value={wowAccount?.id} onChange={(e) => setWowAccount(data.wow_accounts.find(w => w.id === +e.currentTarget.value))} >
          {data.wow_accounts.map((acct, i) => <option value={+acct.id} key={i}>{acct.id}</option>)}
        </select>
        </>}
        {realms &&
        <>
        <span>Realm: </span>
        <select value={realm?.id ?? 'none'} onChange={(e) => setRealm(realms.find(r => r.id === +e.currentTarget.value))}>
          <option>-</option>
          {realms.map((r, i) => <option value={+r.id} key={i}>{r.name} {r.id === auth!.PreferredRealmId && '(Favorite)'}</option>)}
        </select>
        </>
        }
        {realm && <ProtectedCharacterList isFavorite={realm.id === auth!.PreferredRealmId} setFavoriteRealm={handleFavoriteRealmChanged} realm={realm} characters={wowAccount?.characters.filter(c => c.realm.name === realm.name)}/>}
          <hr style={{ width: '100%'}}/>
          <h4>Characters others have uploaded</h4>
          <CharacterList />
      </div>
    )
  }

  return (
    <div className="p-2">
      <h3>Welcome to RecipeShare!</h3>
      <p>This website was made as a way to share profession data between characters.<br/>Made by Kuronai-Dreamscythe &lt;ThinkinBoutThosBeans&gt;</p>
      <p style={{ border: '1px solid red', borderRadius: 5, width: 'fit-content', padding: 10 }}>
        <span style={{ color: 'red' }}>Disclaimer: Starting in this version, some data is now being stored. None of the information stored is shown to other users.</span><br/>
        <span>Here is the complete list of data being stored:</span><br/>
        <ul>
          <li>Last time you logged in to this website</li>
          <li>Last time this website contacted blizzard to update your character data <br />(when you login, if it's been longer than one day.)</li>
          <li>Your BattleTag (only shown to you, on your home page)</li>
          <li>Your account's unique identifier (this value is unique to you and is never shown)</li>
          <li>This website's user preferences: Favorited realm</li>
        </ul>
        <span style={{ color: 'orange' }}>Reminder: This only applies if you log in. Feel free to browse anonymously.</span>
      </p>
      <a href={authorizeUrl}>Login with BattleNet</a> (<Link style={{ fontSize: 12}} to="/disclaimer">What is this?</Link>)
      <hr />
      <h4>Characters others have uploaded</h4>
      <CharacterList />
    </div>
  );
}

interface CharacterListProps {
  characters: Characters[] | undefined;
  realm: Realm;
  isFavorite: boolean;
  setFavoriteRealm: (realmId: number) => void
}

function ProtectedCharacterList({ characters, realm, setFavoriteRealm, isFavorite }: CharacterListProps) {
  if (characters === undefined) return (<span>No characters found for realm {realm.name}.</span>);

  const sorted = characters.sort((a, b) => b.level - a.level);

  if (characters.length == 0) return <></>;

  return (
    <>
      <h2>{realm.name} {isFavorite ? '(Favorite)' : <button onClick={() => setFavoriteRealm(realm.id)}>Set as favorite</button>}</h2>
      <table width={"20%"}>
        <thead>
          <tr>
            <th>Name</th>
            <th>Level</th>
            <th>Class</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {sorted.map((c, i) => (
            <tr key={i}>
              <td>{c.name}</td>
              <td>{c.level}</td>
              <td className={`class-bg ${c.playable_class.name} class`}>{c.playable_class.name}</td>
              <td><Link to="/protected_character/$realm/$name" params={{ realm: c.realm.slug, name: c.name }}>Details</Link></td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}
