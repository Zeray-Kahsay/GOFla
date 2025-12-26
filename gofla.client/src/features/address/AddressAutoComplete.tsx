// AddressAutocomplete.tsx
import { useEffect, useRef } from "react";

interface Props {
  onSelect: (address: {
    street: string;
    city: string;
    state?: string;
    postalCode: string;
    countryCode: string;
  }) => void;
}

export function AddressAutocomplete({ onSelect }: Props) {
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (!inputRef.current || !window.google) return;

    const autocomplete = new google.maps.places.Autocomplete(inputRef.current, {
      types: ["address"],
      fields: ["address_components", "formatted_address"],
      // componentRestrictions: { country: ["US"] }, // optional
    });

    autocomplete.addListener("place_changed", () => {
      const place = autocomplete.getPlace();
      if (!place.address_components) return;

      const addr: any = { street: "", city: "", state: "", postalCode: "", countryCode: "" };

      place.address_components.forEach((c) => {
        if (c.types.includes("street_number")) addr.street = c.long_name + " " + addr.street;
        if (c.types.includes("route")) addr.street += c.long_name;
        if (c.types.includes("locality")) addr.city = c.long_name;
        if (c.types.includes("administrative_area_level_1")) addr.state = c.short_name;
        if (c.types.includes("postal_code")) addr.postalCode = c.long_name;
        if (c.types.includes("country")) addr.countryCode = c.short_name;
      });

      onSelect(addr);
    });
  }, [onSelect]);

  return (
    <input
      ref={inputRef}
      type="text"
      placeholder="Search for your address"
      className="w-full p-2 border rounded"
    />
  );
}


// import { useEffect, useRef } from "react";

// interface Props {
//   onSelect: (address: {
//     street: string;
//     city: string;
//     postalCode: string;
//     countryCode: string;
//   }) => void;
// }

// export function AddressAutocomplete({ onSelect }: Props) {
//   const containerRef = useRef<HTMLDivElement>(null);

//  useEffect(() => {
//   if (!containerRef.current) return;

//   // Create the new Google Autocomplete element
//   const element = document.createElement("google-places-autocomplete");
//   containerRef.current.appendChild(element);

//   // Listen for selection
//   const handler = (ev: any) => {
//     const place = ev.detail;
//     onSelect({
//       street: place.street_number + " " + place.route,
//       city: place.locality,
//       postalCode: place.postal_code,
//       countryCode: place.country_short_name,
//     });
//   };

//   element.addEventListener("google-places-autocomplete.select", handler);

//   // Cleanup function
//   return () => {
//     element.removeEventListener("google-places-autocomplete.select", handler);
//     containerRef.current?.removeChild(element);
//   };
// }, [onSelect]);


//   return <div ref={containerRef} className="w-full" />;
// }



// import { Autocomplete } from "@react-google-maps/api";
// import { useRef } from "react";

// interface Props {
//   onSelect: (data: ParsedAddress) => void;
// }

// export interface ParsedAddress {
//   street: string;
//   city: string;
//   postalCode: string;
//   countryCode: string;
// }

// export function AddressAutocomplete({ onSelect }: Props) {
//   const autocompleteRef = useRef<google.maps.places.Autocomplete | null>(null);

//   const onLoad = (autocomplete: google.maps.places.Autocomplete) => {
//     autocompleteRef.current = autocomplete;
//   };

//   const onPlaceChanged = () => {
//     const place = autocompleteRef.current?.getPlace();
//     if (!place?.address_components) return;

//     const get = (type: string) =>
//       place.address_components?.find(c => c.types.includes(type))?.long_name || "";

//     const getShort = (type: string) =>
//       place.address_components?.find(c => c.types.includes(type))?.short_name || "";

//     onSelect({
//       street: `${get("street_number")} ${get("route")}`.trim(),
//       city: get("locality") || get("postal_town"),
//       postalCode: get("postal_code"),
//       countryCode: getShort("country"),
//     });
//   };

//   return (
//     <Autocomplete
//       onLoad={onLoad}
//       onPlaceChanged={onPlaceChanged}
//       options={{ types: ["address"] }}
//     >
//       <input
//         className="input"
//         placeholder="Start typing your address"
//       />
//     </Autocomplete>
//   );
// }
