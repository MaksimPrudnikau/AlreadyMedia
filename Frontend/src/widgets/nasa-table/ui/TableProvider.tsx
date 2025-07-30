import { tableContext } from "@/widgets/nasa-table/lib";
import { PropsWithChildren, useState } from "react";
import { NasaDatasetFilters } from "@/widgets/nasa-table/api";

export function TableProvider({ children }: PropsWithChildren) {
  const [filters, setFilters] = useState<NasaDatasetFilters>({
    ItemsPerPage: 10,
  });

  const updateFilters = (
    updater: (prev: NasaDatasetFilters) => NasaDatasetFilters,
  ) => {
    setFilters((prev) => {
      const newFilters = updater(prev);
      return { ...newFilters, Page: 0 };
    });
  };
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
    <tableContext.Provider
      value={{
        filters,
        updateFilters,
        prevPage: onPrevPage,
        nextPage: onNextPage,
      }}
    >
      {children}
    </tableContext.Provider>
  );
}
