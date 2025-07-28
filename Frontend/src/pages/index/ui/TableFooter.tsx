import {
  Button,
  TableCell,
  TableFooter as UITableFooter,
  TableRow,
} from "@/shared/ui";
import { useContext, useState } from "react";
import { tableContext } from "@/pages/index/lib/table-context.ts";

type Props = {
  isLoading: boolean;
  pagination: { pageIndex: number; totalPages: number };
};

export function TableFooter(props: Props) {
  const { pagination, isLoading } = props;
  const { pageIndex: page, totalPages } = pagination;
  const { prevPage, nextPage } = useContext(tableContext);

  const [loadingButton, setLoadingButton] = useState<"prev" | "next" | null>(
    null,
  );

  const onPrevPageClick = () => {
    setLoadingButton("prev");
    prevPage();
  };

  const onNextPageClick = () => {
    setLoadingButton("next");
    nextPage();
  };

  return (
    <UITableFooter>
      <TableRow>
        <TableCell>
          <div className="flex justify-end items-center gap-4">
            <span>
              Страница {page + 1} из {totalPages}
            </span>
            <div className="flex gap-3">
              <Button
                onClick={onPrevPageClick}
                disabled={isLoading || page + 1 < 2}
                loading={isLoading && loadingButton === "prev"}
              >
                Назад
              </Button>
              <Button
                onClick={onNextPageClick}
                disabled={isLoading || page + 1 === totalPages}
                loading={isLoading && loadingButton === "next"}
              >
                Вперед
              </Button>
            </div>
          </div>
        </TableCell>
      </TableRow>
    </UITableFooter>
  );
}
