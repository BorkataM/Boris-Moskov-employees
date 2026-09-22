export interface ProjectCollaborationRow {
  employeeId1: number;
  employeeId2: number;
  projectId: number;
  daysWorked: number;
}

export interface CollaborationResult {
  employeeId1: number;
  employeeId2: number;
  totalDaysWorked: number;
  projects: ProjectCollaborationRow[];
}
