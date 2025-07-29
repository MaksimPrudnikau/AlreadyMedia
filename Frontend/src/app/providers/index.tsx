import { PropsWithChildren } from "react";
import { QueryClientProvider } from "./QueryClientProvider.tsx";
import { TableProvider } from "@/widgets/nasa-table/ui/TableProvider.tsx";

export function Providers(props: PropsWithChildren) {
  return (
    <QueryClientProvider>
      <TableProvider>{props.children}</TableProvider>
    </QueryClientProvider>
  );
}
