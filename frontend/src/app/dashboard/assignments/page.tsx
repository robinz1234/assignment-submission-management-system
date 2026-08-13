'use client';

import Link from 'next/link';
import { Plus, Search } from 'lucide-react';
import { useEffect, useState } from 'react';
import { useAuth } from '@/components/auth-provider';
import { EmptyState } from '@/components/empty-state';
import { LoadingScreen } from '@/components/loading-screen';
import { PageHeader } from '@/components/page-header';
import { StatusBadge } from '@/components/status-badge';
import { apiRequest, queryString } from '@/lib/api';
import { formatDate, isPast, truncate } from '@/lib/format';
import type { AssignmentItem, AssignmentStatus, PagedResult } from '@/types';

export default function AssignmentsPage() {
  const { user } = useAuth();
  const [result, setResult] = useState<PagedResult<AssignmentItem> | null>(null);
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState<AssignmentStatus | ''>('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);

  const load = async () => {
    setLoading(true);
    setError('');
    try {
      const data = await apiRequest<PagedResult<AssignmentItem>>(`/assignments${queryString({ search, status, pageSize: 50 })}`);
      setResult(data);
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to load assignments.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void load();
  }, [status]);

  return (
    <div>
      <PageHeader
        eyebrow="Coursework"
        title="Assignments"
        description={user?.role === 'Teacher' ? 'Create, publish, and review work for your assigned classes.' : user?.role === 'Student' ? 'View published assignments for your class and track submission results.' : 'View every assignment in the system.'}
        action={user?.role === 'Teacher' ? <Link href="/dashboard/assignments/new" className="btn-primary"><Plus className="h-4 w-4" />New assignment</Link> : undefined}
      />

      <div className="panel mb-5 grid gap-3 p-4 sm:grid-cols-[1fr_180px_auto]">
        <div className="relative">
          <Search className="pointer-events-none absolute left-3 top-3 h-4 w-4 text-slate-400" />
          <input className="input-field pl-9" placeholder="Search title, description, or subject" value={search} onChange={(event) => setSearch(event.target.value)} onKeyDown={(event) => event.key === 'Enter' && void load()} />
        </div>
        {user?.role !== 'Student' && (
          <select className="input-field" value={status} onChange={(event) => setStatus(event.target.value as AssignmentStatus | '')}>
            <option value="">All statuses</option>
            <option value="Draft">Draft</option>
            <option value="Published">Published</option>
          </select>
        )}
        <button className="btn-secondary" onClick={() => void load()}>Search</button>
      </div>

      {loading ? <LoadingScreen label="Loading assignments..." /> : error ? <div className="panel p-5 text-sm text-red-700">{error}</div> : !result || result.items.length === 0 ? (
        <EmptyState title="No assignments found" description="Try another filter, or create the first assignment if you are a teacher." />
      ) : (
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {result.items.map((assignment) => (
            <Link key={assignment.id} href={`/dashboard/assignments/${assignment.id}`} className="panel group flex min-h-64 flex-col p-5 transition hover:-translate-y-0.5 hover:border-blue-300">
              <div className="flex items-start justify-between gap-3">
                <div>
                  <p className="text-xs font-bold uppercase tracking-wider text-blue-600">{assignment.subjectName}</p>
                  <h2 className="mt-2 text-lg font-bold text-slate-950 group-hover:text-blue-700">{assignment.title}</h2>
                </div>
                <StatusBadge status={assignment.status} />
              </div>
              <p className="mt-3 text-sm leading-6 text-slate-500">{truncate(assignment.description, 150)}</p>
              <div className="mt-auto space-y-2 border-t border-slate-100 pt-4 text-xs text-slate-500">
                <div className="flex justify-between"><span>{assignment.className}</span><span>{assignment.maxMarks} marks</span></div>
                <div className="flex justify-between"><span className={isPast(assignment.deadline) ? 'font-semibold text-red-600' : ''}>Due {formatDate(assignment.deadline)}</span><span>{assignment.submissionCount} submissions</span></div>
                {assignment.mySubmissionStatus && <div className="pt-1"><StatusBadge status={assignment.mySubmissionStatus} /></div>}
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
