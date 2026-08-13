'use client';

import { LoaderCircle, Plus, Trash2 } from 'lucide-react';
import { useEffect, useMemo, useState } from 'react';
import { LoadingScreen } from '@/components/loading-screen';
import { PageHeader } from '@/components/page-header';
import { apiRequest } from '@/lib/api';
import type { ClassItem, PagedResult, SubjectItem, TeachingAssignmentItem, UserItem } from '@/types';

export default function AcademicsPage() {
  const [classes, setClasses] = useState<ClassItem[]>([]);
  const [subjects, setSubjects] = useState<SubjectItem[]>([]);
  const [scopes, setScopes] = useState<TeachingAssignmentItem[]>([]);
  const [teachers, setTeachers] = useState<UserItem[]>([]);
  const [classForm, setClassForm] = useState({ name: '', section: '', academicYear: '2026' });
  const [subjectForm, setSubjectForm] = useState({ name: '', code: '' });
  const [scopeForm, setScopeForm] = useState({ teacherId: '', classId: '', subjectId: '' });
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  const load = async () => {
    setLoading(true);
    try {
      const [classData, subjectData, scopeData, teacherData] = await Promise.all([
        apiRequest<ClassItem[]>('/admin/classes'),
        apiRequest<SubjectItem[]>('/admin/subjects'),
        apiRequest<TeachingAssignmentItem[]>('/admin/teaching-assignments'),
        apiRequest<PagedResult<UserItem>>('/admin/users?role=Teacher&pageSize=100'),
      ]);
      setClasses(classData);
      setSubjects(subjectData);
      setScopes(scopeData);
      setTeachers(teacherData.items.filter((item) => item.isActive));
      setError('');
      setScopeForm((current) => ({
        teacherId: current.teacherId || teacherData.items.find((item) => item.isActive)?.id || '',
        classId: current.classId || classData[0]?.id || '',
        subjectId: current.subjectId || subjectData[0]?.id || '',
      }));
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to load academic setup.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { void load(); }, []);

  const canCreateScope = useMemo(
    () => scopeForm.teacherId && scopeForm.classId && scopeForm.subjectId,
    [scopeForm],
  );

  const createClass = async () => {
    if (!classForm.name.trim() || !classForm.section.trim() || !classForm.academicYear.trim()) {
      setError('Complete all class fields.');
      return;
    }
    setSaving(true);
    try {
      await apiRequest('/admin/classes', { method: 'POST', body: JSON.stringify(classForm) });
      setClassForm({ name: '', section: '', academicYear: '2026' });
      await load();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to create class.');
    } finally {
      setSaving(false);
    }
  };

  const createSubject = async () => {
    if (!subjectForm.name.trim() || !subjectForm.code.trim()) {
      setError('Complete all subject fields.');
      return;
    }
    setSaving(true);
    try {
      await apiRequest('/admin/subjects', { method: 'POST', body: JSON.stringify(subjectForm) });
      setSubjectForm({ name: '', code: '' });
      await load();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to create subject.');
    } finally {
      setSaving(false);
    }
  };

  const createScope = async () => {
    if (!canCreateScope) return;
    setSaving(true);
    try {
      await apiRequest('/admin/teaching-assignments', { method: 'POST', body: JSON.stringify(scopeForm) });
      await load();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to assign teacher.');
    } finally {
      setSaving(false);
    }
  };

  const remove = async (path: string, label: string) => {
    if (!window.confirm(`Delete ${label}?`)) return;
    try {
      await apiRequest(path, { method: 'DELETE' });
      await load();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : `Unable to delete ${label}.`);
    }
  };

  return (
    <div>
      <PageHeader eyebrow="Administration" title="Academic setup" description="Manage classes, subjects, and the teacher assignments that control which coursework a teacher may create." />
      {error && <div className="mb-5 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}
      {loading ? <LoadingScreen label="Loading academic data..." /> : (
        <div className="space-y-6">
          <section className="grid gap-6 xl:grid-cols-2">
            <div className="panel p-5 sm:p-6">
              <h2 className="text-lg font-bold text-slate-950">Classes and courses</h2>
              <p className="mt-1 text-sm text-slate-500">A class groups students and receives assignments.</p>
              <div className="mt-5 grid gap-3 sm:grid-cols-3">
                <input className="input-field" placeholder="Class name" value={classForm.name} onChange={(event) => setClassForm({ ...classForm, name: event.target.value })} />
                <input className="input-field" placeholder="Section" value={classForm.section} onChange={(event) => setClassForm({ ...classForm, section: event.target.value })} />
                <input className="input-field" placeholder="Academic year" value={classForm.academicYear} onChange={(event) => setClassForm({ ...classForm, academicYear: event.target.value })} />
              </div>
              <button className="btn-primary mt-3" onClick={() => void createClass()} disabled={saving}>{saving && <LoaderCircle className="h-4 w-4 animate-spin" />}<Plus className="h-4 w-4" />Add class</button>
              <div className="mt-5 divide-y divide-slate-100 border-t border-slate-200">
                {classes.map((item) => <div key={item.id} className="flex items-center justify-between py-3"><div><p className="font-semibold text-slate-900">{item.name} - {item.section}</p><p className="text-xs text-slate-500">{item.academicYear} | {item.studentCount} students</p></div><button className="rounded-lg p-2 text-red-500 hover:bg-red-50" onClick={() => void remove(`/admin/classes/${item.id}`, `${item.name} ${item.section}`)}><Trash2 className="h-4 w-4" /></button></div>)}
              </div>
            </div>

            <div className="panel p-5 sm:p-6">
              <h2 className="text-lg font-bold text-slate-950">Subjects</h2>
              <p className="mt-1 text-sm text-slate-500">Subjects identify the academic area of each assignment.</p>
              <div className="mt-5 grid gap-3 sm:grid-cols-2">
                <input className="input-field" placeholder="Subject name" value={subjectForm.name} onChange={(event) => setSubjectForm({ ...subjectForm, name: event.target.value })} />
                <input className="input-field" placeholder="Code, for example MATH-10" value={subjectForm.code} onChange={(event) => setSubjectForm({ ...subjectForm, code: event.target.value })} />
              </div>
              <button className="btn-primary mt-3" onClick={() => void createSubject()} disabled={saving}>{saving && <LoaderCircle className="h-4 w-4 animate-spin" />}<Plus className="h-4 w-4" />Add subject</button>
              <div className="mt-5 divide-y divide-slate-100 border-t border-slate-200">
                {subjects.map((item) => <div key={item.id} className="flex items-center justify-between py-3"><div><p className="font-semibold text-slate-900">{item.name}</p><p className="text-xs text-slate-500">{item.code}</p></div><button className="rounded-lg p-2 text-red-500 hover:bg-red-50" onClick={() => void remove(`/admin/subjects/${item.id}`, item.name)}><Trash2 className="h-4 w-4" /></button></div>)}
              </div>
            </div>
          </section>

          <section className="panel p-5 sm:p-6">
            <h2 className="text-lg font-bold text-slate-950">Teacher assignments</h2>
            <p className="mt-1 text-sm text-slate-500">A teacher can create assignments only for combinations listed here.</p>
            <div className="mt-5 grid gap-3 md:grid-cols-[1fr_1fr_1fr_auto]">
              <select className="input-field" value={scopeForm.teacherId} onChange={(event) => setScopeForm({ ...scopeForm, teacherId: event.target.value })}><option value="">Select teacher</option>{teachers.map((item) => <option key={item.id} value={item.id}>{item.fullName}</option>)}</select>
              <select className="input-field" value={scopeForm.classId} onChange={(event) => setScopeForm({ ...scopeForm, classId: event.target.value })}><option value="">Select class</option>{classes.map((item) => <option key={item.id} value={item.id}>{item.name} - {item.section}</option>)}</select>
              <select className="input-field" value={scopeForm.subjectId} onChange={(event) => setScopeForm({ ...scopeForm, subjectId: event.target.value })}><option value="">Select subject</option>{subjects.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}</select>
              <button className="btn-primary" onClick={() => void createScope()} disabled={!canCreateScope || saving}><Plus className="h-4 w-4" />Assign</button>
            </div>
            <div className="mt-5 overflow-x-auto rounded-xl border border-slate-200">
              <table className="w-full min-w-[700px] text-left text-sm"><thead className="bg-slate-50 text-xs uppercase tracking-wider text-slate-500"><tr><th className="px-4 py-3">Teacher</th><th className="px-4 py-3">Class</th><th className="px-4 py-3">Subject</th><th className="px-4 py-3 text-right">Action</th></tr></thead><tbody className="divide-y divide-slate-100">{scopes.map((item) => <tr key={item.id}><td className="px-4 py-3 font-semibold text-slate-900">{item.teacherName}</td><td className="px-4 py-3 text-slate-600">{item.className}</td><td className="px-4 py-3 text-slate-600">{item.subjectName}</td><td className="px-4 py-3 text-right"><button className="rounded-lg p-2 text-red-500 hover:bg-red-50" onClick={() => void remove(`/admin/teaching-assignments/${item.id}`, 'teaching assignment')}><Trash2 className="h-4 w-4" /></button></td></tr>)}</tbody></table>
            </div>
          </section>
        </div>
      )}
    </div>
  );
}
