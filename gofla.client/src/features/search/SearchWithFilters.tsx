import { useSearchParams } from "react-router-dom";
import { SearchFilters, type FilterValues } from "./SearchFilters";
import { useState } from "react";
import SearchBar from "../../app/layout/SearchBar";

export function SearchWithFilters() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [filters, setFilters] = useState<FilterValues>({
    category: searchParams.get('category') || undefined,
    minPrice: searchParams.get('minPrice') ? Number(searchParams.get('minPrice')) : undefined,
    maxPrice: searchParams.get('maxPrice') ? Number(searchParams.get('maxPrice')) : undefined,
    minRating: searchParams.get('minRating') ? Number(searchParams.get('minRating')) : undefined,
    sortBy: searchParams.get('sortBy') || undefined,
  });

  const handleFilterChange = (newFilters: FilterValues) => {
    setFilters(newFilters);
    
    // Update URL params
    const params = new URLSearchParams(searchParams);
    
    Object.entries(newFilters).forEach(([key, value]) => {
      if (value !== undefined) {
        params.set(key, String(value));
      } else {
        params.delete(key);
      }
    });
    
    setSearchParams(params);
  };

  return (
    <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center">
      <div className="flex-1 w-full">
        <SearchBar />
      </div>
      <SearchFilters onFilterChange={handleFilterChange} initialFilters={filters} />
    </div>
  );
}