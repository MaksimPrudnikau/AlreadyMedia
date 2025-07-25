import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "@/app/styles/App.css";
import { IndexPage } from "@/pages/index";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <IndexPage />
  </StrictMode>,
);
