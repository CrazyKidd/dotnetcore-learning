using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NetNote.Models
{
    public interface INoteTypeRepository
    {
        Task<List<NoteType>> ListAsync();
    }
}
