using Microsoft.AspNet.Identity.EntityFramework;
using RGSWeb.Models;
using RGSWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace RGSWeb.Managers
{
    /// <summary>
    /// Manages all Work-Item related actions - creation, update, deletion ...
    /// </summary>
    public class WorkItemManager
    {
        private ApplicationDbContext _db;
        private ApplicationUserManager _userManager;

        /// <summary>
        /// The Database context to use for queries
        /// </summary>
        public ApplicationDbContext Db
        {
            get { return _db; }
            set { _db = value; }
        }

        /// <summary>
        /// The UserManager to use for queries
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get { return _userManager; }
            set { _userManager = value; }
        }

        /// <summary>
        /// Creates a new WorkItemManager with the given Database context
        /// </summary>
        /// <param name="db">The Database context to use</param>
        public WorkItemManager(ApplicationDbContext db)
        {
            Db = db;
            UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
        }

        /// <summary>
        /// Creates a new WorkItemManager with the given Database context and UserManager
        /// </summary>
        /// <param name="db">The Database context to use</param>
        /// <param name="userManager">The UserManager to use</param>
        /// <remarks>The UserManager must have been created from db</remarks>
        public WorkItemManager(ApplicationDbContext db, ApplicationUserManager userManager)
        {
            Db = db;
            UserManager = userManager;
        }

        /// <summary>
        /// Creates a new WorkItem from the given model
        /// </summary>
        /// <param name="cwvm">Model containing the detail of the WorkItem to create</param>
        /// <returns>Null if the class or teacher is not found, Otherwise, returns the WorkItem created</returns>
        public async Task<WorkItem> CreateWorkItem(CreateWorkItemViewModel cwvm)
        {
            var @class = await Db.Classes.FindAsync(cwvm.ClassId);

            if(@class == null)
            {
                return null;
            }

            WorkItem workItem = new WorkItem
            {
                Title = cwvm.Title,
                Description = cwvm.Description,
                DueDate = cwvm.DueDate,
                MaxPoints = cwvm.MaxPoints,
                Type = cwvm.Type,
                Class = @class
            };

            Db.WorkItems.Add(workItem);
            await Db.SaveChangesAsync();
            return workItem;
        }

        /// <summary>
        /// Updates a WorkItem using details in the given model
        /// </summary>
        /// <param name="uwvm">Model containing details of the WorkItem to update</param>
        public async Task UpdateWorkItem(UpdateWorkItemViewModel uwvm)
        {
            // Find the WorkItem and update its properties
            var workItem = await Db.WorkItems.FindAsync(uwvm.Id);
            if(workItem == null)
            {
                throw new Exception("No WorkItem with id: " + uwvm.Id);
            }

            // Update properties
            workItem.Title = uwvm.Title;
            workItem.Description = uwvm.Description;
            workItem.DueDate = uwvm.DueDate;
            workItem.MaxPoints = uwvm.MaxPoints;
            workItem.Type = uwvm.Type;
            _db.Entry(workItem).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!WorkItemExists(workItem.Id))
                {
                    throw new Exception("No workitem with id: " + workItem.Id);
                }
            }
        }

        /// <summary>
        /// Returns all WorkItems that have been assigned in a class
        /// </summary>
        /// <param name="class">Class to return WorkItems for</param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkItem>> GetClassWorkItems(Class @class)
        {
            if(@class == null)
            {
                return null;
            }

            return await Db.WorkItems.Where(e => e.Class.Id == @class.Id).ToListAsync();
        }

        /// <summary>
        /// Returns the WorkItem with the given id.
        /// </summary>
        /// <param name="id">Id of the WorkItem to find</param>
        /// <returns>Null if the WorkItem is not found</returns>
        public async Task<WorkItem> GetWorkItemById(int id)
        {
            return await Db.WorkItems.FindAsync(id);
        }

        /// <summary>
        /// Deletes the WorkItem with the given id
        /// </summary>
        /// <param name="id">Id of the WorkItem to delete</param>
        /// <returns>Null if the WorkItem could not be found. Otherwise,
        /// returns the deleted WorkItem</returns>
        /// <remarks>Delete all data associated with the WorkItem i.e. scoreunits</remarks>
        public async Task<WorkItem> DeleteWorkItemById(int id)
        {
            var workItem = await Db.WorkItems.FindAsync(id);
            if(workItem == null)
            {
                return null;
            }

            // Remove all associated data
            var scoreUnits = Db.ScoreUnits.Where(sc => sc.WorkItem.Id == workItem.Id);
            Db.ScoreUnits.RemoveRange(scoreUnits);
            Db.WorkItems.Remove(workItem);
            await Db.SaveChangesAsync();

            return workItem;
        }
        /// <summary>
        /// Returns true if a WorkItem with the given id exists
        /// and false otherwise
        /// </summary>
        /// <param name="id">The id of the WorkItem to search for</param>
        /// <returns></returns>
        public bool WorkItemExists(int id)
        {
            return Db.WorkItems.Count(e => e.Id == id) > 0;
        }
    }
}