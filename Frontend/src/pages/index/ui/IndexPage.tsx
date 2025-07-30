import { useContext } from "react";
import { tableContext } from "@/widgets/nasa-table/lib";
import { NasaTable } from "@/widgets/nasa-table";
import { useDatasetQuery } from "@/pages/index/api";
import { toast } from "sonner";

export function IndexPage() {
  const { filters } = useContext(tableContext);

  const { data, isFetching, isError, error } = useDatasetQuery(filters);

  if (!data && isFetching) {
    return <div>Загрузка данных...</div>;
  }

  if (isError) {
    toast(error["title"] ?? error.message);
  }

  return (
    <div className={"p-5"}>
      <div className={"flex gap-1"}>
        <div>Количество элементов на странице (по умолчанию):</div>
        <div>{filters?.ItemsPerPage ?? data?.dataset.length}</div>
      </div>
      <NasaTable response={data} isLoading={isFetching} />
    </div>
  );
}
