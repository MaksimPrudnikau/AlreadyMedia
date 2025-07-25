import { PropsWithChildren } from "react";
import { QueryClientProvider } from "./QueryClientProvider.tsx";

export function Providers(props: PropsWithChildren) {
  return <QueryClientProvider>{props.children}</QueryClientProvider>;
}
