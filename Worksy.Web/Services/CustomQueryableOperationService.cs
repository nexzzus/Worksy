using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Worksy.Web.Core;
using Worksy.Web.Data;
using Worksy.Web.Data.Abstractions;

namespace Worksy.Web.Services;

public class CustomQueryableOperationService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public CustomQueryableOperationService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response<TDTO>> CreateAsync<TEntity, TDTO>(TDTO dto) where TEntity : IId
    {
        try
        {
            TEntity entity = _mapper.Map<TEntity>(dto);

            Guid Id = Guid.NewGuid();
            entity.Id = Id;

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            return Response<TDTO>.Success(dto, "Registro creado exitosamente.");
        }
        catch (Exception e)
        {
            return Response<TDTO>.Failure(e);
        }
    }

    public async Task<Response<object>> DeleteAsync<TEntity>(Guid id) where TEntity : class, IId
    {
        try
        {
            TEntity? entity = await _context.Set<TEntity>()
                .FirstOrDefaultAsync(e => e.Id == id);
            if (entity is null)
            {
                return Response<object>.Failure($"No se encontró el registro con Id: {id}");
            }

            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();

            return Response<object>.Success("Registro eliminado exitosamente.");
        }
        catch (Exception e)
        {
            return Response<object>.Failure(e);
        }
    }

    public async Task<Response<TDTO>> UpdateAsync<TEntity, TDTO>(TDTO dto, Guid id) where TEntity : class, IId
    {
        try
        {
            TEntity entity = _mapper.Map<TEntity>(dto);

            entity.Id = id;

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Response<TDTO>.Success(dto, "Registro actualizado exitosamente.");
        }
        catch (Exception e)
        {
            return Response<TDTO>.Failure(e);
        }
    }

    public async Task<Response<TDTO>> GetByIdAsync<TEntity, TDTO>(Guid id) where TEntity : class, IId
    {
        try
        {
            TEntity? entity = await _context.Set<TEntity>()
                .FirstOrDefaultAsync(s => s.Id == id);
            if (entity is null)
            {
                return Response<TDTO>.Failure($"No existe registro con id: {id}");
            }

            TDTO dto = _mapper.Map<TDTO>(entity);

            return Response<TDTO>.Success(dto, "Registro obtenido exitosamente");
        }
        catch (Exception e)
        {
            return Response<TDTO>.Failure(e);
        }
    }

    public async Task<Response<List<TDTO>>> GetAllAsync<TEntity, TDTO>(IQueryable<TEntity> query = null)
        where TEntity : class, IId
    {
        try
        {
            if (query is null)
            {
                query = _context.Set<TEntity>();
            }
            List<TEntity> entities = await query.ToListAsync();
            List<TDTO> dtos = _mapper.Map<List<TDTO>>(entities);
            
            return Response<List<TDTO>>.Success(dtos);
        }
        catch (Exception e)
        {
            return Response<List<TDTO>>.Failure(e);
        }
    }
}