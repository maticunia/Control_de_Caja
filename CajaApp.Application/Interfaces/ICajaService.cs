using CajaApp.Application.DTOs.Caja;
using CajaApp.Domain.Shared;

namespace CajaApp.Application.Interfaces;

public interface ICajaService
{
    Task<Result<CajaDto>> GetByIdAsync(int id);
    Task<Result<IReadOnlyList<CajaDto>>> GetAllAsync();
    Task<Result<CajaDto>> CreateAsync(CreateCajaDto dto);
    Task<Result<CajaDto>> UpdateAsync(UpdateCajaDto dto);
    Task<Result> DeleteAsync(int id);
    Task<Result<CajaDto>> AbrirCajaAsync(int id, decimal saldoInicial);
    Task<Result<CajaDto>> CerrarCajaAsync(int id);
    Task<Result<CajaDto?>> GetCajaAbiertaAsync();
}
