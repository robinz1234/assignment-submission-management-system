'use client';

import { zodResolver } from '@hookform/resolvers/zod';
import { LoaderCircle } from 'lucide-react';
import { useRouter } from 'next/navigation';
import { useEffect, useMemo, useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { apiRequest } from '@/lib/api';
import type { AssignmentItem, TeachingAssignmentItem } from '@/types';

const schema = z.object({
  scopeId: z.string().uuid('Select a class and subject.'),
  title: z.string().min(3, 'Title must contain at least 3 characters.').max(180),
  description: z.string().min(3, 'Description must contain at least 3 characters.').max(5000),
  deadline: z.string().min(1, 'Select a deadline.'),
  maxMarks: z.coerce.number().positive('Maximum marks must be greater than zero.'),
  status: z.enum(['Draft', 'Published']),
  allowResubmission: z.boolean(),
});

type Values = z.infer<typeof schema>;

function toLocalDateTime(value?: string) {
  const date = value ? new Date(value) : new Date(Date.now() + 24 * 60 * 60 * 1000);
  const offset = date.getTimezoneOffset() * 60_000;
  return new Date(date.getTime() - offset).toISOString().slice(0, 16);
}

export function AssignmentForm({ assignment }: { assignment?: AssignmentItem }) {
  const router = useRouter();
  const [scopes, setScopes] = useState<TeachingAssignmentItem[]>([]);
  const [serverError, setServerError] = useState('');
  const [loadingScopes, setLoadingScopes] = useState(true);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<Values>({
    resolver: zodResolver(schema),
    defaultValues: {
      scopeId: '',
      title: assignment?.title ?? '',
      description: assignment?.description ?? '',
      deadline: toLocalDateTime(assignment?.deadline),
      maxMarks: assignment?.maxMarks ?? 10,
      status: assignment?.status ?? 'Draft',
      allowResubmission: assignment?.allowResubmission ?? true,
    },
  });

  useEffect(() => {
    apiRequest<TeachingAssignmentItem[]>('/reference/teacher-scopes')
      .then((items) => {
        setScopes(items);
        const matching = assignment
          ? items.find((item) => item.classId === assignment.classId && item.subjectId === assignment.subjectId)
          : items[0];
        reset({
          scopeId: matching?.id ?? '',
          title: assignment?.title ?? '',
          description: assignment?.description ?? '',
          deadline: toLocalDateTime(assignment?.deadline),
          maxMarks: assignment?.maxMarks ?? 10,
          status: assignment?.status ?? 'Draft',
          allowResubmission: assignment?.allowResubmission ?? true,
        });
      })
      .catch((error) => setServerError(error instanceof Error ? error.message : 'Unable to load teaching assignments.'))
      .finally(() => setLoadingScopes(false));
  }, [assignment, reset]);

  const noScopes = useMemo(() => !loadingScopes && scopes.length === 0, [loadingScopes, scopes]);

  const onSubmit = async (values: Values) => {
    setServerError('');
    const scope = scopes.find((item) => item.id === values.scopeId);
    if (!scope) {
      setServerError('Select a valid class and subject.');
      return;
    }

    const payload = {
      classId: scope.classId,
      subjectId: scope.subjectId,
      title: values.title,
      description: values.description,
      deadline: new Date(values.deadline).toISOString(),
      maxMarks: values.maxMarks,
      status: values.status,
      allowResubmission: values.allowResubmission,
    };

    try {
      const saved = await apiRequest<AssignmentItem>(assignment ? `/assignments/${assignment.id}` : '/assignments', {
        method: assignment ? 'PUT' : 'POST',
        body: JSON.stringify(payload),
      });
      router.push(`/dashboard/assignments/${saved.id}`);
      router.refresh();
    } catch (error) {
      setServerError(error instanceof Error ? error.message : 'Unable to save assignment.');
    }
  };

  return (
    <form className="panel max-w-4xl p-5 sm:p-7" onSubmit={handleSubmit(onSubmit)}>
      {serverError && <div className="mb-5 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{serverError}</div>}
      {noScopes && <div className="mb-5 rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-800">No teaching scope is assigned to this account. Ask an administrator to assign a class and subject.</div>}

      <div className="grid gap-5 sm:grid-cols-2">
        <div className="sm:col-span-2">
          <label className="label" htmlFor="scopeId">Class and subject</label>
          <select id="scopeId" className="input-field" disabled={loadingScopes || noScopes} {...register('scopeId')}>
            <option value="">Select a teaching assignment</option>
            {scopes.map((scope) => <option key={scope.id} value={scope.id}>{scope.className} | {scope.subjectName}</option>)}
          </select>
          {errors.scopeId && <p className="mt-1 text-xs text-red-600">{errors.scopeId.message}</p>}
        </div>

        <div className="sm:col-span-2">
          <label className="label" htmlFor="title">Title</label>
          <input id="title" className="input-field" placeholder="Example: Algebra practice set" {...register('title')} />
          {errors.title && <p className="mt-1 text-xs text-red-600">{errors.title.message}</p>}
        </div>

        <div className="sm:col-span-2">
          <label className="label" htmlFor="description">Instructions</label>
          <textarea id="description" rows={8} className="input-field resize-y" placeholder="Explain what students need to submit." {...register('description')} />
          {errors.description && <p className="mt-1 text-xs text-red-600">{errors.description.message}</p>}
        </div>

        <div>
          <label className="label" htmlFor="deadline">Deadline</label>
          <input id="deadline" type="datetime-local" className="input-field" {...register('deadline')} />
          {errors.deadline && <p className="mt-1 text-xs text-red-600">{errors.deadline.message}</p>}
        </div>

        <div>
          <label className="label" htmlFor="maxMarks">Maximum marks</label>
          <input id="maxMarks" type="number" min="0.01" step="0.01" className="input-field" {...register('maxMarks')} />
          {errors.maxMarks && <p className="mt-1 text-xs text-red-600">{errors.maxMarks.message}</p>}
        </div>

        <div>
          <label className="label" htmlFor="status">Publication status</label>
          <select id="status" className="input-field" {...register('status')}>
            <option value="Draft">Draft</option>
            <option value="Published">Published</option>
          </select>
        </div>

        <label className="flex items-center gap-3 self-end rounded-xl border border-slate-200 px-4 py-3 text-sm font-medium text-slate-700">
          <input type="checkbox" className="h-4 w-4 rounded border-slate-300 text-blue-600" {...register('allowResubmission')} />
          Allow students to update before deadline
        </label>
      </div>

      <div className="mt-7 flex flex-wrap gap-3 border-t border-slate-200 pt-5">
        <button className="btn-primary" disabled={isSubmitting || noScopes}>
          {isSubmitting && <LoaderCircle className="h-4 w-4 animate-spin" />}
          {assignment ? 'Save changes' : 'Create assignment'}
        </button>
        <button type="button" className="btn-secondary" onClick={() => router.back()}>Cancel</button>
      </div>
    </form>
  );
}
