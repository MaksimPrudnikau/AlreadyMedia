import { NasaDataset, NasaDatasetListResponse } from "@/pages/index";
import {
  ColumnDef,
  getCoreRowModel,
  getSortedRowModel,
  SortingState,
  useReactTable,
} from "@tanstack/react-table";
import { useContext, useMemo, useState } from "react";
import { Table } from "@/shared/ui";
import { TableBody, TableFooter, TableHeader } from "@/pages/index/ui";
import { tableContext } from "@/pages/index/lib/table-context.ts";

type Props = {
  response?: NasaDatasetListResponse;
  isLoading: boolean;
};

export function NasaTable(props: Props) {
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

  const data = useMemo(() => props.response?.dataset ?? [], [props.response]);
  const pagination = useMemo(
    () => ({
      pageIndex: filters?.Page ?? 0,
      pageSize: filters?.ItemsPerPage ?? 0,
      totalPages: props.response?.pagination.totalPages ?? 0,
    }),
    [filters, props.response],
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

  return (
    <div className="p-5 w-full flex items-center justify-center text-[1rem] flex-col gap-2">
      <Table className={"w-full overflow-scroll h-full"}>
        <TableHeader table={table} />
        <TableBody table={table} />
        <TableFooter isLoading={props.isLoading} pagination={pagination} />
      </Table>
    </div>
  );
}
