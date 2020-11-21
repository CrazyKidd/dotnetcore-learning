using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace NetNote.Models
{
    public class NoteTypeRepository:INoteTypeRepository
    {
        private NoteContext context;
        public NoteTypeRepository(NoteContext _context){
            context=_context;
        }

        public Task<List<NoteType>> ListAsync(){
            return context.NoteTypes.ToListAsync();
        }

    }
}
