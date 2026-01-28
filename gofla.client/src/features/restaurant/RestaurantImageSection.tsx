import { useState } from "react";
import { toast } from "react-toastify";
import { ImageUploader } from "../../app/layout/ui/ImageUploader";
import { useNavigate, useParams } from "react-router-dom";
import { uploadRestaurantImage } from "../../app/api/restaurant/uploadRestaurantImage";
import { Button } from "../../app/layout/ui/Button";
import { ImageCropModal } from "../../app/layout/ui/ImageCropModal";

export function RestaurantImageSection() {
    const [error, setError] = useState<string | null>(null);
    const {id} = useParams<{id: string}>();
    const navigate = useNavigate();
    const [progress, setProgress] = useState(0);
    const [isUploading, setIsUploading] = useState(false);
    const [file, setFile] = useState<File | null>(null);
    const [showCrop, setShowCrop] = useState(false);
    const [cropSrc, setcropSrc] = useState<string | null>(null);
    const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  

    if (!id) {
        return <div className="text-red-500" >Invalid restaurant ID</div>;
    }

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

            await uploadRestaurantImage(
                 Number(id),
                file,
                setProgress,
                () => {},       
            );
           
             toast.success("Image uploaded successfully");
             navigate(`/restaurants/${id}`);
             setError(null);
        } catch (error) {
            setError("Failed to upload image");
            toast.error("Failed to upload image");
        } finally {
               setIsUploading(false);
        }
    }

    return (
        <section className="space-y-4 font-serif max-w-2xl mx-auto p-4">
            <h3 className="text-lg font-semibold flex justify-center mt-3" >Upload Restaurant Image/Logo</h3>

            <>
                <ImageUploader
                  imageUrl={previewUrl || undefined}
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
                  {error && <p className="text-sm text-red-600">{error}</p>}    
              </>
        </section>
    );
}