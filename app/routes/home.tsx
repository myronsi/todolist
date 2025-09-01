import type { Route } from "./+types/home";
import TodoList from "../main/ToDo";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "ToDo List" },
  ];
}

export default function Home() {
  return <TodoList />;
}
