'use client';

import Link from 'next/link';
import { ArrowRight, BookOpenCheck, ClipboardList, Clock3, Users } from 'lucide-react';
import { useEffect, useState } from 'react';
import { EmptyState } from '@/components/empty-state';
import { LoadingScreen } from '@/components/loading-screen';
import { PageHeader } from '@/components/page-header';
import { StatusBadge } from '@/components/status-badge';
import { apiRequest } from '@/lib/api';
import { formatDate, truncate } from '@/lib/format';
import type { DashboardData } from '@/types';

const metricIcons = [Users, ClipboardList, Clock3, BookOpenCheck];

export default function DashboardPage() {
  const [data, setData] = useState<DashboardData | null>(null);
  const [error, setError] = useState('');

  useEffect(() => {
    apiRequest<DashboardData>('/dashboard')
      .then(setData)
      .catch((reason) => setError(reason instanceof Error ? reason.message : 'Unable to load dashboard.'));
  }, []);

  if (!data && !error) return <LoadingScreen label="Loading dashboard..." />;
  if (error) return <div className="panel p-6 text-sm text-red-700">{error}</div>;
  if (!data) return null;

  return (
    <div>
      <PageHeader
        eyebrow={`${data.role} workspace`}
        title="Overview"
        description="A quick view of the assignments, submissions, and actions that need attention."
      />

      <section className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        {data.metrics.map((metric, index) => {
          const Icon = metricIcons[index % metricIcons.length];
          return (
            <article key={metric.label} className="panel p-5">
              <div className="flex items-start justify-between">
                <div>
                  <p className="text-sm font-semibold text-slate-500">{metric.label}</p>
                  <p className="mt-2 text-3xl font-bold text-slate-950">{metric.value}</p>
                </div>
                <div className="rounded-xl bg-blue-50 p-2.5 text-blue-600"><Icon className="h-5 w-5" /></div>
              </div>
              <p className="mt-4 text-xs text-slate-400">{metric.hint}</p>
            </article>
          );
        })}
      </section>

      <section className="mt-6 grid gap-6 xl:grid-cols-2">
        <div className="panel overflow-hidden">
          <div className="flex items-center justify-between border-b border-slate-200 px-5 py-4">
            <div>
              <h2 className="font-bold text-slate-900">Recent assignments</h2>
              <p className="text-xs text-slate-500">Latest relevant work</p>
            </div>
            <Link className="text-sm font-semibold text-blue-600 hover:text-blue-700" href="/dashboard/assignments">View all</Link>
          </div>
          {data.recentAssignments.length === 0 ? (
            <div className="p-5"><EmptyState title="No assignments yet" description="Assignments will appear here after they are created or published." /></div>
          ) : (
            <div className="divide-y divide-slate-100">
              {data.recentAssignments.map((assignment) => (
                <Link key={assignment.id} href={`/dashboard/assignments/${assignment.id}`} className="flex items-center gap-4 px-5 py-4 hover:bg-slate-50">
                  <div className="min-w-0 flex-1">
                    <div className="flex flex-wrap items-center gap-2">
                      <h3 className="truncate font-semibold text-slate-900">{assignment.title}</h3>
                      <StatusBadge status={assignment.status} />
                    </div>
                    <p className="mt-1 text-xs text-slate-500">{assignment.subjectName} | Due {formatDate(assignment.deadline)}</p>
                    <p className="mt-2 text-sm text-slate-500">{truncate(assignment.description, 90)}</p>
                  </div>
                  <ArrowRight className="h-4 w-4 shrink-0 text-slate-400" />
                </Link>
              ))}
            </div>
          )}
        </div>

        <div className="panel overflow-hidden">
          <div className="border-b border-slate-200 px-5 py-4">
            <h2 className="font-bold text-slate-900">Recent submissions</h2>
            <p className="text-xs text-slate-500">Latest student activity</p>
          </div>
          {data.recentSubmissions.length === 0 ? (
            <div className="p-5"><EmptyState title="No submissions yet" description="Submitted work and review results will appear here." /></div>
          ) : (
            <div className="divide-y divide-slate-100">
              {data.recentSubmissions.map((submission) => (
                <Link key={submission.id} href={`/dashboard/assignments/${submission.assignmentId}`} className="flex items-center gap-4 px-5 py-4 hover:bg-slate-50">
                  <div className="min-w-0 flex-1">
                    <div className="flex flex-wrap items-center gap-2">
                      <h3 className="truncate font-semibold text-slate-900">{submission.assignmentTitle}</h3>
                      <StatusBadge status={submission.status} />
                    </div>
                    <p className="mt-1 text-xs text-slate-500">{submission.studentName} | {formatDate(submission.submittedAt)}</p>
                    <p className="mt-2 text-sm text-slate-500">{truncate(submission.answerText, 85)}</p>
                  </div>
                  <ArrowRight className="h-4 w-4 shrink-0 text-slate-400" />
                </Link>
              ))}
            </div>
          )}
        </div>
      </section>
    </div>
  );
}
