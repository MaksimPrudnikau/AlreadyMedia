import { VscFilter } from "react-icons/vsc";
import {
  Button,
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuTrigger,
  Input,
} from "@/shared/ui";
import { FormEvent, useContext, useState } from "react";
import { SaveIcon } from "lucide-react";
import { tableContext } from "@/pages/index/lib/table-context.ts";

export function YearFilter() {
  const { filters, updateFilters } = useContext(tableContext);
  const [menuOpened, setMenuOpened] = useState(false);
  const [fromYear, setFromYear] = useState(filters?.FromYear);
  const [toYear, setToYear] = useState(filters?.ToYear);

  const onFormSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    e.stopPropagation();

    updateFilters((f) => ({ ...f, FromYear: fromYear, ToYear: toYear }));
    setMenuOpened(false);
  };

  return (
    <DropdownMenu open={menuOpened} onOpenChange={setMenuOpened}>
      <DropdownMenuTrigger asChild>
        <Button variant={"ghost"} className={"p-1"}>
          <VscFilter />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent asChild>
        <form className={"w-28 flex flex-col gap-1"} onSubmit={onFormSubmit}>
          <Input
            id={"fromYear"}
            type={"text"}
            inputMode={"numeric"}
            name={"fromYear"}
            placeholder={"Начало"}
            value={fromYear}
            onChange={(e) => {
              setFromYear(+e.target.value);
            }}
          />
          <Input
            id={"toYear"}
            name={"toYear"}
            type={"text"}
            inputMode={"numeric"}
            placeholder={"Конец"}
            value={toYear}
            onChange={(e) => setToYear(+e.target.value)}
          />
          <div className={"w-full flex justify-end"}>
            <Button
              type={"submit"}
              variant={"secondary"}
              disabled={
                fromYear !== undefined &&
                toYear !== undefined &&
                fromYear > toYear
              }
            >
              <SaveIcon />
            </Button>
          </div>
        </form>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
