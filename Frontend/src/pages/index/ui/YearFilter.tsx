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
  const [fromYear, setFromYear] = useState(filters?.FromYear?.toString() ?? "");
  const [toYear, setToYear] = useState(filters?.ToYear?.toString() ?? "");

  const onFormSubmit = (e?: FormEvent<HTMLFormElement>) => {
    e?.preventDefault();
    e?.stopPropagation();

    updateFilters((f) => ({
      ...f,
      FromYear: fromYear ? +fromYear : undefined,
      ToYear: toYear ? +toYear : undefined,
    }));
    setMenuOpened(false);
  };

  const onOpenChange = (next: boolean) => {
    if (!next) {
      onFormSubmit();
      return;
    }

    setMenuOpened(true);
  };

  return (
    <DropdownMenu open={menuOpened} onOpenChange={onOpenChange}>
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
            allowClear={true}
            onChange={(e) => {
              setFromYear(e.target.value);
            }}
          />
          <Input
            id={"toYear"}
            name={"toYear"}
            type={"text"}
            inputMode={"numeric"}
            placeholder={"Конец"}
            value={toYear}
            allowClear={true}
            onChange={(e) => setToYear(e.target.value)}
          />
          <div className={"w-full flex justify-end"}>
            <Button
              type={"submit"}
              variant={"secondary"}
              disabled={!!fromYear && !!toYear && fromYear > toYear}
            >
              <SaveIcon />
            </Button>
          </div>
        </form>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
