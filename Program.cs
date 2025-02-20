using CreekRiver.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<CreekRiverDbContext>(builder.Configuration["CreekRiverDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/campsites", (CreekRiverDbContext db) =>
{
    return db.Campsites.ToList();
});

app.MapGet("/api/campsites/{id}", (CreekRiverDbContext db, int id) =>
{
    try
    {
        var campsite = db.Campsites.Include(c => c.CampsiteType).Single(c => c.Id == id);
        return Results.Ok(campsite);
    }
    catch (InvalidOperationException)
    {
        return Results.NotFound("Campsite was not found.");
    }
    catch (Exception)
    {
        return Results.Problem("An error has occurred.");
    }
});

app.MapPost("/api/campsites", (CreekRiverDbContext db, Campsite campsite) =>
{
    try
    {
        // Checks if the CampsiteTypeId associated with the Campsite exists.
        var validCampsiteType = db.CampsiteTypes.Any(ct => ct.Id == campsite.CampsiteTypeId);
        if (!validCampsiteType)
        {
            return Results.BadRequest("CampsiteTypeId was not found.");
        }

        db.Campsites.Add(campsite);
        db.SaveChanges();
        return Results.Created($"/api/campsites/{campsite.Id}", campsite);
    }
    catch (DbUpdateException ex)
    {
        return Results.Problem("An error occurred while trying to save the campsite.");
    }
});

app.MapDelete("/api/campsites/{id}", (CreekRiverDbContext db, int id) =>
{
    try
    {
        Campsite campsite = db.Campsites.SingleOrDefault(campsite => campsite.Id == id);
        if (campsite == null)
        {
            return Results.NotFound();
        }
        db.Campsites.Remove(campsite);
        db.SaveChanges();
    }
    catch (InvalidOperationException)
    {
        return Results.NotFound("Campsite was not found.");
    }
    return Results.NoContent();

});

app.MapPut("/api/campsites/{id}", (CreekRiverDbContext db, int id, Campsite campsite) =>
{
    Campsite campsiteToUpdate = db.Campsites.SingleOrDefault(campsite => campsite.Id == id);
    if (campsiteToUpdate == null)
    {
        return Results.NotFound();
    }
    campsiteToUpdate.Nickname = campsite.Nickname;
    campsiteToUpdate.CampsiteTypeId = campsite.CampsiteTypeId;
    campsiteToUpdate.ImageUrl = campsite.ImageUrl;

    db.SaveChanges();
    return Results.NoContent();
});

app.MapGet("/api/reservations", (CreekRiverDbContext db) =>
{
    // Corresponds to a series of SQL operations that will link the columns together.
    return db.Reservations
        .Include(r => r.UserProfile)
        .Include(r => r.Campsite)
        .ThenInclude(c => c.CampsiteType)
        .OrderBy(res => res.CheckinDate)
        .ToList();
});

app.MapPost("/api/reservations", (CreekRiverDbContext db, Reservation newRes) =>
{
    try
    {
        db.Reservations.Add(newRes);
        db.SaveChanges();
        return Results.Created($"/api/reservations/{newRes.Id}", newRes);
    }
    catch (DbUpdateException)
    {
        return Results.BadRequest("The data submitted is invalid.");
    }
});

app.MapDelete("/api/reservations/{id}", (CreekRiverDbContext db, int id) =>
{
    try
    {
        Reservation reservation = db.Reservations.SingleOrDefault(reservation => reservation.Id == id);
        if (reservation == null)
        {
            return Results.NotFound();
        }
        db.Reservations.Remove(reservation);
        db.SaveChanges();
    }
    catch (InvalidOperationException)
    {
        return Results.NotFound("Reservation was not found.");
    }
    return Results.NoContent();

});

app.Run();
