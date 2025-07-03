using Microsoft.EntityFrameworkCore;
using NotesApi.Data;
using NotesApi.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Notes API", Version = "v1" });
});

builder.Services.AddDbContext<NotesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options => options.AddDefaultPolicy(policy => 
    policy.WithOrigins("http://localhost:5173")
          .AllowAnyMethod()
          .AllowAnyHeader()));

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notes API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors();

// API Endpoints
app.MapGet("/", () => "Notes API is running").WithTags("General");
app.MapGet("/api/notes", async (NotesDbContext db) => await db.Notes.ToListAsync()).WithTags("Notes");
app.MapGet("/api/notes/{id}", async (int id, NotesDbContext db) => 
    await db.Notes.FindAsync(id) is Note note ? Results.Ok(note) : Results.NotFound()).WithTags("Notes");

app.MapPost("/api/notes", async (Note note, NotesDbContext db) =>
{
    note.CreatedAt = DateTime.UtcNow;
    note.UpdatedAt = DateTime.UtcNow;
    db.Notes.Add(note);
    await db.SaveChangesAsync();
    return Results.Created($"/api/notes/{note.Id}", note);
}).WithTags("Notes");

app.MapPut("/api/notes/{id}", async (int id, Note inputNote, NotesDbContext db) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note is null) return Results.NotFound();
    
    note.Title = inputNote.Title;
    note.Text = inputNote.Text;
    note.UpdatedAt = DateTime.UtcNow;
    
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Notes");

app.MapDelete("/api/notes/{id}", async (int id, NotesDbContext db) =>
{
    if (await db.Notes.FindAsync(id) is Note note)
    {
        db.Notes.Remove(note);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
}).WithTags("Notes");

app.Run();