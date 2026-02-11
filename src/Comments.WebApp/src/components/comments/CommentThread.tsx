"use client";

import type { Comment } from "@/lib/types";
import CommentCard from "./CommentCard";

interface CommentThreadProps {
  comment: Comment;
  depth?: number;
}

export default function CommentThread({
  comment,
  depth = 0,
}: CommentThreadProps) {
  const maxVisualDepth = 5;
  const visualDepth = Math.min(depth, maxVisualDepth);

  return (
    <div
      className={`${
        depth > 0
          ? "ml-4 sm:ml-8 pl-4 border-l-2 border-gray-200 mt-3"
          : ""
      }`}
    >
      <CommentCard comment={comment} depth={visualDepth} />

      {comment.replies && comment.replies.length > 0 && (
        <div className="mt-1">
          {comment.replies.map((reply) => (
            <CommentThread
              key={reply.id}
              comment={reply}
              depth={depth + 1}
            />
          ))}
        </div>
      )}
    </div>
  );
}
