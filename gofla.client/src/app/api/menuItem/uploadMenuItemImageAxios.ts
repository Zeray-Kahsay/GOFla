import { axiosClient } from "../axiosClient";


export async function uploadMenuItemImageAxios(
    menuItemId: number,
    file: File,
    onProgress: (p: number) => void,
    onProcessing: () => void
){
    const formData = new FormData();
    formData.append("file", file);

    const res = await axiosClient.post(
        `/menuItems/owner/menu-items/${menuItemId}/image`,
        formData,
        {
            headers: {"Content-Type": "multipart/form-data"},
            onUploadProgress: (e) => {
                if (!e.total) return;
                
                const percent = Math.round((e.loaded * 100) / e.total);
                onProgress(percent);

                if (percent === 100){
                    onProcessing();
                }
            },
        }
    );

    return res.data as {imageUrl: string};
}