type StatusBadgeProps = {
  isOpen: boolean;
};

const StatusBadge: React.FC<StatusBadgeProps> = ({ isOpen }) => {
  return (
    <span
      className={`px-2 py-1 text-xs font-medium rounded-full ${
        isOpen
          ? "bg-green-100 text-green-700"
          : "bg-red-100 text-red-600"
      }`}
    >
      {isOpen ? "Open" : "Closed"}
    </span>
  );
};

export default StatusBadge;
