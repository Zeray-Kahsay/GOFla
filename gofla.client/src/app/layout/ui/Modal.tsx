import { useEffect, useRef } from "react";
import { X } from "lucide-react";

type ModalProps = {
  isOpen: boolean;
  title?: string;
  description?: string;
  onClose: () => void;
  children: React.ReactNode;
  footer?: React.ReactNode;
  size?: "sm" | "md" | "lg";
  disableClose?: boolean; // disable closing when loading
};

const sizeMap: Record<NonNullable<ModalProps["size"]>, string> = {
  sm: "max-w-md",
  md: "max-w-xl",
  lg: "max-w-3xl",
};

export function Modal({
  isOpen,
  title,
  description,
  onClose,
  children,
  footer,
  size = "md",
  disableClose = false,
}: ModalProps) {
  const panelRef = useRef<HTMLDivElement | null>(null);
  const lastActiveElRef = useRef<HTMLElement | null>(null);

  // Lock background scroll when modal opens
  useEffect(() => {
    if (!isOpen) return;

    lastActiveElRef.current = document.activeElement as HTMLElement;

    const originalOverflow = document.body.style.overflow;
    document.body.style.overflow = "hidden";

    return () => {
      document.body.style.overflow = originalOverflow;
      lastActiveElRef.current?.focus?.();
    };
  }, [isOpen]);

  // ESC close
  useEffect(() => {
    if (!isOpen) return;

    const handler = (e: KeyboardEvent) => {
      if (e.key === "Escape" && !disableClose) onClose();
    };

    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, [isOpen, onClose, disableClose]);

  // Focus trap
  useEffect(() => {
    if (!isOpen) return;

    const panel = panelRef.current;
    if (!panel) return;

    const focusable = panel.querySelectorAll<HTMLElement>(
      'a[href], button:not([disabled]), textarea, input, select, [tabindex]:not([tabindex="-1"])'
    );

    const first = focusable[0];
    const last = focusable[focusable.length - 1];

    first?.focus();

    const trap = (e: KeyboardEvent) => {
      if (e.key !== "Tab") return;
      if (focusable.length === 0) return;

      if (e.shiftKey && document.activeElement === first) {
        e.preventDefault();
        last?.focus();
      } else if (!e.shiftKey && document.activeElement === last) {
        e.preventDefault();
        first?.focus();
      }
    };

    panel.addEventListener("keydown", trap);
    return () => panel.removeEventListener("keydown", trap);
  }, [isOpen]);

  if (!isOpen) return null;

  return (
    <div
      className="fixed inset-0 z-50"
      aria-modal="true"
      role="dialog"
    >
      {/* Backdrop */}
      <div
        className="absolute inset-0 bg-black/40 backdrop-blur-sm opacity-0 animate-[fadeIn_.18s_ease-out_forwards]"
        onMouseDown={() => {
          if (!disableClose) onClose();
        }}
      />

      {/* Panel */}
      <div className="relative h-full w-full flex items-center justify-center p-4">
        <div
          ref={panelRef}
          onMouseDown={(e) => e.stopPropagation()}
          className={[
            "w-full rounded-2xl bg-white shadow-2xl border border-black/5 overflow-hidden",
            "opacity-0 translate-y-2 animate-[modalIn_.22s_ease-out_forwards]",
            sizeMap[size],
          ].join(" ")}
        >
          {/* Header */}
          {(title || description) && (
            <div className="px-6 py-4 border-b bg-linear-to-b from-amber-50 to-white">
              <div className="flex items-start justify-between gap-4">
                <div>
                  {title && (
                    <h2 className="text-lg font-semibold text-gray-900">
                      {title}
                    </h2>
                  )}
                  {description && (
                    <p className="text-xs text-gray-500 mt-1">
                      {description}
                    </p>
                  )}
                </div>

                <button
                  disabled={disableClose}
                  onClick={onClose}
                  aria-label="Close"
                  className="p-2 rounded-xl hover:bg-gray-100 transition disabled:opacity-50"
                >
                  <X className="w-5 h-5" />
                </button>
              </div>
            </div>
          )}

          {/* Body */}
          <div className="px-6 py-5 max-h-[70vh] overflow-auto">
            {children}
          </div>

          {/* Footer */}
          {footer && (
            <div className="px-6 py-4 border-t bg-white flex justify-end gap-3">
              {footer}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
