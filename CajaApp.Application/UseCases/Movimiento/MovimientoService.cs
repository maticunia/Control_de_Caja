using CajaApp.Application.DTOs.Movimiento;
using CajaApp.Application.Interfaces;
using CajaApp.Domain.Entities;
using CajaApp.Domain.Enums;
using CajaApp.Domain.Interfaces;
using CajaApp.Domain.Shared;
using CajaApp.Domain.ValueObjects;

namespace CajaApp.Application.UseCases.Movimiento;

public class MovimientoService : IMovimientoService
{
    private readonly IUnitOfWork _unitOfWork;

    public MovimientoService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<MovimientoCajaDto>> GetByIdAsync(int id)
    {
        var result = await _unitOfWork.Movimientos.GetByIdAsync(id);
        if (!result.IsSuccess) return Result<MovimientoCajaDto>.Fail(result.Error!);
        
        var movimiento = result.Value;
        if (movimiento is null) return Result<MovimientoCajaDto>.Fail("MOVIMIENTO.NOT_FOUND", "Movimiento no encontrado");
        
        return await MapToDtoAsync(movimiento);
    }

    public async Task<Result<IReadOnlyList<MovimientoCajaDto>>> GetAllAsync()
    {
        var result = await _unitOfWork.Movimientos.GetAllAsync();
        if (!result.IsSuccess) return Result<IReadOnlyList<MovimientoCajaDto>>.Fail(result.Error!);
        
        var dtos = result.Value.Select(m => MapToDto(m)).ToList();
        return Result<IReadOnlyList<MovimientoCajaDto>>.Ok(dtos);
    }

    public async Task<Result<MovimientoCajaDto>> CreateAsync(CreateMovimientoDto dto)
    {
        // Validar que la caja existe y está abierta
        var cajaResult = await _unitOfWork.Cajas.GetByIdAsync(dto.CajaId);
        if (!cajaResult.IsSuccess) return Result<MovimientoCajaDto>.Fail(cajaResult.Error!);
        
        var caja = cajaResult.Value;
        if (caja is null) return Result<MovimientoCajaDto>.Fail("CAJA.NOT_FOUND", "Caja no encontrada");
        
        if (!caja.EstaAbierta)
            return Result<MovimientoCajaDto>.Fail("CAJA.CERRADA", "No se puede registrar movimientos en una caja cerrada");

        // Validar cliente si se proporciona
        if (dto.ClienteId.HasValue)
        {
            var clienteResult = await _unitOfWork.Clientes.GetByIdAsync(dto.ClienteId.Value);
            if (!clienteResult.IsSuccess) return Result<MovimientoCajaDto>.Fail(clienteResult.Error!);
            
            if (clienteResult.Value is null)
                return Result<MovimientoCajaDto>.Fail("CLIENTE.NOT_FOUND", "Cliente no encontrado");
        }

        try
        {
            var movimiento = new MovimientoCaja(
                dto.CajaId,
                dto.Tipo,
                new Monto(dto.Monto),
                dto.Descripcion,
                dto.Categoria,
                dto.ClienteId,
                dto.Referencia);

            var result = await _unitOfWork.Movimientos.AddAsync(movimiento);
            if (!result.IsSuccess) return Result<MovimientoCajaDto>.Fail(result.Error!);

            // Actualizar saldo de la caja
            var ajusteResult = caja.RegistrarMovimiento(dto.Tipo, movimiento.Monto);
            if (!ajusteResult.IsSuccess) return Result<MovimientoCajaDto>.Fail(ajusteResult.Error!);
            await _unitOfWork.Cajas.UpdateAsync(caja);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(result.Value);
        }
        catch (Exception ex)
        {
            return Result<MovimientoCajaDto>.Fail("MOVIMIENTO.CREATE_ERROR", ex.Message);
        }
    }

    public async Task<Result<MovimientoCajaDto>> UpdateAsync(UpdateMovimientoDto dto)
    {
        var result = await _unitOfWork.Movimientos.GetByIdAsync(dto.Id);
        if (!result.IsSuccess) return Result<MovimientoCajaDto>.Fail(result.Error!);
        
        var movimiento = result.Value;
        if (movimiento is null) return Result<MovimientoCajaDto>.Fail("MOVIMIENTO.NOT_FOUND", "Movimiento no encontrado");
        
        try
        {
            movimiento.Actualizar(dto.Descripcion, dto.Categoria, dto.Referencia);
            var updateResult = await _unitOfWork.Movimientos.UpdateAsync(movimiento);
            
            if (!updateResult.IsSuccess) return Result<MovimientoCajaDto>.Fail(updateResult.Error!);
            
            await _unitOfWork.SaveChangesAsync();
            return await MapToDtoAsync(updateResult.Value);
        }
        catch (Exception ex)
        {
            return Result<MovimientoCajaDto>.Fail("MOVIMIENTO.UPDATE_ERROR", ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var result = await _unitOfWork.Movimientos.GetByIdAsync(id);
        if (!result.IsSuccess) return Result.Fail(result.Error!);
        
        var movimiento = result.Value;
        if (movimiento is null) return Result.Fail("MOVIMIENTO.NOT_FOUND", "Movimiento no encontrado");
        
        var deleteResult = await _unitOfWork.Movimientos.GetByIdAsync(id);
        if (!deleteResult.IsSuccess) return Result.Fail(deleteResult.Error!);
        
        var movimiento = deleteResult.Value;
        if (movimiento is null) return Result.Fail("MOVIMIENTO.NOT_FOUND", "Movimiento no encontrado");
        
        // Revertir el saldo
        var cajaResult = await _unitOfWork.Cajas.GetByIdAsync(movimiento.CajaId);
        if (!cajaResult.IsSuccess) return Result.Fail(cajaResult.Error!);
        
        var caja = cajaResult.Value;
        if (caja is null) return Result.Fail("CAJA.NOT_FOUND", "Caja no encontrada");
        
        var tipoReverso = movimiento.Tipo == TipoMovimiento.Ingreso ? TipoMovimiento.Egreso : TipoMovimiento.Ingreso;
        var reversoResult = caja.RegistrarMovimiento(tipoReverso, movimiento.Monto);
        if (!reversoResult.IsSuccess) return Result.Fail(reversoResult.Error!);
        
        await _unitOfWork.Cajas.UpdateAsync(caja);
        await _unitOfWork.Movimientos.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<IReadOnlyList<MovimientoCajaDto>>> GetByCajaAsync(int cajaId)
    {
        var result = await _unitOfWork.Movimientos.GetByCajaAsync(cajaId);
        if (!result.IsSuccess) return Result<IReadOnlyList<MovimientoCajaDto>>.Fail(result.Error!);
        
        var dtos = result.Value.Select(m => MapToDto(m)).ToList();
        return Result<IReadOnlyList<MovimientoCajaDto>>.Ok(dtos);
    }

    public async Task<Result<IReadOnlyList<MovimientoCajaDto>>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var result = await _unitOfWork.Movimientos.GetByFechaAsync(fechaInicio, fechaFin);
        if (!result.IsSuccess) return Result<IReadOnlyList<MovimientoCajaDto>>.Fail(result.Error!);
        
        var dtos = result.Value.Select(m => MapToDto(m)).ToList();
        return Result<IReadOnlyList<MovimientoCajaDto>>.Ok(dtos);
    }

    public async Task<Result<IReadOnlyList<MovimientoCajaDto>>> GetByTipoAsync(TipoMovimiento tipo)
    {
        var result = await _unitOfWork.Movimientos.GetByTipoAsync(tipo);
        if (!result.IsSuccess) return Result<IReadOnlyList<MovimientoCajaDto>>.Fail(result.Error!);
        
        var dtos = result.Value.Select(m => MapToDto(m)).ToList();
        return Result<IReadOnlyList<MovimientoCajaDto>>.Ok(dtos);
    }

    public async Task<Result<decimal>> GetSaldoActualAsync(int cajaId)
    {
        var result = await _unitOfWork.Movimientos.GetSaldoActualAsync(cajaId);
        if (!result.IsSuccess) return Result<decimal>.Fail(result.Error!);
        
        return Result<decimal>.Ok(result.Value);
    }

    public async Task<Result<IReadOnlyList<MovimientoCajaDto>>> GetByClienteAsync(int clienteId)
    {
        var result = await _unitOfWork.Movimientos.GetByClienteAsync(clienteId);
        if (!result.IsSuccess) return Result<IReadOnlyList<MovimientoCajaDto>>.Fail(result.Error!);
        
        var dtos = result.Value.Select(m => MapToDto(m)).ToList();
        return Result<IReadOnlyList<MovimientoCajaDto>>.Ok(dtos);
    }

    private MovimientoCajaDto MapToDto(MovimientoCaja movimiento)
    {
        var cajaNombre = movimiento.Caja?.Nombre ?? "Desconocida";
        var clienteNombre = movimiento.Cliente?.NombreCompleto;
        
        return new MovimientoCajaDto(
            movimiento.Id,
            movimiento.CajaId,
            cajaNombre,
            movimiento.ClienteId,
            clienteNombre,
            movimiento.Tipo,
            (decimal)movimiento.Monto,
            movimiento.Descripcion,
            movimiento.Categoria,
            movimiento.Fecha,
            movimiento.Referencia);
    }
}
