'use client';

import { useParams } from 'next/navigation';
import { useEffect, useState } from 'react';
import { AssignmentForm } from '@/components/assignment-form';
import { LoadingScreen } from '@/components/loading-screen';
import { PageHeader } from '@/components/page-header';
import { apiRequest } from '@/lib/api';
import type { AssignmentItem } from '@/types';

export default function EditAssignmentPage() {
  const params = useParams<{ id: string }>();
  const [assignment, setAssignment] = useState<AssignmentItem | null>(null);
  const [error, setError] = useState('');

  useEffect(() => {
    apiRequest<AssignmentItem>(`/assignments/${params.id}`)
      .then(setAssignment)
      .catch((reason) => setError(reason instanceof Error ? reason.message : 'Unable to load assignment.'));
  }, [params.id]);

  if (!assignment && !error) return <LoadingScreen label="Loading assignment..." />;
  if (error) return <div className="panel p-5 text-sm text-red-700">{error}</div>;
  if (!assignment) return null;

  return (
    <div>
      <PageHeader eyebrow="Teacher tools" title="Edit assignment" description="Update the assignment details. The class and subject cannot be changed after submissions exist." />
      <AssignmentForm assignment={assignment} />
    </div>
  );
}
