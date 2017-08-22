using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi_CRUD.Entity;
using WebApi_CRUD.Models;

namespace WebApi_CRUD.Controllers
{
    public class StudentController : ApiController
    {
        //GetMethods
        private SchoolEntities db = new SchoolEntities();
        public IHttpActionResult GetAllStudents()
        {
            var student = db.Students.Include("StudentAddress")
                .Select(s => new StudentViewModel
                {
                    Id = s.StudentID,
                    FirstName = s.FirstName,
                    LastName = s.LastName
                }).ToList();

            if (student.Count() == 0)
            {
                return NotFound();
            }
            return Ok(student);
        }
        public IHttpActionResult GetAllStudentsWithAddress()
        {
            IList<StudentViewModel> model = null;

            model = db.Students.Include("StudentAddress")
                .Select(s => new StudentViewModel
                {

                    Id = s.StudentID,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Address = s.StudentAddress == null ? null : new AddressViewModel()
                    {

                        Address1 = s.StudentAddress.Address1,
                        Address2 = s.StudentAddress.Address2,
                        City = s.StudentAddress.City,
                        State = s.StudentAddress.State
                    }


                }).ToList();

            if (model.Count() == null)
            {
                return NotFound();
            }
            return Ok(model);
        }
        public IHttpActionResult GetStudentById(int id)
        {
            StudentViewModel std = null;
            std = db.Students.Include("StudentAddress")
                .Where(s => s.StudentID == id)
               .Select(s => new StudentViewModel()
               {
                   Id = s.StudentID,
                   FirstName = s.FirstName,
                   LastName = s.LastName
               }).FirstOrDefault<StudentViewModel>();



            if (std == null)
            {
                return NotFound();
            }

            return Ok(std);
        }
        public IHttpActionResult GetAllStudents(string name)
        {
            IList<StudentViewModel> students = null;



            students = db.Students.Include("StudentAddress")
                .Where(s => s.FirstName.ToLower() == name.ToLower())
                .Select(s => new StudentViewModel()
                {
                    Id = s.StudentID,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Address = s.StudentAddress == null ? null : new AddressViewModel()
                    {
                        StudentId = s.StudentAddress.StudentID,
                        Address1 = s.StudentAddress.Address1,
                        Address2 = s.StudentAddress.Address2,
                        City = s.StudentAddress.City,
                        State = s.StudentAddress.State
                    }
                }).ToList<StudentViewModel>();



            if (students.Count == 0)
            {
                return NotFound();
            }

            return Ok(students);

        }
        public IHttpActionResult GetAllStudentsInSameStandard(int standardId)
        {
            IList<StudentViewModel> students = null;


            students = db.Students.Include("StudentAddress").Include("Standard").Where(s => s.StandardId == standardId)
                        .Select(s => new StudentViewModel()
                        {
                            Id = s.StudentID,
                            FirstName = s.FirstName,
                            LastName = s.LastName,
                            Address = s.StudentAddress == null ? null : new AddressViewModel()
                            {
                                StudentId = s.StudentAddress.StudentID,
                                Address1 = s.StudentAddress.Address1,
                                Address2 = s.StudentAddress.Address2,
                                City = s.StudentAddress.City,
                                State = s.StudentAddress.State
                            },
                            Standard = new StandardViewModel()
                            {
                                StandardId = s.Standard.StandardId,
                                Name = s.Standard.StandardName
                            }
                        }).ToList<StudentViewModel>();



            if (students.Count == 0)
            {
                return NotFound();
            }

            return Ok(students);
        }

        //PostMethods

        public IHttpActionResult Post(StudentViewModel model)
        {

            if (ModelState.IsValid)
            {
                db.Students.Add(new Student
                {
                    StudentID = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                });
                db.SaveChanges();
            }
            else
            {
                return BadRequest("InvalidData");
            }
            return Ok();
        }

        //Put Method

        public IHttpActionResult Put(StudentViewModel model)
        {

            var existingStudent = db.Students.Where(s => s.StudentID == model.Id).FirstOrDefault();
            if (existingStudent != null)
            {
                existingStudent.FirstName = model.FirstName;
                existingStudent.LastName = model.LastName;
                db.SaveChanges();
            }
            else { NotFound(); }
            return Ok();
        }

        //Delete
        public IHttpActionResult Delete(int id)
        {
            if (id <= 0) { return BadRequest("Not a valid student id"); }



            var student = db.Students
                .Where(s => s.StudentID == id)
                .FirstOrDefault();

            db.Entry(student).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();


            return Ok();


        }
    }
}
