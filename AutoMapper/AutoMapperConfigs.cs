using System;
using AutoMapper;
using NetNote.Models;
using NetNote.ViewModel;

namespace NetNote.AutoMapper
{
    public class AutoMapperConfigs : Profile
    {
        public AutoMapperConfigs()
        {
            CreateMap<RegisterViewModel, NoteUser>()
            .ForMember(d => d.Email, opt => { opt.MapFrom(s => s.Email); })
            .ForMember(d => d.UserName, opt => { opt.MapFrom(s => s.UserName); });

            CreateMap<NoteModel, Note>()
            //.ForMember(d => d.Title, opt => { opt.MapFrom(s => s.Title); })
            .ForMember(d => d.Create, opt => { opt.MapFrom(s => DateTime.Now); })
            //.ForMember(d => d.Content, opt => { opt.MapFrom(s => s.Content); })
            .ForMember(d => d.TypeId, opt => { opt.MapFrom(s => s.Type); })
            .ForMember(d => d.Attachment, opt => { opt.Ignore(); })
            .ForMember(d => d.Type, opt => { opt.Ignore(); }
            );

            //.ForMember(d => d.Password, opt => { opt.MapFrom(s => s.Password); }
        }
    }
}
