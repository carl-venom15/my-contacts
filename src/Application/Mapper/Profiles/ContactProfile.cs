using Application.CQRS.Contacts.Commands;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapper.Profiles
{
    public class ContactProfile : Profile
    {
        public ContactProfile()
        {
            CreateMap<InsertContactCommand, Contact>();
        }
    }
}
