import { createFileRoute, Link } from '@tanstack/react-router'

export const Route = createFileRoute('/addon')({
  component: RouteComponent,
})

function RouteComponent() {
  return (
    <div>
        <Link to='/'>Back to home page</Link>
        <h1>Instructions</h1>
        
    </div>
  )
}
