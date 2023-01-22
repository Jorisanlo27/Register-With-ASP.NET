using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Registro.Models;
using System.Data;

namespace Registro.Controllers
{
    public class UsuariosController : Controller
    {
        public IActionResult Index()
        {
            using (SqlConnection con = new(Configuration["ConnectionStrings:conexion"]))
            {
                using (SqlCommand cmd = new("sp_usuarios", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter da = new(cmd);
                    DataTable dt = new();
                    da.Fill(dt);
                    da.Dispose();
                    List<UsuarioModel> lista = new();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        lista.Add(new UsuarioModel()
                        {
                            id = Convert.ToInt32(dt.Rows[i][0]),
                            nombre = (dt.Rows[i][1]).ToString(),
                            edad = Convert.ToInt32(dt.Rows[i][2]),
                            correo = (dt.Rows[i][3]).ToString()
                        });
                    }
                    ViewBag.Usuarios = lista;
                    con.Close();
                }
            }
            return View();
        }

        public IConfiguration Configuration { get; }
        public UsuariosController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(UsuarioModel usuario)
        {
            using (SqlConnection con = new(Configuration["ConnectionStrings:conexion"]))
            {
                using (SqlCommand cmd = new("sp_registrar", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar).Value = usuario.nombre;
                    cmd.Parameters.Add("edad", System.Data.SqlDbType.Int).Value = usuario.edad;
                    cmd.Parameters.Add("correo", System.Data.SqlDbType.VarChar).Value = usuario.correo;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return Redirect("Index");
        }
    }
}
