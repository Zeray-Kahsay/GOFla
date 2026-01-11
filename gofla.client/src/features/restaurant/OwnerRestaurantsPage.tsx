import { Link } from "react-router-dom";
import { useGetMyRestaurantsQuery } from "../../app/api/restaurant/restaurantApi";
import { Button } from "../../app/layout/ui/Button";
import { EmptyState } from "../../app/layout/ui/EmptyState";
import { OwnerRestaurantCard } from "./OwnerRestaurantCard";
import RestaurantCardSkeleton from "./RestaurantCardSkeleton";
import { Heart, Store } from "lucide-react";
import { useAuth } from "../../hooks/useAuth";

export function OwnerRestaurantsPage() {
    const isAuthenticated = useAuth();
    const { data, isLoading } = useGetMyRestaurantsQuery(undefined, {
    skip: !isAuthenticated,
  });

    
  if (isLoading) return <RestaurantCardSkeleton count={6} />;

  if (!data || data.length === 0) {
    return (
         <EmptyState
          icon={Heart}
          title="No restaurants yet"
          description="Create your first restaurant to start selling food"
        />
    )
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <header className="flex flex-col gap-4 sm:flex-row sm-items-center sm:justify-between">
        <h1 className="text-2xl font-semibold text-gray-900">My Restaurants</h1>

        <Link to="/restaurants/new">
          <Button variant="amber">Add Restaurant</Button>
        </Link>
      </header>
      <div className="mt-8">
      {data?.length === 0 ? (
        <EmptyState
        icon={Store}
        title="No restaurants yet"
        description="Create your first restaurant to start selling food"
        action={
          <Link to="/restaurants/new">
              <Button variant="amber">Add Restaurant</Button>
            </Link>
          }
          />       
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
          {data!.map(r => (
            <OwnerRestaurantCard key={r.id} restaurant={r} />
          ))}
        </div>
      )}
      </div>
      </div>
    </div>
  );
}
