import { Link, useNavigate } from "react-router-dom"
import { useAppDispatch } from "../../app/store/store";
import { useRegisterMutation } from "../../app/api/auth/authApi";
import { zodResolver } from "@hookform/resolvers/zod";
import { registerSchema, type RegisterFormData } from "../../utils/registerSchema";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { setCredentials } from "../../app/store/slices/authSlice";
import { Input } from "../../app/layout/ui/Input";
import { Button } from "../../app/layout/ui/Button";

const RegisterPage = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const [register, {isLoading}] = useRegisterMutation();

  const {
    register: registerField, handleSubmit, formState: {errors}} = 
    useForm<RegisterFormData>({
      resolver: zodResolver(registerSchema),
  })

  const onSubmit = async (data: RegisterFormData) => {
    try {
      const response = await register(data).unwrap();
      dispatch(setCredentials(response));
      toast.success('Account created successfully!');
      navigate('/login');
    } catch (error: any) {
      toast.error(error?.data?.message || 'Failed to create account');
    }
  }

  return (
     <div className="min-h-[80vh] flex items-center justify-center px-4 py-8">
      <div className="w-full max-w-md">
        <div className="card p-8">
          <h1 className="text-3xl font-bold text-center mb-2">Create Account</h1>
          <p className="text-gray-600 text-center mb-8">Join us today</p>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <Input
                label="First Name"
                {...registerField('firstName')}
                error={errors.firstName?.message}
                placeholder="John"
              />

              <Input
                label="Last Name"
                {...registerField('lastName')}
                error={errors.lastName?.message}
                placeholder="Doe"
              />
            </div>

            <Input
              label="Email"
              type="email"
              {...registerField('email')}
              error={errors.email?.message}
              placeholder="you@example.com"
            />

            <Input
              label="Password"
              type="password"
              {...registerField('password')}
              error={errors.password?.message}
              placeholder="••••••••"
            />

            <Button type="submit" isLoading={isLoading} className="w-full" size="lg" variant="amber">
              Create Account
            </Button>
          </form>

          <div className="mt-6 text-center">
            <p className="text-sm text-gray-600">
              Already have an account?{' '}
              <Link to="/login" className="text-primary-600 hover:text-primary-700 font-medium">
                Sign in
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}

export default RegisterPage
