import { useEffect, useState } from "react";
import { toast } from "react-toastify";

import { Modal } from "../../app/layout/ui/Modal";
import type { MenuItem } from "../../types/menuItem";
import { ImageUploader } from "../../app/layout/ui/ImageUploader";
import { Button } from "../../app/layout/ui/Button";
import { ImageCropModal } from "../../app/layout/ui/ImageCropModal";
import { uploadMenuItemImage } from "../../app/api/menuItem/uploadMenuItemImage";


interface Props {
  item: MenuItem;
  isOpen: boolean;
  onClose: () => void;
}

export function EditMenuItemImageModal({ item, isOpen, onClose }: Props) {
  const [progress, setProgress] = useState(0);
  const [isUploading, setIsUploading] = useState(false);
  const [file, setFile] = useState<File | null>(null);
  const [showCrop, setShowCrop] = useState(false);
  const [cropSrc, setcropSrc] = useState<string | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);


  useEffect(() => {
    return () => {
      if (previewUrl) URL.revokeObjectURL(previewUrl);
      if (cropSrc) URL.revokeObjectURL(cropSrc);
    }
  }, [previewUrl, cropSrc])


  const handleRawFile = (file: File) => {
    const url = URL.createObjectURL(file);
    setcropSrc(url);
    setFile(file);
    setShowCrop(true);
  };


  const handleCropDone = (croppedFile: File) => {
    // revoke old preview if exists
    if (previewUrl) URL.revokeObjectURL(previewUrl);

    const newPreview = URL.createObjectURL(croppedFile);

    setFile(croppedFile);
    setPreviewUrl(newPreview);

    if (cropSrc) URL.revokeObjectURL(cropSrc);
    setcropSrc(null);
    setShowCrop(false);
  }
  
  const handleUpload = async () => {
    if (!file) return toast.error("Select an image first");

    try {
      setIsUploading(true);
      setProgress(0);

       await uploadMenuItemImage(
        item.id,
        file,
        setProgress,
        () => {}
      );
      toast.success("Image updated");
      onClose();
    } catch {
      toast.error("Failed to upload image");
    } finally {
      setIsUploading(false);

    }
  };

  return (
    <>
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title="Update menu item image"
      description="Upload a new image for this item"
      disableClose={isUploading}
    >
      <ImageUploader
        imageUrl={previewUrl || item.imageUrl}
        onFileSelected={handleRawFile}
        isUploading={isUploading}
        progress={progress}
      />
      <Button 
        onClick={handleUpload} 
        disabled={!file || isUploading} 
        isLoading={isUploading}
        variant="amber"
        >
        Upload Image
      </Button>

    </Modal>

    {showCrop && cropSrc && (
      <ImageCropModal 
        imageSrc={cropSrc}
        isOpen={showCrop}
        onClose={() => {
          URL.revokeObjectURL(cropSrc);
          setcropSrc(null);
          setShowCrop(false);
        }}
        onCropDone={handleCropDone}
      />
    )}
        
    </>
  );
}
