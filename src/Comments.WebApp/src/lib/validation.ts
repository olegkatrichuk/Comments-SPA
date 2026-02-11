import { z } from "zod";

export const commentSchema = z.object({
  userName: z
    .string()
    .min(1, "Username is required")
    .max(50, "Username must be at most 50 characters")
    .regex(
      /^[a-zA-Z0-9]+$/,
      "Username must contain only alphanumeric characters"
    ),
  email: z
    .string()
    .min(1, "Email is required")
    .email("Please enter a valid email address"),
  homePage: z
    .string()
    .url("Please enter a valid URL")
    .optional()
    .or(z.literal("")),
  text: z
    .string()
    .min(1, "Comment text is required")
    .max(10000, "Comment text must be at most 10,000 characters"),
  captchaAnswer: z.string().min(1, "CAPTCHA answer is required"),
});

export type CommentFormValues = z.infer<typeof commentSchema>;
