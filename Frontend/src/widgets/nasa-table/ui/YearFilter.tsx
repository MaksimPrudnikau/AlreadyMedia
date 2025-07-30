import {
  Button,
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuTrigger,
  Form,
  FormControl,
  FormField,
  FormItem,
  FormMessage,
  Input,
} from "@/shared/ui";
import React, { useContext, useState } from "react";
import { tableContext } from "@/widgets/nasa-table/lib";
import { useForm } from "react-hook-form";
import {
  yearFilterSchema,
  YearFilterSchema,
} from "@/widgets/nasa-table/schemas";
import { zodResolver } from "@hookform/resolvers/zod";
import { VscFilter } from "react-icons/vsc";
import { SaveIcon } from "lucide-react";

export function YearFilter() {
  const { filters, updateFilters } = useContext(tableContext);
  const [menuOpened, setMenuOpened] = useState(false);

  const onClick = (e: React.MouseEvent<HTMLDivElement>) => {
    e.stopPropagation();
  };

  const form = useForm<YearFilterSchema>({
    resolver: zodResolver(yearFilterSchema),
    defaultValues: {
      start: filters?.FromYear?.toString() ?? "",
      end: filters?.ToYear?.toString() ?? "",
    },
  });

  const onSubmit = ({ start, end }: YearFilterSchema) => {
    updateFilters((filters) => ({
      ...filters,
      FromYear: start ? +start : undefined,
      ToYear: end ? +end : undefined,
    }));

    setMenuOpened(false);
  };

  const onOpenChange = (next: boolean) => {
    if (!next && !form.formState.isValid) {
      form.reset();
      setMenuOpened(next);
    }

    setMenuOpened(next);
  };

  return (
    <div onClick={onClick}>
      <DropdownMenu open={menuOpened} onOpenChange={onOpenChange}>
        <DropdownMenuTrigger asChild>
          <Button variant={"ghost"} className={"p-1"}>
            <VscFilter />
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)}>
              <div className={"flex flex-col gap-2 p-1 max-w-52"}>
                <FormField
                  control={form.control}
                  name="start"
                  render={({ field: { ...rest } }) => (
                    <FormItem>
                      <FormControl>
                        <Input
                          type={"number"}
                          placeholder={"Начало"}
                          {...rest}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="end"
                  render={({ field }) => (
                    <FormItem>
                      <FormControl>
                        <Input
                          type={"number"}
                          placeholder={"Конец"}
                          {...field}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
              <div className={"w-full flex justify-end"}>
                <Button
                  type={"submit"}
                  variant={"secondary"}
                  disabled={!form.formState.isValid}
                >
                  <SaveIcon />
                </Button>
              </div>
            </form>
          </Form>
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  );
}

// <DropdownMenu open={menuOpened} onOpenChange={onOpenChange}>
//   <DropdownMenuTrigger asChild>
//     <Button variant={"ghost"} className={"p-1"}>
//       <VscFilter />
//     </Button>
//   </DropdownMenuTrigger>
//   <DropdownMenuContent asChild>
//     <form className={"w-28 flex flex-col gap-1"} onSubmit={onFormSubmit}>
//       {props.children}
//       <div className={"w-full flex justify-end"}>
//         <Button
//           type={"submit"}
//           variant={"secondary"}
//           disabled={props.disabled}
//         >
//           <SaveIcon />
//         </Button>
//       </div>
//     </form>
//   </DropdownMenuContent>
// </DropdownMenu>
