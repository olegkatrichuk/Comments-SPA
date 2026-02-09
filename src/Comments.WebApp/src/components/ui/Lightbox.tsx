"use client";

import { useEffect, useState, useCallback, useRef } from "react";

interface LightboxProps {
  src: string;
  alt?: string;
  isTextFile?: boolean;
  onClose: () => void;
}

export default function Lightbox({
  src,
  alt,
  isTextFile,
  onClose,
}: LightboxProps) {
  const [textContent, setTextContent] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [imageLoaded, setImageLoaded] = useState(false);
  const [closing, setClosing] = useState(false);
  const overlayRef = useRef<HTMLDivElement>(null);

  const handleClose = useCallback(() => {
    setClosing(true);
    setTimeout(() => onClose(), 300);
  }, [onClose]);

  const handleKeyDown = useCallback(
    (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        handleClose();
      }
    },
    [handleClose]
  );

  useEffect(() => {
    document.addEventListener("keydown", handleKeyDown);
    document.body.style.overflow = "hidden";

    return () => {
      document.removeEventListener("keydown", handleKeyDown);
      document.body.style.overflow = "";
    };
  }, [handleKeyDown]);

  useEffect(() => {
    if (isTextFile) {
      setLoading(true);
      fetch(src)
        .then((res) => res.text())
        .then((text) => {
          setTextContent(text);
          setLoading(false);
        })
        .catch((err) => {
          console.error("Failed to load text file:", err);
          setTextContent("Failed to load file content.");
          setLoading(false);
        });
    } else {
      setLoading(true);
    }
  }, [src, isTextFile]);

  const handleImageLoad = () => {
    setLoading(false);
    setImageLoaded(true);
  };

  return (
    <div
      ref={overlayRef}
      className={`fixed inset-0 z-50 flex items-center justify-center transition-opacity duration-300 ${
        closing ? "opacity-0" : "opacity-100"
      }`}
      style={{ animation: !closing ? "lightbox-fade-in 0.3s ease-out" : undefined }}
      onClick={handleClose}
    >
      {/* Backdrop */}
      <div className="absolute inset-0 bg-black/80" />

      {/* Content container */}
      <div
        className={`relative z-10 flex flex-col items-center transition-all duration-300 ${
          closing ? "scale-95 opacity-0" : ""
        }`}
        style={{ animation: !closing ? "lightbox-scale-in 0.3s ease-out" : undefined }}
        onClick={(e) => e.stopPropagation()}
      >
        {/* Close button */}
        <button
          onClick={handleClose}
          className="absolute -top-12 right-0 p-2 text-white/70 hover:text-white transition-colors z-20"
          title="Close (Esc)"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="w-8 h-8"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            strokeWidth={2}
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
        </button>

        {isTextFile ? (
          <div className="bg-white rounded-lg shadow-2xl p-6 max-w-2xl max-h-[80vh] overflow-auto">
            {loading ? (
              <div className="flex items-center justify-center py-8">
                <div className="w-6 h-6 border-2 border-primary-500 border-t-transparent rounded-full animate-spin" />
              </div>
            ) : (
              <pre className="text-sm text-gray-800 whitespace-pre-wrap font-mono">
                {textContent}
              </pre>
            )}
          </div>
        ) : (
          <>
            {/* Spinner while image loads */}
            {loading && (
              <div className="flex items-center justify-center py-16">
                <div className="w-8 h-8 border-3 border-white/30 border-t-white rounded-full animate-spin" />
              </div>
            )}

            {/* Image with fade-in */}
            <img
              src={src}
              alt={alt || "Full size image"}
              className={`max-w-[90vw] max-h-[80vh] object-contain rounded-lg shadow-2xl transition-opacity duration-500 ${
                imageLoaded ? "opacity-100" : "opacity-0"
              } ${loading ? "absolute" : ""}`}
              onLoad={handleImageLoad}
            />

            {/* Caption bar */}
            {imageLoaded && alt && (
              <div
                className="mt-3 px-4 py-2 bg-black/50 rounded-md text-white/90 text-sm text-center transition-opacity duration-500"
                style={{ animation: "lightbox-fade-in 0.5s ease-out 0.2s both" }}
              >
                {alt}
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
}
