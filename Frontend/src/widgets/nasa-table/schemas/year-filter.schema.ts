import { z } from "zod";

const yearString = z
  .string()
  .refine(
    (x) => {
      if (!x) {
        return true;
      }

      return +x >= 1;
    },
    { error: "Cannot be less than 1" },
  )
  .refine(
    (x) => {
      if (!x) {
        return true;
      }

      return +x <= 9999;
    },
    { error: "Cannot be greater than 9999" },
  )
  .optional();

export const yearFilterSchema = z
  .object({
    start: yearString,
    end: yearString,
  })
  .refine(
    ({ start, end }) => {
      if (!start || !end) {
        return true;
      }

      return +start <= +end;
    },
    {
      error: "End must be greater than start",
      path: ["end"],
    },
  );

export type YearFilterSchema = z.infer<typeof yearFilterSchema>;
