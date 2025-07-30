import { z } from "zod";

export const yearFilterSchema = z
  .object({
    start: z
      .string()
      .refine((x) => +x >= 1, { error: "Cannot be less than 1" })
      .refine((x) => +x <= 9999, { error: "Cannot be greater than 9999" })
      .optional(),
    end: z
      .string()
      .refine((x) => +x >= 1, { error: "Cannot be less than 1" })
      .refine((x) => +x <= 9999, { error: "Cannot be greater than 9999" })
      .optional(),
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
