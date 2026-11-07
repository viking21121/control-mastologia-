using ControlMastologia.Web.Models;
using ControlMastologiaV2;
using Microsoft.AspNetCore.Mvc;
using Supabase;

namespace ControlMastologia.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacientesController : ControllerBase
    {
        private readonly Supabase.Client _supabase;

        public PacientesController(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        [HttpGet("test-nube")]
        public async Task<IActionResult> TestNube()
        {
            // Intentamos traer datos de tu tabla 'pacientes' en Supabase
            var response = await _supabase.From<PacienteNube>().Get();
            return Ok(response.Models);
        }
    }
}