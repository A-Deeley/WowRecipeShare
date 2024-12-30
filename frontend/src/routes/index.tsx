import { createFileRoute, Link } from "@tanstack/react-router";
import {
  authorizeUrl,
  Characters,
  GetProfileAccountSummary,
  WowAccount
} from "../api/blizzard/profile";
import '../wow.css';
import { useEffect, useState } from "react";
import { LoginToken } from "../api/local/profile";
import { useQuery } from "@tanstack/react-query";
import { parseJwt } from "../config/auth";
import { CharacterList } from "../CharacterList/CharacterList";

export const Route = createFileRoute("/")({
  component: Index,
});

function GetRealms(account: WowAccount): string[] {
  return [...new Set(account?.characters.map(c => c.realm.name))];
}

function Index() {

  const tryGetLoginToken = (): LoginToken | null => {
    const loginToken = sessionStorage.getItem("token");

    if (loginToken === null) return null;

    return JSON.parse(loginToken);
  }
  const [auth] = useState<LoginToken | null>(tryGetLoginToken());
  const isAuthenticated = auth !== null;
  const btag = auth && parseJwt(auth?.id_token).battle_tag;


  const { data, isPending, isError } = useQuery({ queryKey: ['wowaccount'], queryFn: GetProfileAccountSummary, enabled: isAuthenticated})
  const [wowAccount, setWowAccount] = useState<WowAccount | undefined>();
  const [realms, setRealms] = useState<string[] | undefined>();
  const [realm, setRealm] = useState<string | undefined>();

  useEffect(() => {
    if (data)
      setWowAccount(data.wow_accounts[0]);
  }, [data])

  useEffect(() => {
    if (wowAccount)
      setRealms(GetRealms(wowAccount));
    else
      setRealms(undefined);

    setRealm(undefined);
  }, [wowAccount, data])

  if (isAuthenticated && !isPending && !isError) {
    return (
      <div style={{ padding: '1rem', display: 'flex', flexDirection: 'column', gap: 10, alignItems: 'start', width: '30%'}}>
        <h3>Welcome {btag}</h3>
        <Link to="/addon">How do I upload my character's professions?</Link>
        {data?.wow_accounts && data.wow_accounts.length > 1 && 
        <>
        <span>Wow Account: </span>
        <select value={wowAccount?.id} onChange={(e) => setWowAccount(data.wow_accounts.find(w => w.id === +e.currentTarget.value))}>
          {data.wow_accounts.map((acct, i) => <option value={acct.id} key={i}>{acct.id}</option>)}
        </select>
        </>}
        {realms &&
        <>
        <span>Realm: </span>
        <select value={realm ?? 'none'} onChange={(e) => setRealm(realms.find(r => r === e.currentTarget.value))}>
          <option value={'none'}>-</option>
        {realms.map((r, i) => <option value={r} key={i}>{r}</option>)}
        </select>
        </>
        }
        {realm && <ProtectedCharacterList realm={realm} characters={wowAccount?.characters.filter(c => c.realm.name === realm)}/>}
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
      <a href={authorizeUrl}>Login with BattleNet</a> (<Link style={{ fontSize: 12}} to="/disclaimer">What is this?</Link>)
      <hr />
      <h4>Characters others have uploaded</h4>
      <CharacterList />
    </div>
  );
}

interface CharacterListProps {
  characters: Characters[] | undefined;
  realm: string;
}

function ProtectedCharacterList({ characters, realm }: CharacterListProps) {
  if (characters === undefined) return (<span>No characters found for realm {realm}.</span>);

  const sorted = characters.sort((a, b) => b.level - a.level);

  if (characters.length == 0) return <></>;

  return (
    <>
      <h2>{realm}</h2>
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
