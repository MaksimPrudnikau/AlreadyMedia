import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { IndexPage } from "@/pages/index";
import { Providers } from "@/app/providers";
import "@/app/styles/App.css";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <Providers>
      <IndexPage />
    </Providers>
  </StrictMode>,
);
