using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheets.Data;
using Timesheets.Models;

namespace Timesheets.Controllers
{
    public class Stats
    {
        public string ProjectName { get; set; }
        public double ProjectCost { get; set; }
    }

    [Route("Stats")]
    [ApiController]
    public class StatsController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<MyUser> _usermanager;
        
        public StatsController(ApplicationDbContext context, UserManager<MyUser> userManager)
        {
            this._context = context;
            this._usermanager = userManager;
        }

        [HttpGet]
        public ActionResult GetProjectsPerTime()
        {

            //var users = _context.Users.ToList();

            /*var results = from ts in _context.TimesheetEntries
                          join u in _context.Users on ts.RelatedUser equals u
                          group ts by ts.RelatedProject.Name into prj
                          select new
                          {
                              ProjectName = prj.Key,
                              TotalHours = prj.Sum(x => x.HoursWorked * x.RelatedUser.CostPerHour)
                          };
                          */

            List<Stats> results = new List<Stats>();

            string query = @"select p.Name as ProjectName, sum(ts.HoursWorked * u.CostPerHour) as ProjectCost
                            from TimesheetEntries ts
                            inner
                            join Projects p on ts.RelatedProjectId = p.Id
                            inner
                            join AspNetUsers u on ts.RelatedUserId = u.Id
                            group by p.Name";

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                _context.Database.OpenConnection();
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        results.Add(new Stats { ProjectName = reader.GetString(0), ProjectCost = reader.GetDouble(1) });
                    }
                }
            }


                return Json(results.ToList());

        }


    }
}
