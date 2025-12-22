import { useEffect, useState } from "react";
import { useInView } from 'react-intersection-observer';

interface UseInfiniteScrollOptions<T> {
  data: T[] | undefined;
  hasMore: boolean;
  isLoading: boolean;
  fetchMore: () => void;
}

export function useInfiniteScroll<T>({ 
  data, 
  hasMore, 
  isLoading, 
  fetchMore 
}: UseInfiniteScrollOptions<T>) {
  const [allItems, setAllItems] = useState<T[]>([]);
  const { ref, inView } = useInView({
    threshold: 0,
    rootMargin: '100px',
  });

  // Update items when new data arrives
  useEffect(() => {
    if (data) {
      setAllItems((prev) => {
        // Prevent duplicates
        const existingIds = new Set(prev.map((item: any) => item.id));
        const newItems = data.filter((item: any) => !existingIds.has(item.id));
        return [...prev, ...newItems];
      });
    }
  }, [data]);

  // Fetch more when scrolling into view
  useEffect(() => {
    if (inView && hasMore && !isLoading) {
      fetchMore();
    }
  }, [inView, hasMore, isLoading, fetchMore]);

  const reset = () => {
    setAllItems([]);
  };

  return {
    items: allItems,
    loadMoreRef: ref,
    reset,
  };
}