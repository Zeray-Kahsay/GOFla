type RestaurantCardSkeletonProps = {
  count?: number;
};

const RestaurantCardSkeleton: React.FC<RestaurantCardSkeletonProps> = ({
  count = 1,
}) => {
  return (
    <>
      {Array.from({ length: count }).map((_, i) => (
        <div
          key={i}
          className="animate-pulse rounded-2xl bg-white shadow p-4 space-y-4"
        >
          <div className="h-40 w-full bg-gray-200 rounded-xl" />
          <div className="h-4 bg-gray-200 rounded w-3/4" />
          <div className="h-3 bg-gray-200 rounded w-1/2" />
        </div>
      ))}
    </>
  );
};

export default RestaurantCardSkeleton;



// export default function RestaurantCardSkeleton() {
//   return (
//     <div className="rounded-2xl bg-white shadow-sm overflow-hidden animate-pulse">
//       <div className="h-44 bg-gray-200" />

//       <div className="p-4 space-y-4">
//         <div className="h-4 w-2/3 bg-gray-200 rounded" />
//         <div className="h-3 w-full bg-gray-200 rounded" />
//         <div className="h-3 w-5/6 bg-gray-200 rounded" />

//         <div className="flex justify-between pt-2">
//           <div className="h-3 w-20 bg-gray-200 rounded" />
//           <div className="h-3 w-16 bg-gray-200 rounded" />
//         </div>
//       </div>
//     </div>
//   );
// }
