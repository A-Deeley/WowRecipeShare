import { createFileRoute, Link, useLoaderData } from '@tanstack/react-router'
import { GetCharacter } from '../api/local/character'
import { ShowTradeSkill } from '../TradeSkillComponents'
import { useState } from 'react'

export const Route = createFileRoute('/character/$id')({
  component: RouteComponent,
  loader: async ({ params: { id } }) => {
    const response = await GetCharacter(id)
    return {
      response: response
    }
  },
})

function RouteComponent() {
  const [selectedTabIndex, setSelectedTabIndex] = useState<number>(0)
  const { response } = useLoaderData({
    from: '/character/$id',
  })
  const { CharInfo, Professions } = response;
  const displayName = `${CharInfo.Name}-${CharInfo.RealmSlug}`;

  return (
    <>
      <Link to="/">Return to the home page</Link>
      <h1>{displayName}</h1>
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
              {Professions.filter(
                (tc) => tc.Name !== 'Cooking' && tc.Name !== 'First Aid',
              ).map((p, i) => (
                <ShowTradeSkill key={i} tradeskill={p} />
              ))}
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
              {Professions.filter(
                (tc) => tc.Name === 'Cooking' || tc.Name === 'First Aid',
              ).map((p, i) => (
                <ShowTradeSkill key={i} tradeskill={p} />
              ))}
            </div>
          </>
        )}
      </div>
    </>
  )
}
