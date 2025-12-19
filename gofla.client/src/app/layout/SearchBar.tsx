import React from 'react'

const SearchBar = () => {
  return (
    <div>
      Search her...
    </div>
  )
}

export default SearchBar



// export default function SearchPage() {
//   const [searchParams] = useSearchParams();
//   const query = searchParams.get('q') || '';
//   const [activeTab, setActiveTab] = useState<'restaurants' | 'items'>('restaurants');
//   const [filters, setFilters] = useState<FilterValues>({
//     category: searchParams.get('category') || undefined,
//     minPrice: searchParams.get('minPrice') ? Number(searchParams.get('minPrice')) : undefined,
//     maxPrice: searchParams.get('maxPrice') ? Number(searchParams.get('maxPrice')) : undefined,
//     minRating: searchParams.get('minRating') ? Number(searchParams.get('minRating')) : undefined,
//     sortBy: searchParams.get('sortBy') || undefined,
//   });

//   const [searchRestaurants, { data: restaurantData, isLoading: restaurantsLoading }] =
//     useLazySearchRestaurantsQuery();
//   const [searchMenuItems, { data: menuItemData, isLoading: itemsLoading }] =
//     useLazySearchMenuItemsQuery();

//   useEffect(() => {
//     if (query) {
//       searchRestaurants({ query, ...filters, pageSize: 20 });
//       searchMenuItems({ query, ...filters, pageSize: 20 });
//     }
//   }, [query, filters, searchRestaurants, searchMenuItems]);

//   const handleFilterChange = (newFilters: FilterValues) => {
//     setFilters(newFilters);
//   };

//   const isLoading = restaurantsLoading || itemsLoading;

//   if (isLoading && !restaurantData && !menuItemData) {
//     return <LoadingSpinner fullScreen />;
//   }

//   const hasResults =
//     (restaurantData?.items.length || 0) > 0 || (menuItemData?.items.length || 0) > 0;

//   return (
//     <div className="container mx-auto px-4 py-8">
//       <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8">
//         <div>
//           <h1 className="text-3xl font-bold text-gray-900 mb-2">
//             Search Results for "{query}"
//           </h1>
//           <p className="text-gray-600">
//             Found {(restaurantData?.totalCount || 0) + (menuItemData?.totalCount || 0)} results
//           </p>
//         </div>
//         <SearchFilters onFilterChange={handleFilterChange} initialFilters={filters} />
//       </div>

//       {!hasResults ? (
//         <EmptyState
//           icon={Search}
//           title="No results found"
//           description="Try searching with different keywords or adjust your filters"
//         />
//       ) : (
//         <>
//           {/* Tabs */}
//           <div className="flex gap-4 mb-8 border-b">
//             <button
//               onClick={() => setActiveTab('restaurants')}
//               className={`px-4 py-2 font-medium transition-colors ${
//                 activeTab === 'restaurants'
//                   ? 'text-primary-600 border-b-2 border-primary-600'
//                   : 'text-gray-600 hover:text-gray-900'
//               }`}
//             >
//               Restaurants ({restaurantData?.totalCount || 0})
//             </button>
//             <button
//               onClick={() => setActiveTab('items')}
//               className={`px-4 py-2 font-medium transition-colors ${
//                 activeTab === 'items'
//                   ? 'text-primary-600 border-b-2 border-primary-600'
//                   : 'text-gray-600 hover:text-gray-900'
//               }`}
//             >
//               Menu Items ({menuItemData?.totalCount || 0})
//             </button>
//           </div>

//           {/* Loading State */}
//           {isLoading && (
//             <div className="py-8">
//               <LoadingSpinner />
//             </div>
//           )}

//           {/* Content */}
//           {!isLoading && (
//             <>
//               {activeTab === 'restaurants' && restaurantData && restaurantData.items.length > 0 && (
//                 <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
//                   {restaurantData.items.map((restaurant) => (
//                     <RestaurantCard key={restaurant.id} restaurant={restaurant} />
//                   ))}
//                 </div>
//               )}

//               {activeTab === 'restaurants' && restaurantData && restaurantData.items.length === 0 && (
//                 <EmptyState
//                   icon={Search}
//                   title="No restaurants found"
//                   description="Try adjusting your filters or search term"
//                 />
//               )}

//               {activeTab === 'items' && menuItemData && menuItemData.items.length > 0 && (
//                 <div className="space-y-4">
//                   {menuItemData.items.map((item) => (
//                     <MenuItemCard key={item.id} item={item} />
//                   ))}
//                 </div>
//               )}

//               {activeTab === 'items' && menuItemData && menuItemData.items.length === 0 && (
//                 <EmptyState
//                   icon={Search}
//                   title="No menu items found"
//                   description="Try adjusting your filters or search term"
//                 />
//               )}
//             </>
//           )}
//         </>
//       )}
//     </div>
//   );
// }