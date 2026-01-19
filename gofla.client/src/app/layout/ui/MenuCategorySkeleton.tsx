
const MenuCategorySkeleton = () => {
  return (
    <div className="space-y-4 animate-pulse">
        <div className="h-6 w-40 bg-gray-200 rounded" />
            {Array.from({length: 3}).map((_, i) => (
                <div key={i}  className="h-24 bg-gray-100 rounded-lg" />
            ))}
    </div>
  )
}

export default MenuCategorySkeleton
