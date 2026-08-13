'use client';

import Link from 'next/link';
import { ArrowRight, Search } from 'lucide-react';
import { useEffect, useState } from 'react';
import { useAuth } from '@/components/auth-provider';
import { EmptyState } from '@/components/empty-state';
import { LoadingScreen } from '@/components/loading-screen';
import { PageHeader } from '@/components/page-header';
import { StatusBadge } from '@/components/status-badge';
import { apiRequest, queryString } from '@/lib/api';
import { formatDate, truncate } from '@/lib/format';
import type { PagedResult, SubmissionItem, SubmissionStatus } from '@/types';

export default function SubmissionsPage() {
  const { user } = useAuth();
  const [items, setItems] = useState<SubmissionItem[]>([]);
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState<SubmissionStatus | ''>('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const load = async () => {
    if (!user) return;
    setLoading(true);
    setError('');
    try {
      if (user.role === 'Student') {
        setItems(await apiRequest<SubmissionItem[]>('/submissions/my'));
      } else {
        const result = await apiRequest<PagedResult<SubmissionItem>>(`/submissions${queryString({ search, status, pageSize: 100 })}`);
        setItems(result.items);
      }
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to load submissions.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (user) void load();
  }, [user?.id, user?.role, status]);

  const title = user?.role === 'Student' ? 'My submissions' : 'Student submissions';
  const description = user?.role === 'Student'
    ? 'Review everything you submitted and see marks or teacher feedback when available.'
    : user?.role === 'Teacher'
      ? 'Review submissions made to your assignments.'
      : 'View submitted work across all assignments and classes.';

  return (
    <div>
      <PageHeader eyebrow={user?.role === 'Admin' ? 'Administration' : 'Coursework'} title={title} description={description} />

      {user?.role !== 'Student' && (
        <div className="panel mb-5 grid gap-3 p-4 sm:grid-cols-[1fr_180px_auto]">
          <div className="relative">
            <Search className="pointer-events-none absolute left-3 top-3 h-4 w-4 text-slate-400" />
            <input
              className="input-field pl-9"
              placeholder="Search student or assignment"
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              onKeyDown={(event) => event.key === 'Enter' && void load()}
            />
          </div>
          <select className="input-field" value={status} onChange={(event) => setStatus(event.target.value as SubmissionStatus | '')}>
            <option value="">All statuses</option>
            <option value="Submitted">Submitted</option>
            <option value="Reviewed">Reviewed</option>
            <option value="Returned">Returned</option>
          </select>
          <button className="btn-secondary" onClick={() => void load()}>Search</button>
        </div>
      )}

      {loading ? <LoadingScreen label="Loading submissions..." /> : error ? (
        <div className="panel p-5 text-sm text-red-700">{error}</div>
      ) : items.length === 0 ? (
        <EmptyState title="No submissions found" description={user?.role === 'Student' ? 'Open a published assignment to submit your first answer.' : 'Submitted student work will appear here.'} />
      ) : (
        <div className="space-y-4">
          {items.map((item) => (
            <Link key={item.id} href={`/dashboard/assignments/${item.assignmentId}`} className="panel flex flex-col gap-4 p-5 transition hover:border-blue-300 sm:flex-row sm:items-center">
              <div className="min-w-0 flex-1">
                <div className="flex flex-wrap items-center gap-2">
                  <h2 className="font-bold text-slate-900">{item.assignmentTitle}</h2>
                  <StatusBadge status={item.status} />
                </div>
                {user?.role !== 'Student' && <p className="mt-1 text-sm font-medium text-slate-700">Student: {item.studentName}</p>}
                <p className="mt-1 text-xs text-slate-500">Submitted {formatDate(item.submittedAt)}</p>
                <p className="mt-3 text-sm text-slate-500">{truncate(item.answerText, 160)}</p>
                {item.feedback && <p className="mt-2 text-sm text-slate-600"><span className="font-semibold">Feedback:</span> {truncate(item.feedback, 120)}</p>}
              </div>
              <div className="flex shrink-0 items-center gap-4">
                <div className="text-right">
                  <p className="text-xs font-semibold uppercase tracking-wider text-slate-400">Marks</p>
                  <p className="mt-1 font-bold text-slate-900">{item.marks ?? '-'} / {item.maxMarks}</p>
                </div>
                <ArrowRight className="h-5 w-5 text-slate-400" />
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
