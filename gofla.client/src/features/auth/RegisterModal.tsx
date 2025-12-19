import { X } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useAppDispatch } from '../../app/store/store';
import { useRegisterMutation } from '../../app/api/auth/authApi';
import { registerSchema, type RegisterFormData } from '../../utils/registerSchema';
import { setCredentials } from '../../app/store/slices/authSlice';
import { toast } from 'react-toastify';
import { Input } from '../../app/layout/ui/Input';
import { Button } from '../../app/layout/ui/Button';

interface RegisterModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSwitchToLogin?: () => void;
}

export function RegisterModal({ isOpen, onClose, onSwitchToLogin }: RegisterModalProps) {
  const dispatch = useAppDispatch();
  const [register, { isLoading }] = useRegisterMutation();

  const {
    register: registerField,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
  });

  const onSubmit = async (data: RegisterFormData) => {
    try {
      const response = await register(data).unwrap();
      dispatch(setCredentials(response));
      toast.success('Account created successfully!');
      reset();
      onClose();
    } catch (error: any) {
      toast.error(error?.data?.message || 'Failed to create account');
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div 
        className="absolute inset-0 bg-black bg-opacity-50" 
        onClick={onClose}
        aria-hidden="true"
      />
      
      <div className="relative bg-white rounded-lg shadow-xl max-w-md w-full p-6 max-h-[90vh] overflow-y-auto">
        <button
          onClick={onClose}
          className="absolute top-4 right-4 text-gray-400 hover:text-gray-600 transition-colors"
          aria-label="Close"
        >
          <X size={24} />
        </button>

        <h2 className="text-2xl font-bold mb-2">Create Account</h2>
        <p className="text-gray-600 mb-6">Join us today and start ordering!</p>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <Input
              label="First Name"
              {...registerField('firstName')}
              error={errors.firstName?.message}
              placeholder="John"
              autoComplete="given-name"
            />

            <Input
              label="Last Name"
              {...registerField('lastName')}
              error={errors.lastName?.message}
              placeholder="Doe"
              autoComplete="family-name"
            />
          </div>

          <Input
            label="Email"
            type="email"
            {...registerField('email')}
            error={errors.email?.message}
            placeholder="you@example.com"
            autoComplete="email"
          />

          <Input
            label="Password"
            type="password"
            {...registerField('password')}
            error={errors.password?.message}
            placeholder="••••••••"
            autoComplete="new-password"
          />

          <div className="text-xs text-gray-600 bg-gray-50 p-3 rounded-lg">
            <p className="font-medium mb-1">Password must contain:</p>
            <ul className="list-disc list-inside space-y-1">
              <li>At least 8 characters</li>
              <li>One uppercase and lowercase letter</li>
              <li>At least one number</li>
            </ul>
          </div>

          <div className="flex items-start gap-2">
            <input 
              type="checkbox" 
              id="terms" 
              className="mt-1 rounded border-gray-300"
              required
            />
            <label htmlFor="terms" className="text-xs text-gray-600">
              I agree to the{' '}
              <button type="button" className="text-primary-600 hover:underline">
                Terms of Service
              </button>{' '}
              and{' '}
              <button type="button" className="text-primary-600 hover:underline">
                Privacy Policy
              </button>
            </label>
          </div>

          <Button type="submit" isLoading={isLoading} className="w-full">
            Create Account
          </Button>
        </form>

        {onSwitchToLogin && (
          <div className="mt-6 text-center">
            <p className="text-sm text-gray-600">
              Already have an account?{' '}
              <button
                onClick={onSwitchToLogin}
                className="text-primary-600 hover:text-primary-700 font-medium"
              >
                Sign in
              </button>
            </p>
          </div>
        )}
      </div>
    </div>
  );
}