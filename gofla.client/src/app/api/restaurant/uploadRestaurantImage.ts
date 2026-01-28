import { uploadImage } from "../uploadImage";

export const uploadRestaurantImage = (
  restaurantId: number,
  file: File,
  onProgress: (p: number) => void,
  onProcessing: () => void
) =>
  uploadImage({
    url: `/restaurants/${restaurantId}/image`,
    file,
    onProgress,
    onProcessing,
  })
