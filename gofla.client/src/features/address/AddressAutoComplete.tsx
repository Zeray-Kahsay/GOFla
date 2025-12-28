import { useEffect, useRef, useState } from "react";

interface Props {
  onSelect: (address: {
    street: string;
    city: string;
    state?: string;
    postalCode: string;
    countryCode: string;
    latitude: number;
    longitude: number;
  }) => void;
}

export function AddressAutocomplete({ onSelect }: Props) {
  const inputRef = useRef<HTMLInputElement | null>(null);
  const sessionTokenRef = useRef<any>(null);
  const placesLibraryRef = useRef<any>(null);
  const [predictions, setPredictions] = useState<any[]>([]);
  const [isOpen, setIsOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const loadPlacesLibrary = async () => {
      if (!window.google?.maps) return;

      try {
        // Import the places library
        const placesLibrary = await google.maps.importLibrary("places") as any;
        placesLibraryRef.current = placesLibrary;
        
        // Initialize session token
        sessionTokenRef.current = new placesLibrary.AutocompleteSessionToken();
      } catch (error) {
        console.error("Error loading Google Places library:", error);
      }
    };

    loadPlacesLibrary();
  }, []);

  const fetchPredictions = async (value: string) => {
    if (!value.trim() || !placesLibraryRef.current) {
      setPredictions([]);
      setIsOpen(false);
      return;
    }

    setIsLoading(true);
    try {
      const { AutocompleteSuggestion } = placesLibraryRef.current;

      const request = {
        input: value,
        sessionToken: sessionTokenRef.current,
      };

      const { suggestions } = await AutocompleteSuggestion.fetchAutocompleteSuggestions(request);
      setPredictions(suggestions || []);
      setIsOpen(true);
    } catch (error) {
      console.error("Error fetching predictions:", error);
      setPredictions([]);
    } finally {
      setIsLoading(false);
    }
  };

  const handleSelectPrediction = async (prediction: any) => {
    if (!prediction.placePrediction?.placeId || !placesLibraryRef.current) return;

    try {
      const { Place } = placesLibraryRef.current;

      // Create a new Place instance with the id
      const place = new Place({
        id: prediction.placePrediction.placeId,
      });

      // Fetch the fields we need
      await place.fetchFields({
        fields: ["addressComponents", "formattedAddress", "location"],
        sessionToken: sessionTokenRef.current,
      });

      if (!place.addressComponents || !place.location) {
        console.error("Missing required place data");
        return;
      }

      const addressComponents = place.addressComponents;
      const location = place.location;

      const get = (type: string) =>
        addressComponents?.find((c: any) => c.types.includes(type))?.longText || "";

      const getShort = (type: string) =>
        addressComponents?.find((c: any) => c.types.includes(type))?.shortText || "";

      onSelect({
        street: `${get("route")} ${get("street_number")}`.trim(),
        city: get("locality") || get("postal_town"),
        state: getShort("administrative_area_level_1"),
        postalCode: get("postal_code"),
        countryCode: getShort("country"),
        latitude: location.lat(),
        longitude: location.lng(),
      });

      // Reset state
      if (inputRef.current) {
        inputRef.current.value = place.formattedAddress || "";
      }
      setPredictions([]);
      setIsOpen(false);

      // Generate new session token for next search
      const { AutocompleteSessionToken } = placesLibraryRef.current;
      sessionTokenRef.current = new AutocompleteSessionToken();
    } catch (error) {
      console.error("Error fetching place details:", error);
    }
  };

  return (
    <div className="relative w-full">
      <input
        ref={inputRef}
        type="text"
        className="input w-full rounded-xl border border-gray-300 bg-amber-50 px-4 py-3 
         text-gray-800 placeholder-gray-400 shadow-sm 
         focus:border-amber-400 focus:bg-white focus:ring-2 focus:ring-amber-300 
         transition-all duration-200 outline-none"
        placeholder="Start typing your address"
        onChange={(e) => fetchPredictions(e.target.value)}
        onFocus={() => predictions.length > 0 && setIsOpen(true)}
        autoComplete="off"
      />
      
      {isLoading && (
        <div className="absolute top-full left-0 right-0 border border-gray-300 bg-white p-2 text-gray-500 text-sm z-10">
          Loading suggestions...
        </div>
      )}

      {isOpen && predictions.length > 0 && !isLoading && (
        <ul className="absolute top-full left-0 right-0 border border-gray-300 bg-white shadow-lg z-10 max-h-64 overflow-y-auto">
          {predictions.map((prediction, index) => (
            <li
              key={`${prediction.placePrediction?.placeId}-${index}`}
              onClick={() => handleSelectPrediction(prediction)}
              className="px-4 py-3 cursor-pointer hover:bg-gray-100 border-b last:border-b-0 transition-colors"
            >
              <div>
                <div className="font-medium text-gray-900">
                  {prediction.placePrediction?.text?.text || ""}
                </div>
                {prediction.placePrediction?.text && (
                  <div className="text-sm text-gray-500 mt-1">
                    {prediction.placePrediction.text.text}
                  </div>
                )}
              </div>
            </li>
          ))}
        </ul>
      )}

      {isOpen && predictions.length === 0 && !isLoading && (
        <div className="absolute top-full left-0 right-0 border border-gray-300 bg-white p-2 text-gray-500 text-sm z-10">
          No results found
        </div>
      )}
    </div>
  );
}

