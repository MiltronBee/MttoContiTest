using Microsoft.Data.SqlClient;

var connectionString = "Server=localhost;Database=Vacaciones;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True";

Console.WriteLine("=== Fixing Area Names Encoding ===\n");

using (var conn = new SqlConnection(connectionString))
{
    conn.Open();

    var updates = new Dictionary<int, string>
    {
        { 4, "Preparación de Materiales" },
        { 5, "Construcción Llantas Radial" },
        { 7, "Acabado Reparación Inspección" },
        { 8, "Almacén Materias Primas" },
        { 9, "Mantenimiento Área I" },
        { 10, "Mantenimiento Área II" },
        { 11, "Mantenimiento Área III" },
        { 12, "Mantenimiento Eléctrico" },
        { 13, "Mantenimiento Eléctrico II" },
        { 14, "Mantenimiento Eléctrico III" },
        { 15, "Metrología" },
        { 17, "Aire Vapor Vacío Agua" },
        { 18, "Almacén de Producto Terminado" },
        { 19, "Construcción de Bladders" },
        { 20, "Instructores Técnicos" },
        { 24, "Vulcanización MX" }
    };

    foreach (var update in updates)
    {
        var cmd = new SqlCommand("UPDATE Areas SET NombreGeneral = @nombre WHERE AreaId = @id", conn);
        cmd.Parameters.AddWithValue("@id", update.Key);
        cmd.Parameters.AddWithValue("@nombre", update.Value);
        cmd.ExecuteNonQuery();
        Console.WriteLine($"Updated Area {update.Key}: {update.Value}");
    }

    Console.WriteLine("\n=== Area Names Fixed! ===");
}
