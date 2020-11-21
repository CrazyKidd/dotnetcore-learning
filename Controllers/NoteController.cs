using System;
using Microsoft.AspNetCore.Mvc;
using NetNote.Models;
using NetNote.ViewModel;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using System.Reflection;
using Castle.DynamicProxy;
using NetNote.Aop;
using NetNote.Utils;
using System.Collections.Generic;

namespace NetNote.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private INoteRepository _noteRepository;

        private readonly IMapper _mapper;
        private INoteTypeRepository _noteTypeRepository;

        private IResdisClient _resdisClient;
        public NoteController(INoteRepository noteRepository, INoteTypeRepository noteTypeRepository, IMapper iMapper, IResdisClient resdisClient)
        {
            _noteRepository = noteRepository;
            _noteTypeRepository = noteTypeRepository;
            _mapper = iMapper;
            _resdisClient = resdisClient;
        }

        public IActionResult Index(int pageindex = 1)
        {
            var pagesize = 10;
            var notes = _noteRepository.PageList(pageindex, pagesize);
            notes.Item1.ForEach(note =>
            {
                if (!string.IsNullOrEmpty(note.Attachment))
                {
                    note.Attachment = new StringBuilder().Append(HttpContext.Request.Scheme)
                    .Append("://")
                    .Append(HttpContext.Request.Host)
                    .Append("/")
                    .Append(note.Attachment)
                    .ToString();
                }
            });
            ViewBag.PageCount = notes.Item2;
            ViewBag.PageIndex = pageindex;
            //var note = await _noteRepository.ListAsync();
            return View(notes.Item1);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromServices] IWebHostEnvironment env, NoteModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string filename = string.Empty;
            if (model.Attachment != null)
            {
                filename = new StringBuilder()
                .Append("file")
                .Append("/")
                .Append(Guid.NewGuid().ToString() + Path.GetExtension(model.Attachment.FileName))
                .ToString();
                using (var stream = new FileStream(Path.Combine(env.WebRootPath, filename), FileMode.CreateNew))
                {
                    model.Attachment.CopyTo(stream);
                }
            }

            // await _noteRepository.AddAsync(new Note
            // {
            //     Title = model.Title,
            //     Content = model.Content,
            //     Create = DateTime.Now,
            //     TypeId = model.Type,
            //     Password = model.Password,
            //     Attachment = filename
            // });

            //1. 创建代理对象
            //INoteRepository  DispatchProxy.Create<INoteRepository,MyIntercept>;
            ProxyGenerator generator = new ProxyGenerator();
            //var noteRepository = generator.CreateClassProxy<NoteRepository>(new MyIntercept());
            var noteentiy = _mapper.Map<Note>(model);
            noteentiy.Attachment = filename;

            await _noteRepository.AddAsync(noteentiy);

            await _resdisClient.SetAsync(noteentiy.Id.ToString(), noteentiy);
            //await noteRepository.AddAsync(noteentiy);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            List<NoteType> types = await _resdisClient.GetAsync<List<NoteType>>("notelist");
            if (types == null)
            {
                types = await _noteTypeRepository.ListAsync();
                await _resdisClient.SetAsync("notelist", types);
            }

            ViewBag.Types = types.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.id.ToString()
            });
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            var note = await _resdisClient.GetAsync<Note>(id.ToString());
            //var note = await _noteRepository.GetByIdAsync(id);
            if (note == null)
            {
                note = await _noteRepository.GetByIdAsync(id);
                await _resdisClient.SetAsync(id.ToString(), note);
            }

            if (!string.IsNullOrEmpty(note.Password))
            {
                return View();
            }
            return View(note);
        }

        [HttpPost]
        public async Task<IActionResult> Detail(int id, string Password)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            //note.Attachment = Path.Combine(HttpContext.Request.Host.ToString(), note.Attachment);
            note.Attachment = new StringBuilder().Append(HttpContext.Request.Scheme)
            .Append("://")
            .Append(HttpContext.Request.Host)
            .Append("/")
            .Append(note.Attachment)
            .ToString();
            if (!note.Password.Equals(Password))
                return BadRequest("密码错误，返回重新输入");
            return View(note);
        }
    }
}
