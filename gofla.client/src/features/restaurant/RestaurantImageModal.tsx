import { useState } from "react"
import { toast } from "react-toastify"
import { uploadRestaurantImage } from "../../app/api/restaurant/uploadRestaurantImage"
import { Modal } from "../../app/layout/ui/Modal"
import { ImageUploader } from "../../app/layout/ui/ImageUploader"
import { Button } from "../../app/layout/ui/Button"
import { ImageCropModal } from "../../app/layout/ui/ImageCropModal"
import type { Restaurant } from "../../types/restaurant"

type Props = {
    restaurant: Restaurant | null;
    isOpen: boolean;
    onClose: () => void;
}

export function RestaurantImageModal({ restaurant, isOpen, onClose }: Props) {
  const [progress, setProgress] = useState(0)
  const [isUploading, setIsUploading] = useState(false)
  const [file, setFile] = useState<File | null>(null)
  const [showCrop, setShowCrop] = useState(false)
  const [cropSrc, setCropSrc] = useState<string | null>(null)
  const [previewUrl, setPreviewUrl] = useState<string | undefined>(restaurant?.imageUrl ?? undefined)

  if (!restaurant) return null

  const handleRawFile = (file: File) => {
    const url = URL.createObjectURL(file)
    setCropSrc(url)
    setShowCrop(true)
  }

  const handleCropDone = (croppedFile: File) => {
    if (previewUrl) URL.revokeObjectURL(previewUrl)
    const newPreview = URL.createObjectURL(croppedFile)

    setFile(croppedFile)
    setPreviewUrl(newPreview)

    if (cropSrc) URL.revokeObjectURL(cropSrc)
    setCropSrc(null)
    setShowCrop(false)
  }

  const handleUpload = async () => {
    if (!file) return toast.error("Select image first")

    try {
      setIsUploading(true)
      setProgress(0)

      await uploadRestaurantImage(
        restaurant.id,
        file,
        setProgress,
        () => {}
      )

      toast.success("Image updated")
      onClose()
    } catch {
      toast.error("Upload failed")
    } finally {
      setIsUploading(false)
    }
  }

  return (
    <>
      <Modal
        isOpen={isOpen}
        onClose={onClose}
        title="Update restaurant image"
        description="Upload and adjust image"
        disableClose={isUploading}
      >
        <ImageUploader
          imageUrl={previewUrl}
          onFileSelected={handleRawFile}
          isUploading={isUploading}
          progress={progress}
        />

        <Button
          onClick={handleUpload}
          disabled={!file || isUploading}
          isLoading={isUploading}
          variant="amber"
          className="w-full"
        >
          Upload Image
        </Button>
      </Modal>

      {showCrop && cropSrc && (
        <ImageCropModal
          imageSrc={cropSrc}
          isOpen={showCrop}
          onClose={() => {
            URL.revokeObjectURL(cropSrc)
            setCropSrc(null)
            setShowCrop(false)
          }}
          onCropDone={handleCropDone}
        />
      )}
    </>
  )
}
