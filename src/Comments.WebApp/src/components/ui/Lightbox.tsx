"use client";

import { useEffect, useState, useCallback } from "react";

interface LightboxProps {
  src: string;
  alt?: string;
  isTextFile?: boolean;
  onClose: () => void;
}

// Lightbox2-style timings
const FADE_DURATION = 600;
const IMAGE_FADE_DURATION = 600;
const POSITION_FROM_TOP = 50;

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
  const [overlayVisible, setOverlayVisible] = useState(false);

  // Fade in overlay on mount
  useEffect(() => {
    requestAnimationFrame(() => setOverlayVisible(true));
  }, []);

  const handleClose = useCallback(() => {
    setClosing(true);
    setTimeout(() => onClose(), FADE_DURATION);
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
    <div className="fixed inset-0 z-50" onClick={handleClose}>
      {/* Overlay - Lightbox2 style fade */}
      <div
        className="absolute inset-0 bg-black transition-opacity"
        style={{
          opacity: overlayVisible && !closing ? 0.8 : 0,
          transitionDuration: `${FADE_DURATION}ms`,
        }}
      />

      {/* Content container - positioned from top like Lightbox2 */}
      <div
        className="relative z-10 flex flex-col items-center w-full transition-opacity"
        style={{
          paddingTop: `${POSITION_FROM_TOP}px`,
          opacity: overlayVisible && !closing ? 1 : 0,
          transitionDuration: `${FADE_DURATION}ms`,
        }}
        onClick={(e) => e.stopPropagation()}
      >
        {isTextFile ? (
          <div className="bg-white rounded shadow-2xl p-6 max-w-2xl max-h-[80vh] overflow-auto">
            {loading ? (
              <div className="flex items-center justify-center py-8">
                <div className="w-8 h-8 border-2 border-gray-400 border-t-transparent rounded-full animate-spin" />
              </div>
            ) : (
              <pre className="text-sm text-gray-800 whitespace-pre-wrap font-mono">
                {textContent}
              </pre>
            )}
          </div>
        ) : (
          <div className="flex flex-col items-center">
            {/* Loading spinner - Lightbox2 style */}
            {loading && (
              <div className="flex items-center justify-center" style={{ minHeight: "200px" }}>
                <div className="w-10 h-10 border-3 border-white/20 border-t-white rounded-full animate-spin" />
              </div>
            )}

            {/* Image container with Lightbox2-style fade */}
            <div
              className="relative bg-white p-1 rounded shadow-2xl transition-opacity"
              style={{
                opacity: imageLoaded ? 1 : 0,
                transitionDuration: `${IMAGE_FADE_DURATION}ms`,
                display: loading ? "none" : "block",
              }}
            >
              <img
                src={src}
                alt={alt || "Full size image"}
                className="block max-w-[90vw] max-h-[75vh] object-contain"
                style={{
                  maxHeight: `calc(100vh - ${POSITION_FROM_TOP * 2 + 100}px)`,
                }}
                onLoad={handleImageLoad}
              />
            </div>

            {/* Caption and close bar - Lightbox2 style */}
            {imageLoaded && (
              <div
                className="w-full flex justify-between items-center px-1 mt-0 transition-opacity"
                style={{
                  maxWidth: "90vw",
                  opacity: imageLoaded ? 1 : 0,
                  transitionDuration: `${IMAGE_FADE_DURATION}ms`,
                }}
              >
                {/* Caption */}
                <div className="py-2 text-white/90 text-sm">
                  {alt || ""}
                </div>

                {/* Close button - Lightbox2 style */}
                <button
                  onClick={handleClose}
                  className="py-2 px-2 text-white/60 hover:text-white transition-colors text-sm uppercase tracking-wider"
                  title="Close (Esc)"
                >
                  Close
                </button>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
