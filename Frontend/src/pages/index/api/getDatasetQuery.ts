import { useQuery } from "@tanstack/react-query";
import { client } from "@/shared/api/client.ts";
import { NasaDatasetFilters } from "@/widgets/nasa-table/api";

export const useDatasetQuery = (filters: NasaDatasetFilters) =>
  useQuery({
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
