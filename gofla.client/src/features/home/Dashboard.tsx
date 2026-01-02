import { Store } from "lucide-react";
import { EmptyState } from "../../app/layout/ui/EmptyState";
import { LoadingSpinner } from "../../app/layout/ui/LoadingSpinner";
import { RestaurantCard } from "../restaurant/RestaurantCard";
import { useGetRestaurantsQuery } from "../../app/api/restaurant/restaurantApi";
import { useState } from "react";
import type { Restaurant } from "../../types/restaurant";
import { useInfiniteScroll } from "../../hooks/useInfiniteScroll";
import { Link } from "react-router-dom";
import { Button } from "../../app/layout/ui/Button";

export default function Dashboard() {
  const [cursor, setCursor] = useState<string | undefined>();
  
  const { data, isLoading, isFetching } = useGetRestaurantsQuery({
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

  return (
    <div className="container mx-auto px-4 py-8">
      {/* Hero Section */}
      <section className="mb-12 text-center">
        <h1 className="text-4xl  md:text-5xl font-bold font-serif text-amber-900 mb-4">
          Order Food from Your Favorite Restaurants
        </h1>
        <p className="text-xl text-gray-600 max-w-2xl mx-auto font-serif">
          Fast delivery, great food. Order now and enjoy!
        </p>
      </section>
      <div>
        <Link to="restaurant/new">
            <Button variant="amber" className= "font-serif mb-1.5"> Add Your Restaurant </Button>
        </Link>
      </div>

      {/* Categories */}
      <section className="mb-12">
        <h2 className="text-2xl font-bold font-serif text-amber-900 mb-6">Popular Categories</h2>
        <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
          {['Pizza', 'Burger', 'Sushi', 'Habesha', 'Italian', 'Mexican'].map((category) => (
            <button
              key={category}
              className="p-4 bg-white rounded-lg shadow hover:shadow-md transition-shadow text-center"
            >
              <div className="w-16 h-16 mx-auto mb-2 bg-primary-100 rounded-full flex items-center justify-center">
                <span className="text-2xl">🍕</span>
              </div>
              <p className="font-medium text-gray-900">{category}</p>
            </button>
          ))}
        </div>
      </section>

      {/* Restaurants */}
      <section>
        <h2 className="text-2xl font-bold-serif text-gray-900 mb-6">All Restaurants</h2>
        
        {items.length === 0 ? (
          <EmptyState
            icon={Store}
            title="No restaurants found"
            description="Check back later for new restaurants"
          />
        ) : (
          <>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 font-serif">
              {items.map((restaurant: Restaurant) => (
                <RestaurantCard key={restaurant.id} restaurant={restaurant} />
              ))}
            </div>

            {/* Load More Trigger */}
            {data?.hasMore && (
              <div ref={loadMoreRef} className="py-8 flex justify-center">
                {isFetching && <LoadingSpinner />}
              </div>
            )}
          </>
        )}
      </section>
    </div>
  );
}