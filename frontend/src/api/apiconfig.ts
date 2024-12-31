export const apiRoot = import.meta.env.DEV ? "https://localhost:7034" : "https://api.recipeshare.kuronai.dev";

export interface RequestException {
    Message: string
}