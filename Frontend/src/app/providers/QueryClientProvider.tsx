import { PropsWithChildren, useState } from "react";
import {
  QueryClient,
  QueryClientProvider as TanstackQueryClientProvider,
} from "@tanstack/react-query";

export function QueryClientProvider(props: PropsWithChildren) {
  const [client] = useState(() => new QueryClient());

  return (
    <TanstackQueryClientProvider client={client}>
      {props.children}
    </TanstackQueryClientProvider>
  );
}
