using ITCS_3112_Lab_2_Recommendation.Contracts;
using ITCS_3112_Lab_2_Recommendation.Domain;
using ITCS_3112_Lab_2_Recommendation.Repositories;
using ITCS_3112_Lab_2_Recommendation.Services;

// ─────────────────────────────────────────────────────────────────────────────
//  Dependency wiring (manual DI – no container needed for this scope)
// ─────────────────────────────────────────────────────────────────────────────
IBookRepository   bookRepo   = new BookRepository();
IRatingRepository ratingRepo = new RatingRepository();
IMemberRepository memberRepo = new MemberRepository();
IAuthService      authSvc    = new AuthService(memberRepo);
IRecommendationService recSvc =
    new RecommendationService(memberRepo, bookRepo, ratingRepo);

// ─────────────────────────────────────────────────────────────────────────────
//  File loading
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("=================================================");
Console.WriteLine("   📚  Book Recommendation System  📚");
Console.WriteLine("=================================================\n");

Console.Write("Enter path to books file (e.g. books.txt): ");
string booksPath = Console.ReadLine()?.Trim() ?? "books.txt";

Console.Write("Enter path to ratings file (e.g. ratings.txt): ");
string ratingsPath = Console.ReadLine()?.Trim() ?? "ratings.txt";

try
{
    // Books must be loaded before ratings (ratings reference book positions)
    bookRepo.LoadFromFile(booksPath);
    Console.WriteLine($"  ✅  Loaded {bookRepo.GetAll().Count} books.");

    memberRepo.LoadFromFile(ratingsPath, ratingRepo, bookRepo);
    Console.WriteLine($"  ✅  Loaded {memberRepo.GetAll().Count} members and {ratingRepo.GetAll().Count} ratings.\n");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"\n  ❌  {ex.Message}");
    Console.WriteLine("  Please ensure both files exist and re-run the program.\n");
    return;
}
catch (Exception ex)
{
    Console.WriteLine($"\n  ❌  Unexpected error during file loading: {ex.Message}");
    return;
}

// ─────────────────────────────────────────────────────────────────────────────
//  Main menu loop
// ─────────────────────────────────────────────────────────────────────────────
bool running = true;
while (running)
{
    Console.WriteLine("\n─────────────────────────────────────────────────");
    if (authSvc.IsLoggedIn)
        Console.WriteLine($"  Logged in as: {authSvc.CurrentMember!.Name}");
    else
        Console.WriteLine("  Not logged in");
    Console.WriteLine("─────────────────────────────────────────────────");

    // Show appropriate menu depending on login state
    if (!authSvc.IsLoggedIn)
    {
        Console.WriteLine("  1. Login");
        Console.WriteLine("  2. Add New Member");
        Console.WriteLine("  3. View All Books");
        Console.WriteLine("  0. Exit");
    }
    else
    {
        Console.WriteLine("  1. Logout");
        Console.WriteLine("  2. Add New Member");
        Console.WriteLine("  3. Add New Book");
        Console.WriteLine("  4. View All Books");
        Console.WriteLine("  5. Rate a Book");
        Console.WriteLine("  6. View My Ratings");
        Console.WriteLine("  7. Get Book Recommendations");
        Console.WriteLine("  0. Exit");
    }

    Console.Write("\nEnter choice: ");
    string choice = Console.ReadLine()?.Trim() ?? "";

    Console.WriteLine();

    if (!authSvc.IsLoggedIn)
    {
        switch (choice)
        {
            case "1": HandleLogin(); break;
            case "2": HandleAddMember(); break;
            case "3": HandleViewBooks(); break;
            case "0": running = false; break;
            default:  Console.WriteLine("  ⚠️  Invalid choice."); break;
        }
    }
    else
    {
        switch (choice)
        {
            case "1": HandleLogout();          break;
            case "2": HandleAddMember();        break;
            case "3": HandleAddBook();          break;
            case "4": HandleViewBooks();        break;
            case "5": HandleRateBook();         break;
            case "6": HandleViewRatings();      break;
            case "7": HandleRecommendations();  break;
            case "0": running = false;          break;
            default:  Console.WriteLine("  ⚠️  Invalid choice."); break;
        }
    }
}

Console.WriteLine("\nGoodbye! Happy reading! 📖\n");
return;

// ─────────────────────────────────────────────────────────────────────────────
//  Handler methods
// ─────────────────────────────────────────────────────────────────────────────

void HandleLogin()
{
    Console.Write("Enter your name: ");
    string name = Console.ReadLine()?.Trim() ?? "";

    if (authSvc.Login(name))
        Console.WriteLine($"  ✅  Welcome back, {authSvc.CurrentMember!.Name}!");
    else
        Console.WriteLine($"  ❌  No member found with name '{name}'. Please add yourself first.");
}

void HandleLogout()
{
    string name = authSvc.CurrentMember!.Name;
    authSvc.Logout();
    Console.WriteLine($"  ✅  {name} has been logged out.");
}

void HandleAddMember()
{
    Console.Write("Enter new member's full name: ");
    string name = Console.ReadLine()?.Trim() ?? "";

    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("  ⚠️  Name cannot be empty.");
        return;
    }

    var newMember = new Member(name);
    memberRepo.Add(newMember);
    Console.WriteLine($"  ✅  Member added: {newMember}");
}

void HandleAddBook()
{
    Console.Write("Author: ");
    string author = Console.ReadLine()?.Trim() ?? "";

    Console.Write("Title: ");
    string title = Console.ReadLine()?.Trim() ?? "";

    Console.Write("Year (or range, e.g. 1997 or 1954-1955): ");
    string year = Console.ReadLine()?.Trim() ?? "";

    if (string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(title))
    {
        Console.WriteLine("  ⚠️  Author and title are required.");
        return;
    }

    var newBook = new Book(author, title, year);
    bookRepo.Add(newBook);
    Console.WriteLine($"  ✅  Book added: {newBook}");
}

void HandleViewBooks()
{
    var books = bookRepo.GetAll();
    if (books.Count == 0)
    {
        Console.WriteLine("  No books in the system.");
        return;
    }

    Console.WriteLine($"  {'ISBN',-6}  {'Title',-40}  {'Author',-25}  Year");
    Console.WriteLine(new string('-', 85));
    foreach (var b in books)
        Console.WriteLine($"  {b.ISBN,-6}  {b.Title,-40}  {b.Author,-25}  {b.Year}");
}

void HandleRateBook()
{
    var books = bookRepo.GetAll();
    if (books.Count == 0)
    {
        Console.WriteLine("  No books available to rate.");
        return;
    }

    HandleViewBooks();
    Console.Write("\nEnter the ISBN of the book you want to rate: ");

    if (!int.TryParse(Console.ReadLine()?.Trim(), out int isbn))
    {
        Console.WriteLine("  ⚠️  Invalid ISBN.");
        return;
    }

    var book = bookRepo.GetById(isbn);
    if (book == null)
    {
        Console.WriteLine("  ⚠️  Book not found.");
        return;
    }

    Console.WriteLine($"\n  Rating: \"{book.Title}\"");
    Console.WriteLine("  ─────────────────────────────────────────");
    Console.WriteLine("    -5  😡  Hated it!");
    Console.WriteLine("    -3  🙁  Didn't like it");
    Console.WriteLine("     0  🤷  Haven't read it");
    Console.WriteLine("     1  😐  Ok – neither hot nor cold");
    Console.WriteLine("     3  🙂  Liked it!");
    Console.WriteLine("     5  🤩  Really liked it!");
    Console.Write("\n  Enter rating value (-5, -3, 0, 1, 3, 5): ");

    if (!int.TryParse(Console.ReadLine()?.Trim(), out int rawRating))
    {
        Console.WriteLine("  ⚠️  Invalid input.");
        return;
    }

    RatingValue rv = rawRating switch
    {
        -5 => RatingValue.HatedIt,
        -3 => RatingValue.DidntLike,
         0 => RatingValue.NotRead,
         1 => RatingValue.Neutral,
         3 => RatingValue.Liked,
         5 => RatingValue.ReallyLiked,
        _  => (RatingValue)999
    };

    if ((int)rv == 999)
    {
        Console.WriteLine("  ⚠️  Invalid rating value. Choose from: -5, -3, 0, 1, 3, 5");
        return;
    }

    var rating = new Rating(book, authSvc.CurrentMember!, rv);
    ratingRepo.AddOrUpdate(rating);
    Console.WriteLine($"  ✅  Saved: {rating}");
}

void HandleViewRatings()
{
    var ratings = ratingRepo.GetByMember(authSvc.CurrentMember!.AccountId);

    if (ratings.Count == 0)
    {
        Console.WriteLine("  You haven't rated any books yet.");
        return;
    }

    Console.WriteLine($"  Ratings for {authSvc.CurrentMember!.Name}:");
    Console.WriteLine(new string('-', 65));
    foreach (var r in ratings.OrderBy(r => r.Book.Title))
        Console.WriteLine($"  {r.Value,3}  {r.GetLabel(),-28}  {r.Book.Title}");
}

void HandleRecommendations()
{
    var recs = recSvc.GetRecommendations(authSvc.CurrentMember!, 5);

    if (recs.Count == 0)
    {
        Console.WriteLine("  No recommendations available yet. Try rating more books!");
        return;
    }

    Console.WriteLine($"  📖  Top recommendations for {authSvc.CurrentMember!.Name}:");
    Console.WriteLine(new string('-', 65));
    for (int i = 0; i < recs.Count; i++)
    {
        var b = recs[i];
        Console.WriteLine($"  {i + 1}. \"{b.Title}\" by {b.Author} ({b.Year})");
    }
}
