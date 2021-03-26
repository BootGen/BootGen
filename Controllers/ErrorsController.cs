using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; 
using Editor.Services;

namespace Editor.Controllers
{
    [ApiController]
    [Route("errors")]
    public class ErrorsController : ControllerBase
    {
        private IErrorService service;
        public ErrorsController(IErrorService service)
        {
            this.service = service;
        }

        [HttpPost]
        [Route("log")]
        public IActionResult Generate([FromBody] AppError error)
        {
            service.LogError(error);
            return Ok();
        }
    }
}
