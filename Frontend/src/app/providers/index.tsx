import { PropsWithChildren } from "react";
import { QueryClientProvider } from "./QueryClientProvider.tsx";
import { TableProvider } from "@/pages/index/ui/TableProvider.tsx";

export function Providers(props: PropsWithChildren) {
  return (
    <QueryClientProvider>
      <TableProvider>{props.children}</TableProvider>
    </QueryClientProvider>
  );
}
