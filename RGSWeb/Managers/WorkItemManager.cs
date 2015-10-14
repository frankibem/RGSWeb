using RGSWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
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
        /// Returns all WorkItems that have been assigned in a class
        /// </summary>
        /// <param name="class">Class to return WorkItems for</param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkItem>> GetClasWorkItems(Class @class)
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


    }
}