import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/disclaimer')({
  component: RouteComponent,
})

function RouteComponent() {
    return (
        <p style={{ width: '50%'}}>
            The RecipeShare website uses your Battle.net account to link characters to your login. This information is provided by Blizzard. If you change your mind and want to remove access to your account information, visit <a href='https://account.battle.net/connections#authorized-applications'>https://account.battle.net/connections#authorized-applications</a> and remove <strong>Recipe Share</strong> from your list of authorized applications.
        </p>
    )
}
