import { Inbox } from 'lucide-react';

export function EmptyState({ title, description }: { title: string; description: string }) {
  return (
    <div className="panel flex flex-col items-center justify-center px-6 py-16 text-center">
      <div className="mb-4 rounded-2xl bg-slate-100 p-3 text-slate-500">
        <Inbox className="h-6 w-6" />
      </div>
      <h3 className="font-semibold text-slate-900">{title}</h3>
      <p className="mt-1 max-w-md text-sm leading-6 text-slate-500">{description}</p>
    </div>
  );
}
