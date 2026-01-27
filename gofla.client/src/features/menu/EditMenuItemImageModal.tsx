import { useState } from "react";
import { toast } from "react-toastify";

import { Modal } from "../../app/layout/ui/Modal";
import type { MenuItem } from "../../types/menuItem";
import { uploadMenuItemImageAxios } from "../../app/api/menuItem/uploadMenuItemImageAxios";
import { ImageUploader } from "../../app/layout/ui/ImageUploader";


interface Props {
  item: MenuItem;
  isOpen: boolean;
  onClose: () => void;
}

export function EditMenuItemImageModal({ item, isOpen, onClose }: Props) {
  const [progress, setProgress] = useState(0);
  const [isUploading, setIsUploading] = useState(false);
  const [isProcessing, setIsProcessing] = useState(false);


  if (!item?.id) return null;

  const handleUpload = async (file: File) => {
    try {
      setIsUploading(true);
      setIsProcessing(false);
      setProgress(0);

       await uploadMenuItemImageAxios(
        item.id,
        file,
        setProgress,
        () => setIsProcessing(true)
      );
      toast.success("Image updated");
      onClose();
    } catch {
      toast.error("Failed to upload image");
    } finally {
      setIsUploading(false);
      setIsProcessing(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title="Update menu item image"
      description="Upload a new image for this item"
      disableClose={isUploading}
    >
      <ImageUploader
        imageUrl={item.imageUrl}
        onFileSelected={handleUpload}
        isUploading={isUploading}
        progress={progress}
        isProcessing={isProcessing}
      />
    </Modal>
  );
}
