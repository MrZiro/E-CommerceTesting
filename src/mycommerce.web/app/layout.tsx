import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import { Navbar } from "@/components/layout/Navbar";
import { Footer } from "@/components/layout/Footer";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "MyCommerce Store",
  description: "A modern e-commerce storefront built with Next.js 16",
};

/**
 * Root application layout that renders the top-level HTML structure and applies global fonts and styles.
 *
 * @param children - The React nodes to be rendered inside the document body.
 * @returns The root JSX element containing `<html lang="en">` and `<body>` with the configured font CSS variables and antialiasing class.
 */
export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased min-h-screen flex flex-col`}
      >
        <Navbar />
        <div className="flex-grow">
            {children}
        </div>
        <Footer />
      </body>
    </html>
  );
}