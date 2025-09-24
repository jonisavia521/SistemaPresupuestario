using System;
using System.Windows.Forms;
using SistemaPresupuestario.UI.Forms;

namespace SistemaPresupuestario.UI
{
    /// <summary>
    /// Main entry point for the Sistema Presupuestario application
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                // Initialize database if needed
                using (var context = new DAL.Context.SistemaPresupuestarioContext())
                {
                    // This will trigger database creation and seeding if database doesn't exist
                    context.Database.Initialize(force: false);
                }

                // Start the main users form
                Application.Run(new FrmUsuarios());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error starting application: {ex.Message}", 
                    "Sistema Presupuestario - Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }
    }
}