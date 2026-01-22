
import * as React from "react";


type TextAreaProps = React.TextareaHTMLAttributes<HTMLTextAreaElement> & {
  label?: string;
  error?: string;
};

export const TextArea = React.forwardRef<HTMLTextAreaElement, TextAreaProps>(
  ({ label, error, className = "", ...props }, ref) => {
    return (
      <div className="space-y-1">
        {label && (
          <label className="block text-sm font-medium text-gray-800">
            {label}
          </label>
        )}

        <textarea
          ref={ref}
          className={[
            "w-full rounded-xl border bg-white px-4 py-3 text-gray-900 placeholder-gray-400 shadow-sm",
            "focus:outline-none focus:ring-2 focus:ring-amber-300 focus:border-amber-400",
            "transition duration-200",
            error ? "border-red-300 focus:ring-red-200 focus:border-red-400" : "border-gray-200",
            className,
          ].join(" ")}
          {...props}
        />

        {error && <p className="text-xs text-red-600">{error}</p>}
      </div>
    );
  }
);

TextArea.displayName = "TextArea";


