import { useQuery } from "@tanstack/react-query";
import { GetAllCharacters } from "../api/local/character";
import { Link } from "@tanstack/react-router";

export function CharacterList() {

    const allCharactersQuery = useQuery({ queryKey: ['characters'], queryFn: GetAllCharacters});

    return (
        <div style={{ display: 'flex', gap: 10, flexDirection: 'column'}}>
              {allCharactersQuery.data?.map(c => <Link key={`${c.Realm}-${c.Character}`} to={`/character/$id`} params={{ id: `${c.Id}` }}>{c.Character}-{c.Realm}</Link>)}
            </div>
    )
}
