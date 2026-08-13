'use client';

import { LoaderCircle, Pencil, Plus, UserX, X } from 'lucide-react';
import { useEffect, useMemo, useState } from 'react';
import { EmptyState } from '@/components/empty-state';
import { LoadingScreen } from '@/components/loading-screen';
import { PageHeader } from '@/components/page-header';
import { apiRequest } from '@/lib/api';
import type { OptionItem, PagedResult, UserItem, UserRole } from '@/types';

interface UserForm {
  fullName: string;
  email: string;
  password: string;
  role: UserRole;
  classId: string;
  isActive: boolean;
}

const emptyForm: UserForm = { fullName: '', email: '', password: '', role: 'Student', classId: '', isActive: true };

export default function UsersPage() {
  const [users, setUsers] = useState<UserItem[]>([]);
  const [classes, setClasses] = useState<OptionItem[]>([]);
  const [editing, setEditing] = useState<UserItem | null>(null);
  const [form, setForm] = useState<UserForm>(emptyForm);
  const [modalOpen, setModalOpen] = useState(false);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  const load = async () => {
    setLoading(true);
    try {
      const [userResult, classResult] = await Promise.all([
        apiRequest<PagedResult<UserItem>>('/admin/users?pageSize=100'),
        apiRequest<OptionItem[]>('/reference/classes'),
      ]);
      setUsers(userResult.items);
      setClasses(classResult);
      setError('');
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to load users.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { void load(); }, []);

  const teachers = useMemo(() => users.filter((item) => item.role === 'Teacher').length, [users]);
  const students = useMemo(() => users.filter((item) => item.role === 'Student').length, [users]);

  const openCreate = () => {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
  };

  const openEdit = (user: UserItem) => {
    setEditing(user);
    setForm({
      fullName: user.fullName,
      email: user.email,
      password: '',
      role: user.role,
      classId: user.classId ?? '',
      isActive: user.isActive,
    });
    setModalOpen(true);
  };

  const save = async () => {
    if (!form.fullName.trim() || !form.email.trim()) {
      setError('Name and email are required.');
      return;
    }
    if (!editing && form.password.length < 8) {
      setError('A new password must contain at least 8 characters.');
      return;
    }
    if (form.role === 'Student' && !form.classId) {
      setError('Select a class for the student.');
      return;
    }

    setSaving(true);
    setError('');
    try {
      const payload = editing
        ? {
            fullName: form.fullName,
            email: form.email,
            role: form.role,
            classId: form.role === 'Student' ? form.classId : null,
            isActive: form.isActive,
            newPassword: form.password || null,
          }
        : {
            fullName: form.fullName,
            email: form.email,
            password: form.password,
            role: form.role,
            classId: form.role === 'Student' ? form.classId : null,
          };
      await apiRequest(editing ? `/admin/users/${editing.id}` : '/admin/users', {
        method: editing ? 'PUT' : 'POST',
        body: JSON.stringify(payload),
      });
      setModalOpen(false);
      await load();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to save user.');
    } finally {
      setSaving(false);
    }
  };

  const deactivate = async (user: UserItem) => {
    if (!window.confirm(`Deactivate ${user.fullName}?`)) return;
    try {
      await apiRequest(`/admin/users/${user.id}`, { method: 'DELETE' });
      await load();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to deactivate user.');
    }
  };

  return (
    <div>
      <PageHeader eyebrow="Administration" title="Users" description={`Manage administrator, teacher, and student accounts. Current totals: ${teachers} teachers and ${students} students.`} action={<button className="btn-primary" onClick={openCreate}><Plus className="h-4 w-4" />Add user</button>} />
      {error && <div className="mb-4 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}
      {loading ? <LoadingScreen label="Loading users..." /> : users.length === 0 ? <EmptyState title="No users" description="Create the first user account." /> : (
        <div className="panel overflow-x-auto">
          <table className="w-full min-w-[820px] text-left text-sm">
            <thead className="border-b border-slate-200 bg-slate-50 text-xs uppercase tracking-wider text-slate-500"><tr><th className="px-5 py-3">User</th><th className="px-5 py-3">Role</th><th className="px-5 py-3">Class</th><th className="px-5 py-3">Status</th><th className="px-5 py-3 text-right">Actions</th></tr></thead>
            <tbody className="divide-y divide-slate-100">
              {users.map((user) => (
                <tr key={user.id} className="hover:bg-slate-50"><td className="px-5 py-4"><p className="font-semibold text-slate-900">{user.fullName}</p><p className="text-xs text-slate-500">{user.email}</p></td><td className="px-5 py-4 font-medium">{user.role}</td><td className="px-5 py-4 text-slate-500">{user.className ?? '-'}</td><td className="px-5 py-4"><span className={`rounded-full px-2.5 py-1 text-xs font-semibold ${user.isActive ? 'bg-emerald-100 text-emerald-700' : 'bg-slate-100 text-slate-500'}`}>{user.isActive ? 'Active' : 'Inactive'}</span></td><td className="px-5 py-4"><div className="flex justify-end gap-2"><button className="btn-secondary px-3" onClick={() => openEdit(user)}><Pencil className="h-4 w-4" />Edit</button>{user.isActive && <button className="btn-secondary px-3 text-red-600" onClick={() => void deactivate(user)}><UserX className="h-4 w-4" />Deactivate</button>}</div></td></tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {modalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/60 p-4">
          <div className="panel w-full max-w-xl p-5 sm:p-7">
            <div className="flex items-center justify-between"><div><h2 className="text-xl font-bold text-slate-950">{editing ? 'Edit user' : 'Create user'}</h2><p className="mt-1 text-sm text-slate-500">Student accounts must be assigned to a class.</p></div><button className="rounded-lg p-2 text-slate-500 hover:bg-slate-100" onClick={() => setModalOpen(false)}><X className="h-5 w-5" /></button></div>
            <div className="mt-6 grid gap-4 sm:grid-cols-2">
              <div className="sm:col-span-2"><label className="label">Full name</label><input className="input-field" value={form.fullName} onChange={(event) => setForm({ ...form, fullName: event.target.value })} /></div>
              <div className="sm:col-span-2"><label className="label">Email</label><input type="email" className="input-field" value={form.email} onChange={(event) => setForm({ ...form, email: event.target.value })} /></div>
              <div><label className="label">Role</label><select className="input-field" value={form.role} onChange={(event) => setForm({ ...form, role: event.target.value as UserRole, classId: event.target.value === 'Student' ? form.classId : '' })}><option value="Admin">Admin</option><option value="Teacher">Teacher</option><option value="Student">Student</option></select></div>
              <div><label className="label">Class</label><select className="input-field" value={form.classId} disabled={form.role !== 'Student'} onChange={(event) => setForm({ ...form, classId: event.target.value })}><option value="">Select class</option>{classes.map((item) => <option key={item.id} value={item.id}>{item.label}</option>)}</select></div>
              <div className="sm:col-span-2"><label className="label">{editing ? 'New password, optional' : 'Password'}</label><input type="password" className="input-field" value={form.password} onChange={(event) => setForm({ ...form, password: event.target.value })} placeholder={editing ? 'Leave blank to keep current password' : 'At least 8 characters'} /></div>
              {editing && <label className="sm:col-span-2 flex items-center gap-3 rounded-xl border border-slate-200 p-3 text-sm font-medium"><input type="checkbox" checked={form.isActive} onChange={(event) => setForm({ ...form, isActive: event.target.checked })} />Account is active</label>}
            </div>
            <div className="mt-6 flex gap-3 border-t border-slate-200 pt-5"><button className="btn-primary" onClick={() => void save()} disabled={saving}>{saving && <LoaderCircle className="h-4 w-4 animate-spin" />}Save user</button><button className="btn-secondary" onClick={() => setModalOpen(false)} disabled={saving}>Cancel</button></div>
          </div>
        </div>
      )}
    </div>
  );
}
