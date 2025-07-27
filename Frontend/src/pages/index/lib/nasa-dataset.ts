import { components, paths } from "@/shared/lib/api.ts";

export type NasaDatasetFilters =
  paths["/Nasa/dataset"]["get"]["parameters"]["query"];

export type NasaDataset =
  components["schemas"]["NasaDatasetListResponse"]["dataset"][0];

export type NasaDatasetListResponse =
  components["schemas"]["NasaDatasetListResponse"];
