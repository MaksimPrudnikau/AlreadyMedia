import { NameLikeFilter, RecClassFilter } from "@/widgets/nasa-table/ui";

type Props = {
  recClasses: string[];
};

export function Filters({ recClasses }: Props) {
  return (
    <div className={"flex flex-col gap-2 min-w-52"}>
      <RecClassFilter classes={recClasses} />
      <NameLikeFilter />
    </div>
  );
}
