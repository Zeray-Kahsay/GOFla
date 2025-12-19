import { Star } from 'lucide-react';
import clsx from 'clsx';

interface RatingProps {
  rating: number;
  maxRating?: number;
  size?: 'sm' | 'md' | 'lg';
  showNumber?: boolean;
  interactive?: boolean;
  onChange?: (rating: number) => void;
}

export function Rating({
  rating,
  maxRating = 5,
  size = 'md',
  showNumber = true,
  interactive = false,
  onChange,
}: RatingProps) {
  const sizes = {
    sm: 'w-4 h-4',
    md: 'w-5 h-5',
    lg: 'w-6 h-6',
  };

  const handleClick = (value: number) => {
    if (interactive && onChange) {
      onChange(value);
    }
  };

  return (
    <div className="flex items-center gap-1">
      {Array.from({ length: maxRating }, (_, i) => {
        const starValue = i + 1;
        const isFilled = starValue <= Math.floor(rating);
        const isHalfFilled = starValue === Math.ceil(rating) && rating % 1 !== 0;

        return (
          <button
            key={i}
            type="button"
            onClick={() => handleClick(starValue)}
            disabled={!interactive}
            className={clsx(
              'relative',
              interactive && 'cursor-pointer hover:scale-110 transition-transform'
            )}
          >
            {isHalfFilled ? (
              <div className="relative">
                <Star className={clsx(sizes[size], 'text-gray-300')} />
                <div className="absolute inset-0 overflow-hidden" style={{ width: '50%' }}>
                  <Star className={clsx(sizes[size], 'text-yellow-400 fill-yellow-400')} />
                </div>
              </div>
            ) : (
              <Star
                className={clsx(
                  sizes[size],
                  isFilled ? 'text-yellow-400 fill-yellow-400' : 'text-gray-300'
                )}
              />
            )}
          </button>
        );
      })}
      {showNumber && (
        <span className="ml-1 text-sm font-medium text-gray-700">
          {rating.toFixed(1)}
        </span>
      )}
    </div>
  );
}