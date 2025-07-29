import { useContext } from "react";
import { useQuery } from "@tanstack/react-query";
import { client } from "@/shared/api/client.ts";
import { NasaTable } from "@/pages/index/ui";
import { tableContext } from "@/pages/index/lib/table-context.ts";

export function IndexPage() {
  const { filters } = useContext(tableContext);

  const { data, isFetching } = useQuery({
    queryKey: ["dataset", filters],
    queryFn: async () => {
      const { data, error } = await client.GET("/Nasa/dataset", {
        params: {
          query: filters,
        },
      });

      if (error) {
        throw new Error(error);
      }

      return data;
    },
    placeholderData: (data) => data,
  });

  if (!data && isFetching) {
    return <div>Загрузка данных...</div>;
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
