using System;
using System.Windows.Forms;
using CP2077SaveEditor.Views;

namespace CP2077SaveEditor;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // Visual styles, text rendering and high DPI mode, configured in the project file
        ApplicationConfiguration.Initialize();
        Application.Run(new Form2());
    }
}
