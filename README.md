# Student Management System - ASP.NET Core MVC

A full-featured Student Management System built with ASP.NET Core MVC, Entity Framework Core, and SQL Server LocalDB.

## Features

- ? Create new student records
- ? View all students in a table format
- ? Edit existing student information
- ? Delete student records with confirmation
- ? Database persistence using Entity Framework Core
- ? Model-View-Controller architecture

## Technologies Used

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core
- SQL Server LocalDB
- Razor Views
- Bootstrap (for styling)

## Prerequisites

- Visual Studio 2022
- .NET 6.0 or higher
- SQL Server LocalDB (usually included with Visual Studio)

---

## Tutorial: Building from Scratch

Let's start entirely from scratch. Building this from the "File -> New" screen is the best way to ensure all the moving parts (Models, Views, Controllers, and the Database) connect flawlessly.

### Phase 1: Creating the Project

This sets up the MVC (Model-View-Controller) folder structure automatically.

1. Open **Visual Studio 2022** and click **Create a new project**.
2. In the search bar at the top, type **MVC**.
3. Select the template named **ASP.NET Core Web App (Model-View-Controller)**. Make sure it's the one with the C# icon, not F# or Visual Basic.
4. Click **Next**.
5. **Project Name**: Type `StudentApplication`.
6. Click **Next**.
7. **Framework**: Leave it as the default (usually .NET 6.0 or .NET 8.0). Ensure "Configure for HTTPS" is checked.
8. Click **Create**.

---

### Phase 2: Installing Entity Framework (EF) Core

To make C# talk to your LocalDB without writing raw SQL queries, you need specific "translator" packages.

1. At the very top menu of Visual Studio, click **Tools** -> **NuGet Package Manager** -> **Manage NuGet Packages for Solution...**
2. Click on the **Browse** tab.
3. Search for and install these two packages (check the box next to your project name on the right, then click Install):
   - `Microsoft.EntityFrameworkCore.SqlServer` (This lets EF Core talk to SQL Server/LocalDB)
   - `Microsoft.EntityFrameworkCore.Tools` (This gives you the migration commands we'll use later)

---

### Phase 3: Creating the Model (The Blueprint)

The Model is the C# representation of what will become your database table.

1. In the **Solution Explorer** (on the right), right-click the **Models** folder.
2. Hover over **Add** -> Click **Class...**
3. Name the file `Student.cs` and click **Add**.
4. Replace the code inside with this:

```csharp
namespace StudentApplication.Models
{
    public class Student
    {
        public int Id { get; set; } // The Primary Key
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Course { get; set; }
        public int Age { get; set; }
    }
}
```

---

### Phase 4: Setting up the DbContext & Connection

The DbContext is the bridge between your C# code and the actual database.

#### 1. Create the DbContext Class:

1. Right-click your project name (**StudentApplication**) -> **Add** -> **New Folder**. Name it **Data**.
2. Right-click the new **Data** folder -> **Add** -> **Class...**
3. Name it `AppDbContext.cs`.
4. Replace the code inside with this:

```csharp
using Microsoft.EntityFrameworkCore;
using StudentApplication.Models;

namespace StudentApplication.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor needed for Dependency Injection
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // This property creates the 'Students' table in the database
        public DbSet<Student> Students { get; set; }
    }
}
```

#### 2. Configure the Connection String:

1. In the Solution Explorer, open the file named `appsettings.json`.
2. Add the "ConnectionStrings" block so your file looks exactly like this:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### 3. Inject the DbContext (Crucial for Demonstration):

1. Open `Program.cs`.
2. Just below the line `var builder = WebApplication.CreateBuilder(args);`, add these two lines to register your database service:

```csharp
using Microsoft.EntityFrameworkCore;
using StudentApplication.Data;

// Add this right under the builder creation:
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

---

### Phase 5: Migrations (Creating the Physical Database)

Now we tell Visual Studio to read your C# setup and generate the actual SQL database.

1. Go to **Tools** -> **NuGet Package Manager** -> **Package Manager Console**.
2. A terminal window will open at the bottom of your screen.
3. Type the following command and press Enter:
   ```
   Add-Migration InitialCreate
   ```
   (This creates a file detailing the steps to build the database).
4. Once that finishes successfully, type this command and press Enter:
   ```
   Update-Database
   ```
   (This actually executes the SQL to build the database).

---

### Phase 6: The Controller (The Brain)

Now that the database exists, we need the controller to handle the logic.

1. Right-click the **Controllers** folder -> **Add** -> **Controller...**
2. Select **MVC Controller - Empty** and click **Add**.
3. Name it `StudentController.cs`.
4. Replace the code with this:

```csharp
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Data;
using StudentApplication.Models;
using System.Linq;

namespace StudentApplication.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        // Dependency Injection happens here
        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Show the list of students
        public IActionResult List()
        {
            var students = _context.Students.ToList();
            return View(students);
        }

        // GET: Shows the blank form to the user
        public IActionResult Create()
        {
            return View();
        }

        // POST: Receives the data when the user clicks "Submit"
        [HttpPost]
        public IActionResult Create(Student student)
        {
            // 1. The Controller passes the Model to the DbContext
            _context.Students.Add(student); 
            
            // 2. DbContext generates the SQL INSERT command and saves it
            _context.SaveChanges(); 
            
            // 3. Redirect the user back to the list to see the new addition
            return RedirectToAction("List"); 
        }

        // --- EDIT LOGIC ---
        // GET: Fetch the specific student and show them in the form
        public IActionResult Edit(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Save the updated details
        [HttpPost]
        public IActionResult Edit(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
            return RedirectToAction("List");
        }

        // --- DELETE LOGIC ---
        // GET: Show a confirmation page
        public IActionResult Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Actually remove the record from the database
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }
    }
}
```

---

### Phase 7: The List View (Showing the Data)

Right now, your Controller has a List method, but it has no HTML page to send to the browser.

1. In the Solution Explorer, right-click the **Views** folder -> **Add** -> **New Folder**. Name it **Student**.
2. Right-click the new **Student** folder -> **Add** -> **View...**
3. Select **Razor View - Empty** and click **Add**.
4. Name the file `List.cshtml`.
5. Paste this code:

```html
@model IEnumerable<StudentApplication.Models.Student>

<h2>Student Management System</h2>

<a asp-action="Create" class="btn btn-primary" style="margin-bottom: 15px; display:inline-block; padding: 8px 15px; background-color: #007bff; color: white; text-decoration:none; border-radius: 4px;">Add New Student</a>

<table border="1" cellpadding="8" style="width: 100%; border-collapse: collapse;">
    <tr style="background-color: #f2f2f2;">
        <th>Name</th>
        <th>Email</th>
        <th>Course</th>
        <th>Age</th>
        <th>Actions</th>
    </tr>

    @foreach (var student in Model)
    {
        <tr>
            <td>@student.FullName</td>
            <td>@student.Email</td>
            <td>@student.Course</td>
            <td>@student.Age</td>
            <td>
                <a asp-action="Edit" asp-route-id="@student.Id">Edit</a> |
                <a asp-action="Delete" asp-route-id="@student.Id">Delete</a>
            </td>
        </tr>
    }
</table>
```

---

### Phase 8: The Create View

1. Right-click the **Views/Student** folder -> **Add** -> **View...** (Razor View - Empty).
2. Name it `Create.cshtml` and paste this form:

```html
@model StudentApplication.Models.Student

<h2>Add New Student</h2>

<form asp-action="Create" method="post" style="max-width: 400px;">
    <div style="margin-bottom: 10px;">
        <label>Full Name</label><br />
        <input asp-for="FullName" required style="width: 100%;" />
    </div>

    <div style="margin-bottom: 10px;">
        <label>Email</label><br />
        <input asp-for="Email" type="email" required style="width: 100%;" />
    </div>

    <div style="margin-bottom: 10px;">
        <label>Course</label><br />
        <input asp-for="Course" required style="width: 100%;" />
    </div>

    <div style="margin-bottom: 15px;">
        <label>Age</label><br />
        <input asp-for="Age" type="number" required style="width: 100%;" />
    </div>

    <button type="submit" style="padding: 8px 15px; background-color: #28a745; color: white; border: none; cursor: pointer;">Save Student</button>
    <a asp-action="List" style="margin-left: 10px;">Cancel</a>
</form>
```

---

### Phase 9: The Edit View

1. Right-click **Views/Student** -> **Add** -> **View...** (Razor View - Empty).
2. Name it `Edit.cshtml` and paste this:

```html
@model StudentApplication.Models.Student

<h2>Edit Student</h2>

<form asp-action="Edit" method="post" style="max-width: 400px;">
    <input type="hidden" asp-for="Id" />
    
    <div style="margin-bottom: 10px;">
        <label>Full Name</label><br />
        <input asp-for="FullName" required style="width: 100%;" />
    </div>

    <div style="margin-bottom: 10px;">
        <label>Email</label><br />
        <input asp-for="Email" type="email" required style="width: 100%;" />
    </div>

    <div style="margin-bottom: 10px;">
        <label>Course</label><br />
        <input asp-for="Course" required style="width: 100%;" />
    </div>

    <div style="margin-bottom: 15px;">
        <label>Age</label><br />
        <input asp-for="Age" type="number" required style="width: 100%;" />
    </div>

    <button type="submit" style="padding: 8px 15px; background-color: #28a745; color: white; border: none; cursor: pointer;">Update Student</button>
    <a asp-action="List" style="margin-left: 10px;">Cancel</a>
</form>
```

---

### Phase 10: The Delete View

1. Right-click **Views/Student** -> **Add** -> **View...** (Razor View - Empty).
2. Name it `Delete.cshtml` and paste this:

```html
@model StudentApplication.Models.Student

<h2 style="color: red;">Are you sure you want to delete this student?</h2>

<div style="background-color: #ffe6e6; padding: 15px; border: 1px solid red; max-width: 400px; margin-bottom: 15px;">
    <p><strong>Name:</strong> @Model.FullName</p>
    <p><strong>Course:</strong> @Model.Course</p>
    <p><strong>Email:</strong> @Model.Email</p>
</div>

<form asp-action="Delete" method="post">
    <input type="hidden" asp-for="Id" />
    <button type="submit" style="padding: 8px 15px; background-color: #dc3545; color: white; border: none; cursor: pointer;">Yes, Delete</button>
    <a asp-action="List" style="margin-left: 10px;">Cancel</a>
</form>
```

---

### Phase 11: Adding Navigation Links

#### 1. Add a Link to the Top Navigation Bar

1. In the Solution Explorer, expand **Views** -> **Shared**.
2. Double-click to open `_Layout.cshtml`.
3. Find the `<ul class="navbar-nav flex-grow-1">` section.
4. Add this link after the Privacy link:

```html
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-controller="Student" asp-action="List">Manage Students</a>
</li>
```

#### 2. Add a Button to the Home Page

1. In the Solution Explorer, expand **Views** -> **Home**.
2. Double-click to open `Index.cshtml`.
3. Add this button section:

```html
@{
    ViewData["Title"] = "Home Page";
}

<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <p>Learn about <a href="https://docs.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
    
    <div style="margin-top: 30px;">
        <a asp-controller="Student" asp-action="List" class="btn btn-primary btn-lg">
            Go to Student Management System
        </a>
    </div>
</div>
```

---

## Running the Application

1. Hit **F5** (or the Green Play button) to run the app.
2. You can now access the Student Management System directly from:
   - The **"Manage Students"** link in the top navigation bar
   - The **"Go to Student Management System"** button on the home page
   - Or by typing: `https://localhost:yourport/Student/List`

---

## Project Structure

```
StudentApplication/
??? Controllers/
?   ??? HomeController.cs
?   ??? StudentController.cs
??? Data/
?   ??? AppDbContext.cs
??? Models/
?   ??? Student.cs
??? Views/
?   ??? Home/
?   ?   ??? Index.cshtml
?   ??? Student/
?   ?   ??? List.cshtml
?   ?   ??? Create.cshtml
?   ?   ??? Edit.cshtml
?   ?   ??? Delete.cshtml
?   ??? Shared/
?       ??? _Layout.cshtml
??? Migrations/
??? appsettings.json
??? Program.cs
```

---

## License

This project is created for educational purposes.

---

## Author

Vexx-bit

---

## Acknowledgments

- ASP.NET Core Documentation
- Entity Framework Core Documentation
