import { Input } from "@/shared/ui";
import React, { useContext, useState } from "react";
import { TableFilter } from "@/pages/index/ui/TableFilter.tsx";
import { NasaDatasetFilters } from "@/pages/index";
import { tableContext } from "@/pages/index/lib/table-context.ts";

export function YearFilter() {
  const { filters } = useContext(tableContext);

  const [fromYear, setFromYear] = useState(filters?.FromYear?.toString() ?? "");
  const [toYear, setToYear] = useState(filters?.ToYear?.toString() ?? "");

  const onChange = (filters: NasaDatasetFilters): NasaDatasetFilters => {
    return {
      ...filters,
      FromYear: fromYear ? +fromYear : undefined,
      ToYear: toYear ? +toYear : undefined,
    };
  };

  const onClick = (e: React.MouseEvent<HTMLDivElement>) => {
    e.stopPropagation();
  };

  return (
    <div onClick={onClick}>
      <TableFilter onChange={onChange}>
        <Input
          id={"fromYear"}
          type={"text"}
          inputMode={"numeric"}
          name={"fromYear"}
          placeholder={"Начало"}
          value={fromYear}
          onChange={(e) => {
            setFromYear(e.target.value);
          }}
        />
        <Input
          id={"toYear"}
          name={"toYear"}
          type={"text"}
          inputMode={"numeric"}
          placeholder={"Конец"}
          value={toYear}
          onChange={(e) => setToYear(e.target.value)}
        />
      </TableFilter>
    </div>
  );
}
