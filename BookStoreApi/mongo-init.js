// Seed the BookStore database with initial data from the Microsoft Learn tutorial.
// https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-10.0&tabs=visual-studio#configure-mongodb

db = db.getSiblingDB("BookStore");

db.Books.insertMany([
  {
    Name: "Design Patterns",
    Price: 54.93,
    Category: "Computers",
    Author: "Ralph Johnson",
  },
  {
    Name: "Clean Code",
    Price: 43.15,
    Category: "Computers",
    Author: "Robert C. Martin",
  },
]);

