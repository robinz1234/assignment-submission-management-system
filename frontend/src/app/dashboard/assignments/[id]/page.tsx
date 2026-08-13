'use client';

import Link from 'next/link';
import { ArrowLeft, CheckCircle2, Edit3, LoaderCircle, Send, Trash2 } from 'lucide-react';
import { useParams, useRouter } from 'next/navigation';
import { useEffect, useMemo, useState } from 'react';
import { useAuth } from '@/components/auth-provider';
import { EmptyState } from '@/components/empty-state';
import { LoadingScreen } from '@/components/loading-screen';
import { StatusBadge } from '@/components/status-badge';
import { apiRequest } from '@/lib/api';
import { formatDate, isPast } from '@/lib/format';
import type { AssignmentItem, PagedResult, SubmissionItem, SubmissionStatus } from '@/types';

export default function AssignmentDetailPage() {
  const params = useParams<{ id: string }>();
  const router = useRouter();
  const { user } = useAuth();
  const [assignment, setAssignment] = useState<AssignmentItem | null>(null);
  const [submissions, setSubmissions] = useState<SubmissionItem[]>([]);
  const [mySubmission, setMySubmission] = useState<SubmissionItem | null>(null);
  const [answerText, setAnswerText] = useState('');
  const [selectedSubmission, setSelectedSubmission] = useState<SubmissionItem | null>(null);
  const [marks, setMarks] = useState('');
  const [feedback, setFeedback] = useState('');
  const [reviewStatus, setReviewStatus] = useState<SubmissionStatus>('Reviewed');
  const [error, setError] = useState('');
  const [notice, setNotice] = useState('');
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const load = async () => {
    setLoading(true);
    setError('');
    try {
      const item = await apiRequest<AssignmentItem>(`/assignments/${params.id}`);
      setAssignment(item);
      if (user?.role === 'Student' && item.mySubmissionId) {
        const submission = await apiRequest<SubmissionItem>(`/submissions/${item.mySubmissionId}`);
        setMySubmission(submission);
        setAnswerText(submission.answerText);
      } else if (user?.role === 'Student') {
        setMySubmission(null);
        setAnswerText('');
      }
      if (user?.role === 'Teacher' || user?.role === 'Admin') {
        const result = await apiRequest<PagedResult<SubmissionItem>>(`/assignments/${params.id}/submissions?pageSize=100`);
        setSubmissions(result.items);
      }
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to load assignment.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (user) void load();
  }, [params.id, user?.id, user?.role]);

  const canStudentEdit = useMemo(() => {
    if (!assignment || user?.role !== 'Student') return false;
    if (isPast(assignment.deadline)) return false;
    if (!mySubmission) return true;
    return assignment.allowResubmission && mySubmission.status !== 'Reviewed';
  }, [assignment, user?.role, mySubmission]);

  const submitAnswer = async () => {
    if (!assignment || !answerText.trim()) {
      setError('Write an answer before submitting.');
      return;
    }
    setSaving(true);
    setError('');
    setNotice('');
    try {
      await apiRequest<SubmissionItem>(
        mySubmission ? `/submissions/${mySubmission.id}` : `/submissions/assignments/${assignment.id}`,
        {
          method: mySubmission ? 'PUT' : 'POST',
          body: JSON.stringify({ answerText }),
        },
      );
      setNotice(mySubmission ? 'Submission updated successfully.' : 'Answer submitted successfully.');
      await load();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to save submission.');
    } finally {
      setSaving(false);
    }
  };

  const publish = async () => {
    if (!assignment) return;
    setSaving(true);
    setError('');
    try {
      await apiRequest(`/assignments/${assignment.id}/publish`, { method: 'POST' });
      setNotice('Assignment published.');
      await load();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to publish assignment.');
    } finally {
      setSaving(false);
    }
  };

  const remove = async () => {
    if (!assignment || !window.confirm('Delete this assignment? This action cannot be undone.')) return;
    setSaving(true);
    try {
      await apiRequest(`/assignments/${assignment.id}`, { method: 'DELETE' });
      router.replace('/dashboard/assignments');
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to delete assignment.');
      setSaving(false);
    }
  };

  const openReview = (submission: SubmissionItem) => {
    setSelectedSubmission(submission);
    setMarks(submission.marks?.toString() ?? '');
    setFeedback(submission.feedback ?? '');
    setReviewStatus(submission.status === 'Returned' ? 'Returned' : 'Reviewed');
  };

  const saveReview = async () => {
    if (!selectedSubmission || !assignment) return;
    setSaving(true);
    setError('');
    try {
      await apiRequest<SubmissionItem>(`/submissions/${selectedSubmission.id}/review`, {
        method: 'PUT',
        body: JSON.stringify({
          marks: marks === '' ? null : Number(marks),
          feedback,
          status: reviewStatus,
        }),
      });
      setSelectedSubmission(null);
      setNotice('Review saved successfully.');
      await load();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to save review.');
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <LoadingScreen label="Loading assignment details..." />;
  if (!assignment) return <div className="panel p-5 text-sm text-red-700">{error || 'Assignment was not found.'}</div>;

  return (
    <div>
      <div className="mb-5 flex flex-wrap items-center justify-between gap-3">
        <Link href="/dashboard/assignments" className="btn-secondary"><ArrowLeft className="h-4 w-4" />Back</Link>
        {user?.role === 'Teacher' && (
          <div className="flex flex-wrap gap-2">
            <Link className="btn-secondary" href={`/dashboard/assignments/${assignment.id}/edit`}><Edit3 className="h-4 w-4" />Edit</Link>
            {assignment.status === 'Draft' && <button className="btn-primary" onClick={() => void publish()} disabled={saving}><CheckCircle2 className="h-4 w-4" />Publish</button>}
            <button className="btn-danger" onClick={() => void remove()} disabled={saving}><Trash2 className="h-4 w-4" />Delete</button>
          </div>
        )}
      </div>

      {error && <div className="mb-4 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}
      {notice && <div className="mb-4 rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-700">{notice}</div>}

      <article className="panel p-5 sm:p-7">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div>
            <div className="flex flex-wrap items-center gap-2">
              <p className="text-xs font-bold uppercase tracking-[0.18em] text-blue-600">{assignment.subjectName}</p>
              <StatusBadge status={assignment.status} />
            </div>
            <h1 className="mt-3 text-2xl font-bold tracking-tight text-slate-950 sm:text-3xl">{assignment.title}</h1>
            <p className="mt-2 text-sm text-slate-500">{assignment.className} | Teacher: {assignment.teacherName}</p>
          </div>
          <div className="rounded-2xl bg-slate-950 px-5 py-4 text-center text-white">
            <p className="text-2xl font-bold">{assignment.maxMarks}</p>
            <p className="text-xs text-slate-400">Maximum marks</p>
          </div>
        </div>

        <div className="mt-6 grid gap-4 rounded-2xl border border-slate-200 bg-slate-50 p-4 text-sm sm:grid-cols-3">
          <div><p className="text-xs font-semibold uppercase tracking-wider text-slate-400">Deadline</p><p className={`mt-1 font-semibold ${isPast(assignment.deadline) ? 'text-red-600' : 'text-slate-800'}`}>{formatDate(assignment.deadline)}</p></div>
          <div><p className="text-xs font-semibold uppercase tracking-wider text-slate-400">Resubmission</p><p className="mt-1 font-semibold text-slate-800">{assignment.allowResubmission ? 'Allowed before deadline' : 'Not allowed'}</p></div>
          <div><p className="text-xs font-semibold uppercase tracking-wider text-slate-400">Submissions</p><p className="mt-1 font-semibold text-slate-800">{assignment.submissionCount}</p></div>
        </div>

        <div className="mt-7">
          <h2 className="font-bold text-slate-900">Instructions</h2>
          <p className="mt-3 whitespace-pre-wrap text-sm leading-7 text-slate-600">{assignment.description}</p>
        </div>
      </article>

      {user?.role === 'Student' && (
        <section className="panel mt-6 p-5 sm:p-7">
          <div className="flex flex-wrap items-start justify-between gap-3">
            <div>
              <h2 className="text-lg font-bold text-slate-950">Your submission</h2>
              <p className="mt-1 text-sm text-slate-500">Submit or update your answer before the deadline when allowed.</p>
            </div>
            {mySubmission && <StatusBadge status={mySubmission.status} />}
          </div>

          {mySubmission?.status === 'Reviewed' && (
            <div className="mt-5 grid gap-4 rounded-2xl border border-violet-200 bg-violet-50 p-4 sm:grid-cols-[180px_1fr]">
              <div><p className="text-xs font-bold uppercase tracking-wider text-violet-500">Marks</p><p className="mt-1 text-2xl font-bold text-violet-900">{mySubmission.marks ?? 0} / {assignment.maxMarks}</p></div>
              <div><p className="text-xs font-bold uppercase tracking-wider text-violet-500">Teacher feedback</p><p className="mt-1 text-sm leading-6 text-violet-900">{mySubmission.feedback || 'No written feedback was provided.'}</p></div>
            </div>
          )}

          <label className="label mt-5" htmlFor="answerText">Answer</label>
          <textarea id="answerText" className="input-field min-h-56 resize-y" value={answerText} onChange={(event) => setAnswerText(event.target.value)} disabled={!canStudentEdit} placeholder="Write your answer here..." />
          {!canStudentEdit && <p className="mt-2 text-xs text-slate-500">This submission cannot be edited because the deadline passed, resubmission is disabled, or the work has already been reviewed.</p>}
          <button className="btn-primary mt-4" onClick={() => void submitAnswer()} disabled={!canStudentEdit || saving || !answerText.trim()}>
            {saving ? <LoaderCircle className="h-4 w-4 animate-spin" /> : <Send className="h-4 w-4" />}
            {mySubmission ? 'Update submission' : 'Submit answer'}
          </button>
        </section>
      )}

      {(user?.role === 'Teacher' || user?.role === 'Admin') && (
        <section className="mt-6">
          <div className="mb-4">
            <h2 className="text-xl font-bold text-slate-950">Student submissions</h2>
            <p className="mt-1 text-sm text-slate-500">{user.role === 'Teacher' ? 'Open a submission to assign marks and feedback.' : 'Read-only overview of submitted work.'}</p>
          </div>
          {submissions.length === 0 ? <EmptyState title="No submissions yet" description="Student submissions will appear here after answers are submitted." /> : (
            <div className="grid gap-4 lg:grid-cols-2">
              {submissions.map((submission) => (
                <article key={submission.id} className="panel p-5">
                  <div className="flex items-start justify-between gap-3">
                    <div><h3 className="font-bold text-slate-900">{submission.studentName}</h3><p className="mt-1 text-xs text-slate-500">Submitted {formatDate(submission.submittedAt)}</p></div>
                    <StatusBadge status={submission.status} />
                  </div>
                  <p className="mt-4 max-h-36 overflow-auto whitespace-pre-wrap rounded-xl bg-slate-50 p-3 text-sm leading-6 text-slate-600">{submission.answerText}</p>
                  <div className="mt-4 flex items-center justify-between border-t border-slate-100 pt-4">
                    <span className="text-sm font-semibold text-slate-700">Marks: {submission.marks ?? '-'} / {submission.maxMarks}</span>
                    {user.role === 'Teacher' && <button className="btn-secondary" onClick={() => openReview(submission)}>Review</button>}
                  </div>
                  {submission.feedback && <p className="mt-3 text-sm text-slate-500"><span className="font-semibold text-slate-700">Feedback:</span> {submission.feedback}</p>}
                </article>
              ))}
            </div>
          )}
        </section>
      )}

      {selectedSubmission && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/60 p-4">
          <div className="panel max-h-[90vh] w-full max-w-2xl overflow-auto p-5 sm:p-7">
            <h2 className="text-xl font-bold text-slate-950">Review {selectedSubmission.studentName}</h2>
            <p className="mt-1 text-sm text-slate-500">Marks cannot exceed {assignment.maxMarks}.</p>
            <div className="mt-5 rounded-xl bg-slate-50 p-4 text-sm leading-6 text-slate-600 whitespace-pre-wrap">{selectedSubmission.answerText}</div>
            <div className="mt-5 grid gap-4 sm:grid-cols-2">
              <div><label className="label" htmlFor="marks">Marks</label><input id="marks" type="number" min="0" max={assignment.maxMarks} step="0.01" className="input-field" value={marks} onChange={(event) => setMarks(event.target.value)} /></div>
              <div><label className="label" htmlFor="reviewStatus">Status</label><select id="reviewStatus" className="input-field" value={reviewStatus} onChange={(event) => setReviewStatus(event.target.value as SubmissionStatus)}><option value="Reviewed">Reviewed</option><option value="Returned">Returned</option></select></div>
              <div className="sm:col-span-2"><label className="label" htmlFor="feedback">Feedback</label><textarea id="feedback" rows={5} className="input-field" value={feedback} onChange={(event) => setFeedback(event.target.value)} placeholder="Provide helpful feedback..." /></div>
            </div>
            <div className="mt-6 flex gap-3 border-t border-slate-200 pt-5">
              <button className="btn-primary" onClick={() => void saveReview()} disabled={saving}>{saving && <LoaderCircle className="h-4 w-4 animate-spin" />}Save review</button>
              <button className="btn-secondary" onClick={() => setSelectedSubmission(null)} disabled={saving}>Cancel</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
