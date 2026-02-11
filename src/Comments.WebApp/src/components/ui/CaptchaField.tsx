"use client";

import { useEffect, useState, useCallback } from "react";
import { getCaptcha } from "@/lib/api";
import type { CaptchaData } from "@/lib/types";
import type { UseFormRegister, UseFormSetValue } from "react-hook-form";
import type { CommentFormValues } from "@/lib/validation";

interface CaptchaFieldProps {
  register: UseFormRegister<CommentFormValues>;
  setValue: UseFormSetValue<CommentFormValues>;
  error?: string;
}

export default function CaptchaField({
  register,
  setValue,
  error,
}: CaptchaFieldProps) {
  const [captcha, setCaptcha] = useState<CaptchaData | null>(null);
  const [loading, setLoading] = useState(false);

  const loadCaptcha = useCallback(async () => {
    setLoading(true);
    try {
      const data = await getCaptcha();
      setCaptcha(data);
      setValue("captchaAnswer", "");
    } catch (err) {
      console.error("Failed to load captcha:", err);
    } finally {
      setLoading(false);
    }
  }, [setValue]);

  useEffect(() => {
    loadCaptcha();
  }, [loadCaptcha]);

  return (
    <div className="space-y-2">
      <label className="block text-sm font-medium text-gray-700">
        CAPTCHA <span className="text-red-500">*</span>
      </label>

      <div className="flex items-center gap-3">
        <div className="flex-shrink-0 border border-gray-300 rounded bg-white overflow-hidden">
          {loading ? (
            <div className="w-[200px] h-[60px] flex items-center justify-center text-sm text-gray-400">
              Loading...
            </div>
          ) : captcha ? (
            <img
              src={`data:image/png;base64,${captcha.imageBase64}`}
              alt="CAPTCHA"
              className="w-[200px] h-[60px] object-contain"
            />
          ) : (
            <div className="w-[200px] h-[60px] flex items-center justify-center text-sm text-red-400">
              Failed to load
            </div>
          )}
        </div>

        <button
          type="button"
          onClick={loadCaptcha}
          disabled={loading}
          className="p-2 text-gray-500 hover:text-primary-600 hover:bg-gray-100 rounded transition-colors disabled:opacity-50"
          title="Refresh CAPTCHA"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className={`w-5 h-5 ${loading ? "animate-spin" : ""}`}
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            strokeWidth={2}
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
            />
          </svg>
        </button>
      </div>

      {/* Hidden field for captcha key */}
      <input type="hidden" value={captcha?.key || ""} />

      <input
        {...register("captchaAnswer")}
        type="text"
        placeholder="Enter the text shown above"
        className={`w-full px-3 py-2 text-sm border rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent ${
          error ? "border-red-300 bg-red-50" : "border-gray-300"
        }`}
        autoComplete="off"
      />
      {error && <p className="text-xs text-red-500 mt-1">{error}</p>}
    </div>
  );
}

export { type CaptchaFieldProps };
