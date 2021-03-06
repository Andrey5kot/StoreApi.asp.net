﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.BLL.DTO;
using Store.BLL.Infrastructure;
using Store.BLL.Interfaces;
using Store.DAL.Entities;
using StoreAPI.asp.net.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Store.DAL.EF;

namespace StoreAPI.asp.net.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public AccountController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = _mapper.Map<RegisterViewModel, UserDTO>(model);
            var result = await _userService.CreateAsync(user);
            return Ok(result);


        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loginModel = _mapper.Map<LoginViewModel, UserDTO>(model);
            var identity = await _userService.SignInAsync(loginModel);



            return Ok(identity);
        }

        [HttpGet]
        [Route("getuserbytoken")]
        public async Task<ActionResult<UserDTO>> GetUserByToken()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string Token = Request.Headers["token"];    
            var res = await _userService.GetUserFromAccessToken(Token);
            if (res == "Exception")
                return BadRequest();
            return Ok(res);
        }
    }
}