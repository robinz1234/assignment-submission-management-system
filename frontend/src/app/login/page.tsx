'use client';

import { zodResolver } from '@hookform/resolvers/zod';
import { BookOpenCheck, LoaderCircle, ShieldCheck } from 'lucide-react';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useAuth } from '@/components/auth-provider';

const loginSchema = z.object({
  email: z.string().email('Enter a valid email address.'),
  password: z.string().min(6, 'Password must contain at least 6 characters.'),
});

type LoginValues = z.infer<typeof loginSchema>;

const demoAccounts = [
  { role: 'Admin', email: 'admin@school.test', password: 'Admin123!' },
  { role: 'Teacher', email: 'teacher@school.test', password: 'Teacher123!' },
  { role: 'Student', email: 'student@school.test', password: 'Student123!' },
];

export default function LoginPage() {
  const { user, loading, login } = useAuth();
  const router = useRouter();
  const [serverError, setServerError] = useState('');
  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors, isSubmitting },
  } = useForm<LoginValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: '', password: '' },
  });

  useEffect(() => {
    if (!loading && user) {
      router.replace('/dashboard');
    }
  }, [loading, user, router]);

  const onSubmit = async (values: LoginValues) => {
    setServerError('');
    try {
      await login(values.email, values.password);
      router.replace('/dashboard');
    } catch (error) {
      setServerError(error instanceof Error ? error.message : 'Login failed.');
    }
  };

  return (
    <main className="grid min-h-screen lg:grid-cols-[1.05fr_0.95fr]">
      <section className="hidden bg-slate-950 p-12 text-white lg:flex lg:flex-col lg:justify-between">
        <div className="flex items-center gap-3">
          <div className="rounded-2xl bg-blue-600 p-3"><BookOpenCheck className="h-7 w-7" /></div>
          <div>
            <div className="text-lg font-bold">Assignment Hub</div>
            <div className="text-sm text-slate-400">School and college workflow</div>
          </div>
        </div>
        <div className="max-w-xl">
          <div className="mb-5 inline-flex items-center gap-2 rounded-full border border-slate-800 bg-slate-900 px-3 py-1.5 text-xs font-semibold text-blue-300">
            <ShieldCheck className="h-4 w-4" /> JWT authentication and role-based access
          </div>
          <h1 className="text-5xl font-bold leading-tight tracking-tight">Assignments, submissions, and feedback in one clear workspace.</h1>
          <p className="mt-5 max-w-lg text-base leading-7 text-slate-400">Administrators manage academic data, teachers publish and review work, and students submit answers securely.</p>
        </div>
        <p className="text-xs text-slate-500">Full-stack recruitment project</p>
      </section>

      <section className="flex items-center justify-center px-5 py-10 sm:px-10">
        <div className="w-full max-w-md">
          <div className="mb-8 lg:hidden">
            <div className="mb-4 inline-flex rounded-2xl bg-blue-600 p-3 text-white"><BookOpenCheck className="h-7 w-7" /></div>
            <h1 className="text-3xl font-bold text-slate-950">Assignment Hub</h1>
          </div>
          <div className="panel p-6 sm:p-8">
            <p className="text-xs font-bold uppercase tracking-[0.2em] text-blue-600">Welcome back</p>
            <h2 className="mt-2 text-2xl font-bold text-slate-950">Sign in to continue</h2>
            <p className="mt-2 text-sm leading-6 text-slate-500">Use your role-based account to open the correct dashboard.</p>

            <form className="mt-7 space-y-5" onSubmit={handleSubmit(onSubmit)}>
              <div>
                <label className="label" htmlFor="email">Email address</label>
                <input id="email" className="input-field" placeholder="name@example.com" autoComplete="email" {...register('email')} />
                {errors.email && <p className="mt-1.5 text-xs font-medium text-red-600">{errors.email.message}</p>}
              </div>
              <div>
                <label className="label" htmlFor="password">Password</label>
                <input id="password" className="input-field" type="password" autoComplete="current-password" {...register('password')} />
                {errors.password && <p className="mt-1.5 text-xs font-medium text-red-600">{errors.password.message}</p>}
              </div>
              {serverError && <div className="rounded-xl border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">{serverError}</div>}
              <button className="btn-primary w-full" disabled={isSubmitting}>
                {isSubmitting && <LoaderCircle className="h-4 w-4 animate-spin" />}
                Sign in
              </button>
            </form>

            <div className="mt-7 border-t border-slate-200 pt-5">
              <p className="mb-3 text-xs font-bold uppercase tracking-wider text-slate-400">Demo accounts</p>
              <div className="grid gap-2">
                {demoAccounts.map((account) => (
                  <button
                    key={account.role}
                    type="button"
                    className="flex items-center justify-between rounded-xl border border-slate-200 px-3 py-2 text-left text-sm hover:bg-slate-50"
                    onClick={() => {
                      setValue('email', account.email);
                      setValue('password', account.password);
                    }}
                  >
                    <span className="font-semibold text-slate-700">{account.role}</span>
                    <span className="text-xs text-slate-400">Fill credentials</span>
                  </button>
                ))}
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}
