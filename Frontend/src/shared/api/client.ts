import createClient from "openapi-fetch";
import { paths } from "@/shared/lib/api.ts";

export const client = createClient<paths>({
  baseUrl: import.meta.env.VITE_BACKEND_URL as string,
});
