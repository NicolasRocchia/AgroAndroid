using AgroConnect.Mobile.Models.Enums;

namespace AgroConnect.Mobile.Models;

// ═══ Auth ═══
public record LoginRequest(string UserName, string Password);
public record LoginResponse(string Token, string RefreshToken, UserInfo User);
public record UserInfo(string Id, string UserName, string FullName, string Email, string? Phone, string? Cuit, string Role);

// ═══ Recetas ═══
public record RecipeListItem(int Id, string Code, string Status, string? CropName, string? AdvisorName, DateTime ApplicationDate, DateTime? ExpirationDate, bool IsInformal, string? RiskLevel, string? MunicipalityName);
public record RecipeDetail(int Id, string Code, string Status, string? CropName, string? AdvisorName, DateTime ApplicationDate, DateTime? ExpirationDate, bool IsInformal, string? RiskLevel, double? Latitude, double? Longitude, List<RecipeProduct> Products, List<RecipeLot> Lots, ProducerInfo? Producer, ExecutionSummary? ActiveExecution);
public record RecipeProduct(string ProductName, string? ActiveIngredient, string? ToxicologicalClass, decimal? Dose, string? DoseUnit);
public record RecipeLot(int LotId, string LotName, double? AreaHa, List<GeoCoordinate>? Polygon);
public record ProducerInfo(string FullName, string? Cuit, string? Phone, string? Email);

// ═══ Ejecución ═══
public record ExecutionSummary(int Id, string Status, DateTime? StartDate, DateTime? EndDate, string? ApplicatorName);
public record ExecutionStartRequest(int RecipeId, double Latitude, double Longitude, double? Temperature, double? Humidity, double? WindSpeed, string? WindDirection);
public record ChecklistItem(string Key, string Label, bool IsChecked);

// ═══ Lotes ═══
public record LotListItem(int Id, string Code, string Name, double? AreaHa, int ActiveRecipeCount);
public record LotDetail(int Id, string Code, string Name, double? AreaHa, List<GeoCoordinate>? Polygon, List<RecipeListItem> Recipes);

// ═══ Bolsa de trabajo ═══
public record JobListItem(int Id, string RecipeCode, string? CropName, DateTime ApplicationDate, double? Latitude, double? Longitude, double? DistanceKm, string Status);

// ═══ Geo / Común ═══
public record GeoCoordinate(double Lat, double Lng);
public record ApiResponse<T>(bool Success, T? Data, string? Error);
public record WeatherData(double Temperature, double Humidity, double WindSpeed, string? WindDirection);
