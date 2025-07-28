import { VscFilter } from "react-icons/vsc";
import {
  Button,
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from "@/shared/ui";
import { FormEvent, PropsWithChildren, useContext, useState } from "react";
import { SaveIcon } from "lucide-react";
import { tableContext } from "@/pages/index/lib/table-context.ts";
import { NasaDatasetFilters } from "@/pages/index";

type Props = {
  onChange: (oldFilters: NasaDatasetFilters) => NasaDatasetFilters;
  disabled?: boolean;
} & PropsWithChildren;

export function TableFilter(props: Props) {
  const { updateFilters } = useContext(tableContext);
  const [menuOpened, setMenuOpened] = useState(false);

  const onFormSubmit = (e?: FormEvent<HTMLFormElement>) => {
    e?.preventDefault();
    e?.stopPropagation();

    updateFilters(props.onChange);
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
          {props.children}
          <div className={"w-full flex justify-end"}>
            <Button
              type={"submit"}
              variant={"secondary"}
              disabled={props.disabled}
            >
              <SaveIcon />
            </Button>
          </div>
        </form>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
