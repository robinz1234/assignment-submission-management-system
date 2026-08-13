export type UserRole = 'Admin' | 'Teacher' | 'Student';
export type AssignmentStatus = 'Draft' | 'Published';
export type SubmissionStatus = 'Submitted' | 'Reviewed' | 'Returned';

export interface CurrentUser {
  id: string;
  fullName: string;
  email: string;
  role: UserRole;
  classId?: string | null;
  className?: string | null;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  user: CurrentUser;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface OptionItem {
  id: string;
  label: string;
}

export interface UserItem {
  id: string;
  fullName: string;
  email: string;
  role: UserRole;
  classId?: string | null;
  className?: string | null;
  isActive: boolean;
  createdAt: string;
}

export interface ClassItem {
  id: string;
  name: string;
  section: string;
  academicYear: string;
  studentCount: number;
}

export interface SubjectItem {
  id: string;
  name: string;
  code: string;
}

export interface TeachingAssignmentItem {
  id: string;
  teacherId: string;
  teacherName: string;
  classId: string;
  className: string;
  subjectId: string;
  subjectName: string;
  createdAt: string;
}

export interface AssignmentItem {
  id: string;
  teacherId: string;
  teacherName: string;
  classId: string;
  className: string;
  subjectId: string;
  subjectName: string;
  title: string;
  description: string;
  deadline: string;
  maxMarks: number;
  status: AssignmentStatus;
  allowResubmission: boolean;
  submissionCount: number;
  mySubmissionId?: string | null;
  mySubmissionStatus?: SubmissionStatus | null;
  myMarks?: number | null;
  myFeedback?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface SubmissionItem {
  id: string;
  assignmentId: string;
  assignmentTitle: string;
  maxMarks: number;
  studentId: string;
  studentName: string;
  answerText: string;
  status: SubmissionStatus;
  marks?: number | null;
  feedback?: string | null;
  submittedAt: string;
  updatedAt: string;
  reviewedAt?: string | null;
}

export interface DashboardMetric {
  label: string;
  value: number;
  hint: string;
}

export interface DashboardData {
  role: string;
  metrics: DashboardMetric[];
  recentAssignments: AssignmentItem[];
  recentSubmissions: SubmissionItem[];
}

export interface SettingItem {
  id: number;
  key: string;
  value: string;
  description?: string | null;
  updatedAt: string;
}
