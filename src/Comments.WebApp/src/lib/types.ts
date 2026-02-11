export interface Attachment {
  id: string;
  fileName: string;
  contentType: string;
  url: string;
}

export interface Comment {
  id: string;
  userName: string;
  email: string;
  homePage?: string;
  text: string;
  createdAt: string;
  attachment?: Attachment;
  replies: Comment[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  hasNextPage: boolean;
}

export interface CaptchaData {
  key: string;
  imageBase64: string;
}

export interface CreateCommentData {
  userName: string;
  email: string;
  homePage?: string;
  text: string;
  parentCommentId?: string;
  captchaKey: string;
  captchaAnswer: string;
  attachment?: File;
}

export type SortField = "UserName" | "Email" | "CreatedAt";
export type SortDirection = "Ascending" | "Descending";
