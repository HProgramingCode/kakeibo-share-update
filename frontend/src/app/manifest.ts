import type { MetadataRoute } from "next";

export default function manifest(): MetadataRoute.Manifest {
  return {
    name: "かぞく精算帳",
    short_name: "精算帳",
    description: "家族の立て替えを記録して、最小回数で精算するアプリ。",
    start_url: "/",
    display: "standalone",
    background_color: "#f7f7f4",
    theme_color: "#1a9b5e",
    lang: "ja",
    orientation: "portrait",
    icons: [
      {
        src: "/icon.svg",
        sizes: "any",
        type: "image/svg+xml",
        purpose: "any",
      },
      {
        src: "/icon-maskable.svg",
        sizes: "any",
        type: "image/svg+xml",
        purpose: "maskable",
      },
    ],
  };
}
