import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import QueryProvider from "@/components/providers/QueryProvider";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Comments - SPA Application",
  description:
    "A single-page comments application with real-time updates, nested replies, full-text search, and GraphQL support. Built with .NET 9, Next.js, PostgreSQL, Redis, and Elasticsearch.",
  openGraph: {
    title: "Comments SPA",
    description:
      "Real-time comments with nested replies, full-text search & GraphQL. Built with .NET 9, Next.js, PostgreSQL, Redis, Elasticsearch, and SignalR.",
    type: "website",
    siteName: "Comments SPA",
    locale: "en_US",
  },
  twitter: {
    card: "summary_large_image",
    title: "Comments SPA",
    description:
      "Real-time comments with nested replies, full-text search & GraphQL.",
  },
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className={inter.className}>
        <QueryProvider>{children}</QueryProvider>
      </body>
    </html>
  );
}
