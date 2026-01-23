import { Link, useNavigate } from "react-router-dom";
import type { Restaurant } from "../../types/restaurant";
import { Button } from "../../app/layout/ui/Button";
import { ImageIcon, Plus, ChevronRight } from "lucide-react";

interface OwnerRestaurantCardProps {
  restaurant: Restaurant;
  onAddMenuItem: () => void;
}

export function OwnerRestaurantCard({
  restaurant,
  onAddMenuItem,
}: OwnerRestaurantCardProps) {
  const navigate = useNavigate();

  const handleNavigate = () => {
    navigate(`/owner/restaurants/${restaurant.id}/menu`);
  };

  return (
    <div
      role="button"
      tabIndex={0}
      onClick={handleNavigate}
      onKeyDown={(e) => {
        if (e.key === "Enter" || e.key === " ") {
          e.preventDefault();
          handleNavigate();
        }
      }}
      className="group cursor-pointer rounded-2xl bg-white shadow-sm border border-black/5 hover:shadow-lg transition overflow-hidden focus:outline-none focus:ring-2 focus:ring-amber-400"
    >
      {/* IMAGE */}
      <div className="relative h-44 overflow-hidden">
        <img
          src={restaurant.imageUrl || "/images/foodImage.avif"}
          alt={restaurant.name}
          className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105"
        />

        {/* Gradient overlay */}
        <div className="absolute inset-0 bg-linear-to-t from-black/50 via-black/10 to-transparent" />

        {/* Status badge */}
        <span
          className={`absolute top-3 left-3 rounded-full px-3 py-1 text-xs font-medium ${
            restaurant.isActive
              ? "bg-green-100 text-green-700"
              : "bg-gray-200 text-gray-600"
          }`}
        >
          {restaurant.isActive ? "Active" : "Inactive"}
        </span>

        {/* subtle arrow hint */}
        <div className="absolute bottom-3 right-3 flex items-center gap-1 rounded-full bg-white/90 backdrop-blur px-2.5 py-1 text-xs font-medium text-gray-800">
          Manage <ChevronRight size={14} />
        </div>
      </div>

      {/* CONTENT */}
      <div className="p-4">
        <h3 className="text-lg font-semibold text-gray-900 truncate">
          {restaurant.name}
        </h3>

        <p className="mt-1 text-sm text-gray-600 line-clamp-2">
          {restaurant.description}
        </p>

        {/* ACTIONS */}
        <div className="mt-4 flex gap-2">
          {/* Image button */}
          <Link
            to={`/owner/restaurants/${restaurant.id}/images`}
            onClick={(e) => e.stopPropagation()}
            className="flex-1"
          >
            <Button
              type="button"
              variant="outline"
              className="w-full justify-center"
              onClick={(e) => e.stopPropagation()}
            >
              <ImageIcon size={16} className="mr-2" />
              Image
            </Button>
          </Link>

          {/* Add menu item */}
          <Button
            type="button"
            onClick={(e) => {
              e.stopPropagation();
              onAddMenuItem();
            }}
            className="flex-1 bg-amber-600 hover:bg-amber-700"
          >
            <Plus size={16} className="mr-2" />
            Add Menu
          </Button>
        </div>
      </div>
    </div>
  );
}
