using CajaApp.Application.DTOs.Caja;
using CajaApp.Application.Interfaces;
using CajaApp.Domain.Entities;
using CajaApp.Domain.Interfaces;
using CajaApp.Domain.Shared;
using CajaApp.Domain.ValueObjects;

namespace CajaApp.Application.UseCases.Caja;

public class CajaService : ICajaService
{
    private readonly IUnitOfWork _unitOfWork;

    public CajaService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<CajaDto>> GetByIdAsync(int id)
    {
        var result = await _unitOfWork.Cajas.GetByIdAsync(id);
        if (!result.IsSuccess) return Result<CajaDto>.Fail(result.Error!);
        
        var caja = result.Value;
        if (caja is null) return Result<CajaDto>.Fail("CAJA.NOT_FOUND", "Caja no encontrada");
        
        return MapToDto(caja);
    }

    public async Task<Result<IReadOnlyList<CajaDto>>> GetAllAsync()
    {
        var result = await _unitOfWork.Cajas.GetAllAsync();
        if (!result.IsSuccess) return Result<IReadOnlyList<CajaDto>>.Fail(result.Error!);
        
        return Result<IReadOnlyList<CajaDto>>.Ok(result.Value.Select(MapToDto).ToList());
    }

    public async Task<Result<CajaDto>> CreateAsync(CreateCajaDto dto)
    {
        try
        {
            var caja = new Caja(dto.Nombre, dto.Descripcion, new Monto(dto.SaldoInicial));
            var result = await _unitOfWork.Cajas.AddAsync(caja);
            
            if (!result.IsSuccess) return Result<CajaDto>.Fail(result.Error!);
            
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(result.Value);
        }
        catch (Exception ex)
        {
            return Result<CajaDto>.Fail("CAJA.CREATE_ERROR", ex.Message);
        }
    }

    public async Task<Result<CajaDto>> UpdateAsync(UpdateCajaDto dto)
    {
        var result = await _unitOfWork.Cajas.GetByIdAsync(dto.Id);
        if (!result.IsSuccess) return Result<CajaDto>.Fail(result.Error!);
        
        var caja = result.Value;
        if (caja is null) return Result<CajaDto>.Fail("CAJA.NOT_FOUND", "Caja no encontrada");
        
        try
        {
            caja.Actualizar(dto.Nombre, dto.Descripcion);
            var updateResult = await _unitOfWork.Cajas.UpdateAsync(caja);
            
            if (!updateResult.IsSuccess) return Result<CajaDto>.Fail(updateResult.Error!);
            
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(updateResult.Value);
        }
        catch (Exception ex)
        {
            return Result<CajaDto>.Fail("CAJA.UPDATE_ERROR", ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var result = await _unitOfWork.Cajas.GetByIdAsync(id);
        if (!result.IsSuccess) return Result.Fail(result.Error!);
        
        var caja = result.Value;
        if (caja is null) return Result.Fail("CAJA.NOT_FOUND", "Caja no encontrada");
        
        if (caja.EstaAbierta)
            return Result.Fail("CAJA.CANNOT_DELETE_OPEN", "No se puede eliminar una caja abierta");
        
        var deleteResult = await _unitOfWork.Cajas.DeleteAsync(id);
        if (!deleteResult.IsSuccess) return Result.Fail(deleteResult.Error!);
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<CajaDto>> AbrirCajaAsync(int id, decimal saldoInicial)
    {
        var result = await _unitOfWork.Cajas.GetByIdAsync(id);
        if (!result.IsSuccess) return Result<CajaDto>.Fail(result.Error!);
        
        var caja = result.Value;
        if (caja is null) return Result<CajaDto>.Fail("CAJA.NOT_FOUND", "Caja no encontrada");
        
        var openResult = caja.Abrir(new Monto(saldoInicial));
        if (!openResult.IsSuccess) return Result<CajaDto>.Fail(openResult.Error!);
        
        if (saldoInicial > 0)
        {
            var movimientoApertura = new MovimientoCaja(
                caja.Id,
                TipoMovimiento.Ingreso,
                new Monto(saldoInicial),
                "Apertura de caja - Saldo inicial",
                "Apertura",
                null,
                $"APE-{caja.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}");

            await _unitOfWork.Movimientos.AddAsync(movimientoApertura);
        }
        
        var updateResult = await _unitOfWork.Cajas.UpdateAsync(caja);
        if (!updateResult.IsSuccess) return Result<CajaDto>.Fail(updateResult.Error!);
        
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(updateResult.Value);
    }

    public async Task<Result<CajaDto>> CerrarCajaAsync(int id)
    {
        var result = await _unitOfWork.Cajas.GetByIdAsync(id);
        if (!result.IsSuccess) return Result<CajaDto>.Fail(result.Error!);
        
        var caja = result.Value;
        if (caja is null) return Result<CajaDto>.Fail("CAJA.NOT_FOUND", "Caja no encontrada");
        
        var closeResult = caja.Cerrar();
        if (!closeResult.IsSuccess) return Result<CajaDto>.Fail(closeResult.Error!);
        
        var updateResult = await _unitOfWork.Cajas.UpdateAsync(caja);
        if (!updateResult.IsSuccess) return Result<CajaDto>.Fail(updateResult.Error!);
        
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(updateResult.Value);
    }

    public async Task<Result<CajaDto?>> GetCajaAbiertaAsync()
    {
        var result = await _unitOfWork.Cajas.GetCajaAbiertaAsync();
        if (!result.IsSuccess) return Result<CajaDto?>.Fail(result.Error!);
        
        var caja = result.Value;
        return Result<CajaDto?>.Ok(caja is null ? null : MapToDto(caja));
    }

    private static CajaDto MapToDto(Caja caja) => new CajaDto(
        caja.Id,
        caja.Nombre,
        caja.Descripcion,
        (decimal)caja.SaldoActual,
        caja.FechaApertura,
        caja.FechaCierre,
        caja.Estado,
        caja.FechaCreacion);
}
