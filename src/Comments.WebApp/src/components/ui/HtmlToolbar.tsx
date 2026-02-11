"use client";

import { RefObject } from "react";

interface HtmlToolbarProps {
  textareaRef: RefObject<HTMLTextAreaElement | null>;
  onTextChange: (newText: string) => void;
}

export default function HtmlToolbar({
  textareaRef,
  onTextChange,
}: HtmlToolbarProps) {
  const wrapSelection = (tagName: string, attribute?: string) => {
    const textarea = textareaRef.current;
    if (!textarea) return;

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selectedText = textarea.value.substring(start, end);
    const text = textarea.value;

    let wrapped: string;

    if (tagName === "a") {
      const href = attribute || prompt("Enter URL:", "https://");
      if (!href) return;
      wrapped = `<a href="${href}">${selectedText || "link text"}</a>`;
    } else {
      wrapped = `<${tagName}>${selectedText}</${tagName}>`;
    }

    const newText = text.substring(0, start) + wrapped + text.substring(end);
    onTextChange(newText);

    requestAnimationFrame(() => {
      textarea.focus();
      const cursorPos = start + wrapped.length;
      textarea.setSelectionRange(cursorPos, cursorPos);
    });
  };

  const buttons = [
    { label: "[i]", tag: "i", title: "Italic" },
    { label: "[strong]", tag: "strong", title: "Bold" },
    { label: "[code]", tag: "code", title: "Code" },
    { label: "[a]", tag: "a", title: "Link" },
  ];

  return (
    <div className="flex items-center gap-1 p-1.5 bg-gray-50 border border-b-0 border-gray-300 rounded-t-md">
      {buttons.map((btn) => (
        <button
          key={btn.tag}
          type="button"
          onClick={() => wrapSelection(btn.tag)}
          title={btn.title}
          className="px-2 py-1 text-xs font-mono font-medium text-gray-600 bg-white border border-gray-300 rounded hover:bg-gray-100 hover:text-gray-900 transition-colors"
        >
          {btn.label}
        </button>
      ))}
    </div>
  );
}
