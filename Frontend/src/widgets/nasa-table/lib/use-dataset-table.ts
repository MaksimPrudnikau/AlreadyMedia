import { useContext, useMemo, useState } from "react";
import { tableContext } from "@/widgets/nasa-table/lib/table-context.ts";
import {
  ColumnDef,
  getCoreRowModel,
  getSortedRowModel,
  SortingState,
  useReactTable,
} from "@tanstack/react-table";
import { NasaDataset, NasaDatasetListResponse } from "@/widgets/nasa-table/api";

export const useDatasetTable = (response?: NasaDatasetListResponse) => {
  const { filters } = useContext(tableContext);

  const [sorting, setSorting] = useState<SortingState>([
    {
      id: "year",
      desc: false,
    },
  ]);

  const columns = useMemo<ColumnDef<NasaDataset>[]>(
    () => [
      {
        accessorKey: "year",
        header: "Год",
        cell: (info) => {
          const year = info.getValue<number | null>();
          return year ?? "Не указан";
        },
      },
      {
        accessorKey: "count",
        header: "Количество метеоритов",
        cell: (info) => info.getValue<number>(),
      },
      {
        accessorKey: "mass",
        header: "Суммарная масса",
        cell: (info) => {
          const mass = info.getValue<number>();
          return mass.toFixed(2);
        },
      },
    ],
    [],
  );

  const data = useMemo(() => response?.dataset ?? [], [response]);
  const pagination = useMemo(
    () => ({
      pageIndex: filters?.Page ?? 0,
      pageSize: filters?.ItemsPerPage ?? 0,
      totalPages: response?.pagination.totalPages ?? 0,
    }),
    [filters, response],
  );

  const table = useReactTable({
    columns,
    data,
    debugTable: true,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    onSortingChange: setSorting,
    state: {
      sorting,
      pagination,
    },
    manualPagination: true,
    pageCount: pagination.totalPages,
  });

  return { table, pagination };
};
