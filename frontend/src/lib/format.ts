export function formatDate(value: string) {
  return new Intl.DateTimeFormat('en-BD', {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(new Date(value));
}

export function isPast(value: string) {
  return new Date(value).getTime() < Date.now();
}

export function truncate(value: string, length = 120) {
  return value.length > length ? `${value.slice(0, length).trim()}...` : value;
}
