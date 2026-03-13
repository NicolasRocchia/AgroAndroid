namespace AgroConnect.Mobile.Models.Enums;

public enum RecipeStatus
{
    ABIERTA, PENDIENTE, APROBADA, RECHAZADA,
    OBSERVADA, REDIRIGIDA, CERRADA, ANULADA
}

public enum ToxicologicalClass { Ia, Ib, II, III, IV }

public enum RiskLevel { BAJO, MEDIO, ALTO }

/// <summary>
/// State machine completa de ejecución:
/// ASIGNADA → ACEPTADA → EN_CAMINO → EN_PROGRESO → PAUSADA ↔ EN_PROGRESO → COMPLETADA
///                                                                          → CANCELADA
/// </summary>
public enum ExecutionStatus
{
    ASIGNADA, ACEPTADA, EN_CAMINO,
    EN_PROGRESO, PAUSADA,
    COMPLETADA, CANCELADA
}

public enum JobStatus
{
    ABIERTO, ASIGNADO, EN_PROGRESO, COMPLETADO, CANCELADO
}
