import { X } from "lucide-react";
import { Button } from "../../app/layout/ui/Button";
import { Input } from "../../app/layout/ui/Input";
import { Rating } from "../../app/layout/ui/Rating";
import { toast } from "react-toastify";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { reviewSchema, type ReviewFormData } from "../../utils/validators/reviewSchema";
import { useState } from "react";
import { useCreateReviewMutation } from "../../app/api/review/ReviewApi";

interface ReviewModalProps {
  restaurantId: number;
  orderId?: number;
  isOpen: boolean;
  onClose: () => void;
}

export function ReviewModal({ restaurantId, orderId, isOpen, onClose }: ReviewModalProps) {
  const [rating, setRating] = useState(5);
  const [createReview, { isLoading }] = useCreateReviewMutation();

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<ReviewFormData>({
    resolver: zodResolver(reviewSchema),
    defaultValues: {
      rating: 5,
    },
  });

  const onSubmit = async (data: ReviewFormData) => {
    try {
      await createReview({
        restaurantId,
        orderId,
        rating,
        title: data.title,
        comment: data.comment,
      }).unwrap();

      toast.success('Review submitted successfully!');
      reset();
      onClose();
    } catch (error) {
      toast.error('Failed to submit review');
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div className="absolute inset-0 bg-black bg-opacity-50" onClick={onClose} />
      
      <div className="relative bg-white rounded-lg shadow-xl max-w-md w-full p-6">
        <button
          onClick={onClose}
          className="absolute top-4 right-4 text-gray-400 hover:text-gray-600"
        >
          <X size={24} />
        </button>

        <h2 className="text-2xl font-bold mb-6">Write a Review</h2>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Your Rating
            </label>
            <Rating
              rating={rating}
              interactive
              onChange={setRating}
              showNumber={false}
              size="lg"
            />
          </div>

          <Input
            label="Title"
            {...register('title')}
            error={errors.title?.message}
            placeholder="Sum up your experience"
          />

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Comment
            </label>
            <textarea
              {...register('comment')}
              rows={4}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500"
              placeholder="Share your experience..."
            />
            {errors.comment && (
              <p className="mt-1 text-sm text-red-600">{errors.comment.message}</p>
            )}
          </div>

          <div className="flex gap-3 pt-4">
            <Button type="button" variant="outline" onClick={onClose} className="flex-1">
              Cancel
            </Button>
            <Button type="submit" isLoading={isLoading} className="flex-1 bg-amber-500 hover:bg-amber-600">
              Submit Review
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}