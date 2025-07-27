import { useState } from "react";
import { NasaDatasetFilters } from "@/pages/index";
import { useQuery } from "@tanstack/react-query";
import { client } from "@/shared/api/client.ts";
import { NasaTable } from "@/pages/index/ui/NasaTable.tsx";

export function IndexPage() {
  const [filters] = useState<NasaDatasetFilters>({});

  const { data, isLoading } = useQuery({
    queryKey: [""],
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
  });

  return (
    <div className={"text-2xl font-bold"}>
      <div>{isLoading ? "Loading..." : data?.pagination.totalPages}</div>
      {data && <NasaTable dataset={data.dataset} />}
    </div>
  );
}
