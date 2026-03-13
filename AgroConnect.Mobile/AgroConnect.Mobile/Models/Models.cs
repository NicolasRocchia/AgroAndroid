namespace AgroConnect.Mobile.Models;

// ══════════════════════════════════════════════════════════════
// AUTH — Alineado con APIAgroConnect.Contracts
// ══════════════════════════════════════════════════════════════

/// <summary>API espera Email (no UserName) para login</summary>
public record LoginRequest(string Email, string Password);

/// <summary>Respuesta real de POST /api/auth/login</summary>
public class LoginResponse
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

/// <summary>POST /api/auth/register</summary>
public class RegisterRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string RoleType { get; set; } = "productor";
}

public class RegisterResponse
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

/// <summary>GET /api/users/me</summary>
public class MeResponse
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
}

/// <summary>GET /api/auth/profile</summary>
public class UserProfileDto
{
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? TaxId { get; set; }
    public List<string> Roles { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

// ══════════════════════════════════════════════════════════════
// RECETAS — Alineado con RecipeListItemDto / RecipeDetailDto
// ══════════════════════════════════════════════════════════════

/// <summary>Respuesta paginada genérica (PagedResponse&lt;T&gt; de la API)</summary>
public class PagedResponse<T>
{
    public List<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}

public class RecipeListItem
{
    public long Id { get; set; }
    public long? RfdNumber { get; set; }
    public string PublicCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? PossibleStartDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string AdvisorName { get; set; } = string.Empty;
    public string? Crop { get; set; }
    public string? ApplicationType { get; set; }
    public decimal? UnitSurfaceHa { get; set; }
    public int ProductsCount { get; set; }
    public int LotsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? AssignedMunicipalityName { get; set; }
    public string? RiskLevel { get; set; }
    public decimal? EiqScore { get; set; }
    public string? EiqLevel { get; set; }
}

public class RecipeDetail
{
    public long Id { get; set; }
    public long? RfdNumber { get; set; }
    public string PublicCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    // Fechas
    public DateTime IssueDate { get; set; }
    public DateTime? PossibleStartDate { get; set; }
    public DateTime? RecommendedDate { get; set; }
    public DateTime? ExpirationDate { get; set; }

    // Actores
    public RequesterDto? Requester { get; set; }
    public AdvisorDto? Advisor { get; set; }

    // Cultivo
    public string? ApplicationType { get; set; }
    public string? Crop { get; set; }
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    public string? MachineToUse { get; set; }
    public decimal? UnitSurfaceHa { get; set; }

    // Condiciones ambientales
    public decimal? TempMin { get; set; }
    public decimal? TempMax { get; set; }
    public decimal? HumidityMin { get; set; }
    public decimal? HumidityMax { get; set; }
    public decimal? WindMinKmh { get; set; }
    public decimal? WindMaxKmh { get; set; }
    public string? WindDirection { get; set; }

    // Máquina
    public string? MachinePlate { get; set; }
    public string? MachineLegalName { get; set; }
    public string? MachineType { get; set; }

    public string? Notes { get; set; }

    // Relaciones
    public List<RecipeProductDto> Products { get; set; } = [];
    public List<RecipeLotDto> Lots { get; set; } = [];
    public List<RecipeSensitivePointDto> SensitivePoints { get; set; } = [];

    // Auditoría / productor
    public DateTime CreatedAt { get; set; }
    public long? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public string? CreatedByUserEmail { get; set; }
    public string? CreatedByUserPhone { get; set; }
    public string? CreatedByUserTaxId { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Municipal
    public long? AssignedMunicipalityId { get; set; }
    public string? AssignedMunicipalityName { get; set; }
    public DateTime? AssignedAt { get; set; }
    public List<RecipeReviewLogDto> ReviewLogs { get; set; } = [];
    public List<RecipeMessageDto> Messages { get; set; } = [];
    public List<RecipeExecutionDto> Executions { get; set; } = [];

    // EIQ
    public decimal? EiqScore { get; set; }
    public string? EiqLevel { get; set; }
}

public class RequesterDto
{
    public long Id { get; set; }
    public string LegalName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Contact { get; set; }
}

public class AdvisorDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Contact { get; set; }
}

public class RecipeProductDto
{
    public long Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? SenasaRegistry { get; set; }
    public string? ToxicologicalClass { get; set; }
    public string? ProductType { get; set; }
    public decimal? DoseValue { get; set; }
    public string? DoseUnit { get; set; }
    public string? DosePerUnit { get; set; }
    public decimal? TotalValue { get; set; }
    public string? TotalUnit { get; set; }
}

public class RecipeLotDto
{
    public long Id { get; set; }
    public string LotName { get; set; } = string.Empty;
    public string? Locality { get; set; }
    public string? Department { get; set; }
    public decimal? SurfaceHa { get; set; }
    public List<RecipeLotVertexDto> Vertices { get; set; } = [];
}

public class RecipeLotVertexDto
{
    public long Id { get; set; }
    public int Order { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}

public class RecipeSensitivePointDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Type { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Locality { get; set; }
    public string? Department { get; set; }
}

public class RecipeReviewLogDto
{
    public long Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class RecipeMessageDto
{
    public long Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderRole { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class RecipeExecutionDto
{
    public long Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string ApplicatorName { get; set; } = string.Empty;
    public DateTime? AcceptedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public int? TotalActiveMinutes { get; set; }
}

// ══════════════════════════════════════════════════════════════
// EJECUCIÓN — Alineado con ExecutionController + DTOs
// ══════════════════════════════════════════════════════════════

public class ExecutionListDto
{
    public long Id { get; set; }
    public long RecipeId { get; set; }
    public string RecipePublicCode { get; set; } = string.Empty;
    public string? Crop { get; set; }
    public decimal? SurfaceHa { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ProducerName { get; set; } = string.Empty;
    public string ApplicatorBusinessName { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? TotalActiveMinutes { get; set; }
    public int? ReviewRating { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ExecutionDetailDto
{
    public long Id { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public long RecipeId { get; set; }
    public string RecipePublicCode { get; set; } = string.Empty;
    public string? Crop { get; set; }
    public string? ApplicationType { get; set; }
    public decimal? SurfaceHa { get; set; }
    public string? Locality { get; set; }
    public string? Department { get; set; }

    public decimal? LotCentroidLat { get; set; }
    public decimal? LotCentroidLng { get; set; }

    public long ProducerId { get; set; }
    public string ProducerName { get; set; } = string.Empty;
    public long ApplicatorProfileId { get; set; }
    public string ApplicatorBusinessName { get; set; } = string.Empty;
    public string? ApplicatorPhone { get; set; }

    public long? JobPostingId { get; set; }
    public OperatorInfoDto? Operator { get; set; }

    public DateTime? AcceptedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    public int? TotalActiveMinutes { get; set; }
    public int? TotalPauseMinutes { get; set; }

    public string? WeatherAtStart { get; set; }
    public string? WeatherAtEnd { get; set; }

    public decimal? GpsLatStart { get; set; }
    public decimal? GpsLngStart { get; set; }
    public decimal? GpsLatEnd { get; set; }
    public decimal? GpsLngEnd { get; set; }

    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public ExecutionChecklistDto? Checklist { get; set; }
    public List<ExecutionEventDto> Events { get; set; } = [];
    public ReviewDto? Review { get; set; }
}

public class OperatorInfoDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
}

public class ExecutionEventDto
{
    public long Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public decimal? GpsLat { get; set; }
    public decimal? GpsLng { get; set; }
    public double? DistanceToLotKm { get; set; }
    public string? WeatherSnapshot { get; set; }
    public string? PauseReason { get; set; }
    public string? Notes { get; set; }
}

public class ExecutionChecklistDto
{
    public bool EquipmentCalibrated { get; set; }
    public bool PPEEquipped { get; set; }
    public bool MixturePrepared { get; set; }
    public bool ExclusionZonesVerified { get; set; }
    public bool WindConditionsOk { get; set; }
    public string? CustomNotes { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class ReviewDto
{
    public long Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>Transición de estado (body para accept/start/pause/resume/complete/cancel)</summary>
public class ExecutionTransitionRequest
{
    public decimal? GpsLat { get; set; }
    public decimal? GpsLng { get; set; }
    public string? PauseReason { get; set; }
    public string? Notes { get; set; }
}

/// <summary>Checklist pre-aplicación</summary>
public class SubmitChecklistRequest
{
    public bool EquipmentCalibrated { get; set; }
    public bool PPEEquipped { get; set; }
    public bool MixturePrepared { get; set; }
    public bool ExclusionZonesVerified { get; set; }
    public bool WindConditionsOk { get; set; }
    public string? CustomNotes { get; set; }
}

/// <summary>Review del productor al aplicador</summary>
public class CreateReviewRequest
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

// ══════════════════════════════════════════════════════════════
// LOTES — Alineado con LotsController
// ══════════════════════════════════════════════════════════════

/// <summary>Item del listado GET /api/lots/my-lots</summary>
public class LotListItem
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Locality { get; set; }
    public string? Department { get; set; }
    public decimal? AreaHa { get; set; }
    public decimal? CentroidLat { get; set; }
    public decimal? CentroidLng { get; set; }
    public int RecipesCount { get; set; }
    public List<LotVertexDto> Vertices { get; set; } = [];
}

/// <summary>Detalle GET /api/lots/by-code/{code} o /api/lots/{id}</summary>
public class LotDetail
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Locality { get; set; }
    public string? Department { get; set; }
    public decimal? AreaHa { get; set; }
    public decimal? CentroidLat { get; set; }
    public decimal? CentroidLng { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<LotVertexDto> Vertices { get; set; } = [];
    public List<LotRecipeDto> Recipes { get; set; } = [];
}

public class LotVertexDto
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}

public class LotRecipeDto
{
    public long Id { get; set; }
    public long? RfdNumber { get; set; }
    public string? PublicCode { get; set; }
    public string? Status { get; set; }
    public DateTime? IssueDate { get; set; }
    public string? Crop { get; set; }
    public string? RequesterName { get; set; }
    public string? AdvisorName { get; set; }
}

// ══════════════════════════════════════════════════════════════
// BOLSA DE TRABAJO — Alineado con JobPostingController + DTOs
// ══════════════════════════════════════════════════════════════

public class JobPostingListDto
{
    public long Id { get; set; }
    public long? RecipeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Crop { get; set; }
    public string? ApplicationType { get; set; }
    public decimal? SurfaceHa { get; set; }
    public string? Locality { get; set; }
    public string? Department { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ProducerName { get; set; } = string.Empty;
    public int ApplicationCount { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public double? DistanceKm { get; set; }
    public bool AlreadyApplied { get; set; }
}

public class ApplyToJobRequest
{
    public string? Message { get; set; }
    public decimal? ProposedPrice { get; set; }
}

// ══════════════════════════════════════════════════════════════
// NOTIFICACIONES
// ══════════════════════════════════════════════════════════════

public class NotificationDto
{
    public long Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string? LinkUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UnreadCountResponse
{
    public int Count { get; set; }
}

// ══════════════════════════════════════════════════════════════
// PERFIL APLICADOR — /api/applicator/*
// ══════════════════════════════════════════════════════════════

public class ApplicatorProfileDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Description { get; set; }
    public List<string> MachineTypes { get; set; } = [];
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public List<ApplicatorLocationDto> Locations { get; set; } = [];
}

public class ApplicatorLocationDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsPrimary { get; set; }
}

// ══════════════════════════════════════════════════════════════
// METEO / COMÚN
// ══════════════════════════════════════════════════════════════

public class WeatherData
{
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public string? WindDirection { get; set; }
}

// ══════════════════════════════════════════════════════════════
// DASHBOARD — GET /api/dashboard/applicator
// ══════════════════════════════════════════════════════════════

public class ApplicatorDashboardDto
{
    public string Role { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsVerified { get; set; }

    // KPIs principales
    public int ActiveExecutions { get; set; }
    public int CompletedExecutions { get; set; }
    public int AvailableJobs { get; set; }
    public int UnreadNotifications { get; set; }

    // KPIs reputación
    public double? AverageRating { get; set; }
    public double? CompletionRate { get; set; }
    public decimal TotalHectares { get; set; }

    // Listas top 3
    public List<DashboardExecutionItem> RecentActiveExecutions { get; set; } = [];
    public List<DashboardJobItem> NearbyJobs { get; set; } = [];
}

public class DashboardExecutionItem
{
    public long Id { get; set; }
    public string RecipePublicCode { get; set; } = string.Empty;
    public string? Crop { get; set; }
    public decimal? SurfaceHa { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class DashboardJobItem
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Crop { get; set; }
    public decimal? SurfaceHa { get; set; }
    public string? Locality { get; set; }
    public double? DistanceKm { get; set; }
}

/// <summary>Respuesta genérica de error de la API</summary>
public class ApiErrorResponse
{
    public string? Error { get; set; }
    public List<string>? Errors { get; set; }
}

// ══════════════════════════════════════════════════════════════
// CAMBIAR CONTRASEÑA — PUT /api/auth/change-password
// ══════════════════════════════════════════════════════════════

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

// ══════════════════════════════════════════════════════════════
// EDITAR PERFIL — PUT /api/auth/profile
// ══════════════════════════════════════════════════════════════

public class UpdateProfileRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? TaxId { get; set; }
}

// ══════════════════════════════════════════════════════════════
// OPERARIOS — /api/operators/*
// ══════════════════════════════════════════════════════════════

public class OperatorListDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ExecutionsCount { get; set; }
}

public class OperatorDetailDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateOperatorRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}

public class CreateOperatorResponse
{
    public OperatorDetailDto? OperatorProfile { get; set; }
    public string TempPassword { get; set; } = string.Empty;
}

public class UpdateOperatorRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AssignOperatorRequest
{
    public long OperatorProfileId { get; set; }
}

/// <summary>Respuesta genérica con mensaje</summary>
public class MessageResponse
{
    public string Message { get; set; } = string.Empty;
}
