import { Link, useNavigate } from "react-router-dom"
import { useAppDispatch } from "../../app/store/store";
import { useLoginMutation } from "../../app/api/auth/authApi";
import { loginSchema, type LoginFormData } from "../../utils/validators";
import {useForm} from 'react-hook-form';
import {zodResolver } from '@hookform/resolvers/zod';
import { toast } from "react-toastify";
import { setCredentials } from "../../app/store/slices/authSlice";
import { Input } from "../../app/layout/ui/Input";
import { Button } from "../../app/layout/ui/Button";

const LoginPage = () => {
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const [login, {isLoading }]  = useLoginMutation();

    const {register,handleSubmit, formState: {errors}, } = useForm<LoginFormData>({
        resolver: zodResolver(loginSchema),
    });

    const onSubmit = async (data: LoginFormData) => {
      try {
        const response = await login(data).unwrap();
        dispatch(setCredentials(response));
        toast.success('Welcome back!');
        navigate('/');
      } catch (error: any) {
        toast.error(error?.data?.message || 'Invalid credentials');
      }
    }

  return (
    <div className="min-h-[80vh] flex items-center justify-center px-4">
      <div className="w-full max-w-md">
        <div className="card p-8">
          <h1 className="text-3xl font-bold text-center mb-2">Welcome Back</h1>
          <p className="text-gray-600 text-center mb-8">Sign in to your account</p>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <Input
              label="Email"
              type="email"
              {...register('email')}
              error={errors.email?.message}
              placeholder="you@example.com"
            />

            <Input
              label="Password"
              type="password"
              {...register('password')}
              error={errors.password?.message}
              placeholder="••••••••"
            />

            <Button type="submit" isLoading={isLoading} className="w-full" size="lg" variant="amber">
              Sign In
            </Button>
          </form>

          <div className="mt-6 text-center">
            <p className="text-sm text-gray-600">
              Don't have an account?{' '}
              <Link to="/register" className="text-primary-600 hover:text-primary-700 font-medium">
                Sign up
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

export default LoginPage
