import { uploadImage } from "../uploadImage"

export const uploadUserAvatar = (
    file: File, 
    onProgress: (p: number) => void) => {

        uploadImage({
            url: `/users/me/avatar`,  // will be updated
            file,
            onProgress,
        })

}