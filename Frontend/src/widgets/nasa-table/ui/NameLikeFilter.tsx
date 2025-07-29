import { Button, Input } from "@/shared/ui";
import { ChangeEvent, FormEvent, useContext, useState } from "react";
import { CiSearch } from "react-icons/ci";
import { tableContext } from "@/widgets/nasa-table/lib";

export function NameLikeFilter() {
  const { filters, updateFilters } = useContext(tableContext);

  const [value, setValue] = useState(filters?.NameContains ?? "");
  const onChange = (e: ChangeEvent<HTMLInputElement>) => {
    setValue(e.target.value);
  };

  const updateFilter = () => {
    updateFilters((f) => ({ ...f, NameContains: value }));
  };

  const onSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    updateFilter();
  };

  return (
    <div className={"flex gap-2 items-center"}>
      <div className={"w-52"}>Часть названия метеорита:</div>
      <form className={"flex gap-2"} onSubmit={onSubmit}>
        <div className={"w-52"}>
          <Input
            type={"text"}
            placeholder={"Часть названия"}
            value={value}
            onChange={onChange}
          />
        </div>

        <div>
          <Button className={"p-2"} type={"submit"} variant={"secondary"}>
            <CiSearch size={15} />
          </Button>
        </div>
      </form>
    </div>
  );
}
