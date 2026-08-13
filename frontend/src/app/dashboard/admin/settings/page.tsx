'use client';

import { LoaderCircle, Save } from 'lucide-react';
import { useEffect, useState } from 'react';
import { LoadingScreen } from '@/components/loading-screen';
import { PageHeader } from '@/components/page-header';
import { apiRequest } from '@/lib/api';
import { formatDate } from '@/lib/format';
import type { SettingItem } from '@/types';

export default function SettingsPage() {
  const [items, setItems] = useState<SettingItem[]>([]);
  const [values, setValues] = useState<Record<number, string>>({});
  const [loading, setLoading] = useState(true);
  const [savingId, setSavingId] = useState<number | null>(null);
  const [error, setError] = useState('');
  const [notice, setNotice] = useState('');

  const load = async () => {
    setLoading(true);
    try {
      const data = await apiRequest<SettingItem[]>('/admin/settings');
      setItems(data);
      setValues(Object.fromEntries(data.map((item) => [item.id, item.value])));
      setError('');
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to load settings.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { void load(); }, []);

  const save = async (item: SettingItem) => {
    setSavingId(item.id);
    setError('');
    setNotice('');
    try {
      await apiRequest(`/admin/settings/${item.id}`, { method: 'PUT', body: JSON.stringify({ value: values[item.id] ?? '' }) });
      setNotice(`${item.key} was updated.`);
      await load();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to save setting.');
    } finally {
      setSavingId(null);
    }
  };

  return (
    <div>
      <PageHeader eyebrow="Administration" title="Application settings" description="Update safe application-level values. Secrets and connection details are configured through environment variables, not this screen." />
      {error && <div className="mb-4 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}
      {notice && <div className="mb-4 rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-700">{notice}</div>}
      {loading ? <LoadingScreen label="Loading settings..." /> : (
        <div className="grid gap-4 lg:grid-cols-2">
          {items.map((item) => (
            <article key={item.id} className="panel p-5 sm:p-6">
              <div className="flex items-start justify-between gap-4"><div><h2 className="font-bold text-slate-950">{item.key}</h2><p className="mt-1 text-sm leading-6 text-slate-500">{item.description}</p></div><span className="text-xs text-slate-400">{formatDate(item.updatedAt)}</span></div>
              <label className="label mt-5" htmlFor={`setting-${item.id}`}>Value</label>
              <input id={`setting-${item.id}`} className="input-field" value={values[item.id] ?? ''} onChange={(event) => setValues({ ...values, [item.id]: event.target.value })} />
              <button className="btn-primary mt-4" onClick={() => void save(item)} disabled={savingId === item.id}>{savingId === item.id ? <LoaderCircle className="h-4 w-4 animate-spin" /> : <Save className="h-4 w-4" />}Save</button>
            </article>
          ))}
        </div>
      )}
    </div>
  );
}
