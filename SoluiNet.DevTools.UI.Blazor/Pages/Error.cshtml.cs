// <copyright file="Error.cshtml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UI.Blazor.Pages
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    /// <summary>
    /// The error page model.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name should match first type name",
        Justification = "In Blazor it isn't necessary to match the file names")]
    public class ErrorModel : PageModel
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ErrorModel> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorModel"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ErrorModel(ILogger<ErrorModel> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Gets or sets the request ID.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Gets a value indicating whether the request ID should be shown.
        /// </summary>
        public bool ShowRequestId
        {
            get
            {
                return !string.IsNullOrEmpty(this.RequestId);
            }
        }

        /// <summary>
        /// The GET event handler.
        /// </summary>
        public void OnGet()
        {
            this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
        }
    }
}