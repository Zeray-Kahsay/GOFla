import type { MenuItem } from "../../../types/menuItem";
import type { PagedResult } from "../../../types/pagedResult";
import type { Restaurant } from "../../../types/restaurant";
import { apiSlice } from "../apiSlice";

interface SearchParams {
  query: string;
  category?: string;
  minPrice?: number;
  maxPrice?: number;
  minRating?: number;
  isAvailable?: boolean;
  sortBy?: 'relevance' | 'rating' | 'price' | 'distance';
  pageSize?: number;
  cursor?: string;
}

interface RestaurantSearchResult extends Restaurant {
  averageRating: number;
  reviewCount: number;
  isFavorite: boolean;
}

interface MenuItemSearchResult extends MenuItem {
  restaurantName: string;
}

interface SearchResult {
  restaurants: RestaurantSearchResult[];
  menuItems: MenuItemSearchResult[];
  totalResults: number;
}

export const searchApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    search: builder.query<SearchResult, SearchParams>({
      query: (params) => ({
        url: '/search',
        params,
      }),
    }),
    searchRestaurants: builder.query<PagedResult<RestaurantSearchResult>, SearchParams>({
      query: (params) => ({
        url: '/search/restaurants',
        params,
      }),
    }),
    searchMenuItems: builder.query<PagedResult<MenuItemSearchResult>, SearchParams>({
      query: (params) => ({
        url: '/search/menu-items',
        params,
      }),
    }),
    getPopularSearches: builder.query<string[], void>({
      query: () => '/search/popular',
    }),
    getSuggestions: builder.query<string[], string>({
      query: (query) => ({
        url: '/search/suggestions',
        params: { query },
      }),
    }),
  }),
});

export const {
    useSearchQuery,
    useLazySearchQuery,
    useSearchRestaurantsQuery,
    useLazySearchRestaurantsQuery,
    useSearchMenuItemsQuery,
    useLazySearchMenuItemsQuery,
    useGetPopularSearchesQuery,
    useLazyGetSuggestionsQuery
} = searchApi;