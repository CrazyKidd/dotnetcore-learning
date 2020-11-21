using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NetNote.Models
{
    public interface INoteRepository
    {
        Task<Note> GetByIdAsync(int id);
        Task<List<Note>> ListAsync();
        Task AddAsync(Note note);
        Task UpdateAsync(Note note);
        public Tuple<List<Note>,int> PageList(int pageindex,int pagesize);
    }
}
