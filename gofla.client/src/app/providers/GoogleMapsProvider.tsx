import type { PropsWithChildren } from "react";
import { useJsApiLoader } from "@react-google-maps/api";
import { LoadingSpinner } from "../layout/ui/LoadingSpinner";

const libraries: ("places")[] = ["places"];

export function GoogleMapsProvider({ children }: PropsWithChildren) {
  const { isLoaded } = useJsApiLoader({
    googleMapsApiKey: import.meta.env.VITE_GOOGLE_MAPS_API_KEY!,
    libraries,
  });

  if (!isLoaded) {
    return <LoadingSpinner fullScreen />; // <-- must return the spinner
  }

  return <>{children}</>;
}
