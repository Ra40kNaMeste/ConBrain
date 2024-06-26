﻿using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers.ActionResults
{
    public class PeopleActionResult : IActionResult
    {
        public PeopleActionResult(int[] ignores, int size, string? pattern, IEnumerable<PersonData> people)
        {

            _people = people
                .Where(i => !ignores.Contains(i.Id));
            if(pattern != null && pattern != "")
                _people = _people.Where(i => i.Nick.Contains(pattern) || i.Name.Contains(pattern) || i.Family.Contains(pattern));
            _people = _people.Take(size);            
        }
        public PeopleActionResult(IEnumerable<PersonData> people)
        {
            _people = people;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsJsonAsync(_people);
        }

        private IEnumerable<PersonData> _people;
    }
}
