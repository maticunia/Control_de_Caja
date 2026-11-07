namespace CajaApp.Application.DTOs.Caja;

public record CreateCajaDto(
    string Nombre,
    string Descripcion,
    decimal SaldoInicial);
