export interface Board {
  id: number;
  title: string;
  ownerId: string;
  createdAt: Date;
  columns: Column[];
  members: BoardMember[];
}

export interface Column {
  id: number;
  boardId: number;
  title: string;
  sortOrder: number;
  tasks: Task[];
}

export interface Task {
  id: number;
  columnId: number;
  title: string;
  description?: string;
  sortOrder: number;
  dueDate?: Date;
  isCompleted: boolean;
  createdAt: Date;
  createdById: string;
  assignedToId?: string;
  tags: Tag[];
  comments: Comment[];
}

export interface Comment {
  id: number;
  taskId: number;
  authorId: string;
  authorName: string;
  content: string;
  createdAt: Date;
}

export interface Tag {
  id: number;
  name: string;
  color: string;
}

export interface BoardMember {
  userId: string;
  displayName: string;
  email: string;
  role: 'admin' | 'member';
}

export interface BoardRequest {
  title: string;
  ownerId: string;
  createdAt: Date;
}

export interface ColumnRequest {
  boardId: number;
  title: string;
  sortOrder: number;
}

export interface TaskRequest {
  columnId: number;
  title: string;
  description?: string;
  sortOrder: number;
  dueDate?: Date;
  createdAt: Date;
  createdById: string;
  assignedToId?: string;
  isCompleted: boolean;
}

export interface MoveTaskRequest {
  targetColumnId: number;
  sortOrder: number;
}

export interface CommentRequest {
  authorId: string;
  authorName: string;
  taskId: number;
  content: string;
  createdAt: Date;
}

export interface TagRequest {
  name: string;
  color: string;
}

export interface AddBoardMemberRequest {
  userId: string;
  role: 'admin' | 'member';
}
