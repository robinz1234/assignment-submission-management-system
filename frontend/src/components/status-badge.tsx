import type { AssignmentStatus, SubmissionStatus } from '@/types';

export function StatusBadge({ status }: { status: AssignmentStatus | SubmissionStatus }) {
  const styles: Record<string, string> = {
    Draft: 'bg-slate-100 text-slate-700',
    Published: 'bg-emerald-100 text-emerald-700',
    Submitted: 'bg-blue-100 text-blue-700',
    Reviewed: 'bg-violet-100 text-violet-700',
    Returned: 'bg-amber-100 text-amber-700',
  };

  return (
    <span className={`inline-flex rounded-full px-2.5 py-1 text-xs font-semibold ${styles[status] ?? styles.Draft}`}>
      {status}
    </span>
  );
}
