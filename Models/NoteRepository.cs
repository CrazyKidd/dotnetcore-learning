using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace NetNote.Models
{
    public class NoteRepository : INoteRepository
    {
        private NoteContext context;

        public NoteRepository(NoteContext _context)
        {
            context = _context;
        }

        public Task AddAsync(Note note)
        {
            //throw new NotImplementedException();
            context.Notes.Add(note);
            return context.SaveChangesAsync();
        }

        public Task<Note> GetByIdAsync(int id)
        {
            //throw new NotImplementedException();
            return context.Notes.Include(type => type.Type).FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task<List<Note>> ListAsync()
        {
            //throw new NotImplementedException();
            return context.Notes.Include(type => type.Type).ToListAsync();
        }

        public Tuple<List<Note>, int> PageList(int pageindex, int pagesize)
        {
            //throw new NotImplementedException();
            var query = context.Notes.Include(type => type.Type).AsQueryable();
            var count = query.Count();
            var pagecount = count % pagesize == 0 ? count / pagesize : count / pagesize + 1;
            var notes = query.OrderByDescending(r => r.Create).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
            return new Tuple<List<Note>, int>(notes, count);
        }

        public Task UpdateAsync(Note note)
        {
            //throw new NotImplementedException();
            context.Entry(note).State = EntityState.Modified;
            return context.SaveChangesAsync();
        }
    }
}
