import { Link, useParams } from "react-router-dom";
import { useMemo, useState } from "react";
import { Plus, Store } from "lucide-react";
import { Button } from "../../app/layout/ui/Button";
import { EmptyState } from "../../app/layout/ui/EmptyState";
import { useGetOwnerMenuItemsQuery } from "../../app/api/menuItem/menuItemApi";
import type { MenuItem } from "../../types/menuItem";
import RestaurantCardSkeleton from "../restaurant/RestaurantCardSkeleton";
import { AddMenuItemModal } from "./AddMenuItemModal";
import { OwnerMenuItemCard } from "./OwnerMenuItemCard";
import { EditMenuItemModal } from "./EditMenuItemModal";

export function OwnerRestaurantMenuPage() {
  const { restaurantId } = useParams<{ restaurantId: string }>();
  const id = Number(restaurantId);

  const [showAdd, setShowAdd] = useState(false);
  const [editItem, setEditItem] = useState<MenuItem | null>(null);

  const { data: menuItems = [], isLoading } = useGetOwnerMenuItemsQuery(id, {
    skip: !id,
  });

  const availableCount = useMemo(
    () => menuItems.filter((m) => m.isAvailable).length,
    [menuItems]
  );

  if (isLoading) return <RestaurantCardSkeleton count={6} />;

  if (!id) {
    return (
      <EmptyState
        icon={Store}
        title="Restaurant not selected"
        description="Please open one of your restaurants first."
        action={
          <Link to="/owner/restaurants">
            <Button variant="amber">Back to My Restaurants</Button>
          </Link>
        }
      />
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <header className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-gray-900">Menu Items</h1>
            <p className="text-sm text-gray-600 mt-1">
              {availableCount}/{menuItems.length} available
            </p>
          </div>

          <div className="flex items-center gap-2">
            <Link to="/owner/restaurants">
              <Button variant="outline">Back</Button>
            </Link>

            <Button variant="amber" onClick={() => setShowAdd(true)}>
              <Plus size={18} className="mr-2" />
              Add Menu Item
            </Button>
          </div>
        </header>

        {/* Content */}
        <div className="mt-8">
          {menuItems.length === 0 ? (
            <EmptyState
              icon={Store}
              title="No menu items yet"
              description="Add your first menu item so customers can start ordering."
              action={
                <Button variant="amber" onClick={() => setShowAdd(true)}>
                  <Plus size={18} className="mr-2" />
                  Add Menu Item
                </Button>
              }
            />
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
              {menuItems.map((item) => (
                <OwnerMenuItemCard
                  key={item.id}
                  item={item}
                  restaurantId={id}
                  onEdit={() => setEditItem(item)}
                />
              ))}
            </div>
          )}
        </div>
      </div>

      {/* Add modal */}
      <AddMenuItemModal
        restaurantId={id}
        isOpen={showAdd}
        onClose={() => setShowAdd(false)}
      />

      {/* Edit modal */}
      {editItem && (
        <EditMenuItemModal
          restaurantId={id}
          item={editItem}
          isOpen={!!editItem}
          onClose={() => setEditItem(null)}
        />
      )}
    </div>
  );
}
