﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGSWeb.Models
{
    /// <summary>
    /// Alternative model for representing specific information from a ScoreUnit
    /// </summary>
    public class ScoreUnitBindingModel
    {
        /// <summary>
        /// Id of the score unit to update
        /// </summary>
        /// <remarks>Set to 0 if adding for the first time</remarks>
        public int Id { get; set; }
        /// <summary>
        /// Username of the student
        /// </summary>
        public string StudentUserName { get; set; }
        /// <summary>
        /// Id of the work item
        /// </summary>
        public int WorkItemId { get; set; }
        /// <summary>
        /// Student's grade
        /// </summary>
        public float? Grade { get; set; }

        /// <summary>
        /// Default constructor to create a ScoreUnitBindingModel
        /// </summary>
        public ScoreUnitBindingModel() { }

        /// <summary>
        /// Creates a model from the given ScoreUnit
        /// </summary>
        /// <param name="scoreUnit">ScoreUnit to create the model from</param>
        public ScoreUnitBindingModel(ScoreUnit scoreUnit)
        {
            Id = scoreUnit.Id;
            StudentUserName = scoreUnit.Student.UserName;
            WorkItemId = scoreUnit.WorkItem.Id;
            Grade = scoreUnit.Grade;
        }
    }
}