import { NasaDataset, NasaDatasetListResponse } from "@/pages/index";
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
import {
  Button,
  Table,
  TableBody,
  TableCell,
  TableFooter,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/ui";

type Props = {
  response?: NasaDatasetListResponse;
  onNextPage: () => void;
  onPrevPage: () => void;
  currentPage: number;
  isLoading: boolean;
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

  const totalPages = props.response?.pagination.totalPages ?? 0;
  const table = useReactTable({
    columns,
    data: props.response?.dataset ?? [],
    debugTable: true,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    onSortingChange: setSorting,
    state: {
      sorting,
    },
    manualPagination: true,
    pageCount: totalPages,
  });

  const [loadingButton, setLoadingButton] = useState<"prev" | "next" | null>(
    null,
  );

  const onNextPageClick = () => {
    setLoadingButton("next");
    props.onNextPage();
  };

  const onPrevPageClick = () => {
    setLoadingButton("prev");
    props.onPrevPage();
  };

  return (
    <div className="p-5 w-full flex items-center justify-center text-[1rem] flex-col gap-2">
      <Table>
        <TableHeader>
          {table.getHeaderGroups().map((headerGroup) => (
            <TableRow key={headerGroup.id}>
              {headerGroup.headers.map((header) => {
                return (
                  <TableHead
                    key={header.id}
                    colSpan={header.colSpan}
                    className={"p-2 text-center w-[20rem]"}
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
                  </TableHead>
                );
              })}
            </TableRow>
          ))}
        </TableHeader>
        <TableBody>
          {table
            .getRowModel()
            .rows.slice(0, 10)
            .map((row) => {
              return (
                <TableRow key={row.id}>
                  {row.getVisibleCells().map((cell) => {
                    return (
                      <TableCell key={cell.id} className={"p-2 text-center"}>
                        {flexRender(
                          cell.column.columnDef.cell,
                          cell.getContext(),
                        )}
                      </TableCell>
                    );
                  })}
                </TableRow>
              );
            })}
        </TableBody>
        <TableFooter>
          <TableRow>
            <TableCell>
              <div className="flex justify-end items-center gap-4">
                <span>
                  Страница {props.currentPage} из {totalPages}
                </span>
                <div className="flex gap-3">
                  <Button
                    onClick={onPrevPageClick}
                    disabled={props.isLoading || props.currentPage < 2}
                    loading={props.isLoading && loadingButton === "prev"}
                  >
                    Назад
                  </Button>
                  <Button
                    onClick={onNextPageClick}
                    disabled={
                      props.isLoading || props.currentPage === totalPages
                    }
                    loading={props.isLoading && loadingButton === "next"}
                  >
                    Вперед
                  </Button>
                </div>
              </div>
            </TableCell>
          </TableRow>
        </TableFooter>
      </Table>
    </div>
  );
}
