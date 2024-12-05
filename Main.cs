using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.Library;
using Wox.Plugin;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.PerplexitySearch
{
    /// <summary>
    /// Main class for Perplexity Search plugin.
    /// </summary>
    public class Main : IPlugin, IDisposable
    {
        /// <summary>
        /// ID of the plugin.
        /// </summary>
        public static string PluginID => "DIV53C974C2241878F282EA18A7769E4";

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public string Name => "Perplexity Search";

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        public string Description => "Search on Perplexity.ai";

        private PluginInitContext? Context { get; set; }

        private string? IconPath { get; set; }

        private bool Disposed { get; set; }

        /// <summary>
        /// Return a filtered list based on the given query.
        /// </summary>
        public List<Result> Query(Query query)
        {
            // If no search query, return empty list
            if (string.IsNullOrWhiteSpace(query.Search))
            {
                return [];
            }

            Log.Info($"Perplexity Search Query: {query.Search}", GetType());

            return [
                new()
                {
                    Title = "Search Perplexity",
                    SubTitle = query.Search,
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        // Encode the query for URL
                        string encodedQuery = Uri.EscapeDataString(query.Search);
                        string searchUrl = $"https://www.perplexity.ai/search?q={encodedQuery}";
                        
                        // Open default browser with search URL
                        try
                        {
                            Process.Start(new ProcessStartInfo(searchUrl) { UseShellExecute = true });
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Error opening Perplexity search: {ex.Message}", GetType());
                            return false;
                        }
                    }
                }
            ];
        }

        /// <summary>
        /// Initialize the plugin with the given context.
        /// </summary>
        public void Init(PluginInitContext context)
        {
            Log.Info("Init", GetType());

            Context = context ?? throw new ArgumentNullException(nameof(context));
            Context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(Context.API.GetCurrentTheme());
        }

        /// <summary>
        /// Dispose of the plugin resources.
        /// </summary>
        public void Dispose()
        {
            Log.Info("Dispose", GetType());

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Internal dispose method.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed || !disposing)
            {
                return;
            }

            if (Context?.API != null)
            {
                Context.API.ThemeChanged -= OnThemeChanged;
            }

            Disposed = true;
        }

        /// <summary>
        /// Update icon path based on current theme.
        /// </summary>
        private void UpdateIconPath(Theme theme) =>
            IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite
                ? Context?.CurrentPluginMetadata.IcoPathLight
                : Context?.CurrentPluginMetadata.IcoPathDark;

        /// <summary>
        /// Handle theme changes.
        /// </summary>
        private void OnThemeChanged(Theme currentTheme, Theme newTheme) =>
            UpdateIconPath(newTheme);
    }
}