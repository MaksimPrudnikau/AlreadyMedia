import { TableBody as UITableBody, TableCell, TableRow } from "@/shared/ui";
import { flexRender, Table } from "@tanstack/react-table";
import { NasaDataset } from "@/pages/index";

type Props = {
  table: Table<NasaDataset>;
};

export function TableBody({ table }: Props) {
  return (
    <UITableBody className={"font-normal"}>
      {table.getRowModel().rows.map((row) => {
        return (
          <TableRow key={row.id}>
            {row.getVisibleCells().map((cell) => {
              return (
                <TableCell key={cell.id} className={"p-2 text-center"}>
                  {flexRender(cell.column.columnDef.cell, cell.getContext())}
                </TableCell>
              );
            })}
          </TableRow>
        );
      })}
    </UITableBody>
  );
}
