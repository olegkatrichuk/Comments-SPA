"use client";

import type { SortDirection } from "@/lib/types";

interface SortButtonProps {
  label: string;
  active: boolean;
  direction: SortDirection;
  onClick: () => void;
}

export default function SortButton({
  label,
  active,
  direction,
  onClick,
}: SortButtonProps) {
  return (
    <button
      onClick={onClick}
      className={`inline-flex items-center gap-1 px-2 py-1 text-xs font-medium rounded transition-colors ${
        active
          ? "bg-primary-100 text-primary-700 border border-primary-300"
          : "bg-gray-100 text-gray-600 border border-gray-200 hover:bg-gray-200"
      }`}
    >
      {label}
      {active && (
        <span className="text-sm">
          {direction === "Ascending" ? "\u2191" : "\u2193"}
        </span>
      )}
    </button>
  );
}
