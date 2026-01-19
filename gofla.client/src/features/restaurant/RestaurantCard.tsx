import { Link } from 'react-router-dom';
import { Clock,  Heart, MapPin } from 'lucide-react';
import type { Restaurant } from '../../types/restaurant';
import { useAuth } from '../../hooks/useAuth';
import { useAddFavoriteMutation, useRemoveFavoriteMutation } from '../../app/api/favorite/FavoriteApi';
import { toast } from 'react-toastify';
import { Rating } from '../../app/layout/ui/Rating';

interface RestaurantCardProps {
  restaurant: Restaurant;
}

export default function RestaurantCard({ restaurant }: RestaurantCardProps) {
  const { isAuthenticated } = useAuth();
  const [addFavorite] = useAddFavoriteMutation();
  const [removeFavorite] = useRemoveFavoriteMutation();

  const handleFavoriteClick = async (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    
    if (!isAuthenticated) {
      toast.info('Please login to add favorites');
      return;
    }

    try {
      if (restaurant.isFavorite) {
        await removeFavorite(restaurant.id).unwrap();
        toast.success('Removed from favorites');
      } else {
        await addFavorite(restaurant.id).unwrap();
        toast.success('Added to favorites');
      }
    } catch (error) {
      toast.error('Failed to update favorites');
    }
  };
  return (
    <Link
      to={`/restaurants/${restaurant.id}`}
      className="group block rounded-2xl overflow-hidden bg-white shadow-sm hover:shadow-xl transition-shadow duration-300"
    >
      {/* IMAGE */}
      <div className="relative h-44 overflow-hidden">
        <img
          src={restaurant.imageUrl || "/images/img2.jpg"}
          alt={restaurant.name}
          className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105"
        />

          {restaurant.isActive === false && (
          <span className="absolute top-3 left-3 rounded-full bg-red-600 px-3 py-1 text-xs text-white" >
            Closed
          </span>
        )}
        {isAuthenticated && (
          <button
          type="button"
          aria-label={
            restaurant.isFavorite
              ? "Remove frm favorite"
              : "Add to favorite"
          }
            onClick={handleFavoriteClick}
            className="absolute top-3 right-3 z-20 p-2 bg-white rounded-full shadow-md hover:scale-110 transition-transform"
          >
            <Heart
              className={restaurant.isFavorite ? 'text-red-500 fill-red-500' : 'text-gray-400'}
              size={20}
            />
          </button>
        )}

        {/* Overlay */}
        <div className="absolute inset-0 bg-linear-to-t from-black/60 via-black/20 to-transparent" />

        {/* Delivery time badge */}
        <div className="absolute bottom-3 left-3 flex items-center gap-1 bg-white/90 backdrop-blur px-2.5 py-1 rounded-full text-xs font-medium">
          <Clock size={14} />
          {restaurant.estimatedDeliveryTime} min
        </div>
      </div>

      {/* CONTENT */}
      <div className="p-4 space-y-2">
        {/* Name */}
        <h3 className="text-lg font-semibold text-gray-900 truncate">
          {restaurant.name}
        </h3>

         <div className="flex items-center justify-between mb-3">
           {restaurant.averageRating ? (
            <Rating rating={restaurant.averageRating} size="sm" />
          ) : (
            <span className="text-sm text-gray-500">No reviews yet</span>
          )}
          {restaurant.reviewCount && restaurant.reviewCount > 0 && (
            <span className="text-xs text-gray-500">({restaurant.reviewCount})</span>
          )}
        </div>

        {/* Address */}
        {restaurant.addressDto && (
          <div className="flex items-start gap-1.5 text-sm text-gray-500">
            <MapPin size={14} className="mt-0.5 shrink-0" />
            <span className="line-clamp-1">
              {restaurant.addressDto.street}, {restaurant.addressDto.city}
            </span>
          </div>
        )}

        {/* Meta */}
        <div className="flex items-center justify-between pt-2 text-sm">
          <span className="text-gray-600">
            Delivery fee{" "}
            <span className="font-medium text-gray-900">
              {restaurant.deliveryFee === 0
                ? "Free"
                : `${restaurant.deliveryFee} kr`}
            </span>
          </span>

          <span
            className={`px-2 py-0.5 rounded-full text-xs font-medium ${
              restaurant.isActive
                ? "bg-green-100 text-green-700"
                : "bg-gray-100 text-gray-500"
            }`}
          >
            {restaurant.isActive !== false ? "Open" : "Closed"}
          </span>
        </div>
      </div>
    </Link>
  );
}

// export function RestaurantCard({ restaurant }: RestaurantCardProps) {
//   const { isAuthenticated } = useAuth();
//   const [addFavorite] = useAddFavoriteMutation();
//   const [removeFavorite] = useRemoveFavoriteMutation();

//   const handleFavoriteClick = async (e: React.MouseEvent) => {
//     e.preventDefault();
    
//     if (!isAuthenticated) {
//       toast.info('Please login to add favorites');
//       return;
//     }

//     try {
//       if (restaurant.isFavorite) {
//         await removeFavorite(restaurant.id).unwrap();
//         toast.success('Removed from favorites');
//       } else {
//         await addFavorite(restaurant.id).unwrap();
//         toast.success('Added to favorites');
//       }
//     } catch (error) {
//       toast.error('Failed to update favorites');
//     }
//   };

//   return (
//     <Link to={`/restaurants/${restaurant.id}`} className="group rounded-2xl bg-white shadow-sm hover:shadow-xl transition-all overflow-hidden">
//       <div className="relative h-48 overflow-hidden">
//         <img
//           src={restaurant.imageUrl}
//           alt={restaurant.name}
//           className="h-full w-full object-cover group-hover:scale-105 transition-transform duration-300"
//         />
//         <div className="absolute inset-0 bg-linear-to-t from-black/60 via-black/20 to-transparent">
//                {/* Delivery time badge */}
//         <div className="absolute bottom-3 left-3 flex items-center gap-1 bg-white/90 backdrop-blur px-2.5 py-1 rounded-full text-xs font-medium">
//           <Clock size={14} />
//           {restaurant.estimatedDeliveryTime} min
//         </div>
//         </div>
//         {!restaurant.isActive && (
//           <span className="absolute top-3 left-3 rounded-full bg-red-600 px-3 py-1 text-xs text-white" >
//             Closed
//           </span>
//         )}
//         {isAuthenticated && (
//           <button
//             onClick={handleFavoriteClick}
//             className="absolute top-3 right-3 p-2 bg-white rounded-full shadow-md hover:scale-110 transition-transform"
//           >
//             <Heart
//               className={restaurant.isFavorite ? 'text-red-500 fill-red-500' : 'text-gray-400'}
//               size={20}
//             />
//           </button>
//         )}
//       </div>
      
//       <div className="p-4">
//         <h3 className="text-lg font-semibold text-gray-900 mb-1">{restaurant.name}</h3>
//         <p className="text-sm text-gray-600 mb-3 line-clamp-2">{restaurant.description}</p>
        
//         <div className="flex items-center justify-between mb-3">
//           {restaurant.averageRating ? (
//             <Rating rating={restaurant.averageRating} size="sm" />
//           ) : (
//             <span className="text-sm text-gray-500">No reviews yet</span>
//           )}
//           {restaurant.reviewCount && restaurant.reviewCount > 0 && (
//             <span className="text-xs text-gray-500">({restaurant.reviewCount})</span>
//           )}
//         </div>
        
//         <div className="flex items-center justify-between text-sm text-gray-600">
//           <div className="flex items-center gap-1">
//                {restaurant.isActive && (
//           <span className="absolute top-3 left-3 rounded-full bg-red-600 px-3 py-1 text-xs text-white" >
//             Closed
//           </span>
//         )}
         
//             {/* <Clock size={16} />
//             <span>{restaurant.estimatedDeliveryTime} min</span> */}
//           </div>
//           <div className="flex items-center gap-1">
//             <DollarSign size={16} />
//             <span>{formatCurrency(restaurant.deliveryFee)} delivery</span>
//           </div>
//         </div>
//       </div>
//     </Link>
//   );
// }


// import { Link } from 'react-router-dom';
// import { Clock, DollarSign, Heart } from 'lucide-react';
// import { Rating } from '../../app/layout/ui/Rating';
// import type { Restaurant } from '../../types/restaurant';
// import { useAuth } from '../../hooks/useAuth';
// import { useAddFavoriteMutation, useRemoveFavoriteMutation } from '../../app/api/favorite/FavoriteApi';
// import { toast } from 'react-toastify';
// import { formatCurrency } from '../../utils/formatters';

// interface RestaurantCardProps {
//   restaurant: Restaurant;
// }

// export function RestaurantCard({ restaurant }: RestaurantCardProps) {
//   const { isAuthenticated } = useAuth();
//   const [addFavorite] = useAddFavoriteMutation();
//   const [removeFavorite] = useRemoveFavoriteMutation();

//   const handleFavoriteClick = async (e: React.MouseEvent) => {
//     e.preventDefault();
    
//     if (!isAuthenticated) {
//       toast.info('Please login to add favorites');
//       return;
//     }

//     try {
//       if (restaurant.isFavorite) {
//         await removeFavorite(restaurant.id).unwrap();
//         toast.success('Removed from favorites');
//       } else {
//         await addFavorite(restaurant.id).unwrap();
//         toast.success('Added to favorites');
//       }
//     } catch (error) {
//       toast.error('Failed to update favorites');
//     }
//   };

//   return (
//     <Link to={`/restaurants/${restaurant.id}`} className="card hover:shadow-lg transition-shadow">
//       <div className="relative h-48 overflow-hidden">
//         <img
//           src={restaurant.imageUrl}
//           alt={restaurant.name}
//           className="w-full h-full object-cover"
//         />
//         {isAuthenticated && (
//           <button
//             onClick={handleFavoriteClick}
//             className="absolute top-3 right-3 p-2 bg-white rounded-full shadow-md hover:scale-110 transition-transform"
//           >
//             <Heart
//               className={restaurant.isFavorite ? 'text-red-500 fill-red-500' : 'text-gray-400'}
//               size={20}
//             />
//           </button>
//         )}
//       </div>
      
//       <div className="p-4">
//         <h3 className="text-lg font-semibold text-gray-900 mb-1">{restaurant.name}</h3>
//         <p className="text-sm text-gray-600 mb-3 line-clamp-2">{restaurant.description}</p>
        
//         <div className="flex items-center justify-between mb-3">
//           {restaurant.averageRating ? (
//             <Rating rating={restaurant.averageRating} size="sm" />
//           ) : (
//             <span className="text-sm text-gray-500">No reviews yet</span>
//           )}
//           {restaurant.reviewCount && restaurant.reviewCount > 0 && (
//             <span className="text-xs text-gray-500">({restaurant.reviewCount})</span>
//           )}
//         </div>
        
//         <div className="flex items-center justify-between text-sm text-gray-600">
//           <div className="flex items-center gap-1">
//             <Clock size={16} />
//             <span>{restaurant.estimatedDeliveryTime} min</span>
//           </div>
//           <div className="flex items-center gap-1">
//             <DollarSign size={16} />
//             <span>{formatCurrency(restaurant.deliveryFee)} delivery</span>
//           </div>
//         </div>
//       </div>
//     </Link>
//   );
// }