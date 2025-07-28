import {
  Button,
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui";
import { MdOutlineClear } from "react-icons/md";

type Props = {
  classes: string[];
  onSelect: (item: string | undefined) => void;
  initialValue?: string;
  value: string;
};

export function RecClassFilter({ classes, onSelect, value }: Props) {
  const onClearClick = () => {
    onSelect("");
  };

  return (
    <div className={"flex gap-2 items-center"}>
      <div>Класс метеорита:</div>
      <Select onValueChange={onSelect} value={value}>
        <SelectTrigger className="w-[180px]">
          <SelectValue placeholder="Клас метеорита" />
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
