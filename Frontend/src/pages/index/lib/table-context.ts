import { createContext } from "react";
import { NasaDatasetFilters } from "@/pages/index";

export type TableContext = {
  filters: NasaDatasetFilters;
  updateFilters: (
    oldFilters: (f: NasaDatasetFilters) => NasaDatasetFilters,
  ) => void;

  prevPage: () => void;
  nextPage: () => void;
};

export const tableContext = createContext<TableContext>({
  filters: {},
  updateFilters: () => ({}),
  prevPage: () => ({}),
  nextPage: () => ({}),
});
