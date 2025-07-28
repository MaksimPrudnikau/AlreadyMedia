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

  return (
    <div className={"p-5"}>
      <div>
        Количество строк на одной странице:{" "}
        {filters?.ItemsPerPage ?? data?.dataset.length}
      </div>
      {!data && isFetching ? (
        <div>Загрузка данных...</div>
      ) : (
        <NasaTable response={data} isLoading={isFetching} />
      )}
    </div>
  );
}
