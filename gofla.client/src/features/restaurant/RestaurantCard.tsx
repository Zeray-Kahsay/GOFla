import { Link } from 'react-router-dom';
import { Clock, DollarSign, Heart } from 'lucide-react';
import { Rating } from '../../app/layout/ui/Rating';
import type { Restaurant } from '../../types/restaurant';
import { useAuth } from '../../hooks/useAuth';
import { useAddFavoriteMutation, useRemoveFavoriteMutation } from '../../app/api/favorite/FavoriteApi';
import { toast } from 'react-toastify';
import { formatCurrency } from '../../utils/formatters';

interface RestaurantCardProps {
  restaurant: Restaurant;
}

export function RestaurantCard({ restaurant }: RestaurantCardProps) {
  const { isAuthenticated } = useAuth();
  const [addFavorite] = useAddFavoriteMutation();
  const [removeFavorite] = useRemoveFavoriteMutation();

  const handleFavoriteClick = async (e: React.MouseEvent) => {
    e.preventDefault();
    
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
    <Link to={`/restaurant/${restaurant.id}`} className="card hover:shadow-lg transition-shadow">
      <div className="relative h-48 overflow-hidden">
        <img
          src={restaurant.imageUrl}
          alt={restaurant.name}
          className="w-full h-full object-cover"
        />
        {isAuthenticated && (
          <button
            onClick={handleFavoriteClick}
            className="absolute top-3 right-3 p-2 bg-white rounded-full shadow-md hover:scale-110 transition-transform"
          >
            <Heart
              className={restaurant.isFavorite ? 'text-red-500 fill-red-500' : 'text-gray-400'}
              size={20}
            />
          </button>
        )}
      </div>
      
      <div className="p-4">
        <h3 className="text-lg font-semibold text-gray-900 mb-1">{restaurant.name}</h3>
        <p className="text-sm text-gray-600 mb-3 line-clamp-2">{restaurant.description}</p>
        
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
        
        <div className="flex items-center justify-between text-sm text-gray-600">
          <div className="flex items-center gap-1">
            <Clock size={16} />
            <span>{restaurant.estimatedDeliveryTime} min</span>
          </div>
          <div className="flex items-center gap-1">
            <DollarSign size={16} />
            <span>{formatCurrency(restaurant.deliveryFee)} delivery</span>
          </div>
        </div>
      </div>
    </Link>
  );
}