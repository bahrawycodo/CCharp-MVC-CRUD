using AutoMapper;
using Demo.PL.Models;
using Microsoft.AspNetCore.Identity;

namespace Demo.PL.Mapper
{
    public class RoleProfile:Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleViewModel,IdentityRole>().ReverseMap();
        }
    }
}
