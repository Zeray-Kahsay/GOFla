import { useRef } from "react";
import { Image as ImageIcon } from "lucide-react";

interface ImageUploaderProps {
  imageUrl?: string;
  onFileSelected: (file: File) => void;
  isUploading?: boolean;
  progress?: number;
  error?: string;

}


export function ImageUploader({
  imageUrl,
  onFileSelected,
  isUploading = false,
  progress,
  error,
}: ImageUploaderProps) {
  const inputRef = useRef<HTMLInputElement>(null);


  return (
    <div className="space-y-2">
      <div
        onClick={() => !isUploading && inputRef.current?.click()}
       className={`relative cursor-pointer overflow-hidden rounded-xl border border-dashed transition
      ${isUploading ? "opacity-60 cursor-not-allowed" : "hover:bg-gray-100"}`}
      >
        { imageUrl ? (
          <img
            src={imageUrl}
            className="h-48 w-full object-cover"
            alt="Restaurant"
          />
        ) : (
          <div className="flex h-48 flex-col items-center justify-center gap-2 text-gray-500">
            <ImageIcon size={32} />
            <span className="text-sm">Click to upload image</span>
          </div>
        )}

        {isUploading && (
          <div className="absolute inset-0 flex items-center justify-center bg-black/40 text-white" >
              Uploading...{progress}%
          </div>
        )}
      </div>

      <input
        ref={inputRef}
        type="file"
        accept="image/*"
        hidden
        onChange={(e) => {
          const file = e.target.files?.[0];
          if (file) onFileSelected(file);
        }}
      />

      {error && <p className="text-sm text-red-600">{error}</p>}

    </div>
  );
}
