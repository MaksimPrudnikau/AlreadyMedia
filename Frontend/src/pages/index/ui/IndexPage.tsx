import { useState } from "react";
import { NasaDatasetFilters } from "@/pages/index";
import { useQuery } from "@tanstack/react-query";
import { client } from "@/shared/api/client.ts";
import { NasaTable } from "@/pages/index/ui";

export function IndexPage() {
  const [filters, setFilters] = useState<NasaDatasetFilters>({
    ItemsPerPage: 20,
  });

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

  const onNextPage = () => {
    setFilters((f) => ({ ...f, Page: (f?.Page ?? 0) + 1 }));
  };

  const onPrevPage = () => {
    setFilters((f) => {
      if (!f?.Page) {
        return f;
      }
      if (f.Page - 1 < 0) {
        return f;
      }

      return { ...f, Page: (f?.Page ?? 0) - 1 };
    });
  };

  return (
    <div className={"p-5"}>
      <div>Количество строк на одной странице: {filters!.ItemsPerPage}</div>
      {!data && isFetching ? (
        <div>Загрузка данных...</div>
      ) : (
        <NasaTable
          response={data}
          onNextPage={onNextPage}
          onPrevPage={onPrevPage}
          isLoading={isFetching}
          filters={filters}
        />
      )}
    </div>
  );
}
