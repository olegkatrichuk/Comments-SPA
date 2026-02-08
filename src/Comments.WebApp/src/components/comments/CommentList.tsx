"use client";

import { useState, useCallback } from "react";
import { useComments } from "@/hooks/useComments";
import { useSignalR } from "@/hooks/useSignalR";
import type { SortField, SortDirection } from "@/lib/types";
import CommentThread from "./CommentThread";
import Pagination from "@/components/ui/Pagination";
import SortButton from "@/components/ui/SortButton";
import SearchBar from "@/components/ui/SearchBar";

export default function CommentList() {
  const [page, setPage] = useState(1);
  const [pageSize] = useState(25);
  const [sortField, setSortField] = useState<SortField>("CreatedAt");
  const [sortDirection, setSortDirection] =
    useState<SortDirection>("Descending");
  const [searchQuery, setSearchQuery] = useState("");

  // Connect to SignalR for real-time updates
  useSignalR();

  const { data, isLoading, isError, error } = useComments({
    page,
    pageSize,
    sortField,
    sortDirection,
    searchQuery,
  });

  const handleSearch = useCallback((query: string) => {
    setSearchQuery(query);
    setPage(1);
  }, []);

  const handleSortClick = (field: SortField) => {
    if (sortField === field) {
      setSortDirection((prev) =>
        prev === "Ascending" ? "Descending" : "Ascending"
      );
    } else {
      setSortField(field);
      setSortDirection("Descending");
    }
    setPage(1);
  };

  const totalPages = data ? Math.ceil(data.totalCount / pageSize) : 0;

  return (
    <div className="space-y-4">
      {/* Search + Sort controls */}
      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3">
        <div className="w-full sm:w-72">
          <SearchBar onSearch={handleSearch} />
        </div>

        <div className="flex items-center gap-2">
          <span className="text-xs text-gray-500 font-medium">Sort by:</span>
          <SortButton
            label="User Name"
            active={sortField === "UserName"}
            direction={sortDirection}
            onClick={() => handleSortClick("UserName")}
          />
          <SortButton
            label="Email"
            active={sortField === "Email"}
            direction={sortDirection}
            onClick={() => handleSortClick("Email")}
          />
          <SortButton
            label="Date"
            active={sortField === "CreatedAt"}
            direction={sortDirection}
            onClick={() => handleSortClick("CreatedAt")}
          />
        </div>
      </div>

      {/* Comments count */}
      {data && (
        <p className="text-sm text-gray-500">
          {data.totalCount === 0
            ? "No comments yet"
            : `${data.totalCount} comment${data.totalCount !== 1 ? "s" : ""}`}
          {searchQuery && (
            <span className="text-primary-600">
              {" "}
              matching &quot;{searchQuery}&quot;
            </span>
          )}
        </p>
      )}

      {/* Loading state */}
      {isLoading && (
        <div className="flex items-center justify-center py-12">
          <div className="flex flex-col items-center gap-3">
            <div className="w-8 h-8 border-3 border-primary-500 border-t-transparent rounded-full animate-spin" />
            <p className="text-sm text-gray-400">Loading comments...</p>
          </div>
        </div>
      )}

      {/* Error state */}
      {isError && (
        <div className="p-4 bg-red-50 border border-red-200 rounded-lg">
          <p className="text-sm text-red-600">
            Failed to load comments.{" "}
            {error instanceof Error ? error.message : "Please try again."}
          </p>
        </div>
      )}

      {/* Comments list */}
      {data && data.items.length > 0 && (
        <div className="space-y-4">
          {data.items.map((comment) => (
            <div
              key={comment.id}
              className="bg-white border border-gray-200 rounded-xl p-4 shadow-sm hover:shadow-md transition-shadow"
            >
              <CommentThread comment={comment} />
            </div>
          ))}
        </div>
      )}

      {/* Empty state */}
      {data && data.items.length === 0 && !isLoading && (
        <div className="text-center py-12">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="w-12 h-12 mx-auto text-gray-300 mb-3"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            strokeWidth={1}
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"
            />
          </svg>
          <p className="text-gray-400 text-sm">
            {searchQuery
              ? "No comments match your search."
              : "Be the first to leave a comment!"}
          </p>
        </div>
      )}

      {/* Pagination */}
      {data && totalPages > 1 && (
        <Pagination
          currentPage={page}
          totalPages={totalPages}
          onPageChange={setPage}
        />
      )}
    </div>
  );
}
