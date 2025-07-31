export interface Link {
  id: number;
  description: string;
  url: string;
  read: boolean;
  comments?: string;
  tags?: number[]; // Array of tag IDs
  favorite?: boolean; // Whether the link is favorited
}
