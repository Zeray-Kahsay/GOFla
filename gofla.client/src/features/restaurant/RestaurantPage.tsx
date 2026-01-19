import { Clock, DollarSign, MapPin, Star, Store } from "lucide-react";
import { clsx as cn } from "clsx";
import { Button } from "../../app/layout/ui/Button";
import { LoadingSpinner } from "../../app/layout/ui/LoadingSpinner";
import { Rating } from "../../app/layout/ui/Rating";
import { MenuItemCard } from "../menu/MenuItemCard";
import { ReviewCard } from "../review/ReviewCard";
import { ReviewModal } from "../review/ReviewModal";
import { formatCurrency } from "../../utils/formatters";
import { useInfiniteScroll } from "../../hooks/useInfiniteScroll";
import { useGetRestaurantRatingQuery, useGetRestaurantReviewsQuery } from "../../app/api/review/ReviewApi";
import { useGetMenuItemsByRestaurantQuery } from "../../app/api/menuItem/menuItemApi";
import { useGetRestaurantByIdQuery } from "../../app/api/restaurant/restaurantApi";
import { useEffect, useMemo, useRef, useState } from "react";
import { useAuth } from "../../hooks/useAuth";
import { useParams } from "react-router-dom";
import type { AddressDto } from "../../types/restaurant";
import { EmptyState } from "../../app/layout/ui/EmptyState";
import type { MenuItem } from "../../types/menuItem";
import MenuCategorySkeleton from "../../app/layout/ui/MenuCategorySkeleton";

export default function RestaurantPage() {
  // Router & Auth Hooks
  const { id } = useParams<{ id: string }>();
  const { isAuthenticated } = useAuth();
  const restaurantId = Number(id);
  const categoryRefs = useRef<Record<number, HTMLElement | null>>({});

  // State Hooks
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | 'all'>('all');
  const [showReviewModal, setShowReviewModal] = useState(false);
  const [menuCursor, setMenuCursor] = useState<string | undefined>();
  const [reviewCursor, setReviewCursor] = useState<string | undefined>();

  // API Query Hooks
  const { data: restaurant, isLoading: restaurantLoading } = useGetRestaurantByIdQuery(restaurantId, {
    skip: !restaurantId,
  });
  const { data: menuData, isFetching: menuFetching } = useGetMenuItemsByRestaurantQuery({
    restaurantId,
    cursor: menuCursor,
    pageSize: 20,
  });
  const { data: ratingData } = useGetRestaurantRatingQuery(restaurantId);
  const { data: reviewsData, isFetching: reviewsFetching } = useGetRestaurantReviewsQuery({
    restaurantId,
    cursor: reviewCursor,
    pageSize: 10,
  });

  // Custom Hooks (data transformation)
  const { items: menuItems, loadMoreRef: menuLoadMoreRef } = useInfiniteScroll({
    data: menuData?.items,
    hasMore: menuData?.hasMore || false,
    isLoading: menuFetching,
    fetchMore: () => {
      if (menuData?.nextCursor) {
        setMenuCursor(menuData.nextCursor);
      }
    },
  });

  const { items: reviews, loadMoreRef: reviewLoadMoreRef } = useInfiniteScroll({
    data: reviewsData?.items,
    hasMore: reviewsData?.hasMore || false,
    isLoading: reviewsFetching,
    fetchMore: () => {
      if (reviewsData?.nextCursor) {
        setReviewCursor(reviewsData.nextCursor);
      }
    },
  });
    
    const safeMenuItems = menuItems ?? [];
    
    // Side Effects

  // Only activates when 'All' is selected
  useEffect(() => {
    if (selectedCategoryId !== 'all') return;

    const observer = new IntersectionObserver(
      (entries) => {
        const visible = entries.find((e) => e.isIntersecting);
        if (!visible) return;

        const categoryId = Number(visible.target.getAttribute('data-category-id'));
        if (!Number.isNaN(categoryId)){
          setSelectedCategoryId(categoryId);
        }
      },
      {
        rootMargin: '-100px 0px -60px 0px',
        threshold: 0.1,
      }
    );

    Object.entries(categoryRefs.current).forEach(([id, el]) => {
      if (el){
        el.setAttribute('data-category-id', id);
        observer.observe(el);
      }
    });

    return () => observer.disconnect();
  }, [safeMenuItems, selectedCategoryId]);

  // Memoized Values
  const categories = useMemo(() => {
    const map = new Map<number, string>();

    for (const item of safeMenuItems) {
      map.set(item.categoryId, item.categoryName);
    }

    return [
      { categoryId: 'all' as const, categoryName: 'All' },
      ...Array.from(map.entries()).map(([categoryId, categoryName]) => ({ categoryId, categoryName })),
    ];
  }, [safeMenuItems]);

  const filteredMenuItems = selectedCategoryId === 'all'
    ? safeMenuItems
    : safeMenuItems.filter((item) => item.categoryId === selectedCategoryId);

  const itemsByCategory = useMemo(() => {
    const map = new Map<number, MenuItem[]>();

    for (const item of filteredMenuItems) {
      if (!map.has(item.categoryId)) {
        map.set(item.categoryId, []);
      }
      map.get(item.categoryId)!.push(item);
    }
    return map;
  }, [filteredMenuItems])

  const formatAddress = (address?: AddressDto) => 
      address 
        ? [address.street, address.postalCode, address.city]
           .filter(Boolean)
           .join(", ")
        : "Address not available";
      

  const handleCategoryClick = (categoryId: number | 'all') => {
    setSelectedCategoryId(categoryId);

    if (categoryId === 'all'){
      window.scrollTo({top: 0, behavior: 'smooth'});
      return;
    }

    categoryRefs.current[categoryId]?.scrollIntoView({
      behavior: 'smooth',
      block: 'start'
    });
  };

  
  
  if (restaurantLoading) {
    return <LoadingSpinner fullScreen />;
  }

  if (!restaurant) {
    return <div>Restaurant not found</div>;
  }


  return (
    <div>
      {/* Restaurant Header */}
      <div className="relative h-64 md:h-80">
        <img
          src={restaurant.imageUrl || '/images/img2.jpg'}
          alt={restaurant.name}
          className="w-full h-full object-cover"
        />
        <div className="absolute inset-0 bg-linear-to-t from-black/60 to-transparent" />
        <div className="absolute bottom-0 left-0 right-0 text-white p-8">
          <div className="container mx-auto">
            <h1 className="text-3xl md:text-4xl font-bold mb-2">{restaurant.name}</h1>
            <p className="text-lg mb-4">{restaurant.description}</p>
            <div className="flex flex-wrap items-center gap-6 text-sm">
              <div className="flex items-center gap-2">
                {ratingData && (
                  <Rating rating={ratingData.averageRating} size="sm" showNumber />
                )}
                <span>({ratingData?.totalReviews || 0} reviews)</span>
              </div>
              <div className="flex items-center gap-2">
                <Clock size={16} />
                <span>{restaurant.estimatedDeliveryTime} min</span>
              </div>
              <div className="flex items-center gap-2">
                <DollarSign size={16} />
                <span>{formatCurrency(restaurant.deliveryFee)} delivery</span>
              </div>
              <div className="flex items-center gap-2">
                <MapPin size={16} />
                <span>
                  {formatAddress(restaurant.addressDto)}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="container mx-auto px-4 py-8">
        {/* Menu Section */}
        <section className="mb-12">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-2xl font-bold">Menu</h2>
          </div>

          {/* Category Filter */}
         <div className="sticky top-16 z-30 bg-white border-b">
           <div className="bg-white/95 backdrop-blur">     
               <div className="container mx-auto px-4">
                  <div className="relative flex gap-2 py-3 overflow-x-auto no-scrollbar">
                      {categories.map((category) => (
                        <button
                        key={category.categoryId}
                        onClick={() => handleCategoryClick(category.categoryId)}
                        className={cn(
                          "relative px-4 py-2 rounded-full whitespace-nowrap text-sm font-medium transition",
                          selectedCategoryId === category.categoryId
                          ? "bg-primary-600"
                          : "text-gray-700 hover:bg-gray-900"
                        )}
                        >
                            {category.categoryName}
                            <span className="absolute -bottom-1 left-1/2 h-0.5 w-6 -translate-x-1/2 rounded-full bg-primary-600 transition-all duration-300"/>
                          </button>
                       ))}
                   </div>
                </div>
               </div>
         </div>

          {/* Menu Items */}
          <div className="space-y-10" >
            {Array.from(itemsByCategory.entries()).map(([categoryId, items]) => (
              <section 
                key={categoryId}
                ref={(el) => { 
                  categoryRefs.current[categoryId] = el
                }}
                className="scroll-mt-32"
                >
                <h3 className="text-xl font-semibold mb-4">
                  {items[0]?.categoryName}
                </h3>
                  <div className="space-y-4">
                    {items.map((item) => (
                      <MenuItemCard
                         key={item.id} 
                         item={item}
                         disabled={!item.isAvailable}
                       />
                    ))}
                  </div>

              </section>
            ))}

          </div>

          {menuData?.hasMore && (
            <div ref={menuLoadMoreRef} className="py-8 flex justify-center">
              {menuFetching && (
                <div className="space-y-10" >
                  {Array.from({length: 3}).map((_, i) => (
                    <MenuCategorySkeleton key={i} />
                  ))}
                </div>
              )}
            </div>
          )}
        </section>

        {!menuFetching && filteredMenuItems.length === 0 && (
          <EmptyState 
            icon={Store}
            title="No menu items found"
            description="There are no menu items available for the selected category."
          />
        )}

        {/* Reviews Section */}
        <section>
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-2xl font-bold">Customer Reviews</h2>
            {isAuthenticated && (
              <Button onClick={() => setShowReviewModal(true)} className="bg-amber-500 hover:bg-amber-600">Write a Review</Button>
            )}
          </div>

          {/* Rating Summary */}
          {ratingData && (
            <div className="card p-6 mb-6">
              <div className="flex items-center gap-8">
                <div className="text-center">
                  <p className="text-5xl font-bold text-gray-900">
                    {ratingData.averageRating.toFixed(1)}
                  </p>
                  <Rating rating={ratingData.averageRating} showNumber={false} size="lg" />
                  <p className="text-sm text-gray-600 mt-2">
                    {ratingData.totalReviews} reviews
                  </p>
                </div>

                <div className="flex-1">
                  {[5, 4, 3, 2, 1].map((rating) => (
                    <div key={rating} className="flex items-center gap-3 mb-2">
                      <span className="text-sm w-8">{rating} <Star size={12} className="inline" /></span>
                      <div className="flex-1 h-2 bg-gray-200 rounded-full overflow-hidden">
                        <div
                          className="h-full bg-yellow-400"
                          style={{
                            width: `${
                              ((ratingData.ratingDistribution[rating] || 0) /
                                ratingData.totalReviews) *
                              100
                            }%`,
                          }}
                        />
                      </div>
                      <span className="text-sm text-gray-600 w-12 text-right">
                        {ratingData.ratingDistribution[rating] || 0}
                      </span>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          )}

          {/* Reviews List */}
          <div className="space-y-4">
            {reviews.map((review) => (
              <div key={review.id} className="card p-6">
                <ReviewCard review={review} />
              </div>
            ))}
          </div>

          {reviewsData?.hasMore && (
            <div ref={reviewLoadMoreRef} className="py-8 flex justify-center">
              {reviewsFetching && <LoadingSpinner />}
            </div>
          )}
        </section>
      </div>

      {/* Review Modal */}
      <ReviewModal
        restaurantId={restaurantId}
        isOpen={showReviewModal}
        onClose={() => setShowReviewModal(false)}
      />
    </div>
  );
}