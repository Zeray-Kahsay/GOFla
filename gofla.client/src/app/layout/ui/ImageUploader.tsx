import { useRef, useState } from "react";
import { Image as ImageIcon } from "lucide-react";

interface ImageUploaderProps {
  imageUrl?: string;
  onFileSelected: (file: File) => void;
  isUploading?: boolean;
  error?: string;
}


export function ImageUploader({
  imageUrl,
  onFileSelected,
  isUploading,
  error,
}: ImageUploaderProps) {
  const inputRef = useRef<HTMLInputElement>(null);
  const [preview, setPreview] = useState<string | null>(null);

  const handleSelect = (file: File) => {
    setPreview(URL.createObjectURL(file));
    onFileSelected(file);
  };

  return (
    <div className="space-y-2">
      <div
        onClick={() => inputRef.current?.click()}
        className="relative cursor-pointer overflow-hidden rounded-xl border border-dashed border-gray-300 bg-gray-50 hover:bg-gray-100 transition"
      >
        {preview || imageUrl ? (
          <img
            src={preview || imageUrl}
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
          <div className="absolute inset-0 flex items-center justify-center bg-black/40 text-white">
            Uploading…
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
          if (file) handleSelect(file);
        }}
      />

      {error && <p className="text-sm text-red-600">{error}</p>}
    </div>
  );
}
