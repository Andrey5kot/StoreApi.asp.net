﻿using Store.BLL.DTO;
using Store.BLL.Infrastructure;
using Store.BLL.Interfaces;
using Store.DAL.Entities;
using Store.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.BLL.Services
{
    public class UserService : IUserService
    {
        IUnitOfWork Database { get; set; }

        public UserService(IUnitOfWork uow)
        {
            Database = uow;
        }


        public async Task<OperationDetails> CreateAsync(UserDTO userDto)
        {
            User user = await Database.UserManager.FindByEmailAsync(userDto.Email);
            if (user == null)
            {

                user = new User { Email = userDto.Email, UserName = userDto.Email, FirstName = userDto.FirstName, LastName = userDto.LastName, PhoneNumber = userDto.PhoneNumber };
                var result = await Database.UserManager.CreateAsync(user, userDto.Password);
                if (result.Errors.Count() > 0)
                    return new OperationDetails(false, result.Errors.FirstOrDefault().ToString(), "");

                await Database.UserManager.AddToRoleAsync(user, userDto.Role);



                await Database.SaveAsync();
                return new OperationDetails(true, "Congratulations! Your account has been created.", "");
            }
            else
            {
                return new OperationDetails(false, "User with this login already exists", "Email");
            }
        }
        public async Task<bool> SignInAsync(UserDTO userDto)
        {

            var user = await Database.UserManager.FindByEmailAsync(userDto.Email);
            if (user == null)
            {
                return false;
            }
            //var username = user.UserName;
            var email = user.Email;

            var auth = await Database.SignInManager.PasswordSignInAsync(email, userDto.Password, false, lockoutOnFailure: false);

            return auth.Succeeded;
        }
        public async Task SignOutAsync()
        {
            await Database.SignInManager.SignOutAsync();
        }
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
