"use client";

interface CommentPreviewProps {
  text: string;
  userName: string;
}

export default function CommentPreview({ text, userName }: CommentPreviewProps) {
  if (!text.trim()) {
    return (
      <div className="p-4 bg-gray-50 border border-gray-200 rounded-lg text-sm text-gray-400 italic">
        Start typing to see a preview...
      </div>
    );
  }

  return (
    <div className="border border-gray-200 rounded-lg overflow-hidden">
      <div className="px-4 py-2 bg-gray-50 border-b border-gray-200">
        <span className="text-xs font-medium text-gray-500 uppercase tracking-wider">
          Preview
        </span>
      </div>
      <div className="p-4">
        <div className="flex items-center gap-2 mb-2">
          <div className="w-7 h-7 rounded-full bg-primary-100 text-primary-700 flex items-center justify-center text-xs font-bold">
            {userName ? userName[0].toUpperCase() : "?"}
          </div>
          <span className="text-sm font-semibold text-gray-800">
            {userName || "Anonymous"}
          </span>
          <span className="text-xs text-gray-400">Just now</span>
        </div>
        <div
          className="text-sm text-gray-700 leading-relaxed comment-content"
          dangerouslySetInnerHTML={{ __html: text }}
        />
      </div>
    </div>
  );
}
