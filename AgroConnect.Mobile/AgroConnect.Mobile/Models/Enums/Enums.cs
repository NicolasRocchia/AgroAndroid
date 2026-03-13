namespace AgroConnect.Mobile.Models.Enums;

public enum RecipeStatus
{
    ABIERTA, PENDIENTE, APROBADA, RECHAZADA,
    OBSERVADA, REDIRIGIDA, CERRADA, ANULADA
}

public enum ToxicologicalClass { Ia, Ib, II, III, IV }

public enum RiskLevel { BAJO, MEDIO, ALTO }

/// <summary>
/// State machine completa de ejecución (alineada con API real):
/// PENDIENTE → ACEPTADA → EN_CAMINO → (checklist) → EN_CURSO → PAUSADA ↔ EN_CURSO → COMPLETADA
/// Cancel desde cualquier estado activo → CANCELADA
/// Operario puede ejecutar mismas transiciones que aplicador dueño.
/// </summary>
public enum ExecutionStatus
{
    PENDIENTE, ACEPTADA, EN_CAMINO,
    EN_CURSO, PAUSADA,
    COMPLETADA, COMPLETADA_ADMIN, CANCELADA
}

public enum JobStatus
{
    ABIERTO, ASIGNADO, EN_PROGRESO, COMPLETADO, CANCELADO
}
