Versions used:
- v4.8.1 .net framework  (visual studio 2019)
- v16.0.1000.6 (Microsoft SQL Server 2022)
- SQL Server Management Studio 2019

ApiServer (http://localhost:55898): 
1. click into the folder and run the .sln, 
2. edit <connectionStrings> in the web.config if necessary for credentials to your sql server. Most of the time (integrated security = True;) should work without credentials.
3. click on the play button.

Microsoft SQL Server: 
1. Ensure services is running
2. Use SSMS to restore ToppanTechnicalChallengeDB.bak file

[Optional] WebApplication (http://localhost:55830)
1. click into the folder and run the .sln, 
2. click on the play button.


Dummy accounts for testing:
1.username: User1
  password: P@ssw0rd123
2. username: User2
   password: P@ssw0rd321

POST/Login Parameters:
- [Required: uri] Username 
- [Required: uri] Password

GET/university Parameters:
- [Optional: uri] Token 
- [Optional: uri] SearchFilter
- [Optional: uri] CountryNameFilter
- [Optional: uri] PageNumber 
- [Optional- uri] PageSize


POST/university Parameters:
- [Required: uri] Token 
- [Required: uri] CountryName
- [Required: uri] UniversityName
- [Required: uri] Webpages

GET/university/{Id} Parameters:
- [Required] Id
- [Optional: Body] Token

PUT/university/{Id} Parameters:
- [Required] Id
- [Required: Body] Token
- [Required: uri] CountryName
- [Required: uri] UniversityName
- [Required: uri] Webpages
- [Required: uri] IsActive

DELETE/university/{Id}Parameters:
- [Required] Id
- [Required: Body] Token

POST/university/bookmark/{Id}:
- [Required] Id
- [Required: Body] Token
- [Required: Body] BookmarkDescription



