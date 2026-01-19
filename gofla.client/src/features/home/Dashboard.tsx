import { Store } from "lucide-react";
import { EmptyState } from "../../app/layout/ui/EmptyState";
import { useGetRestaurantsQuery } from "../../app/api/restaurant/restaurantApi";
import { useEffect, useState } from "react";
import type { Restaurant } from "../../types/restaurant";
import { useInfiniteScroll } from "../../hooks/useInfiniteScroll";
import { Link } from "react-router-dom";
import { Button } from "../../app/layout/ui/Button";
import RestaurantCard from "../restaurant/RestaurantCard";
import RestaurantCardSkeleton from "../restaurant/RestaurantCardSkeleton";

export default function Dashboard() {
  const [cursor, setCursor] = useState<string | undefined>();
  
  const { data, isLoading, isFetching } = useGetRestaurantsQuery({
    cursor,
    pageSize:10,
  });

  
  const { items, loadMoreRef } = useInfiniteScroll({
    data: data?.items ?? [],
    hasMore: data?.hasMore || false,
    isLoading: isFetching,
    fetchMore: () => {
      if (!isFetching && data?.nextCursor) {
        setCursor(data.nextCursor);
      }
    },
  });
  const isInitialLoading = isLoading && !items?.length;


  useEffect(() => {
    setCursor(undefined);
  },[])

  return (
    <div className="container mx-auto px-4 py-10 space-y-16">
      {/* Hero Section */}
     <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-6 gap-4">
       <div>
        <h1 className="text-3xl md:text-4xl font-bold font-serif text-amber-900">
           Discover Restaurants
       </h1>
       <p className="text-gray-600 font-serif">
         Order from the best places near you
       </p>
       </div>

        <Link to="/restaurants/new">
           <Button
            variant="amber"
            className="w-full sm:w-auto font-serif"
          >
            + Add Restaurant
          </Button>
       </Link>
     </div>
      {/* Categories --- will be replaced with data from API*/}
      <section className="mb-12">
        <h2 className="text-2xl font-serif font-semibold text-amber-900 tracking-tight mb-2.5">Popular Categories</h2>
        <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
          {['Pizza', 'Burger', 'Sushi', 'Habesha', 'Italian', 'Mexican'].map((category) => (
            <button
              key={category}
              className="
                 group p-4 rounded-xl bg-white
                border border-amber-100
                shadow-sm hover:shadow-md
                hover:-translate-y-0.5
               transition-all duration-200
               text-center
              "
            >
              <div className="w-14 h-14 mx-auto mb-3 bg-amber-100 rounded-full flex items-center justify-center group-hover:scale-105 transition">
                <span className="text-2xl">🍕</span>
              </div>
              <p className="text-gray-800 font-medium">{category}</p>
            </button>
          ))}
        </div>
      </section>

      {/* Restaurants */}
      <section>
        <h2 className="text-2xl font-serif font-semibold text-amber-900 mb-6 relative inline-block">
          All Restaurants
          <span className="absolute left-0 -bottom-1 h-1 w-1/2 bg-amber-300 rounded-full" ></span>
        </h2>
        
        {isInitialLoading ? (
            <div>
              {Array.from({length: 12}).map((_, i) => (
                <RestaurantCardSkeleton key={i} />
              ))}
            </div>
        ) : items.length === 0 ? (
            <EmptyState 
              icon={Store}
              title="No restaurants found"
              description="Check back later for new restaurants"
            />
        ) : (
          <>
               {/* DATA */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {items.map((restaurant: Restaurant) => (
          <RestaurantCard key={restaurant.id} restaurant={restaurant} />
        ))}
      </div>

      {/* PAGINATION LOADING */}
      {data?.hasMore && (
        <div ref={loadMoreRef} className="py-6">
          {isFetching && (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
              {Array.from({ length: 4 }).map((_, i) => (
                <RestaurantCardSkeleton key={`loading-${i}`} />
              ))}
            </div>
          )}
        </div>
      )}
          </>
        )}
      </section>
    </div>
  );
}