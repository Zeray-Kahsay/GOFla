import { Link } from "react-router-dom";
import { useGetMyRestaurantsQuery } from "../../app/api/restaurant/restaurantApi";
import { Button } from "../../app/layout/ui/Button";
import { EmptyState } from "../../app/layout/ui/EmptyState";
import { OwnerRestaurantCard } from "./OwnerRestaurantCard";
import RestaurantCardSkeleton from "./RestaurantCardSkeleton";
import { Store } from "lucide-react";
import { useAuth } from "../../hooks/useAuth";
import { useMemo, useState } from "react";
import { AddMenuItemModal } from "../menu/AddMenuItemModal";
import { RestaurantImageModal } from "./RestaurantImageModal";
import type { Restaurant } from "../../types/restaurant";

export function OwnerRestaurantsPage() {
  const { isAuthenticated } = useAuth();
  const { data: restaurants = [], isLoading } = useGetMyRestaurantsQuery(undefined, {
    skip: !isAuthenticated,
  });
  const [showAddMenuModal, setShowAddMenuModal] = useState(false);
  const [activeRestaurantId, setActiveRestaurantId] = useState<number | null>(null);
  const [imageRestaurant, setImageRestaurant] = useState<Restaurant | null>(null);

  const activeRestaurant = useMemo(() => {
    if (!activeRestaurantId) return null;
    return restaurants.find((r) => r.id === activeRestaurantId) ?? null;
  }, [activeRestaurantId, restaurants]);

  const handleOpenAddMenu = (restaurantId: number) => {
    setActiveRestaurantId(restaurantId);
    setShowAddMenuModal(true);
  };

  const handleCloseAddMenu = () => {
    setShowAddMenuModal(false);
    setActiveRestaurantId(null);
  };

  if (isLoading) return <RestaurantCardSkeleton count={6} />;

  if (!restaurants || restaurants.length === 0) {
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
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
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* HEADER */}
        <header className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <h1 className="text-2xl font-semibold text-gray-900">My Restaurants</h1>

          <Link to="/restaurants/new">
            <Button variant="amber">Add Restaurant</Button>
          </Link>
  
        </header>

        {/* GRID */}
        <div className="mt-8 grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
          {restaurants.map((r) => (
            <OwnerRestaurantCard
              key={r.id}
              restaurant={r}
              onAddMenuItem={() => handleOpenAddMenu(r.id)}
              onEditImage={() => setImageRestaurant(r)}
            />
          ))}
        </div>

        {/* SINGLE MODAL */}
        {activeRestaurantId !== null && (
          <AddMenuItemModal
            restaurantId={activeRestaurantId}
            isOpen={showAddMenuModal}
            onClose={handleCloseAddMenu}
          />
        )}

      </div>
      <RestaurantImageModal 
        restaurant={imageRestaurant}
        isOpen={!!imageRestaurant}
        onClose={() => setImageRestaurant(null)}
      />
    </div>
  );
}
