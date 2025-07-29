import { tableContext } from "@/widgets/nasa-table/lib";
import { PropsWithChildren, useState } from "react";
import { NasaDatasetFilters } from "@/widgets/nasa-table/api";

export function TableProvider({ children }: PropsWithChildren) {
  const [filters, updateFilters] = useState<NasaDatasetFilters>({
    ItemsPerPage: 10,
  });

  const onNextPage = () => {
    updateFilters((f) => ({ ...f, Page: (f?.Page ?? 0) + 1 }));
  };

  const onPrevPage = () => {
    updateFilters((f) => {
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
