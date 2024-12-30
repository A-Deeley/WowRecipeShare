declare global {
  interface Window {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    $WowheadPower: any
  }
}

import { createFileRoute, Link, useLoaderData } from '@tanstack/react-router'
import { GetProtectedCharacter } from '../api/blizzard/profile'
import { FormEvent, useState } from 'react'
import {
  getProfessions,
  ProfessionSkills,
  updateProfessions,
} from '../api/local/character'
import { ShowTradeSkill } from '../TradeSkillComponents'
import { ShowCraftSkill } from '../CraftSkillComponents'
export const Route = createFileRoute('/protected_character/$realm/$name')({
  loader: async ({ params: { realm, name } }) => {
    const response = await GetProtectedCharacter(name, realm)
    console.log(response)
    const professions = await getProfessions(response.id)

    return { ...response, professions }
  },
  component: RouteComponent,
})

function RouteComponent() {
  const { name, level, character_class, professions, id } = useLoaderData({
    from: '/protected_character/$realm/$name',
  })

  const [proffs, setProffs] = useState<ProfessionSkills | undefined>(
    professions?.Professions,
  )
  // const { data, isError, isPending, refetch, error } = useQuery({
  //   queryKey: [realm.slug, name],
  //   queryFn: () => getProfessions(name, realm.slug),
  // });

  const [selectedTabIndex, setSelectedTabIndex] = useState<number>(0)

  const handleUpdateProfessionsSubmit = async (
    e: FormEvent<HTMLFormElement>,
  ) => {
    e.preventDefault()

    await updateProfessions(id, e.currentTarget.file.files[0])
    setProffs((await getProfessions(id))?.Professions)
    setSelectedTabIndex(0);
  }

  return (
    <div style={{ height: '1vh' }}>
      <Link to="/">Back to home page</Link>
      <h3>{name}</h3>
      <h4>
        Level {level} - {character_class.name}
      </h4>
      <ul style={{ listStyleType: 'none', margin: 0, padding: 0 }}>
        <li
          onClick={() => setSelectedTabIndex(0)}
          style={{
            display: 'inline-block',
            marginInline: 5,
            cursor: 'pointer',
            textDecoration: selectedTabIndex == 0 ? 'underline' : 'none',
            fontSize: selectedTabIndex == 0 ? '16pt' : '12pt',
            color: 'blue',
          }}
        >
          Primary
        </li>
        <li
          onClick={() => setSelectedTabIndex(1)}
          style={{
            display: 'inline-block',
            marginInline: 5,
            cursor: 'pointer',
            textDecoration: selectedTabIndex == 1 ? 'underline' : 'none',
            fontSize: selectedTabIndex == 1 ? '16pt' : '12pt',
            color: 'blue',
          }}
        >
          Secondary
        </li>
        <li
          onClick={() => setSelectedTabIndex(2)}
          style={{
            display: 'inline-block',
            marginInline: 5,
            cursor: 'pointer',
            textDecoration: selectedTabIndex == 2 ? 'underline' : 'none',
            fontSize: selectedTabIndex == 2 ? '16pt' : '12pt',
            color: 'blue',
          }}
        >
          Update
        </li>
      </ul>

      <div style={{ maxHeight: '100px' }}>
        {selectedTabIndex == 0 && (
          <>
            <h1>Primary Professions</h1>
            <hr />
            <div
              style={{
                width: '100%',
                display: 'flex',
              }}
            >
              {proffs &&
                proffs.TradeSkills.filter(
                  (tc) => tc.Name !== 'Cooking' && tc.Name !== 'First Aid',
                ).map((p, i) => <ShowTradeSkill key={i} tradeskill={p} />)}
              {proffs &&
                proffs.CraftSkills.filter(
                  (tc) => tc.Name !== 'Cooking' && tc.Name !== 'First Aid',
                ).map((p, i) => <ShowCraftSkill key={i} tradeskill={p} />)}
            </div>
          </>
        )}
        {selectedTabIndex == 1 && (
          <>
            <h1>Secondary Professions</h1>
            <hr />
            <div
              style={{
                width: '100%',
                display: 'flex',
              }}
            >
              {proffs &&
                proffs.TradeSkills.filter(
                  (tc) => tc.Name === 'Cooking' || tc.Name === 'First Aid',
                ).map((p, i) => <ShowTradeSkill key={i} tradeskill={p} />)}
            </div>
          </>
        )}
        {selectedTabIndex == 2 && (
          <>
            <h1>Update professions</h1>
            <form
              onSubmit={handleUpdateProfessionsSubmit}
              style={{ marginBottom: 10 }}
            >
              <input type="file" name="file" accept='lua'/>
              <button>Submit</button>
            </form>
          </>
        )}
      </div>
    </div>
  )
}
