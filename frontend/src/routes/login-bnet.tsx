import { createFileRoute, Navigate } from '@tanstack/react-router'
import { login } from '../api/local/profile'


export const Route = createFileRoute('/login-bnet')({
    validateSearch: (search) =>  search as {
        code: string
    },
    loaderDeps: ({ search: { code } }) => ({ code }),
    loader: async ({ deps: { code } }) => {
        const response = await login(code);
        sessionStorage.setItem("token", JSON.stringify(response));
    },
  component: () => <Navigate to='/'/>,
})
