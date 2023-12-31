
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
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", ()=> { return Results.Ok(new { result = "success" }); });
app.MapGet("/p", async (IHttpClientFactory fact, HttpContext ctx, [FromQuery] string weburl) => {
    var httpClient = fact.CreateClient();
    foreach (var hd in ctx.Request.Headers) {
        httpClient.DefaultRequestHeaders.Add(hd.Key, hd.Value.ToString());
    }
    string response = "";
    try {
        response = await httpClient.GetStringAsync(weburl);
    }
    catch {
        httpClient.DefaultRequestHeaders.Clear();
        response = await httpClient.GetStringAsync(weburl);
    }
    return Results.Ok(JsonSerializer.Deserialize<object>(response));
});

app.MapPost("/p", async (IHttpClientFactory fact, HttpContext ctx, [FromQuery] string weburl) => {
    var httpClient = fact.CreateClient();
    foreach (var hd in ctx.Request.Headers) {
        httpClient.DefaultRequestHeaders.Add(hd.Key, hd.Value.ToString());
    }
    var postval = await ctx.Request.ReadFromJsonAsync<object>();
    string resmsg = "";
    try {
        var response = await httpClient.PostAsJsonAsync(weburl, postval);
        resmsg = await response.Content.ReadAsStringAsync();
    }
    catch (Exception) {
        httpClient.DefaultRequestHeaders.Clear();
        var response = await httpClient.PostAsJsonAsync(weburl, postval);
        resmsg = await response.Content.ReadAsStringAsync();
    }
    return Results.Ok(JsonSerializer.Deserialize<object>(resmsg));
});

app.MapPut("/p", async (IHttpClientFactory fact, HttpContext ctx, [FromQuery] string weburl) => {
    var httpClient = fact.CreateClient();
    foreach (var hd in ctx.Request.Headers) {
        httpClient.DefaultRequestHeaders.Add(hd.Key, hd.Value.ToString());
    }
    var postval = await ctx.Request.ReadFromJsonAsync<object>();
    string resmsg = "";
    try {
        var response = await httpClient.PutAsJsonAsync(weburl, postval);
        resmsg = await response.Content.ReadAsStringAsync();
    }
    catch (Exception) {
        httpClient.DefaultRequestHeaders.Clear();
        var response = await httpClient.PutAsJsonAsync(weburl, postval);
        resmsg = await response.Content.ReadAsStringAsync();
    }
    return Results.Ok(JsonSerializer.Deserialize<object>(resmsg));
});

app.MapDelete("/p", async (IHttpClientFactory fact, HttpContext ctx, [FromQuery] string weburl) => {
    var httpClient = fact.CreateClient();
    foreach (var hd in ctx.Request.Headers) {
        httpClient.DefaultRequestHeaders.Add(hd.Key, hd.Value.ToString());
    }
    string resmsg = "";
    try {
        var response = await httpClient.DeleteFromJsonAsync<object>(weburl);
        resmsg = JsonSerializer.Serialize(response);
    }
    catch {
        httpClient.DefaultRequestHeaders.Clear();
        var response = await httpClient.DeleteFromJsonAsync<object>(weburl);
        resmsg = JsonSerializer.Serialize(response);
    }
    return Results.Ok(JsonSerializer.Deserialize<object>(resmsg));
});

app.Run();
