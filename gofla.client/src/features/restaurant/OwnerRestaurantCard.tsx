
import { Link } from "react-router-dom";
import type { Restaurant } from "../../types/restaurant";
import { Button } from "../../app/layout/ui/Button";

interface OwnerRestaurantCardProps {
  restaurant: Restaurant;
  onAddMenuItem: () => void;
}

export function OwnerRestaurantCard({ restaurant, onAddMenuItem }: OwnerRestaurantCardProps) {
  return (
    <div className="rounded-2xl bg-white shadow-sm hover:shadow-md transition p-4">
      <div className="relative h-40 overflow-hidden rounded-lg">
      <img
        src={restaurant.imageUrl || "/images/foodImage.avif"}
        className="h-full w-full object-cover"
      />

       <span
         className={`absolute top-3 left-3 px-3 py-1 rounded-full text-xs font-medium
            ${restaurant.isActive
             ? "bg-green-100 text-green-700"
             : "bg-gray-200 text-gray-600"}`}
      >
        {restaurant.isActive ? "Active" : "Inactive"}
      </span>
      <h3 className="text-lg font-semibold text-gray-900 truncate">
        {restaurant.name}
      </h3>
        <p className="text-sm text-gray-500 line-clamp-2">
        {restaurant.description}
        </p>
      </div>
      <div className="flex gap-2 pt-4">
            <Link to={`/owner/restaurants/${restaurant.id}/images`}
            className="flex-1 text-center rounded-lg border px-3 py-2 text-sm hover:bg-gray-50"
            >
              Image
            </Link>
            <Button
              onClick={onAddMenuItem}
              className="flex-1 text-center rounded-lg bg-amber-600 text-white px-3 py-2 text-sm hover:bg-amber-700"
            >
             Add Menu
            </Button>
      </div>
    </div>
  );
}
