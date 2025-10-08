import { httpClient } from "@/services/httpClient";
import type { ApiResponse } from "@/interfaces/Api.interface";

export interface UpdateGroupLeaderRequest {
  userId: number;
}

export interface UpdateGroupShiftRequest {
  personasPorTurno: number;
  duracionDeturno: number;
}

export const groupService = {
  async updateGroupLeader(groupId: number, userId: number): Promise<void> {
    try {
      await httpClient.put<ApiResponse<void>>(
        `/api/Grupo/${groupId}/Lider`,
        userId 
      );
    } catch (error) {
      console.error("Error in groupService.updateGroupLeader:", error);
      throw error;
    }
  },

  async updateGroupShift(groupId: number, shiftData: UpdateGroupShiftRequest): Promise<void> {
    try {
      await httpClient.put<ApiResponse<void>>(
        `/api/Grupo/${groupId}/Turno`,
        shiftData
      );
    } catch (error) {
      console.error("Error in groupService.updateGroupShift:", error);
      throw error;
    }
  }
};
