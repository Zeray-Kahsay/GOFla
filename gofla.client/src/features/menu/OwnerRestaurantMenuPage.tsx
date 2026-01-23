import { useEffect, useMemo, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { Plus, Search, Store, LayoutGrid, Table2 } from "lucide-react";

import { Button } from "../../app/layout/ui/Button";
import { EmptyState } from "../../app/layout/ui/EmptyState";
import type { MenuItem } from "../../types/menuItem";
import { OwnerMenuItemCard } from "./OwnerMenuItemCard";
import { AddMenuItemModal } from "../menu/AddMenuItemModal";
import { EditMenuItemModal } from "../menu/EditMenuItemModal";
import { useDebounce } from "../../hooks/useDebounce";
import { useDeleteMenuItemMutation, useGetOwnerMenuItemsQuery, useToggleMenuItemAvailabilityMutation } from "../../app/api/menuItem/menuItemApi";
import { useGetCategoriesByRestaurantQuery } from "../../app/api/category/categoryApi";
import RestaurantCardSkeleton from "../restaurant/RestaurantCardSkeleton";
import { OwnerMenuItemTable } from "./OwnerMenuItemTable";
import { toast } from "react-toastify";

type Availability = "all" | "available" | "unavailable";

export function OwnerRestaurantMenuPage() {
  const { restaurantId } = useParams<{ restaurantId: string }>();
  const id = Number(restaurantId);

  const [showAdd, setShowAdd] = useState(false);
  const [editItem, setEditItem] = useState<MenuItem | null>(null);
  const [busyId, setBusyId] = useState<number | undefined>(undefined);

  // Filters
  const [search, setSearch] = useState("");
  const debouncedSearch = useDebounce(search, 400);

  const [categoryId, setCategoryId] = useState<number | "all">("all");
  const [availability, setAvailability] = useState<Availability>("all");

  // View: cards/table
  const [view, setView] = useState<"cards" | "table">("cards");

  // Pagination
  const [cursor, setCursor] = useState<string | undefined>(undefined);
  const [allItems, setAllItems] = useState<MenuItem[]>([]);
  const [hasMore, setHasMore] = useState(false);
  const [nextCursor, setNextCursor] = useState<string | null>(null);

  // Category list
  const { data: categories = [] } = useGetCategoriesByRestaurantQuery(id, {
    skip: !id,
  });

  const isAvailableParam = useMemo(() => {
    if (availability === "available") return true;
    if (availability === "unavailable") return false;
    return undefined;
  }, [availability]);

  const queryArgs = useMemo(
    () => ({
      restaurantId: id,
      cursor,
      pageSize: 12,
      search: debouncedSearch || undefined,
      categoryId: categoryId === "all" ? undefined : categoryId,
      isAvailable: isAvailableParam,
    }),
    [id, cursor, debouncedSearch, categoryId, isAvailableParam]
  );

  const { data, isLoading, isFetching } = useGetOwnerMenuItemsQuery(queryArgs, {
    skip: !id,
  });

  const [deleteMenuItem, {isLoading: isDeleting}] = useDeleteMenuItemMutation();
  const [toggleAvailability, {isLoading: isToggling}] = useToggleMenuItemAvailabilityMutation();

  // Reset list when filters change
  useEffect(() => {
    setCursor(undefined);
    setAllItems([]);
    setNextCursor(null);
    setHasMore(false);
  }, [debouncedSearch, categoryId, availability]);

  // Append paged data
  useEffect(() => {
    if (!data) return;

    setAllItems((prev) => {
      const incoming = data.items ?? [];
      const map = new Map<number, MenuItem>();

      [...prev, ...incoming].forEach((item) => map.set(item.id, item));
      return Array.from(map.values());
    });

    setHasMore(data.hasMore);
    setNextCursor(data.nextCursor ?? null);
  }, [data]);

  const availableCount = useMemo(
    () => allItems.filter((m) => m.isAvailable).length,
    [allItems]
  );

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

  if (isLoading && allItems.length === 0) return <RestaurantCardSkeleton count={6} />;

  const loadMore = () => {
    if (!nextCursor || isFetching) return;
    setCursor(nextCursor);
  };

  const handleDelete = async (item: MenuItem) => {
    if (!confirm(`Delete ${item.name}`)) return;

    try {
      setBusyId(item.id);
      await deleteMenuItem({restaurantId: id, menuItemId: item.id}).unwrap();
      toast.success("Deleted successfully!");
    } catch {
      toast.error("Deleting failed");
    }
    finally {
      setBusyId(undefined);
    }
  } 

  const handleToggleAvailability = async (item: MenuItem) => {
    if (!confirm(`Change availability ${item.name}`)) return;

    try {
      await toggleAvailability({restaurantId: id, menuItemId: item.id}).unwrap();
      toast.success(item.isAvailable ? "Marked unavailable" : "Marked available");
    } catch {
      toast.error("Failed to update availability");
    }

  }



  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <header className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-gray-900">Menu Items</h1>
            <p className="text-sm text-gray-600 mt-1">
              {availableCount}/{allItems.length} available
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

        {/* Controls */}
        <div className="mt-8 rounded-2xl bg-white border shadow-sm p-4">
          <div className="grid grid-cols-1 md:grid-cols-12 gap-3 items-center">
            {/* Search */}
            <div className="md:col-span-5">
              <div className="relative">
                <Search size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
                <input
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                  placeholder="Search menu items..."
                  className="w-full rounded-xl border border-gray-200 bg-gray-50 pl-10 pr-3 py-2.5 text-sm outline-none focus:ring-2 focus:ring-amber-200 focus:border-amber-300"
                />
              </div>
            </div>

            {/* Category */}
            <div className="md:col-span-3">
              <select
                value={categoryId}
                onChange={(e) =>
                  setCategoryId(e.target.value === "all" ? "all" : Number(e.target.value))
                }
                className="w-full rounded-xl border border-gray-200 bg-gray-50 px-3 py-2.5 text-sm outline-none focus:ring-2 focus:ring-amber-200 focus:border-amber-300"
              >
                <option value="all">All Categories</option>
                {categories.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name}
                  </option>
                ))}
              </select>
            </div>

            {/* Availability */}
            <div className="md:col-span-2">
              <select
                value={availability}
                onChange={(e) => setAvailability(e.target.value as Availability)}
                className="w-full rounded-xl border border-gray-200 bg-gray-50 px-3 py-2.5 text-sm outline-none focus:ring-2 focus:ring-amber-200 focus:border-amber-300"
              >
                <option value="all">All</option>
                <option value="available">Available</option>
                <option value="unavailable">Hidden</option>
              </select>
            </div>

            {/* View Toggle */}
            <div className="md:col-span-2 flex justify-end gap-2">
              <Button
                variant={view === "cards" ? "amber" : "outline"}
                size="sm"
                onClick={() => setView("cards")}
              >
                <LayoutGrid size={16} className="mr-2" />
                Cards
              </Button>
              <Button
                variant={view === "table" ? "amber" : "outline"}
                size="sm"
                onClick={() => setView("table")}
              >
                <Table2 size={16} className="mr-2" />
                Table
              </Button>
            </div>
          </div>
        </div>

        {/* Results */}
        <div className="mt-8">
          {allItems.length === 0 && !isFetching ? (
            <EmptyState
              icon={Store}
              title="No menu items found"
              description="Try changing your filters or add a new menu item."
              action={
                <Button variant="amber" onClick={() => setShowAdd(true)}>
                  <Plus size={18} className="mr-2" />
                  Add Menu Item
                </Button>
              }
            />
          ) : (
            <>
              {/* Cards (mobile-first) */}
              {view === "cards" && (
                <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
                  {allItems.map((item) => (
                    <OwnerMenuItemCard
                      key={item.id}
                      item={item}
                      restaurantId={id}
                      onEdit={() => setEditItem(item)}
                    />
                  ))}
                </div>
              )}

              {/* Table (desktop) */}
              {view === "table" && (
                <OwnerMenuItemTable
                  items={allItems}
                  restaurantId={id}
                  onEdit={(item: MenuItem) => setEditItem(item)}
                  onToggle={handleToggleAvailability }
                  onDelete={handleDelete }
                  busyId={busyId}
                />
              )}

              {/* Load more */}
              {hasMore && (
                <div className="mt-8 flex justify-center">
                  <Button
                    variant="outline"
                    onClick={loadMore}
                    isLoading={isFetching}
                  >
                    Load more
                  </Button>
                </div>
              )}
            </>
          )}
        </div>
      </div>

      {/* Add */}
      <AddMenuItemModal restaurantId={id} isOpen={showAdd} onClose={() => setShowAdd(false)} />

      {/* Edit */}
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



// import { Link, useParams } from "react-router-dom";
// import { useMemo, useState } from "react";
// import { Plus, Store } from "lucide-react";
// import { Button } from "../../app/layout/ui/Button";
// import { EmptyState } from "../../app/layout/ui/EmptyState";
// import { useGetOwnerMenuItemsQuery } from "../../app/api/menuItem/menuItemApi";
// import type { MenuItem } from "../../types/menuItem";
// import RestaurantCardSkeleton from "../restaurant/RestaurantCardSkeleton";
// import { AddMenuItemModal } from "./AddMenuItemModal";
// import { OwnerMenuItemCard } from "./OwnerMenuItemCard";
// import { EditMenuItemModal } from "./EditMenuItemModal";
// import type { PagedResult } from "../../types/pagedResult";

// export function OwnerRestaurantMenuPage() {
//   const { restaurantId } = useParams<{ restaurantId: string }>();
//   const id = Number(restaurantId);

//   const [showAdd, setShowAdd] = useState(false);
//   const [editItem, setEditItem] = useState<MenuItem | null>(null);

//   const { data, isLoading } = useGetOwnerMenuItemsQuery( 
//     {
//       restaurantId: id,
//       pageSize: 24,
//       cursor: undefined,
//       search: "",
//       categoryId: undefined,
//       isAvailable: undefined,
//    },
//    {skip: !id}
// );

//   const paged: PagedResult<MenuItem> | undefined = data;
//   const items = paged?.items ?? [];

//   const availableCount = useMemo(
//     () => items.filter((m) => m.isAvailable).length,
//     [items]
//   );

  
//   if (!id) {
//     return (
//       <EmptyState
//         icon={Store}
//         title="Restaurant not selected"
//         description="Please open one of your restaurants first."
//         action={
//         <Link to="/owner/restaurants">
//             <Button variant="amber">Back to My Restaurants</Button>
//           </Link>
//         }
//         />
//       );
//     }
    
//     if (isLoading) return <RestaurantCardSkeleton count={6} />;


//   return (
//     <div className="min-h-screen bg-gray-50">
//       <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
//         {/* Header */}
//         <header className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
//           <div>
//             <h1 className="text-2xl font-semibold text-gray-900">Menu Items</h1>
//             <p className="text-sm text-gray-600 mt-1">
//               {availableCount}/{items.length} available
//             </p>
//           </div>

//           <div className="flex items-center gap-2">
//             <Link to="/owner/restaurants">
//               <Button variant="outline">Back</Button>
//             </Link>

//             <Button variant="amber" onClick={() => setShowAdd(true)}>
//               <Plus size={18} className="mr-2" />
//               Add Menu Item
//             </Button>
//           </div>
//         </header>

//         {/* Content */}
//         <div className="mt-8">
//           {items.length === 0 ? (
//             <EmptyState
//               icon={Store}
//               title="No menu items yet"
//               description="Add your first menu item so customers can start ordering."
//               action={
//                 <Button variant="amber" onClick={() => setShowAdd(true)}>
//                   <Plus size={18} className="mr-2" />
//                   Add Menu Item
//                 </Button>
//               }
//             />
//           ) : (
//             <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
//               {items.map((item) => (
//                 <OwnerMenuItemCard
//                   key={item.id}
//                   item={item}
//                   restaurantId={id}
//                   onEdit={() => setEditItem(item)}
//                 />
//               ))}
//             </div>
//           )}
//         </div>
//       </div>

//       {/* Add modal */}
//       <AddMenuItemModal
//         restaurantId={id}
//         isOpen={showAdd}
//         onClose={() => setShowAdd(false)}
//       />

//       {/* Edit modal */}
//       {editItem && (
//         <EditMenuItemModal
//           restaurantId={id}
//           item={editItem}
//           isOpen={!!editItem}
//           onClose={() => setEditItem(null)}
//         />
//       )}
//     </div>
//   );
// }
