import { RecClassFilter } from "@/pages/index/ui/RecClassFilter.tsx";
import { useContext } from "react";
import { tableContext } from "@/pages/index/lib/table-context.ts";

type Props = {
  recClasses: string[];
};

export function Filters({ recClasses }: Props) {
  const { filters, updateFilters } = useContext(tableContext);

  const onSelectRecClass = (value: string | undefined) => {
    updateFilters((f) => ({ ...f, RecClass: value }));
  };

  return (
    <div className={"flex gap-2"}>
      <div>
        <RecClassFilter
          classes={recClasses}
          onSelect={onSelectRecClass}
          value={filters?.RecClass ?? ""}
        />
      </div>
    </div>
  );
}
