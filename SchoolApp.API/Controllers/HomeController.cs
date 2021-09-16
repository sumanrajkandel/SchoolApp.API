using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolApp.API.Contracts;
using SchoolApp.API.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApp.API.Controllers
{
    //  [Authorize(Roles = UserRoles.Manager + "," + UserRoles.Student)]
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IClientConfiguration _clientConfiguration;
        public HomeController(IClientConfiguration clientConfiguration)
        {
            _clientConfiguration = clientConfiguration;
        }

        //  [Authorize(Roles = UserRoles.Student)]
        [HttpGet("Student")]
        public IActionResult GetStudent()
        {
            string ClientInfo = "Welcome to HomeController Student " +

                _clientConfiguration.ClientName + " " + _clientConfiguration.ClientName;

            if ("A" == "a")
            {
                throw new InvalidSchoolAPIException("A is not equals to a."); //NotImplementedException();                
            }


            return Ok(ClientInfo/*"Welcome to HomeController Student"*/);
        }

        //  [Authorize(Roles = UserRoles.Manager)]
        [HttpGet("Manager")]
        public IActionResult GetManager()
        {
            return Ok("Welcome to HomeController Manager");
        }

    }
}
