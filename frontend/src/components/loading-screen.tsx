import { LoaderCircle } from 'lucide-react';

export function LoadingScreen({ label = 'Loading...' }: { label?: string }) {
  return (
    <div className="flex min-h-[260px] items-center justify-center">
      <div className="flex items-center gap-3 text-sm font-medium text-slate-500">
        <LoaderCircle className="h-5 w-5 animate-spin" />
        {label}
      </div>
    </div>
  );
}
