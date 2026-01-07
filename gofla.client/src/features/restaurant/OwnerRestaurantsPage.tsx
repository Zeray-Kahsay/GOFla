import { Link } from "react-router-dom";
import { useGetMyRestaurantsQuery } from "../../app/api/restaurant/restaurantApi";
import { Button } from "../../app/layout/ui/Button";
import { EmptyState } from "../../app/layout/ui/EmptyState";
import { OwnerRestaurantCard } from "./OwnerRestaurantCard";
import RestaurantCardSkeleton from "./RestaurantCardSkeleton";
import { Heart } from "lucide-react";
import { useAuth } from "../../hooks/useAuth";

export function OwnerRestaurantsPage() {
    const isAuthenticated = useAuth();
    const { data, isLoading, isError } = useGetMyRestaurantsQuery(undefined, {
    skip: !isAuthenticated,
  });

    console.log(data, isLoading, isError);
  if (isLoading) return <RestaurantCardSkeleton count={4} />;

  if (!data || data.length === 0) {
    return (
         <EmptyState
          icon={Heart}
          title="No restaurants yet"
          description="Create your first restaurant to start selling"
        />
    )
  }

  return (
    <div className="space-y-6">
      <header className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">My Restaurants</h1>

        <Link to="/restaurants/new">
          <Button variant="amber">Add Restaurant</Button>
        </Link>
      </header>

      {data?.length === 0 ? (
        <EmptyState
          icon={Heart}
          title="No restaurants yet"
          description="Create your first restaurant to start selling"
        />
       
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
          {data!.map(r => (
            <OwnerRestaurantCard key={r.id} restaurant={r} />
          ))}
        </div>
      )}
    </div>
  );
}
