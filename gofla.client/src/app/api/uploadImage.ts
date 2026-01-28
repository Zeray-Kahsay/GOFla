import { axiosClient } from "./axiosClient";

type uploadOptions<TResponse> = {
    url: string;
    file: File;
    fieldName?: string; //default = "file"
    onProgress?: (percent: number) => void;
    onProcessing?: () => void;
}

export async function uploadImage<TResponse = {imageUrl: string}>({
    url,
    file,
    fieldName = "file",
    onProgress,
    onProcessing
} : uploadOptions<TResponse>) : Promise<TResponse>{
    const formData = new FormData();
    formData.append(fieldName, file);

    const res = await axiosClient.post(url, formData, {
        headers: {"Content-Type": "multipart/form-data"},
        onUploadProgress: (e) => {
            if (!e.total) return;

            const percent = Math.round((e.loaded * 100) / e.total);
            onProgress?.(percent);

            if (percent === 100) onProcessing?.();
        }
    })

    return res.data; 
}

// TODO: 

// cancelToken?: CancelTokenSource
// maxSizeMB?: number
// validateType?: (file: File) => boolean
