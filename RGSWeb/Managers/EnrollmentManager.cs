using RGSWeb.Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace RGSWeb.Managers
{
    /// <summary>
    /// Manages all enrollment related actions - request, removal and update.
    /// </summary>
    public class EnrollmentManager
    {
        private ApplicationDbContext _db;

        /// <summary>
        /// Enrolls a new student into the class
        /// </summary>
        /// <param name="class">Class to enroll the student in</param>
        /// <param name="student">Student to enroll in a class</param>
        /// <returns>Null if the an enrollment has already been created. Otherwise, the
        /// enrollment that was just created.</returns>
        public async Task<Enrollment> RequestEnrollment(Class @class, ApplicationUser student)
        {
            var status = await (from enrollment in _db.Enrollments
                                where enrollment.Student.Id == student.Id && enrollment.Class.Id == @class.Id
                                select enrollment).FirstOrDefaultAsync();

            if(status != null)
            {
                return null;
            }

            // Create the enrollment as pending
            Enrollment newEnroll = new Enrollment { Class = @class, Student = student, Pending = true };
            _db.Enrollments.Add(newEnroll);
            await _db.SaveChangesAsync();

            return newEnroll;
        }

        /// <summary>
        /// Deletes a students enrollment in a class
        /// </summary>
        /// <param name="class">The class the student is enrolled in or has applied for enrollment</param>
        /// <param name="student">The student to unenroll from the class</param>
        /// <returns>Null if the student is not enrolled in class. Otherwise, returns the
        /// enrollment that was removed</returns>
        /// <remarks>This clears all data associated with the student in this class and
        /// cannot be undone</remarks>
        public async Task<Enrollment> DropStudent(Class @class, ApplicationUser student)
        {
            // Check that the student is actually enrolled
            var status = await _db.Enrollments.Where(e => e.Class.Id == @class.Id && e.Student.Id == student.Id).FirstOrDefaultAsync();
            if(status == null)
            {
                return null;
            }

            // Delete all student related data
            var scoreUnits = await _db.ScoreUnits.Where(sc => sc.Student.Id == student.Id).ToListAsync();
            _db.ScoreUnits.RemoveRange(scoreUnits);
            _db.Enrollments.Remove(status);
            await _db.SaveChangesAsync();

            return status;
        }

        /// <summary>
        /// Accepts or rejects a students enrollment into a class
        /// </summary>
        /// <param name="enrollment">The enrollment to update</param>
        /// <param name="state">True if the student should be accepted. Otherwise false.</param>
        /// <remarks>Deletes the enrollment if rejected</remarks>
        public async Task AcceptEnrollment(Enrollment enrollment, bool state)
        {
            if(!state)
            {
                _db.Enrollments.Remove(enrollment);
            }
            else
            {
                enrollment.Pending = false;
                _db.Entry(enrollment).State = EntityState.Modified;
            }

            await _db.SaveChangesAsync();
        }
    }
}