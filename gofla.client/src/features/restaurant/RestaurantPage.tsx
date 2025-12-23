import { Clock, DollarSign, MapPin, Star } from "lucide-react";
import { Button } from "../../app/layout/ui/Button";
import { LoadingSpinner } from "../../app/layout/ui/LoadingSpinner";
import { Rating } from "../../app/layout/ui/Rating";
import { MenuItemCard } from "../menu/MenuItemCard";
import { ReviewCard } from "../review/ReviewCard";
import { ReviewModal } from "../review/ReviewModal";
import { formatCurrency } from "../../utils/formatters";
import { useInfiniteScroll } from "../../hooks/useInfiniteScroll";
import { useGetRestaurantRatingQuery, useGetRestaurantReviewsQuery } from "../../app/api/review/ReviewApi";
import { useGetMenuItemsByRestaurantQuery } from "../../app/api/menuItem/menuItemApi";
import { useGetRestaurantByIdQuery } from "../../app/api/restaurant/restaurantApi";
import { useState } from "react";
import { useAuth } from "../../hooks/useAuth";
import { useParams } from "react-router-dom";

export default function RestaurantPage() {
  const { id } = useParams<{ id: string }>();
  const { isAuthenticated } = useAuth();
  const restaurantId = parseInt(id!);
  
  const [selectedCategory, setSelectedCategory] = useState<string>('all');
  const [showReviewModal, setShowReviewModal] = useState(false);
  const [menuCursor, setMenuCursor] = useState<string | undefined>();
  const [reviewCursor, setReviewCursor] = useState<string | undefined>();

  const { data: restaurant, isLoading: restaurantLoading } = useGetRestaurantByIdQuery(restaurantId);
  const { data: menuData, isFetching: menuFetching } = useGetMenuItemsByRestaurantQuery({
    restaurantId,
    cursor: menuCursor,
    pageSize: 20,
  });
  const { data: ratingData } = useGetRestaurantRatingQuery(restaurantId);
  const { data: reviewsData, isFetching: reviewsFetching } = useGetRestaurantReviewsQuery({
    restaurantId,
    cursor: reviewCursor,
    pageSize: 10,
  });

  const { items: menuItems, loadMoreRef: menuLoadMoreRef } = useInfiniteScroll({
    data: menuData?.items,
    hasMore: menuData?.hasMore || false,
    isLoading: menuFetching,
    fetchMore: () => {
      if (menuData?.nextCursor) {
        setMenuCursor(menuData.nextCursor);
      }
    },
  });

  const { items: reviews, loadMoreRef: reviewLoadMoreRef } = useInfiniteScroll({
    data: reviewsData?.items,
    hasMore: reviewsData?.hasMore || false,
    isLoading: reviewsFetching,
    fetchMore: () => {
      if (reviewsData?.nextCursor) {
        setReviewCursor(reviewsData.nextCursor);
      }
    },
  });

  if (restaurantLoading) {
    return <LoadingSpinner fullScreen />;
  }

  if (!restaurant) {
    return <div>Restaurant not found</div>;
  }

  const filteredMenuItems = selectedCategory === 'all'
    ? menuItems
    : menuItems.filter((item) => item.category === selectedCategory);

  const categories = ['all', ...Array.from(new Set(menuItems.map((item) => item.category)))];

  return (
    <div>
      {/* Restaurant Header */}
      <div className="relative h-64 md:h-80">
        <img
          src={restaurant.imageUrl}
          alt={restaurant.name}
          className="w-full h-full object-cover"
        />
        <div className="absolute inset-0 bg-linear-to-t from-black/60 to-transparent" />
        <div className="absolute bottom-0 left-0 right-0 text-white p-8">
          <div className="container mx-auto">
            <h1 className="text-3xl md:text-4xl font-bold mb-2">{restaurant.name}</h1>
            <p className="text-lg mb-4">{restaurant.description}</p>
            <div className="flex flex-wrap items-center gap-6 text-sm">
              <div className="flex items-center gap-2">
                {ratingData && (
                  <Rating rating={ratingData.averageRating} size="sm" showNumber />
                )}
                <span>({ratingData?.totalReviews || 0} reviews)</span>
              </div>
              <div className="flex items-center gap-2">
                <Clock size={16} />
                <span>{restaurant.estimatedDeliveryTime} min</span>
              </div>
              <div className="flex items-center gap-2">
                <DollarSign size={16} />
                <span>{formatCurrency(restaurant.deliveryFee)} delivery</span>
              </div>
              <div className="flex items-center gap-2">
                <MapPin size={16} />
                <span>{restaurant.address}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="container mx-auto px-4 py-8">
        {/* Menu Section */}
        <section className="mb-12">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-2xl font-bold">Menu</h2>
          </div>

          {/* Category Filter */}
          <div className="flex gap-2 mb-6 overflow-x-auto pb-2">
            {categories.map((category) => (
              <button
                key={category}
                onClick={() => setSelectedCategory(category)}
                className={`px-4 py-2 rounded-full whitespace-nowrap transition-colors ${
                  selectedCategory === category
                    ? 'bg-primary-600 text-white'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                }`}
              >
                {category === 'all' ? 'All' : category}
              </button>
            ))}
          </div>

          {/* Menu Items */}
          <div className="space-y-4">
            {filteredMenuItems.map((item) => (
              <MenuItemCard key={item.id} item={item} />
            ))}
          </div>

          {menuData?.hasMore && (
            <div ref={menuLoadMoreRef} className="py-8 flex justify-center">
              {menuFetching && <LoadingSpinner />}
            </div>
          )}
        </section>

        {/* Reviews Section */}
        <section>
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-2xl font-bold">Customer Reviews</h2>
            {isAuthenticated && (
              <Button onClick={() => setShowReviewModal(true)} className="bg-amber-500 hover:bg-amber-600">Write a Review</Button>
            )}
          </div>

          {/* Rating Summary */}
          {ratingData && (
            <div className="card p-6 mb-6">
              <div className="flex items-center gap-8">
                <div className="text-center">
                  <p className="text-5xl font-bold text-gray-900">
                    {ratingData.averageRating.toFixed(1)}
                  </p>
                  <Rating rating={ratingData.averageRating} showNumber={false} size="lg" />
                  <p className="text-sm text-gray-600 mt-2">
                    {ratingData.totalReviews} reviews
                  </p>
                </div>

                <div className="flex-1">
                  {[5, 4, 3, 2, 1].map((rating) => (
                    <div key={rating} className="flex items-center gap-3 mb-2">
                      <span className="text-sm w-8">{rating} <Star size={12} className="inline" /></span>
                      <div className="flex-1 h-2 bg-gray-200 rounded-full overflow-hidden">
                        <div
                          className="h-full bg-yellow-400"
                          style={{
                            width: `${
                              ((ratingData.ratingDistribution[rating] || 0) /
                                ratingData.totalReviews) *
                              100
                            }%`,
                          }}
                        />
                      </div>
                      <span className="text-sm text-gray-600 w-12 text-right">
                        {ratingData.ratingDistribution[rating] || 0}
                      </span>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          )}

          {/* Reviews List */}
          <div className="space-y-4">
            {reviews.map((review) => (
              <div key={review.id} className="card p-6">
                <ReviewCard review={review} />
              </div>
            ))}
          </div>

          {reviewsData?.hasMore && (
            <div ref={reviewLoadMoreRef} className="py-8 flex justify-center">
              {reviewsFetching && <LoadingSpinner />}
            </div>
          )}
        </section>
      </div>

      {/* Review Modal */}
      <ReviewModal
        restaurantId={restaurantId}
        isOpen={showReviewModal}
        onClose={() => setShowReviewModal(false)}
      />
    </div>
  );
}