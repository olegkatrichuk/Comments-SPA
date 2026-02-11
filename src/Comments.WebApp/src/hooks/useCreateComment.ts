import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createComment } from "@/lib/api";
import type { CreateCommentData } from "@/lib/types";

export function useCreateComment() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateCommentData) => createComment(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["comments"] });
    },
  });
}
