import type { FieldError, UseFormRegisterReturn } from "react-hook-form";


type Props = {
  label?: string;
  registration?: UseFormRegisterReturn;
  placeholder?: string;
  error?: FieldError;
  helperText?: string;
  rows?: number;
  className?: string;
};

export const TextArea = ({
  label,
  registration,
  placeholder,
  error,
  helperText,
  rows = 4,
  className = "",
}: Props) => {
  return (
    <div className="flex flex-col gap-1 w-full">
      {label && (
        <label className="text-sm font-medium text-gray-700">
          {label}
        </label>
      )}

      <textarea
        {...registration}
        placeholder={placeholder}
        rows={rows}
        className={`
          w-full rounded-xl border border-gray-300 bg-amber-50 
          px-4 py-3 text-gray-800 placeholder-gray-400 shadow-sm
          focus:border-amber-400 focus:bg-white focus:ring-2 
          focus:ring-amber-300 transition-all duration-200 outline-none
          ${error ? "border-red-400 focus:ring-red-300" : ""}
          ${className}
        `}
      />

      <div className="min-h-5 text-xs">
        {error ? (
          <p className="text-red-500">{error.message}</p>
        ) : helperText ? (
          <p className="text-gray-400">{helperText}</p>
        ) : null}
      </div>
    </div>
  );
};
