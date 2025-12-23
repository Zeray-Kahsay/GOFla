import { Heart } from "lucide-react";
import { LoadingSpinner } from "../../app/layout/ui/LoadingSpinner";
import { RestaurantCard } from "../restaurant/RestaurantCard";
import { EmptyState } from "../../app/layout/ui/EmptyState";
import { useInfiniteScroll } from "../../hooks/useInfiniteScroll";
import { useGetFavoritesQuery } from "../../app/api/favorite/FavoriteApi";
import { useState } from "react";

export default function FavoritesPage() {
  const [cursor, setCursor] = useState<string | undefined>();

  const { data, isLoading, isFetching } = useGetFavoritesQuery({
    cursor,
    pageSize: 12,
  });

  const { items, loadMoreRef } = useInfiniteScroll({
    data: data?.items,
    hasMore: data?.hasMore || false,
    isLoading: isFetching,
    fetchMore: () => {
      if (data?.nextCursor) {
        setCursor(data.nextCursor);
      }
    },
  });

  if (isLoading) {
    return <LoadingSpinner fullScreen />;
  }

  // Convert favorites to restaurant format
  const restaurants = items.map((fav) => ({
    id: fav.restaurantId,
    name: fav.restaurantName,
    description: '',
    imageUrl: fav.restaurantImage,
    address: fav.restaurantAddress,
    phone: '',
    deliveryFee: fav.deliveryFee,
    estimatedDeliveryTime: 30,
    isActive: true,
    isFavorite: true,
  }));

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-900 mb-8">My Favorites</h1>

      {restaurants.length === 0 ? (
        <EmptyState
          icon={Heart}
          title="No favorites yet"
          description="Start adding your favorite restaurants to quickly access them later"
          actionLabel="Browse Restaurants"
          onAction={() => window.location.href = '/'}
        />
      ) : (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {restaurants.map((restaurant) => (
              <RestaurantCard key={restaurant.id} restaurant={restaurant} />
            ))}
          </div>

          {data?.hasMore && (
            <div ref={loadMoreRef} className="py-8 flex justify-center">
              {isFetching && <LoadingSpinner />}
            </div>
          )}
        </>
      )}
    </div>
  );
}