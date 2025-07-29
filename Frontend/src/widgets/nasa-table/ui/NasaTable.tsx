import { Table } from "@/shared/ui";
import {
  Filters,
  TableBody,
  TableFooter,
  TableHeader,
} from "@/widgets/nasa-table/ui";
import { NasaDatasetListResponse } from "@/widgets/nasa-table/api";
import { useDatasetTable } from "@/widgets/nasa-table/lib";

type Props = {
  response?: NasaDatasetListResponse;
  isLoading: boolean;
};

export function NasaTable(props: Props) {
  const { table, pagination } = useDatasetTable(props.response);

  return (
    <div className="p-5 w-full flex items-center justify-center text-[1rem] flex-col gap-2">
      <Filters recClasses={props.response?.recClasses ?? []} />
      <Table className={"w-full overflow-scroll h-full"}>
        <TableHeader table={table} />
        <TableBody table={table} />
        <TableFooter isLoading={props.isLoading} pagination={pagination} />
      </Table>
    </div>
  );
}
