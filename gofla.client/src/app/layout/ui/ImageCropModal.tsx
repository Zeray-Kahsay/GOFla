import Cropper from "react-easy-crop";
import { useState, useCallback } from "react";
import { Modal } from "./Modal";
import { Button } from "./Button";
import { getCroppedImg } from "../../../utils/cropImage";

interface Props {
  imageSrc: string;
  isOpen: boolean;
  onClose: () => void;
  onCropDone: (file: File) => void;
}

export function ImageCropModal({ imageSrc, isOpen, onClose, onCropDone }: Props) {
  const [crop, setCrop] = useState({ x: 0, y: 0 });
  const [zoom, setZoom] = useState(1);
  const [croppedAreaPixels, setCroppedAreaPixels] = useState<any>(null);

  const onCropComplete = useCallback((_area: any, areaPixels: any) => {
    setCroppedAreaPixels(areaPixels);
  }, []);

  const handleSave = async () => {
    const croppedFile = await getCroppedImg(imageSrc, croppedAreaPixels);
    onCropDone(croppedFile);
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Adjust Image">
      <div className="relative h-96 w-full bg-black">
        <Cropper
          image={imageSrc}
          crop={crop}
          zoom={zoom}
          aspect={4 / 3} // perfect for food cards
          onCropChange={setCrop}
          onZoomChange={setZoom}
          onCropComplete={onCropComplete}
        />
      </div>

      <input
        type="range"
        min={1}
        max={3}
        step={0.1}
        value={zoom}
        onChange={(e) => setZoom(Number(e.target.value))}
        className="w-full mt-4"
      />

      <div className="flex justify-end mt-4">
        <Button onClick={handleSave} variant="amber">
          Save Crop
        </Button>
      </div>
    </Modal>
  );
}
