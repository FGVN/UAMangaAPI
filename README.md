# UAMangaAPI

UAMangaAPI is a RESTful API built using ASP.NET Core for managing manga-related data.

## Endpoints

### Manga Controller

#### Get All Manga
- **GET** `/manga`
  - Returns all manga in the database.

#### Get Manga by Publisher
- **GET** `/manga/{publisher}`
  - Returns manga published by the specified publisher.

#### Search Manga by Name
- **GET** `/manga/Search/{name}`
  - Returns manga whose name contains the specified text. Optionally, you can provide a publisher name to narrow down the search results.

### User Controller

#### Get User Information
- **GET** `/user/GetInfo/{userId}`
  - Returns information about the user with the specified ID.

#### Register User
- **POST** `/user/Register/{username}/{password}`
  - Registers a new user with the provided username and password. Returns the user ID upon successful registration.

#### Login User
- **POST** `/user/Login/{username}/{password}`
  - Logs in a user with the provided username and password. Returns the user ID upon successful login.

### User Manga Controller

#### Add Manga to Wishlist
- **POST** `/wishlist/Add/{userId}/{mangaId}`
  - Adds a manga to the wishlist of the specified user.

#### Remove Manga from Wishlist
- **POST** `/wishlist/Remove/{userId}/{mangaId}`
  - Removes a manga from the wishlist of the specified user.

#### Add Manga to Owned List
- **POST** `/own/Add/{userId}/{mangaId}`
  - Adds a manga to the owned list of the specified user.

#### Remove Manga from Owned List
- **POST** `/own/Remove/{userId}/{mangaId}`
  - Removes a manga from the owned list of the specified user.

## Technologies Used
- ASP.NET Core
- Entity Framework Core
- HTMLAgilityPack
- Microsoft.EntityFrameworkCore

## Getting Started
To get started with this API, follow these steps:
1. Clone this repository.
2. Install the required dependencies.
3. Configure your database connection in `appsettings.json`.
4. Run the application.

## Contributing
Contributions are welcome! Please feel free to open an issue or submit a pull request with any improvements or additional features.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
