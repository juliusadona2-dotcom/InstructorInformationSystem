using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InstructorInformationSystem;
using InstructorInformationSystem.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "This field is required.");
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPasswordHasher<UserAccount>, PasswordHasher<UserAccount>>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Apply EF Core migrations on startup (creates the SQLite database on first run).
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var connection = database.Database.GetDbConnection();
    database.Database.OpenConnection();

    long Scalar(string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        return Convert.ToInt64(command.ExecuteScalar() ?? 0);
    }

    var hasInstructors = Scalar("SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'Instructors'") > 0;
    var hasHistory = Scalar("SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = '__EFMigrationsHistory'") > 0;

    if (hasInstructors)
    {
        // Older databases stored Instructor ID as an integer. IDs like 24-0000 need a text column,
        // so rebuild the Instructors table once and keep any existing records.
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT type FROM pragma_table_info('Instructors') WHERE name = 'InstructorId'";
        var columnType = command.ExecuteScalar() as string;

        if (string.Equals(columnType, "INTEGER", StringComparison.OrdinalIgnoreCase))
        {
            using var transaction = database.Database.BeginTransaction();
            database.Database.ExecuteSqlRaw("DROP INDEX IF EXISTS \"IX_Instructors_EmailAddress\"");
            database.Database.ExecuteSqlRaw("ALTER TABLE \"Instructors\" RENAME TO \"Instructors_old\"");
            database.Database.ExecuteSqlRaw(
                "CREATE TABLE \"Instructors\" (" +
                "\"InstructorId\" TEXT NOT NULL CONSTRAINT \"PK_Instructors\" PRIMARY KEY, " +
                "\"FullName\" TEXT NOT NULL, \"Age\" INTEGER NOT NULL, \"Address\" TEXT NOT NULL, " +
                "\"Designation\" TEXT NOT NULL, \"Gender\" TEXT NOT NULL, \"ContactNumber\" TEXT NOT NULL, " +
                "\"Majorship\" TEXT NOT NULL, \"BachelorDegree\" TEXT NOT NULL, \"EmailAddress\" TEXT NOT NULL)");
            database.Database.ExecuteSqlRaw(
                "CREATE UNIQUE INDEX \"IX_Instructors_EmailAddress\" ON \"Instructors\" (\"EmailAddress\")");
            database.Database.ExecuteSqlRaw(
                "INSERT INTO \"Instructors\" SELECT CAST(\"InstructorId\" AS TEXT), \"FullName\", \"Age\", \"Address\", " +
                "\"Designation\", \"Gender\", \"ContactNumber\", \"Majorship\", \"BachelorDegree\", \"EmailAddress\" " +
                "FROM \"Instructors_old\"");
            database.Database.ExecuteSqlRaw("DROP TABLE \"Instructors_old\"");
            transaction.Commit();
        }
    }

    if (hasInstructors && !hasHistory)
    {
        // This database was created before migrations existed (EnsureCreated), so its tables already
        // match InitialCreate. Mark that migration as applied so Migrate() does not recreate them.
        database.Database.ExecuteSqlRaw(
            "CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\" (" +
            "\"MigrationId\" TEXT NOT NULL CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY, " +
            "\"ProductVersion\" TEXT NOT NULL)");
        database.Database.ExecuteSqlRaw(
            "INSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") " +
            "VALUES ('20261111100000_InitialCreate', '10.0.14')");
    }

    database.Database.Migrate();
    database.Database.CloseConnection();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
