using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NetNote.Api
{
    [ApiController]  
    public class JWTController : ControllerBase
    {
        [HttpGet]
        [Route("api/value1")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[]{"value","value1"};
        }

        [HttpGet]
       [Route("api/value2")]
        [Authorize]
        public ActionResult<IEnumerable<string>> Get2(){
            return new string[]{"value","value1"};
        } 
    }
}
