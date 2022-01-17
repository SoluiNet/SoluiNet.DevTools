// <copyright file="Program.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directives should be placed correctly
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SoluiNet.DevTools.UI.Blazor.Application;
#pragma warning restore SA1200 // Using directives should be placed correctly

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<SoluiNetBlazorApp>();
/* builder.Services.AddSingleton<WeatherForecastService>(); */

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();