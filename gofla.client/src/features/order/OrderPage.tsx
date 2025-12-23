import { useState } from "react";
import { useGetOrdersQuery } from "../../app/api/order/orderApi";
import { EmptyState } from "../../app/layout/ui/EmptyState";
import { LoadingSpinner } from "../../app/layout/ui/LoadingSpinner";
import { useInfiniteScroll } from "../../hooks/useInfiniteScroll";
import { OrderCard } from "./OrderCard";
import { Package } from "lucide-react";

export default function OrdersPage() {
  const [cursor, setCursor] = useState<string | undefined>();

  const { data, isLoading, isFetching } = useGetOrdersQuery({
    cursor,
    pageSize: 10,
  });

  const { items, loadMoreRef } = useInfiniteScroll({
    data: data?.items,
    hasMore: data?.hasMore || false,
    isLoading: isFetching,
    fetchMore: () => {
      if (data?.nextCursor) {
        setCursor(data.nextCursor);
      }
    },
  });

  if (isLoading) {
    return <LoadingSpinner fullScreen />;
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-900 mb-8">My Orders</h1>

      {items.length === 0 ? (
        <EmptyState
          icon={Package}
          title="No orders yet"
          description="Start ordering delicious food from your favorite restaurants"
          actionLabel="Browse Restaurants"
          onAction={() => window.location.href = '/'}
        />
      ) : (
        <>
          <div className="space-y-4">
            {items.map((order) => (
              <OrderCard key={order.id} order={order} />
            ))}
          </div>

          {data?.hasMore && (
            <div ref={loadMoreRef} className="py-8 flex justify-center">
              {isFetching && <LoadingSpinner />}
            </div>
          )}
        </>
      )}
    </div>
  );
}
