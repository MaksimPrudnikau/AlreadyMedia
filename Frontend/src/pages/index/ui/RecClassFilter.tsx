import {
  Button,
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui";
import { MdOutlineClear } from "react-icons/md";
import { useContext } from "react";
import { tableContext } from "@/pages/index/lib/table-context.ts";

type Props = {
  classes: string[];
};

export function RecClassFilter({ classes }: Props) {
  const { filters, updateFilters } = useContext(tableContext);

  const onSelectRecClass = (value: string | undefined) => {
    updateFilters((f) => ({ ...f, RecClass: value }));
  };

  const onClearClick = () => {
    onSelectRecClass("");
  };

  return (
    <div className={"flex gap-2 items-center"}>
      <div className={"w-52"}>Класс метеорита:</div>
      <Select onValueChange={onSelectRecClass} value={filters?.RecClass}>
        <SelectTrigger className="w-[180px]">
          <SelectValue placeholder="Класс метеорита" />
        </SelectTrigger>
        <SelectContent>
          {classes.map((c) => {
            return (
              <SelectItem key={c} value={c}>
                {c}
              </SelectItem>
            );
          })}
        </SelectContent>
      </Select>
      <Button className={"p-1"} variant={"ghost"} onClick={onClearClick}>
        <MdOutlineClear size={15} />
      </Button>
    </div>
  );
}
