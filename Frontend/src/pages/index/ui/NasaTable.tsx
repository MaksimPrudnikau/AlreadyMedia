import { NasaDataset } from "@/pages/index";

type Props = {
  dataset: NasaDataset[];
};

export function NasaTable(props: Props) {
  if (!props.dataset) {
    return null;
  }

  return (
    <div className={"p-5 flex items-center justify-center text-[18px]"}>
      <table className={"w-2/3 text-center"}>
        <thead>
          <tr>
            <th className={"border-1"}>Год</th>
            <th className={"border-1"}>Количество матеоритов</th>
            <th className={"border-1"}>Суммарная масса</th>
          </tr>
        </thead>
        <tbody>
          {props.dataset.map((set) => {
            return (
              <tr key={set.year ?? "null"}>
                <td>{set.year}</td>
                <td>{set.mass}</td>
                <td>{set.count}</td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}
