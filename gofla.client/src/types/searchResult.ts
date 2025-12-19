import type { MenuItem } from "./menuItem";
import type { Restaurant } from "./restaurant";

export interface SearchResult {
  restaurants: Restaurant[];
  menuItems: MenuItem[];
  totalResults: number;
}