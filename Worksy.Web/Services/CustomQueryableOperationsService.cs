using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Worksy.Web.Core;
using Worksy.Web.Core.Pagination;
using Worksy.Web.Data;
using Worksy.Web.Data.Abstractions;

namespace Worksy.Web.Services;

public class CustomQueryableOperationsService
{
    private protected readonly DataContext _context;
    private protected readonly IMapper _mapper;

    public CustomQueryableOperationsService(DataContext context, IMapper mapper)
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

            //dto.Id = Id;

            return Response<TDTO>.Success(dto, "Registro creado exitosamente");
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
                .FirstOrDefaultAsync(s => s.Id == id);

            if (entity is null)
            {
                return Response<object>.Failure($"No existe el registro con id: {id}");
            }

            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();

            return Response<object>.Success("Registro eliminado exitosamente");
        }
        catch (Exception e)
        {
            return Response<object>.Failure(e);
        }
    }

    public async Task<Response<TDTO>> EditAsync<TEntity, TDTO>(TDTO dto, Guid id) where TEntity : IId
    {
        try
        {
            TEntity entity = _mapper.Map<TEntity>(dto);

            entity.Id = id;

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            //dto.Id = Id;

            return Response<TDTO>.Success(dto, "Registro actualizado exitosamente");
        }
        catch (Exception e)
        {
            return Response<TDTO>.Failure(e);
        }
    }

    public async Task<Response<TDTO>> GetOneAsync<TEntity, TDTO>(Guid id) where TEntity : class, IId
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

            List<TEntity> entity = await query.ToListAsync();
            List<TDTO> dto = _mapper.Map<List<TDTO>>(entity);

            return Response<List<TDTO>>.Success(dto);
        }
        catch (Exception e)
        {
            return Response<List<TDTO>>.Failure(e);
        }
    }

    public async Task<Response<PaginationResponse<TDTO>>> GetPaginationAsync<TEntity, TDTO>(PaginationRequest request,
        IQueryable<TEntity> query = null) where TEntity: class
        where TDTO: class
    {
        try
        {
            if (query is null)
            {
                query = _context.Set<TEntity>();
            }

            PagedList<TEntity> list = await PagedList<TEntity>.ToPagedListAsync(query, request);

            PaginationResponse<TDTO> response = new PaginationResponse<TDTO>
            {
                List = _mapper.Map<PagedList<TDTO>>(list),
                TotalCount = list.TotalCount,
                CurrentPage = list.CurrentPage,
                TotalPages = list.TotalPages,
                RecordsPerPage = list.RecordsPerPage,
                Filter = request.Filter
            };
            
            return Response<PaginationResponse<TDTO>>.Success(response);
        }
        catch (Exception e)
        {
            return Response<PaginationResponse<TDTO>>.Failure(e);
        }
    }
}