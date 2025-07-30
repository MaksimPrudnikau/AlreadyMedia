import { PropsWithChildren } from "react";
import { QueryClientProvider } from "./QueryClientProvider.tsx";
import { TableProvider } from "@/widgets/nasa-table/ui/TableProvider.tsx";
import { Toaster } from "@/shared/ui";

export function Providers(props: PropsWithChildren) {
  return (
    <QueryClientProvider>
      <TableProvider>{props.children}</TableProvider>
        <Toaster />
    </QueryClientProvider>
  );
}
 