using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using VetClassLibrary.Services;

namespace Domain
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IEnumerable<T> GetAll() => _dbSet.ToList();

        public T? GetById(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null)
                throw new KeyNotFoundException("Entity not found.");
            return entity;
        }

        public void Create(T item) { 
            _dbSet.Add(item);
            _context.SaveChanges();
        }

        public void Update(T item)
        {
            _dbSet.Update(item);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            T? entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                _context.SaveChanges();
            }
            else
                throw new KeyNotFoundException("Entity not found.");
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task CreateAsync(T item)
        {
            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T item)
        {
            _dbSet.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
                throw new KeyNotFoundException("Entity not found.");
        }
    }
}