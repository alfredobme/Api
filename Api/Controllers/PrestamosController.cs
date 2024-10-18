using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrestamosController : ControllerBase
    {
        private readonly IConfiguration configurationSettings;
        private readonly string conexionSQL = "";
        string jsonString = "";

        public PrestamosController(IConfiguration configuration)
        {
            configurationSettings = configuration;
            conexionSQL = configurationSettings.GetConnectionString("ConexionSQL");
        }

        [HttpGet("consultar")]
        public string Get([FromHeader] int identificacion)
        {
            SqlConnection conexion = new SqlConnection(conexionSQL);
            string sqlString = "";

            sqlString =
            "SELECT  " +
                "Identificacion, " +
                "Monto, " +
                "Plazo, " +
                "FechaSolicitud " +
                "Estado " +
            "FROM " +
                "Prestamos " +
            "WHERE Identificacion = " + identificacion;

            SqlDataAdapter daPrestamos;
            DataTable dtPrestamos = new DataTable();
            try
            {
                daPrestamos = new SqlDataAdapter(sqlString, conexion);
                daPrestamos.Fill(dtPrestamos);
            }
            catch (Exception e)
            {
                jsonString = @"{ ""status"": ""Error"", ""prestamos"": [] }";
                return jsonString;
            }
            finally
            {
                conexion.Close();
            }

            jsonString = @"{ ""status"": ""Ok"", ""prestamos"": " + JsonConvert.SerializeObject(dtPrestamos) + " }";
            return jsonString;
        }

        [HttpPost("ingresar")]
        public string Ingresar([FromBody] Prestamo request)
        {
            SqlConnection conexion = new SqlConnection(conexionSQL);
            string sqlString = "";
            string jsonString = "";

            if (request.Identificacion <= 0 || request.Monto <= 0)
            {
                jsonString = @"{ ""status"": ""Error"", ""mensaje"": ""Verifique los datos ingresados"" }";
                return jsonString;
            }

            sqlString =
            "INSERT INTO Prestamos (Identificacion, Monto, Plazo, FechaSolicitud, Estado) " +
            "VALUES (@Identificacion, @Monto, @Plazo, @FechaSolicitud, 'Pendiente')";

            SqlCommand cmd = new(sqlString, conexion);
            cmd.Parameters.AddWithValue("@Identificacion", request.Identificacion);
            cmd.Parameters.AddWithValue("@Monto", request.Monto);
            cmd.Parameters.AddWithValue("@Plazo", request.Monto);
            cmd.Parameters.AddWithValue("@FechaSolicitud", request.FechaSolicitud);
            try
            {
                conexion.Open();
                int filasAfectadas = cmd.ExecuteNonQuery();

                if (filasAfectadas > 0)
                {
                    jsonString = @"{ ""status"": ""Ok"", ""mensaje"": ""Prestamo ingresado exitosamente"" }";
                }
                else
                {
                    jsonString = @"{ ""status"": ""Error"", ""mensaje"": ""Error al crear el prestamo"" }";
                }
            }
            catch (Exception e)
            {
                jsonString = @"{ ""status"": ""Error"", ""mensaje"": ""Error al conectar con la base de datos: " + e.Message + @""" }";
            }
            finally
            {
                conexion.Close();
            }

            return jsonString;
        }

        [HttpPost("modificar")]
        public string modificar([FromHeader] int idPrestamo, [FromHeader] string estado)
        {
            SqlConnection conexion = new SqlConnection(conexionSQL);
            string sqlString = "";
            string jsonString = "";

            if (idPrestamo <= 0 || estado == "")
            {
                jsonString = @"{ ""status"": ""Error"", ""mensaje"": ""Verifique los datos ingresados"" }";
                return jsonString;
            }

            sqlString =
            "UPDATE Prestamos " +
            "SET " +
                "Estado = '" + estado + "' " +
            "WHERE Id = " + idPrestamo;

            SqlCommand cmd = new(sqlString, conexion);
            try
            {
                conexion.Open();
                int filasAfectadas = cmd.ExecuteNonQuery();

                if (filasAfectadas > 0)
                {
                    jsonString = @"{ ""status"": ""Ok"", ""mensaje"": ""Prestamo modificado exitosamente"" }";
                }
                else
                {
                    jsonString = @"{ ""status"": ""OK"", ""mensaje"": ""Prestamo no modificado. Verifique datos"" }";
                }
            }
            catch (Exception e)
            {
                jsonString = @"{ ""status"": ""Error"", ""mensaje"": ""Error al conectar con la base de datos: " + e.Message + @""" }";
            }
            finally
            {
                conexion.Close();
            }

            return jsonString;
        }

    }
}
