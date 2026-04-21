using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Memora.Api.Models;
using Memora.Api.Data;

namespace Memora.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly DataContext _db;
        public DoctorsController(DataContext db)
        {
            _db = db;
        }

        // ADMIN - Ajouter un médecin
        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Add(Doctor doctor)
        {
            _db.Doctors.Add(doctor);
            _db.SaveChanges();
            return Ok(doctor);
        }

        // Lister tous les médecins
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            return Ok(_db.Doctors.ToList());
        }

        // ADMIN - Modifier un médecin
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Update(int id, Doctor doctor)
        {
            var dbDoc = _db.Doctors.Find(id);
            if (dbDoc == null) return NotFound();
            dbDoc.Name = doctor.Name;
            dbDoc.Specialty = doctor.Specialty;
            dbDoc.BookingUrl = doctor.BookingUrl;
            _db.SaveChanges();
            return Ok(dbDoc);
        }
    }
}