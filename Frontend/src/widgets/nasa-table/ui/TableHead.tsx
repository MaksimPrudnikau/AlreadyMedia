import {
  TableHead as UITableHead,
  TableHeader as UITableHeader,
  TableRow,
} from "@/shared/ui";
import { flexRender, Table } from "@tanstack/react-table";
import { FaCaretDown, FaCaretUp } from "react-icons/fa6";
import { TbCaretUpDownFilled } from "react-icons/tb";
import { NasaDataset } from "@/widgets/nasa-table/api";
import { YearFilter } from "@/widgets/nasa-table/ui";

type Props = {
  table: Table<NasaDataset>;
};

export function TableHeader({ table }: Props) {
  return (
    <UITableHeader className={"sticky"}>
      {table.getHeaderGroups().map((headerGroup) => (
        <TableRow key={headerGroup.id}>
          {headerGroup.headers.map((header) => {
            return (
              <UITableHead
                key={header.id}
                colSpan={header.colSpan}
                className={"p-2 text-center w-[20rem] font-bold"}
              >
                {header.isPlaceholder ? null : (
                  <div
                    className={"cursor-pointer select-none"}
                    onClick={header.column.getToggleSortingHandler()}
                  >
                    <div className={"flex items-center justify-center gap-2"}>
                      {header.id === "year" ? <YearFilter /> : null}
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
              </UITableHead>
            );
          })}
        </TableRow>
      ))}
    </UITableHeader>
  );
}
