import { useQuery } from "@tanstack/react-query";
import { getComments, searchComments } from "@/lib/api";
import type { SortField, SortDirection } from "@/lib/types";

interface UseCommentsOptions {
  page: number;
  pageSize: number;
  sortField: SortField;
  sortDirection: SortDirection;
  searchQuery?: string;
}

export function useComments({
  page,
  pageSize,
  sortField,
  sortDirection,
  searchQuery,
}: UseCommentsOptions) {
  return useQuery({
    queryKey: ["comments", page, pageSize, sortField, sortDirection, searchQuery],
    queryFn: () => {
      if (searchQuery && searchQuery.trim().length > 0) {
        return searchComments(searchQuery.trim(), page, pageSize);
      }
      return getComments(page, pageSize, sortField, sortDirection);
    },
    staleTime: 30_000,
  });
}
