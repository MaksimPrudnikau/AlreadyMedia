import { NasaDataset } from "@/pages/index";
import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  getSortedRowModel,
  SortingState,
  useReactTable,
} from "@tanstack/react-table";
import { useMemo, useState } from "react";
import { FaCaretDown, FaCaretUp } from "react-icons/fa6";
import { TbCaretUpDownFilled } from "react-icons/tb";

type Props = {
  dataset: NasaDataset[];
};

export function NasaTable(props: Props) {
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

  const table = useReactTable({
    columns,
    data: props.dataset,
    debugTable: true,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    onSortingChange: setSorting,
    state: {
      sorting,
    },
  });

  return (
    <div className="p-5 w-full flex items-center justify-center text-[1rem]">
      <table>
        <thead>
          {table.getHeaderGroups().map((headerGroup) => (
            <tr key={headerGroup.id}>
              {headerGroup.headers.map((header) => {
                return (
                  <th
                    key={header.id}
                    colSpan={header.colSpan}
                    className={"p-2 border-1 text-center w-[20rem]"}
                  >
                    {header.isPlaceholder ? null : (
                      <div
                        className={"cursor-pointer select-none"}
                        onClick={header.column.getToggleSortingHandler()}
                      >
                        <div
                          className={"flex items-center justify-center gap-2"}
                        >
                          {flexRender(
                            header.column.columnDef.header,
                            header.getContext(),
                          )}
                          <div>
                            {header.column.getIsSorted() === "asc" ? (
                              <FaCaretUp />
                            ) : header.column.getIsSorted() === "desc" ? (
                              <FaCaretDown />
                            ) : (
                              <TbCaretUpDownFilled />
                            )}
                          </div>
                        </div>
                      </div>
                    )}
                  </th>
                );
              })}
            </tr>
          ))}
        </thead>
        <tbody>
          {table
            .getRowModel()
            .rows.slice(0, 10)
            .map((row) => {
              return (
                <tr key={row.id}>
                  {row.getVisibleCells().map((cell) => {
                    return (
                      <td key={cell.id} className={"border-1 p-2 text-center"}>
                        {flexRender(
                          cell.column.columnDef.cell,
                          cell.getContext(),
                        )}
                      </td>
                    );
                  })}
                </tr>
              );
            })}
        </tbody>
      </table>
    </div>
  );
}
