'use client';

import Link from 'next/link';
import { usePathname, useRouter } from 'next/navigation';
import {
  BookOpenCheck,
  ChevronRight,
  ClipboardList,
  GraduationCap,
  LayoutDashboard,
  LogOut,
  Menu,
  Settings,
  Users,
  X,
} from 'lucide-react';
import { useEffect, useMemo, useState } from 'react';
import { useAuth } from '@/components/auth-provider';
import { LoadingScreen } from '@/components/loading-screen';
import type { UserRole } from '@/types';

interface NavItem {
  label: string;
  href: string;
  icon: React.ComponentType<{ className?: string }>;
  roles: UserRole[];
}

const navItems: NavItem[] = [
  { label: 'Dashboard', href: '/dashboard', icon: LayoutDashboard, roles: ['Admin', 'Teacher', 'Student'] },
  { label: 'Assignments', href: '/dashboard/assignments', icon: ClipboardList, roles: ['Admin', 'Teacher', 'Student'] },
  { label: 'Submissions', href: '/dashboard/submissions', icon: BookOpenCheck, roles: ['Admin', 'Teacher', 'Student'] },
  { label: 'Users', href: '/dashboard/admin/users', icon: Users, roles: ['Admin'] },
  { label: 'Academic setup', href: '/dashboard/admin/academics', icon: GraduationCap, roles: ['Admin'] },
  { label: 'Settings', href: '/dashboard/admin/settings', icon: Settings, roles: ['Admin'] },
];

export function AppShell({ children }: { children: React.ReactNode }) {
  const { user, loading, logout } = useAuth();
  const router = useRouter();
  const pathname = usePathname();
  const [mobileOpen, setMobileOpen] = useState(false);

  useEffect(() => {
    if (!loading && !user) {
      router.replace('/login');
    }
  }, [loading, user, router]);

  useEffect(() => setMobileOpen(false), [pathname]);

  const visibleItems = useMemo(
    () => navItems.filter((item) => user && item.roles.includes(user.role)),
    [user],
  );

  if (loading || !user) {
    return <LoadingScreen label="Checking your session..." />;
  }

  const handleLogout = () => {
    logout();
    router.replace('/login');
  };

  const sidebar = (
    <aside className="flex h-full w-72 flex-col bg-slate-950 px-4 py-5 text-white">
      <div className="flex items-center gap-3 px-2">
        <div className="rounded-2xl bg-blue-600 p-2.5">
          <BookOpenCheck className="h-6 w-6" />
        </div>
        <div>
          <div className="font-bold">Assignment Hub</div>
          <div className="text-xs text-slate-400">School management portal</div>
        </div>
      </div>

      <nav className="mt-8 space-y-1.5">
        {visibleItems.map((item) => {
          const active = pathname === item.href || (item.href !== '/dashboard' && pathname.startsWith(item.href));
          const Icon = item.icon;
          return (
            <Link
              key={item.href}
              href={item.href}
              className={`flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium transition ${
                active ? 'bg-white text-slate-950' : 'text-slate-300 hover:bg-slate-900 hover:text-white'
              }`}
            >
              <Icon className="h-4 w-4" />
              <span>{item.label}</span>
              {active && <ChevronRight className="ml-auto h-4 w-4" />}
            </Link>
          );
        })}
      </nav>

      <div className="mt-auto rounded-2xl border border-slate-800 bg-slate-900 p-3">
        <p className="truncate text-sm font-semibold">{user.fullName}</p>
        <p className="truncate text-xs text-slate-400">{user.email}</p>
        <div className="mt-3 flex items-center justify-between">
          <span className="rounded-full bg-slate-800 px-2.5 py-1 text-xs font-semibold text-blue-300">{user.role}</span>
          <button onClick={handleLogout} className="rounded-lg p-2 text-slate-400 hover:bg-slate-800 hover:text-white" title="Log out">
            <LogOut className="h-4 w-4" />
          </button>
        </div>
      </div>
    </aside>
  );

  return (
    <div className="min-h-screen lg:grid lg:grid-cols-[18rem_1fr]">
      <div className="hidden lg:block">{sidebar}</div>
      {mobileOpen && (
        <div className="fixed inset-0 z-50 lg:hidden">
          <button className="absolute inset-0 bg-slate-950/60" onClick={() => setMobileOpen(false)} aria-label="Close navigation" />
          <div className="relative h-full w-72">
            {sidebar}
            <button className="absolute right-3 top-3 rounded-lg p-2 text-white" onClick={() => setMobileOpen(false)}>
              <X className="h-5 w-5" />
            </button>
          </div>
        </div>
      )}

      <div className="min-w-0">
        <header className="sticky top-0 z-30 flex h-16 items-center border-b border-slate-200 bg-white/90 px-4 backdrop-blur sm:px-6 lg:px-8">
          <button className="mr-3 rounded-xl border border-slate-200 p-2 lg:hidden" onClick={() => setMobileOpen(true)}>
            <Menu className="h-5 w-5" />
          </button>
          <div>
            <p className="text-xs font-medium text-slate-500">Signed in as {user.role}</p>
            <p className="text-sm font-semibold text-slate-900">{user.className ?? 'Assignment and Submission Management'}</p>
          </div>
        </header>
        <main className="mx-auto max-w-[1500px] px-4 py-6 sm:px-6 lg:px-8 lg:py-8">{children}</main>
      </div>
    </div>
  );
}
