/** @type {import('tailwindcss').Config} */
const appFontSans = [
  "ui-sans-serif",
  "system-ui",
  "-apple-system",
  "Segoe UI",
  "Roboto",
  "Helvetica Neue",
  "Arial",
  "sans-serif"
];

const appFontMono = [
  "ui-monospace",
  "SFMono-Regular",
  "SF Mono",
  "Menlo",
  "Consolas",
  "Liberation Mono",
  "monospace"
];

module.exports = {
  content: [
    "./src/AiEng.Platform.App/Components/**/*.razor",
    "./src/AiEng.Platform.App/Components/**/*.razor.cs",
    "./src/AiEng.Platform.App/Components/**/*.razor.css",
    "./src/AiEng.Platform.App/Components/**/*.cs",
    "./src/AiEng.Platform.App/Components/**/*.html",
    "./src/AiEng.Platform.App/Layouts/**/*.razor",
    "./src/AiEng.Platform.App/Layouts/**/*.razor.css",
    "./src/AiEng.Platform.App/Styles/**/*.css"
  ],
  theme: {
    extend: {
      colors: {
        "app-bg": "var(--app-bg)",
        "app-surface": "var(--app-surface)",
        "app-surface-2": "var(--app-surface-2)",
        "app-border": "var(--app-border)",
        "app-fg": "var(--app-fg)",
        "app-fg-muted": "var(--app-fg-muted)",
        "app-fg-subtle": "var(--app-fg-subtle)",
        "app-accent": "var(--app-accent)",
        "app-accent-fg": "var(--app-accent-fg)",
        "app-success": "var(--app-success)",
        "app-warning": "var(--app-warning)",
        "app-error": "var(--app-error)",
        "app-info": "var(--app-info)"
      },
      borderRadius: {
        "app-sm": "var(--app-radius-sm)",
        "app": "var(--app-radius)",
        "app-lg": "var(--app-radius-lg)",
        "app-xl": "var(--app-radius-xl)"
      },
      boxShadow: {
        "app-1": "var(--app-shadow-1)",
        "app-2": "var(--app-shadow-2)",
        "app-3": "var(--app-shadow-3)"
      },
      fontFamily: {
        "app-sans": appFontSans,
        "app-mono": appFontMono
      },
      fontSize: {
        "app-xs": ["11px", { lineHeight: "16px" }],
        "app-sm": ["13px", { lineHeight: "18px" }],
        "app-base": ["14px", { lineHeight: "20px" }],
        "app-md": ["15px", { lineHeight: "22px" }],
        "app-lg": ["17px", { lineHeight: "24px" }],
        "app-xl": ["20px", { lineHeight: "28px" }],
        "app-2xl": ["24px", { lineHeight: "32px" }],
        "app-3xl": ["32px", { lineHeight: "40px" }]
      }
    }
  },
  plugins: []
};
