import { components, paths } from "@/shared/lib/api.ts";

export type NasaDatasetFilters =
  paths["/Nasa/dataset"]["get"]["parameters"]["query"];

export type NasaDatasetListResponse =
  components["schemas"]["NasaDatasetListResponse"];

export type NasaDataset = NasaDatasetListResponse["dataset"][0];
