import {  use, useState } from "react";
import { useUploadRestaurantImageMutation } from "../../app/api/restaurant/restaurantApi";
import { toast } from "react-toastify";
import { ImageUploader } from "../../app/layout/ui/ImageUploader";
import { useNavigate, useParams } from "react-router-dom";

export function RestaurantImageSection() {
    const [uploadImage, { isLoading: isUploading }] = useUploadRestaurantImageMutation();
    const [error, setError] = useState<string | null>(null);
    const {id} = useParams<{id: string}>();
    const navigate = useNavigate();
    // const restaurantId = window.location.pathname.split("/")[2];
    // const restaurant = {
    //     id: Number(restaurantId),
    //     imageUrl: ""
    // };

    if (!id) {
        return <div className="text-red-500" >Invalid restaurant ID</div>;
    }

    const handleUpload = async (file: File) => {
        try {
            await uploadImage({
                restaurantId: Number(id),
                file
            }).unwrap();
        toast.success("Image uploaded successfully");
        navigate(`/restaurants/${id}`);
            setError(null);
        } catch (error) {
            setError("Failed to upload image");
            toast.error("Failed to upload image");
        }
    }

    return (
        <section className="space-y-4 font-serif max-w-2xl mx-auto p-4">
            <h3 className="text-lg font-semibold flex justify-center mt-3" >Upload Restaurant Image/Logo</h3>

            <ImageUploader 
                //imageUrl={restaurant.imageUrl}
                onFileSelected={handleUpload}
                isUploading={isUploading}
                error={error ?? undefined} 
            />
        </section>
    );
}