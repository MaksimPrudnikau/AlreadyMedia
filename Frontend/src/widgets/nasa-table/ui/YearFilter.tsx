import { Input } from "@/shared/ui";
import React, { useContext, useState } from "react";
import { tableContext } from "@/widgets/nasa-table/lib";
import { NasaDatasetFilters } from "@/widgets/nasa-table/api";
import { TableFilter } from "@/widgets/nasa-table/ui";

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
