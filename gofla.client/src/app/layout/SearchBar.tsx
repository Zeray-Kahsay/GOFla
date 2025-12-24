import { Search, X } from "lucide-react";
import { useEffect, useState } from "react";
import { useLazyGetSuggestionsQuery } from "../api/search/SearchApi";
import { useDebounce } from "../../hooks/useDebounce";
import { useNavigate } from "react-router-dom";


export function SearchBar() {
  // 1. STATE MANAGEMENT
  const navigate = useNavigate();
  const [query, setQuery] = useState('');  // User's search input
  const [showSuggestions, setShowSuggestions] = useState(false);  // Show/hide dropdown
  
  // 2. DEBOUNCING
  // Wait 300ms after user stops typing before fetching suggestions
  const debouncedQuery = useDebounce(query, 300);
  
  // 3. API CALL (Lazy = only called manually)
  const [getSuggestions, { data: suggestions }] = useLazyGetSuggestionsQuery();

  // 4. FETCH SUGGESTIONS WHEN DEBOUNCED QUERY CHANGES
  useEffect(() => {
    if (debouncedQuery.length >= 2) {  // Only search if 2+ characters
      getSuggestions(debouncedQuery);
      setShowSuggestions(true);
    } else {
      setShowSuggestions(false);
    }
  }, [debouncedQuery, getSuggestions]);

  // 5. HANDLE SEARCH SUBMISSION
  const handleSearch = (searchQuery: string) => {
    if (searchQuery.trim()) {
      // Navigate to search results page with query parameter
      navigate(`/search?q=${encodeURIComponent(searchQuery.trim())}`);
      setQuery('');  // Clear input
      setShowSuggestions(false);  // Hide suggestions
    }
  };

  // 6. FORM SUBMISSION (Enter key)
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    handleSearch(query);
  };

  // 7. CLICKING A SUGGESTION
  const handleSuggestionClick = (suggestion: string) => {
    handleSearch(suggestion);
  };

  return (
    <div className="relative w-full max-w-2xl hover:bg-amber-100">
      {/* SEARCH FORM */}
      <form onSubmit={handleSubmit} className="relative">
        {/* SEARCH ICON (left side) */}
        <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400" size={20} />
        
        {/* INPUT FIELD */}
        <input
          type="text"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Search for restaurants, cuisines, or dishes..."
          className="w-full pl-12 pr-12 py-3 border border-amber-800 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
        />
        
        {/* CLEAR BUTTON (right side) - only shows if there's text */}
        {query && (
          <button
            type="button"
            onClick={() => {
              setQuery('');
              setShowSuggestions(false);
            }}
            className="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
          >
            <X size={20} />
          </button>
        )}
      </form>

      {/* SUGGESTIONS DROPDOWN */}
      {showSuggestions && suggestions && suggestions.length > 0 && (
        <div className="absolute top-full left-0 right-0 mt-2 bg-white border border-gray-200 rounded-lg shadow-lg z-50 max-h-80 overflow-y-auto">
          {suggestions.map((suggestion, index) => (
            <button
              key={index}
              onClick={() => handleSuggestionClick(suggestion)}
              className="w-full px-4 py-3 text-left hover:bg-gray-50 flex items-center gap-3 border-b last:border-b-0"
            >
              <Search size={16} className="text-gray-400" />
              <span className="text-gray-900">{suggestion}</span>
            </button>
          ))}
        </div>
      )}
    </div>
  );
}
