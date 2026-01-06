export default function RestaurantCardSkeleton() {
  return (
    <div className="rounded-2xl bg-white shadow-sm overflow-hidden animate-pulse">
      <div className="h-44 bg-gray-200" />

      <div className="p-4 space-y-4">
        <div className="h-4 w-2/3 bg-gray-200 rounded" />
        <div className="h-3 w-full bg-gray-200 rounded" />
        <div className="h-3 w-5/6 bg-gray-200 rounded" />

        <div className="flex justify-between pt-2">
          <div className="h-3 w-20 bg-gray-200 rounded" />
          <div className="h-3 w-16 bg-gray-200 rounded" />
        </div>
      </div>
    </div>
  );
}
