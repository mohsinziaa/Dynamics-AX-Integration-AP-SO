using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace ax.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        // Initialize the list of clients properly
        public List<itemInfo> itemsList { get; private set; } = new List<itemInfo>();

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            try
            {
                string connString = _configuration.GetConnectionString("DefaultConnection");

                await using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();
                    Console.WriteLine(" Connection Established Successfully!");  // Print success message

                    //CUSTTABLE
                    //INVENTTABLE
                    string sql = "SELECT ITEMID, ITEMNAME FROM INVENTTABLE WHERE DATAAREAID = 'mrp' AND DIMENSION2_ = '0600005'";

                    await using (SqlCommand command = new SqlCommand(sql, connection))
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        itemsList.Clear(); // Clear existing data before adding new items

                        while (await reader.ReadAsync())
                        {
                            var item = new itemInfo
                            {
                                itemNumber = reader["ITEMID"].ToString() ?? string.Empty,
                                itemName = reader["ITEMNAME"].ToString() ?? string.Empty
                            };

                            itemsList.Add(item);
                        }
                    }

                    Console.WriteLine($"📊 Retrieved {itemsList.Count} Items.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database Connection Failed! Error: {ex.Message}");
                _logger.LogError($"Database Connection Failed! Error: {ex.Message}");
            }
        }
    }

    public class itemInfo
    {
        public string itemNumber = string.Empty;
        public string itemName = string.Empty;
    }

}
