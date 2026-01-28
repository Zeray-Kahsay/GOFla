import { uploadImage } from "../uploadImage";

export const uploadMenuItemImage = (
  menuItemId: number,
  file: File,
  onProgress: (p: number) => void,
  onProcessing: () => void
) =>
  uploadImage({
    url: `/menuItems/owner/menu-items/${menuItemId}/image`,
    file,
    onProgress,
    onProcessing,
  })
