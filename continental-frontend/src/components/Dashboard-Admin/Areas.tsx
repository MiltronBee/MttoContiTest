import { useState, useEffect, useCallback } from "react";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { ActionButtons } from "@/components/ui/action-buttons";
import { SearchableSelect } from "@/components/ui/searchable-select";
import { GroupsTable } from "./GroupsTable";
import { useAreas } from "@/hooks/useAreas";
import { useAreaIngenieros } from "@/hooks/useAreaIngenieros";
import { areasService } from "@/services/areasService";
import { userService } from "@/services/userService";
import { groupService } from "@/services/groupService";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { UserRole, type User } from "@/interfaces/User.interface";
import { sortApiGroups } from '@/utils/sort';
import { toast } from "sonner";
import { X } from "lucide-react";

interface AreaFormData {
  nombreArea: string;
  nombreSAP: string;
  nombreJefe: string;
  correoJefe: string;
  default_manning?: number;
}

interface GroupData {
  id: number;
  grupo: string;
  identificadorSAP: string;
  personasPorTurno: string;
  duracionDeturno: string;
  liderGrupo: string;
}

export const Areas = () => {
  const { areas, loading, error, getAreaById, loadingAreaDetails, refetch } = useAreas();
  const [selectedArea, setSelectedArea] = useState<string>("");
  const selectedAreaId = selectedArea ? parseInt(selectedArea) : null;
  const { 
    assignedIngenieros: initialAssignedIngenieros, 
    availableIngenieros, 
    loading: ingenierosLoading, 
    assignIngeniero, 
    unassignIngeniero 
  } = useAreaIngenieros(selectedAreaId);
  
  // Local state to track assigned engineers before saving
  const [assignedIngenieros, setAssignedIngenieros] = useState(initialAssignedIngenieros);
  const [pendingAssignments, setPendingAssignments] = useState<number[]>([]);
  const [pendingUnassignments, setPendingUnassignments] = useState<number[]>([]);
  const [formData, setFormData] = useState<AreaFormData>({
    nombreArea: "",
    nombreSAP: "",
    nombreJefe: "",
    correoJefe: "",
    default_manning: 0,
  });
  const [isUpdating, setIsUpdating] = useState(false);
  const [usersByRole, setUsersByRole] = useState<User[]>([]);
  const [loadingUsers, setLoadingUsers] = useState(false);
  const [selectedIngenieroValue, setSelectedIngenieroValue] = useState<string>("");

  // Initialize with empty groups array
  const [groups, setGroups] = useState<GroupData[]>([]);
  const [groupChanges, setGroupChanges] = useState<Map<number, Partial<GroupData>>>(new Map());

  // Handle area selection
  const handleAreaSelection = useCallback(async () => {
    // Reset ingeniero selection and pending changes when area changes
    setSelectedIngenieroValue("");
    setPendingAssignments([]);
    setPendingUnassignments([]);
    
    if (selectedArea) {
      const areaId = parseInt(selectedArea);
      
      try {
        const areaDetails = await getAreaById(areaId);
        
        // Update form data
        setFormData({
          nombreArea: areaDetails.nombreGeneral,
          nombreSAP: areaDetails.unidadOrganizativaSap,
          nombreJefe: areaDetails?.jefeId?.toString() || "",
          correoJefe: areaDetails?.jefe?.username || "",
          default_manning: areaDetails?.manning || 0,
        });
        
        if (areaDetails.grupos && areaDetails.grupos.length > 0) {
          console.log("areaDetails.grupos", areaDetails.grupos)
          // Sort groups directly from API response before mapping
          const sortedApiGroups = sortApiGroups([...areaDetails.grupos]);
          
          const mappedGroups = sortedApiGroups.map(grupo => ({
            id: grupo.grupoId,
            grupo: grupo.rol,
            identificadorSAP: grupo.identificadorSAP || "", // Map from API field
            personasPorTurno: grupo.personasPorTurno ? grupo.personasPorTurno.toString() : "",
            duracionDeturno: grupo.duracionDeturno ? grupo.duracionDeturno.toString() : "",
            liderGrupo: grupo.liderId ? grupo.liderId.toString() : ""
          }));
          
          setGroups(mappedGroups);
        } else {
          // Reset to empty array if no groups found
          setGroups([]);
        }
      } catch (error) {
        console.error('Error fetching area details:', error);
        // Reset groups on error
        setGroups([]);
      }
    } else {
      // Reset form and groups when no area is selected
      setFormData({
        nombreArea: "",
        nombreSAP: "",
        nombreJefe: "",
        correoJefe: "",
        default_manning: 0,
      });
      setGroups([]);
    }
  }, [selectedArea, getAreaById]);
  // Fetch users by role on component mount
  const fetchUsersByRole = async () => {
    setLoadingUsers(true);
    try {
      const users = await userService.getUsersByRole(UserRole.AREA_ADMIN);
      setUsersByRole(users);
    } catch (error) {
      console.error('Error fetching users by role:', error);
      toast.error("Error al cargar los jefes de área");
    } finally {
      setLoadingUsers(false);
    }
  };

  useEffect(() => {
    handleAreaSelection();
  }, [handleAreaSelection]);

  useEffect(() => {
    fetchUsersByRole();
  }, []);

  const handleFormChange = (field: keyof AreaFormData, value: string) => {
    // Prevent negative numbers for default_manning
    if (field === 'default_manning') {
      const numericValue = parseInt(value, 10);
      if (isNaN(numericValue) || numericValue < 0) {
        value = '0';
      } else {
        value = numericValue.toString();
      }
    }
    
    // Auto-update correo when nombreJefe changes
    if (field === 'nombreJefe') {
      const selectedUser = usersByRole.find(user => user.id.toString() === value);
      const correoJefe = selectedUser?.username || '';
      
      setFormData(prev => ({
        ...prev,
        [field]: value,
        correoJefe: correoJefe
      }));
      return;
    }
    
    setFormData(prev => ({
      ...prev,
      [field]: field === 'default_manning' ? parseInt(value) || 0 : value
    }));
  };

  const handleGroupChange = (groupId: number, field: keyof GroupData, value: string) => {
    // Update the groups state
    setGroups(prev => prev.map(group => 
      group.id === groupId ? { ...group, [field]: value } : group
    ));

    // Track changes for this group
    setGroupChanges(prev => {
      const newChanges = new Map(prev);
      const existingChanges = newChanges.get(groupId) || {};
      newChanges.set(groupId, { ...existingChanges, [field]: value });
      return newChanges;
    });
  };

  const updateGroupChanges = async () => {
    const promises: Promise<void>[] = [];

    for (const [groupId, changes] of groupChanges.entries()) {
      // Update group leader if changed
      if (changes.liderGrupo) {
        const userId = parseInt(changes.liderGrupo);
        if (userId > 0) {
          promises.push(groupService.updateGroupLeader(groupId, userId));
        }
        // Note: If userId is 0 (Sin asignar), we might need a different endpoint to remove the leader
      }

      // Update shift info if personasPorTurno or duracionDeturno changed
      if (changes.personasPorTurno || changes.duracionDeturno) {
        // Get current group data to fill missing values
        const currentGroup = groups.find(g => g.id === groupId);
        if (currentGroup) {
          const shiftData = {
            personasPorTurno: changes.personasPorTurno ? parseInt(changes.personasPorTurno) : parseInt(currentGroup.personasPorTurno),
            duracionDeturno: changes.duracionDeturno ? parseInt(changes.duracionDeturno) : parseInt(currentGroup.duracionDeturno)
          };
          promises.push(groupService.updateGroupShift(groupId, shiftData));
        }
      }
    }

    // Execute all updates
    await Promise.all(promises);
    toast.success("Grupos actualizados exitosamente");
  };

  // Handlers para ingenieros
  const handleAssignIngeniero = (ingenieroId: string) => {
    const id = parseInt(ingenieroId);
    // Add to pending assignments and update local state
    setPendingAssignments(prev => [...prev, id]);
    // Remove from unassignments if it was previously unassigned
    setPendingUnassignments(prev => prev.filter(item => item !== id));
    setSelectedIngenieroValue(""); // Reset selection after assigning
  };

  const handleUnassignIngeniero = (ingenieroId: number) => {
    // Add to pending unassignments and update local state
    setPendingUnassignments(prev => [...prev, ingenieroId]);
    // Remove from assignments if it was previously assigned
    setPendingAssignments(prev => prev.filter(id => id !== ingenieroId));
  };

  // Update local assigned engineers when initial data changes
  useEffect(() => {
    setAssignedIngenieros(initialAssignedIngenieros);
    // Reset pending changes when area changes
    setPendingAssignments([]);
    setPendingUnassignments([]);
  }, [initialAssignedIngenieros]);

  // Update the displayed list of assigned engineers based on pending changes
  useEffect(() => {
    let updated = [...initialAssignedIngenieros];
    
    // Remove unassigned engineers
    updated = updated.filter(ing => !pendingUnassignments.includes(ing.id));
    
    // Add newly assigned engineers from available list
    const newAssignments = availableIngenieros.filter(ing => 
      pendingAssignments.includes(ing.id) && 
      !updated.some(assigned => assigned.id === ing.id)
    );
    updated = [...updated, ...newAssignments];
    
    setAssignedIngenieros(updated);
  }, [pendingAssignments, pendingUnassignments, initialAssignedIngenieros, availableIngenieros]);

  const handleUpdate = async () => {
    if (!selectedArea) {
      toast.error("Por favor selecciona un área para actualizar");
      return;
    }

    // Validar que los campos requeridos estén llenos
    if (!formData.nombreArea.trim() || !formData.nombreSAP.trim()) {
      toast.error("Por favor completa los campos Nombre del área y Nombre SAP");
      return;
    }

    setIsUpdating(true);
    
    try {
      const areaId = parseInt(selectedArea);
      
      // 1. Actualizar datos básicos del área
      const updateData = {
        UnidadOrganizativaSap: formData.nombreSAP.trim(),
        NombreGeneral: formData.nombreArea.trim(),
        Manning: formData.default_manning || 0
      };

      const updatedArea = await areasService.updateArea(areaId, updateData);
      
      // 2. Asignar jefe si está seleccionado
      if (formData.nombreJefe && formData.nombreJefe.trim()) {
        const jefeId = parseInt(formData.nombreJefe);
        await areasService.assignBoss(areaId, jefeId);
        toast.success(`Área actualizada y jefe asignado exitosamente: ${updatedArea.nombreGeneral}`);
      } else {
        toast.success(`Área actualizada exitosamente: ${updatedArea.nombreGeneral}`);
      }
      
      // 3. Actualizar asignaciones de ingenieros si hay cambios
      if (pendingAssignments.length > 0 || pendingUnassignments.length > 0) {
        try {
          // Process unassignments first
          await Promise.all(
            pendingUnassignments.map(ingenieroId => 
              unassignIngeniero(ingenieroId)
            )
          );
          
          // Then process new assignments
          await Promise.all(
            pendingAssignments.map(ingenieroId => 
              assignIngeniero(ingenieroId)
            )
          );
          
          toast.success("Asignaciones de ingenieros actualizadas exitosamente");
        } catch (error) {
          console.error('Error updating engineer assignments:', error);
          
          // Check if it's a SQL null value error
          const errorMessage = error instanceof Error ? error.message : String(error);
          if (errorMessage.includes('SqlNullValueException') || errorMessage.includes('500')) {
            toast.error("Error en la base de datos: Hay valores faltantes en el área. Contacta al administrador del sistema.");
          } else {
            toast.error("Error al actualizar las asignaciones de ingenieros");
          }
          
          // Reset pending changes on error to avoid inconsistent state
          setPendingAssignments([]);
          setPendingUnassignments([]);
        }
      }
      
      // 4. Actualizar grupos si hay cambios
      if (groupChanges.size > 0) {
        try {
          await updateGroupChanges();
        } catch (error) {
          console.error('Error updating groups:', error);
          toast.error("Error al actualizar algunos grupos");
        }
      }

      // 5. Actualizar la lista de áreas para reflejar los cambios
      await refetch();
      
      // 6. Limpiar cambios pendientes
      setGroupChanges(new Map());
      setPendingAssignments([]);
      setPendingUnassignments([]);
      
    } catch (error) {
      console.error('Error updating area:', error);
      const errorMessage = error instanceof Error ? error.message : 'Error desconocido al actualizar el área';
      toast.error(`Error al actualizar el área: ${errorMessage}`);
    } finally {
      setIsUpdating(false);
    }
  };

  return (
    <div className="p-6 bg-white min-h-screen h-auto flex flex-col overflow-hidden">
      <div className="max-w-5xl mx-auto w-full flex-1 flex flex-col space-y-6">
        {/* Title */}
        <h1 className="text-3xl font-bold text-continental-black text-left">
          Áreas
        </h1>

        {/* Area Select */}
        <div className="w-full max-w-6xl">
          <SearchableSelect
            options={areas.map((area) => ({
              value: area.areaId.toString(),
              label: area.nombreGeneral,
              area: area
            }))}
            value={selectedArea}
            onValueChange={setSelectedArea}
            placeholder={loading ? "Cargando áreas..." : "Selecciona un área"}
            searchPlaceholder="Buscar área..."
            disabled={loading}
            loading={loading}
            error={error}
            emptyMessage="No se encontraron áreas"
            className="w-full"
          />
        </div>

        {
          selectedArea ? (
            <div>
              {/* Form Section */}
              <div className="grid grid-cols-2 gap-6">
                {/* First Row */}
                <div className="space-y-2">
                  <Label htmlFor="nombreArea" className="text-sm font-medium text-continental-gray-1">
                    Nombre del área
                  </Label>
                  <Input
                    id="nombreArea"
                    value={formData.nombreArea}
                    onChange={(e) => handleFormChange('nombreArea', e.target.value)}
                    placeholder="Ingresa el nombre del área"
                    className="w-full"
                  />
                </div>
                
                <div className="space-y-2">
                  <Label htmlFor="nombreSAP" className="text-sm font-medium text-continental-gray-1">
                    Nombre SAP
                  </Label>
                  <Input
                    id="nombreSAP"
                    value={formData.nombreSAP}
                    onChange={(e) => handleFormChange('nombreSAP', e.target.value)}
                    placeholder="Ingresa el nombre SAP"
                    className="w-full"
                  />
                </div>
                {/* Select con usuarios de tipo jefe de area */}
                <div className="space-y-2">
                  <Label htmlFor="nombreJefe" className="text-sm font-medium text-continental-gray-1">
                    Jefe de área
                  </Label>
                  <Select
                    value={formData.nombreJefe}
                    onValueChange={(value) => handleFormChange('nombreJefe', value)}
                    disabled={loadingUsers}
                  >
                    <SelectTrigger className="w-full">
                      <SelectValue placeholder={loadingUsers ? "Cargando jefes..." : "Selecciona un jefe de área"} />
                    </SelectTrigger>
                    <SelectContent>
                      {usersByRole.map((user) => (
                        <SelectItem key={user.id} value={user.id.toString()}>
                          {user.fullName}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
      
                
                <div className="space-y-2">
                  <Label htmlFor="correoJefe" className="text-sm font-medium text-continental-gray-1">
                    Correo de jefe de área
                  </Label>
                  <Input
                    id="correoJefe"
                    type="email"
                    value={formData.correoJefe}
                    onChange={(e) => handleFormChange('correoJefe', e.target.value)}
                    placeholder="jefe@continental.com"
                    className="w-full"
                  />
                </div>
              </div>

              {/* Ingenieros Industriales */}
              <div className="space-y-4 mt-4">
                <Label className="text-sm font-medium text-continental-gray-1">
                  Ingenieros Industriales
                </Label>
                
                {/* Lista de ingenieros asignados */}
                {assignedIngenieros.length > 0 && (
                  <div className="space-y-2">
                    <Label className="text-xs text-continental-gray-2">Asignados:</Label>
                    <div className="flex flex-wrap gap-2">
                      {assignedIngenieros.map((ingeniero) => (
                        <div 
                          key={ingeniero.id} 
                          className="flex items-center gap-2 bg-continental-yellow/20 text-continental-gray-1 px-3 py-1 rounded-md text-sm"
                        >
                          <span>{ingeniero.fullName}</span>
                          <button
                            type="button"
                            onClick={() => handleUnassignIngeniero(ingeniero.id)}
                            className="text-red-500 hover:text-red-700 transition-colors cursor-pointer"
                          >
                            <X size={14} />
                          </button>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
                
                {/* Select para agregar nuevos ingenieros */}
                <div>
                  <Select value={selectedIngenieroValue} onValueChange={handleAssignIngeniero}>
                    <SelectTrigger className="w-full">
                      <SelectValue placeholder="Seleccionar ingeniero para asignar" />
                    </SelectTrigger>
                    <SelectContent>
                      {(() => {
                        console.log('=== DEBUGGING ENGINEER FILTERING ===');
                        console.log('Available ingenieros (all):', availableIngenieros.map(i => ({ id: i.id, name: i.fullName })));
                        console.log('Initial assigned ingenieros (from server):', initialAssignedIngenieros.map(i => ({ id: i.id, name: i.fullName })));
                        console.log('Current assigned ingenieros (local state):', assignedIngenieros.map(i => ({ id: i.id, name: i.fullName })));
                        console.log('Pending assignments:', pendingAssignments);
                        console.log('Pending unassignments:', pendingUnassignments);
                        console.log('Selected area:', selectedArea);
                        
                        console.log('🚨 CRITICAL ISSUE: ID MISMATCH DETECTED!');
                        console.log('Available engineers have IDs: 2061, 2062');
                        console.log('Assigned engineers have IDs: 21, 22');
                        console.log('These are completely different engineers or there is a data mapping issue!');
                        
                        // TEMPORARY FIX: Since there's an ID mismatch issue, we need to compare by name instead of ID
                        // This is a workaround until the backend data inconsistency is resolved
                        const filteredIngenieros = availableIngenieros.filter(ingeniero => {
                          // Check if engineer is currently assigned by comparing names (temporary fix)
                          const isCurrentlyAssigned = initialAssignedIngenieros.some(assigned => 
                            assigned.fullName === ingeniero.fullName
                          );
                          
                          // Check if engineer is in pending assignments (local state)
                          const isPendingAssignment = pendingAssignments.includes(ingeniero.id);
                          
                          // Check if engineer was unassigned by comparing names (since IDs don't match)
                          const wasUnassignedByName = initialAssignedIngenieros.some(assigned => {
                            const matchesName = assigned.fullName === ingeniero.fullName;
                            const wasUnassignedById = pendingUnassignments.includes(assigned.id);
                            return matchesName && wasUnassignedById;
                          });
                          
                          // Engineer should show in dropdown if:
                          // 1. Not currently assigned AND not pending assignment
                          // OR
                          // 2. Was unassigned (by name comparison) AND not pending reassignment
                          const shouldShow = (!isCurrentlyAssigned && !isPendingAssignment) || (wasUnassignedByName && !isPendingAssignment);
                          
                          console.log(`Engineer ${ingeniero.fullName} (ID: ${ingeniero.id}):`, {
                            isCurrentlyAssigned: isCurrentlyAssigned,
                            isPendingAssignment,
                            wasUnassignedByName,
                            shouldShow,
                            comparisonMethod: 'BY_NAME (temporary fix)',
                            pendingUnassignments: pendingUnassignments
                          });
                          
                          if (shouldShow) {
                            if (wasUnassignedByName) {
                              console.log(`  🔄 ${ingeniero.fullName} was unassigned (by name), now available in dropdown`);
                            } else {
                              console.log(`  ❌ ${ingeniero.fullName} is NOT assigned, available in dropdown`);
                            }
                          } else {
                            console.log(`  ✅ ${ingeniero.fullName} is assigned or pending, NOT in dropdown`);
                          }
                          
                          return shouldShow;
                        });
                        
                        console.log('Filtered ingenieros (should show in dropdown):', filteredIngenieros.map(i => ({ id: i.id, name: i.fullName })));
                        
                        if (initialAssignedIngenieros.length > 0 && filteredIngenieros.length > 0) {
                          console.log('⚠️ STILL SHOWING: Some engineers still in dropdown despite being assigned');
                        } else if (initialAssignedIngenieros.length > 0 && filteredIngenieros.length === 0) {
                          console.log('✅ FIXED: Engineers are assigned and dropdown is now empty!');
                        } else if (initialAssignedIngenieros.length === 0 && filteredIngenieros.length > 0) {
                          console.log('✅ CORRECT: No engineers assigned, dropdown shows available engineers');
                        }
                        
                        console.log('🔧 USING NAME COMPARISON as temporary fix for ID mismatch issue');
                        
                        console.log('=== END DEBUGGING ===');
                        
                        return filteredIngenieros.map((ingeniero) => (
                          <SelectItem key={ingeniero.id} value={ingeniero.id.toString()}>
                            {ingeniero.fullName}
                          </SelectItem>
                        ));
                      })()}
                    </SelectContent>
                  </Select>
                </div>
                
                {ingenierosLoading && (
                  <div className="text-sm text-continental-gray-2">Cargando ingenieros...</div>
                )}
              </div>      

              
              <div className="space-y-2 mt-4">
                <Label htmlFor="default_manning" className="text-sm font-medium text-continental-gray-1">
                  Manning por defecto
                </Label>
                <Input
                  id="default_manning"
                  type="number"
                  min="0"
                  value={formData.default_manning}
                  onChange={(e) => handleFormChange('default_manning', e.target.value)}
                  onKeyDown={(e) => {
                    // Prevent typing negative numbers
                    if (e.key === '-' || e.key === 'e' || e.key === 'E' || e.key === '+' || e.key === '.') {
                      e.preventDefault();
                    }
                  }}
                  placeholder="10"
                  className="w-full"
                />
              </div>
      
              {/* Groups Table */}
              <div className="relative mt-6">
                {loadingAreaDetails && (
                  <div className="absolute inset-0 bg-white/80 flex items-center justify-center z-10 rounded-md">
                    <div className="flex items-center gap-2 text-sm text-continental-gray-1">
                      <div className="animate-spin rounded-full h-4 w-4 border-2 border-continental-yellow border-t-transparent"></div>
                      Cargando detalles del área...
                    </div>
                  </div>
                )}
                <GroupsTable
                  groups={groups}
                  onGroupChange={handleGroupChange}
                />
              </div>
      
              {/* Action Buttons */}
              <ActionButtons
                buttons={[
                  {
                    key: 'create',
                    label: isUpdating ? 'Guardando...' : 'Guardar',
                    variant: 'continental',
                    onClick: handleUpdate,
                    className: 'w-28 h-10',
                    disabled: isUpdating || !selectedArea || !formData.nombreArea.trim() || !formData.nombreSAP.trim()
                  }
                ]}
              />
              
            </div>
          ) : (
            <p className="text-center text-continental-gray-1">Selecciona un área para ver los detalles</p>
          )
        }
      </div>
    </div>
  );
};