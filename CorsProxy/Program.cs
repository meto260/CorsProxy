
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddCors();


var app = builder.Build();
app.UseCors(o => { o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/{weburl}", async (IHttpClientFactory fact, HttpContext ctx, string weburl) => {
    var httpClient = fact.CreateClient();
    foreach (var hd in ctx.Request.Headers) {
        httpClient.DefaultRequestHeaders.Add(hd.Key, hd.Value.ToString());
    }
    string url = Uri.UnescapeDataString(weburl);
    string response = "";
    try {
        response = await httpClient.GetStringAsync(url);
    }
    catch {
        httpClient.DefaultRequestHeaders.Clear();
        response = await httpClient.GetStringAsync(url);
    }
    return Results.Ok(JsonSerializer.Deserialize<object>(response));
});

app.MapPost("/{weburl}", async (IHttpClientFactory fact, HttpContext ctx, string weburl) => {
    var httpClient = fact.CreateClient();
    foreach (var hd in ctx.Request.Headers) {
        httpClient.DefaultRequestHeaders.Add(hd.Key, hd.Value.ToString());
    }
    string url = Uri.UnescapeDataString(weburl);
    var postval = await ctx.Request.ReadFromJsonAsync<object>();
    string resmsg = "";
    try {
        var response = await httpClient.PostAsJsonAsync(url, postval);
        resmsg = await response.Content.ReadAsStringAsync();
    }
    catch (Exception) {
        httpClient.DefaultRequestHeaders.Clear();
        var response = await httpClient.PostAsJsonAsync(url, postval);
        resmsg = await response.Content.ReadAsStringAsync();
    }
    return Results.Ok(JsonSerializer.Deserialize<object>(resmsg));
});

app.MapPut("/{weburl}", async (IHttpClientFactory fact, HttpContext ctx, string weburl) => {
    var httpClient = fact.CreateClient();
    foreach (var hd in ctx.Request.Headers) {
        httpClient.DefaultRequestHeaders.Add(hd.Key, hd.Value.ToString());
    }
    string url = Uri.UnescapeDataString(weburl);
    var postval = await ctx.Request.ReadFromJsonAsync<object>();
    string resmsg = "";
    try {
        var response = await httpClient.PutAsJsonAsync(url, postval);
        resmsg = await response.Content.ReadAsStringAsync();
    }
    catch (Exception) {
        httpClient.DefaultRequestHeaders.Clear();
        var response = await httpClient.PutAsJsonAsync(url, postval);
        resmsg = await response.Content.ReadAsStringAsync();
    }
    return Results.Ok(JsonSerializer.Deserialize<object>(resmsg));
});

app.MapDelete("/{weburl}", async (IHttpClientFactory fact, HttpContext ctx, string weburl) => {
    var httpClient = fact.CreateClient();
    foreach (var hd in ctx.Request.Headers) {
        httpClient.DefaultRequestHeaders.Add(hd.Key, hd.Value.ToString());
    }
    string url = Uri.UnescapeDataString(weburl);
    string resmsg = "";
    try {
        var response = await httpClient.DeleteFromJsonAsync<object>(url);
        resmsg = JsonSerializer.Serialize(response);
    }
    catch {
        httpClient.DefaultRequestHeaders.Clear();
        var response = await httpClient.DeleteFromJsonAsync<object>(url);
        resmsg = JsonSerializer.Serialize(response);
    }
    return Results.Ok(JsonSerializer.Deserialize<object>(resmsg));
});

app.Run();
