using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using NetNote.Aop;
using Autofac.Extras.DynamicProxy;

namespace NetNote.Models
{
    [Intercept(typeof(LogInterceptor))]
    public interface INoteRepository
    {
        Task<Note> GetByIdAsync(int id);
        Task<List<Note>> ListAsync();
        Task AddAsync(Note note);
        Task UpdateAsync(Note note);
        public Tuple<List<Note>, int> PageList(int pageindex, int pagesize);
    }
}
