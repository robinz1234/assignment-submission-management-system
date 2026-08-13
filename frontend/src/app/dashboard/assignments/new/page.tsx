'use client';

import { AssignmentForm } from '@/components/assignment-form';
import { PageHeader } from '@/components/page-header';

export default function NewAssignmentPage() {
  return (
    <div>
      <PageHeader eyebrow="Teacher tools" title="Create assignment" description="Choose one of your assigned class and subject combinations, then set the deadline and publication status." />
      <AssignmentForm />
    </div>
  );
}
