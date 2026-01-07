import { Link } from "react-router-dom";
import { Button } from "../../app/layout/ui/Button";
import type { Restaurant } from "../../types/restaurant";
import StatusBadge from "../../app/layout/ui/StatusBadge";

export function OwnerRestaurantCard({ restaurant }: { restaurant: Restaurant }) {
  return (
    <div className="rounded-xl border bg-white shadow-sm p-4 space-y-3">
      <img
        src={restaurant.imageUrl || "/images/foodImage.avif"}
        className="h-36 w-full rounded-lg object-cover"
      />

      <div className="flex items-center justify-between">
        <h3 className="font-semibold truncate">{restaurant.name}</h3>

        <StatusBadge isOpen={restaurant.isActive} />
      </div>

      <div className="flex gap-2">
        <Link to={`/owner/restaurants/${restaurant.id}/edit`}>
          <Button size="sm" variant="outline">Edit</Button>
        </Link>

        <Link to={`/owner/restaurants/${restaurant.id}/menu`}>
          <Button size="sm" variant="amber">Menu</Button>
        </Link>
      </div>
    </div>
  );
}
