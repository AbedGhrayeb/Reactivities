using System;
using Application.Activities;
using Application.Activities.Dtos;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class ActivitiesController : BaseController
    {

        [HttpGet]
        public async Task<ActionResult<List<ActivityDto>>> List()
        {
            return await Mediator.Send(new List.Query());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDto>> Details(Guid id)
        {
            return await Mediator.Send(new Details.Query { Id = id });
        }
        [HttpPost]
        public async Task<ActionResult<Unit>> Create(Create.Command command)
        {
            return await Mediator.Send(command);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Edit(Edit.Command command,Guid id)
        {
            command.Id=id;
            return await Mediator.Send(command);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Delete(Guid id)
        {
            
            return await Mediator.Send(new Delete.Command{Id=id});
        }
         [HttpPost("{id}/attend")]
        public async Task<ActionResult<Unit>> Attend(Guid id)
        {      
            return await Mediator.Send(new Attend.Command{Id=id});
        }
          [HttpDelete("{id}/attend")]
        public async Task<ActionResult<Unit>> UnAttend(Guid id)
        {      
            return await Mediator.Send(new Unattend.Command{Id=id});
        }
    }
}