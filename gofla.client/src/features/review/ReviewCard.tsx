import { Rating } from "../../app/layout/ui/Rating";
import type { Review } from "../../types/review";
import { formatRelativeTime } from "../../utils/formatters";


interface ReviewCardProps {
    review: Review;
}


export function ReviewCard({ review }: ReviewCardProps) {
  return (
    <div className="border-b last:border-b-0 py-4">
      <div className="flex items-start gap-4">
        <img
          src={review.userProfileImage || '/default-avatar.png'}
          alt={review.userName}
          className="w-12 h-12 rounded-full object-cover"
        />
        
        <div className="flex-1">
          <div className="flex items-center justify-between mb-2">
            <div>
              <p className="font-semibold text-gray-900">{review.userName}</p>
              <p className="text-sm text-gray-500">{formatRelativeTime(review.createdAt)}</p>
            </div>
            <Rating rating={review.rating} showNumber={false} size="sm" />
          </div>
          
          <h4 className="font-medium text-gray-900 mb-1">{review.title}</h4>
          <p className="text-gray-700">{review.comment}</p>
          
          {review.responses.length > 0 && (
            <div className="mt-4 pl-4 border-l-2 border-gray-200">
              {review.responses.map((response) => (
                <div key={response.id} className="mb-3 last:mb-0">
                  <p className="text-sm font-semibold text-gray-900">
                    {response.responderName}
                  </p>
                  <p className="text-sm text-gray-600 mt-1">{response.responseText}</p>
                  <p className="text-xs text-gray-500 mt-1">
                    {formatRelativeTime(response.createdAt)}
                  </p>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}